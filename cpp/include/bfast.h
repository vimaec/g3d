/*
	BFAST Binary Format for Array Streaming and Transmission
	Copyright 2019, VIMaec LLC
	Copyright 2018, Ara 3D, Inc.
	Usage licensed under terms of MIT Licenese
	https://github.com/vimaec/bfast
*/
#pragma once

#include <vector>
#include <assert.h>
#include <ostream> 
#include <sstream>
#include <algorithm>
#include <iterator>

namespace bfast
{
	#define BFAST_VERSION = { 1, 0, 1, "2019.9.24" };

	using namespace std;

	// Convenient typedefs (easier to read and type)
	typedef uint8_t byte;
	typedef uint64_t ulong;

	// Magic numbers for identifying a BFAST format
	const ulong MAGIC = 0xBFA5;
	const ulong TMP = 0xA5BF;
	const ulong SWAPPED_MAGIC = TMP << 48;

	// The size of the header
	static const int header_size = 32;

	// The size of array offsets 
	static const int array_offset_size = 16;

	// This is the size of the header + padding to bring to alignment 
	static const int array_offsets_start = 64;

	// This is sufficient alignment to fit objects natively into 256-bit (32 byte) registers 
	static const int alignment = 64;

	// Returns true if the given value is aligned. 
	static bool is_aligned(size_t n) { return n % alignment == 0; }

	// Returns an aligned version of the given value to bring it to alignment 
	static size_t aligned_value(size_t n) {
		if (is_aligned(n)) return n;
		auto r = n + alignment - (n % alignment);
		assert(is_aligned(r));
		return r;
	}

	// The array offset indicates where in the raw byte array (offset from beginning of BFAST byte stream) that a particular array's data can be found. 
	struct ArrayOffset {
		ulong _begin;
		ulong _end;
	};

	// A data structure at the top of the file. This is followed by 32 bytes of padding, then an array of n array_offsets (where n is equal to num_arrays)
	struct alignas(8) Header {
		ulong magic;		// Either MAGIC (same-endian) of SWAPPED_MAGIC (different-endian)
		ulong data_start;	// >= desc_end and modulo 64 == 0 and <= file_size
		ulong data_end;		// >= data_start and <= file_size
		ulong num_arrays;	// number of array_headers
	};

	// A helper struct for representing a range of bytes 
	struct ByteRange {
		const byte* _begin;
		const byte* _end;
		const byte* begin() { return _begin; }
		const byte* end() { return _end; }
		const size_t size() { return end() - begin(); }
		const string to_string() { return string(begin(), end()); }
	};

	// A Bfast buffer conceptually is a name and a byte-range
	struct Buffer
	{
		string name;
		ByteRange data;
	};
	// The Bfast container implementation is a container of date ranges: the first one contains the names 
	struct RawData
	{
		// Each data buffer 
		vector<ByteRange> ranges;

		// Computes where the data offsets are relative to the beginning of the BFAST byte stream.
		vector<ArrayOffset> compute_offsets() {
			size_t n = compute_data_start();
			vector<ArrayOffset> r;
			for (auto range : ranges) {
				assert(is_aligned(n));
				ArrayOffset offset = { n, n + range.size() };
				r.push_back(offset);
				n += range.size();
				n = aligned_value(n);
			}
			return r;
		}

		// Computes where the first array data starts 
		size_t compute_data_start() {
			size_t r = 0;
			r += header_size;
			r = aligned_value(r);
			r += array_offset_size * ranges.size();
			r = aligned_value(r);
			return r;
		}

		// Computes how many bytes are needed to store the current BFAST blob
		size_t compute_needed_size() {
			auto tmp = compute_offsets();
			if (tmp.size() == 0)
				return compute_data_start();
			return tmp.back()._end;
		}

		// Copies the data structure to the bytes stream and update the current index
		template<typename T, typename OutIter_T>
		OutIter_T copy_to(T& x, OutIter_T out, size_t& current) {
			auto begin = (char*)&x;
			auto end = begin + sizeof(T);
			current += sizeof(T);
			return copy(begin, end, out);
		}

		// Adds zero bytes to the bytes stream for null padding 
		template<typename OutIter_T>
		OutIter_T output_padding(OutIter_T out, size_t& current) {
			while (!is_aligned(current)) {
				*out++ = (char)0;
				current++;
			}
			return out;
		}

		// Copies the BFAST data structure to the byte stream. 
		template<typename OutIter_T>
		void copy_to(OutIter_T out)
		{
			// Initialize and get the data offsets 
			auto offsets = compute_offsets();
			assert(offsets.size() == ranges.size());
			auto n = offsets.size();
			size_t current = 0;

			// Fill out the header
			Header h;
			h.magic = MAGIC;
			h.num_arrays = n;
			h.data_start = n == 0 ? 0 : offsets.front()._begin;
			h.data_end = n == 0 ? 0 : offsets.back()._end;

			// Copy the header and add padding 
			out = copy_to(h, out, current);
			out = output_padding(out, current);
			assert(is_aligned(current));

			// Early escape if there are no offsets 
			if (n == 0)
				return;

			// Copy the array offsets and add padding 
			for (auto off : offsets)
				out = copy_to(off, out, current);
			out = output_padding(out, current);
			assert(is_aligned(current));
			assert(current = compute_data_start());

			// Copy the arrays 
			for (auto i = 0; i < ranges.size(); ++i) {
				auto range = ranges[i];
				auto offset = offsets[i];
				assert(current == offset._begin);
				out = copy(range.begin(), range.end(), out);
				current += range.size();
				assert(current == offset._end);
				output_padding(out, current);
			}
		}

		vector<byte> pack() {
			vector<byte> r(compute_needed_size());
			copy_to(r.data());
			return r;
		}

		// Unpacks a vector of bytes into a 
		static RawData unpack(const vector<byte>& data)
		{
			const auto& h = *(Header*)& data[0];
			if (h.magic != MAGIC)
				throw runtime_error("invalid magic number, either not a BFast, or was created on a machine with different endianess");
			if (h.data_end < h.data_start)
				throw runtime_error("data ends before it starts");

			const auto* array_offsets = (ArrayOffset*)& data[array_offsets_start];
			
			RawData r;
			for (auto i = 0; i < h.num_arrays; ++i)
			{
				auto offset = array_offsets[i];
				if (offset._begin > offset._end)
					throw runtime_error("Offset begin is after the offset end");
				if (offset._end > data.size())
					throw runtime_error("Offset end is after the end of the data");
				if (i > 0)
					if (offset._begin < array_offsets[i]._end)
						throw runtime_error("Offset begin is before the end of the previous offset");
				auto begin = &data[0] + offset._begin;
				auto end = &data[0] + offset._end;
				r.ranges.push_back(ByteRange{ begin, end });
			}

			return r;
		}
	};


	// A Bfast conceptually is a collection of buffers: named byte arrays 
	struct Bfast
	{
		vector<Buffer> buffers;

		// Construct a raw BFast data block, using the names string argument to store the names data. 
		RawData to_raw_data(string& name_data) {
			// Compute the name data
			name_data.clear();
			for (auto b : buffers)
				name_data += b.name + '\0';
			RawData r;
			auto begin = (byte*)name_data.c_str();
			auto end = begin + name_data.size();
			r.ranges.push_back(ByteRange{ begin, end });
			for (auto b : buffers)
				r.ranges.push_back(b.data);
			return r;
		}

		// Returns a vector of bytes containing the byte stream. 
		vector<byte> pack() {
			string name_data;
			return to_raw_data(name_data).pack();
		}

		// Adds a buffer with the given name and data
		Bfast& add(const string& name, byte* begin, byte* end)
		{
			buffers.push_back(Buffer{ name, ByteRange { begin, end } });
			return *this;
		}

		// Adds a buffer with the given name and the string data
		Bfast& add(const string& name, const char* data)
		{
			return add(name, (byte*)data, (byte*)data + strlen(data));
		}

		// Splits names separated by null characters
		static vector<string> split_names(ByteRange b)
		{
			vector<string> r;
			auto data = (const char*)b.begin();
			auto end = (const char*)b.end();
			while (data < end)
			{
				string tmp(data);
				r.push_back(tmp);
				data += tmp.size();
			}
			return r;
		}

		// Unpacks an array of buffers into a BFastData package
		static Bfast unpack(const vector<byte>& data)
		{
			auto raw_data = RawData::unpack(data);
			auto names = split_names(raw_data.ranges[0]);
			if (names.size() != raw_data.ranges.size() - 1)
				throw runtime_error("The number of names does not match the raw data size");
			Bfast r;
			for (size_t i = 0; i < names.size(); ++i)
			{
				r.buffers.push_back(Buffer{ names[i], raw_data.ranges[i + 1] });
			}
			return r;
		}

		void write_file(string file) {
			auto data = pack();
			FILE* f = nullptr;
			if (fopen_s(&f, file.c_str(), "wb") != 0)
				throw runtime_error("Failed to open file");
			if (f == nullptr)
				return;
			fwrite(&data[0], 1, data.size(), f);
			fclose(f);
		}

		static Bfast read_file(string file) {
			FILE* f;
			fopen_s(&f, file.c_str(), "rb");
			if (f == NULL)
				throw runtime_error("Couldn't read file");

			// obtain file size:
			fseek(f, 0, SEEK_END);
			auto size = ftell(f);
			rewind(f);

			vector<byte> buffer;
			buffer.resize(size);

			// copy the file into the buffer:
			auto tmp = fread(&buffer[0], 1, size, f);

			// terminate
			fclose(f);
			if (tmp != size)
				throw runtime_error("Could not read all data");

			return Bfast::unpack(buffer);
		}
	};
}