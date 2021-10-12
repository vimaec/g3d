/*
    BFAST Binary Format for Array Streaming and Transmission
    Copyright 2019, VIMaec LLC
    Copyright 2018, Ara 3D, Inc.
    Usage licensed under terms of MIT Licenese
    https://github.com/vimaec/bfast
*/

#ifndef __BFAST_H__
#define __BFAST_H__

#include <vector>
#include <assert.h>
#include <ostream> 
#include <sstream>
#include <fstream>
#include <algorithm>
#include <iterator>
#include <stdexcept>

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
    static const int array_offsets_start = 32;

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
    struct alignas(8) ArrayOffset {
        ulong _begin;
        ulong _end;
    };

    // A data structure at the top of the file. This is followed by 32 bytes of padding, then an array of n array_offsets (where n is equal to num_arrays)
    struct alignas(8) Header {
        ulong magic;         // Either MAGIC (same-endian) of SWAPPED_MAGIC (different-endian)
        ulong data_start;    // >= desc_end and modulo 64 == 0 and <= file_size
        ulong data_end;      // >= data_start and <= file_size
        ulong num_arrays;    // number of array_headers
    };

    // A helper struct for representing a range of bytes 
    struct ByteRange {
        const byte* _begin;
        const byte* _end;
        const byte* begin() const { return _begin; }
        const byte* end() const { return _end; }
        const size_t size() const { return end() - begin(); }
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
            vector<ArrayOffset> r(ranges.size());
            for (auto i = 0; i < ranges.size(); i++)
            {
                auto& range = ranges[i];
                assert(is_aligned(n));
                r[i] = { n, n += range.size() };
                n = aligned_value(n);
            }
            return r;
        }

        // Computes where the first array data starts 
        size_t compute_data_start() {
            size_t r = 0;
            r += header_size;
            r += array_offset_size * ranges.size();
            r = aligned_value(r);
            return r;
        }

        // Computes how many bytes are needed to store the current BFAST blob
        size_t compute_needed_size() {
            auto tmp = compute_offsets();
            if (tmp.size() == 0)
                return compute_data_start();
            return aligned_value(tmp.back()._end);
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

        // Copies the BFAST data structure to any output iterator
        template<typename OutIter_T>
        void copy_to(OutIter_T out)
        {
            OutIter_T start = out;
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

            // Copy the header 
            out = copy_to(h, out, current);
            assert(current == 32);
            // Early escape if there are no offsets 
            if (n == 0)

                return;

            // Copy the array offsets and add padding 
            for (auto off : offsets)
                out = copy_to(off, out, current);
            assert(current == 32 + offsets.size() * 16);
            out = output_padding(out, current);
            assert(is_aligned(current));
            assert(current = compute_data_start());

            // Copy the arrays 
            for (auto i = 0; i < ranges.size(); ++i) {
                out = output_padding(out, current);
                const auto& range = ranges[i];
                const auto& offset = offsets[i];
                assert(range.size() == (offset._end - offset._begin));
                assert(current == offset._begin);
                out = copy(range.begin(), range.end(), out);
                current += range.size();
                assert(current == offset._end);
            }
        }

        // Converts the BFast into a byte-array.
        vector<byte> pack() {
            vector<byte> r(compute_needed_size());
            copy_to(r.data());
            return r;
        }

        // Unpacks a vector of bytes into a 
        static RawData unpack(const ByteRange& data)
        {
            const auto& h = *(Header*)data.begin();
            if (h.magic != MAGIC)
                throw std::runtime_error("invalid magic number, either not a BFast, or was created on a machine with different endianess");
            if (h.data_end < h.data_start)
                throw std::runtime_error("data ends before it starts");

            const auto* array_offsets = (ArrayOffset*)(data.begin() + array_offsets_start);

            RawData r;
            r.ranges.resize(h.num_arrays);
            for (auto i = 0; i < h.num_arrays; ++i)
            {
                const auto& offset = array_offsets[i];
                if (offset._begin > offset._end)
                    throw std::runtime_error("Offset begin is after the offset end");
                if (offset._end > data.size())
                    throw std::runtime_error("Offset end is after the end of the data");
                if (i > 0 && offset._begin < array_offsets[i - 1]._end)
                    throw std::runtime_error("Offset begin is before the end of the previous offset");
                auto begin = data.begin() + offset._begin;
                auto end = data.begin() + offset._end;
                r.ranges[i] = ByteRange{ begin, end };
            }

            return r;
        }
    };


    // A Bfast conceptually is a collection of buffers: named byte arrays. 
    // It contains the raw data contained within.
    struct Bfast
    {
        vector<byte> name_data;
        ByteRange data;
        vector<byte> dataBuffer;
        vector<Buffer> buffers;

        // Construct a raw BFast data block, using the names string argument to store the names data. 
        RawData to_raw_data() {
            // Compute the name data
            name_data.clear();

            size_t count = 0;
            for (auto b : buffers)
            {
                for (auto c : b.name)
                    count++;
                count++;
            }

            name_data.resize(count);
            count = 0;
            for (auto b : buffers)
            {
                for (auto c : b.name)
                    name_data[count++] = c;
                name_data[count++] = 0;
            }

            RawData r;
            size_t index = 0;
            r.ranges.resize(1 + buffers.size());
            r.ranges[index++] = ByteRange{ name_data.data(), name_data.data() + name_data.size() };
            for (auto b : buffers)
                r.ranges[index++] = b.data;
            return r;
        }

        // Returns a vector of bytes containing the byte stream. 
        vector<byte> pack() {
            return to_raw_data().pack();
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
            size_t count = 0;
            auto data = (const char*)b.begin();
            auto end = (const char*)b.end();
            while (data < end)
            {
                string tmp(data);
                count++;
                data += tmp.size() + 1;
            }

            vector<string> r(count);
            count = 0;

            data = (const char*)b.begin();
            end = (const char*)b.end();
            while (data < end)
            {
                string tmp(data);
                r[count++] = tmp;
                data += tmp.size() + 1;
            }
            return r;
        }

        // Unpacks an array of buffers into a BFastData package
        static Bfast unpack(const ByteRange& data)
        {
            auto raw_data = RawData::unpack(data);
            auto names = split_names(raw_data.ranges[0]);
            if (names.size() != raw_data.ranges.size() - 1)
                throw std::runtime_error("The number of names does not match the raw data size");
            Bfast r;
            r.data = data;
            r.buffers.resize(names.size());
            for (size_t i = 0; i < names.size(); ++i)
            {
                r.buffers[i] = Buffer{ names[i], raw_data.ranges[i + 1] };
            }
            return r;
        }

        static Bfast unpack(vector<byte>&& data)
        {
            Bfast r;
            r.dataBuffer = move(data);
            r.data = ByteRange{ r.dataBuffer.data(), r.dataBuffer.data() + r.dataBuffer.size() };
            auto raw_data = RawData::unpack(r.data);
            auto names = split_names(raw_data.ranges[0]);
            if (names.size() != raw_data.ranges.size() - 1)
                throw std::runtime_error("The number of names does not match the raw data size");
            r.buffers.resize(names.size());
            for (size_t i = 0; i < names.size(); ++i)
            {
                r.buffers[i] = Buffer{ names[i], raw_data.ranges[i + 1] };
            }
            return r;
        }

        void write_file(string file) {
            auto data = pack();
            FILE* f = nullptr;
            if (fopen_s(&f, file.c_str(), "wb") != 0)
                throw std::runtime_error("Failed to open file");
            if (f == nullptr)
                return;
            fwrite(&data[0], 1, data.size(), f);
            fclose(f);
        }

        static Bfast read_file(string file) {
            std::ifstream fstrm(file, ios_base::in | ios_base::binary);
            fstrm.seekg(0, ios_base::end);
            auto filesize = fstrm.tellg();
            fstrm.seekg(0, ios_base::beg);

            if (!fstrm.is_open())
                throw std::runtime_error("Couldn't read file");

            vector<byte> buffer;
            buffer.resize(filesize);

            // copy the file into the buffer:
            fstrm.read((char*)&buffer[0], filesize);

            return Bfast::unpack(move(buffer));
        }
    };
}

#endif
