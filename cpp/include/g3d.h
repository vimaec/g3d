/*
    G3D Data Format
    Copyright 2019, VIMaec LLC
    Copyright 2018, Ara 3D, Inc.
    Usage licensed under terms of MIT Licenese.
*/
#pragma once

#include <vector>
#include <sstream>
#include <map>

#include "bfast.h"

namespace g3d
{
	#define G3D_VERSION = { 1, 0, 1, "2019.9.24" };

	using namespace std;

    /// The different types of data types that can be used as elements. 
    enum DataType 
    {
        dt_int8,
        dt_int16,
        dt_int32,
        dt_int64,
        dt_int128,
        dt_float16,
        dt_float32,
        dt_float64,
        dt_float128,
    };

    /// What geometric element each attribute is associated with 
    enum Association 
    {
        assoc_vertex,
        assoc_face,
        assoc_corner,
        assoc_edge,
		assoc_group,
        assoc_all,
        assoc_none,
    };

    // The type of the attribute
    enum Semantic 
    {
        sem_unknown,
        sem_user,
        sem_coordinate,
        sem_index,
        sem_faceindex,
        sem_facesize,
        sem_normal,
        sem_binormal,
        sem_tangent,
        sem_materialid,
        sem_polygroup,
        sem_uv,
        sem_color,
        sem_smoothing,
        sem_crease,
        sem_hole,
        sem_visibility, 
        sem_selection,
        sem_pervertex,
        sem_mapchannel_data,
        sem_mapchannel_index,
        sem_custom,
        sem_invalid,
    };

    // Contains all the information necessary to parse an attribute data channel and associate it with some part of the geometry 
    struct AttributeDescriptor
    {
        /// The type of individual data values. There are n of these per element where n is the arity.
		DataType data_type;

        /// The number of primitive values associated with each element 
		int data_arity;
        
        /// What part of the geometry each tuple of data values is associated with 
		Association association;
        
        /// The semantic of the attribute (e.g. normals, uv)
		Semantic semantic;

        /// The size of each data element in bytes (not counting the arity).
        int32_t data_type_size() const {
            return data_type_size(data_type);
        }

        /// Retrieves the size of individual data types
        static int32_t data_type_size(DataType dt) {
            switch (dt) {
                case dt_int8:       return 1;
                case dt_int16:      return 2;
                case dt_int32:      return 4;
                case dt_int64:      return 8;
                case dt_int128:     return 16;
                case dt_float16:    return 2;
                case dt_float32:    return 4;
                case dt_float64:    return 8;
                case dt_float128:   return 16;
                default:            throw runtime_error("invalid data type");
            }
        }

        /// Given a map from keys to values, creates a new map from values to keys 
        template<typename K, typename V>
        static map<V, K> reverse_map(const map<K, V>& m) {
            map<V, K> r;
            for (const auto& kv : m)
                r[kv.second] = kv.first;
            return r;
        }

        /// Returns a lookup table of data-type enumerations to strings 
        static const map<DataType, string>& data_types_to_strings() {
            static map<DataType, string> names = {
                { dt_int8,      "int8" },
                { dt_int16,     "int16" },
                { dt_int32,     "int32" },
                { dt_int64,     "int64" },
                { dt_int128,    "int128" },
                { dt_float16,   "float16" },
                { dt_float32,   "float32" },
                { dt_float64,   "float64" },
                { dt_float128,  "float128" },
            };
            return names;
        }

        static const map<string, DataType>& data_types_from_strings() {
            static auto r = reverse_map(data_types_to_strings());
            return r;
        }

        static const map<Association, string>& associations_to_strings() {
            static map<Association, string> names = {
                { assoc_vertex,     "vertex" },
                { assoc_face,       "face" },
                { assoc_corner,     "corner" },
                { assoc_edge,       "edge" },
				{ assoc_group,		"group" },
				{ assoc_all,		"all" },
                { assoc_none,       "none" },
            };
            return names;
        }

        static const map<string, Association>& associations_from_strings() {
            static auto r = reverse_map(associations_to_strings());
            return r;
        }

        static map<Semantic, string> semantics_to_strings() {
            static map<Semantic, string> names = {
                { sem_unknown,         "unknown" },
                { sem_user,            "user" },
                { sem_coordinate,      "coordinate" },
                { sem_index,           "index" },
                { sem_faceindex,       "faceindex" },
                { sem_facesize,        "facesize" },
                { sem_normal,          "normal" },
                { sem_binormal,        "binormal" },
                { sem_tangent,         "tangent" },
                { sem_materialid,      "materialid" },
                { sem_polygroup,       "polygroup" },
                { sem_uv,              "uv", },
                { sem_color,           "color" },
                { sem_smoothing,       "smoothing" },
                { sem_crease,          "crease" },
                { sem_hole,            "hole" },
                { sem_visibility,      "visibility" },
                { sem_selection,       "selection" },
                { sem_pervertex,       "pervertex" },
                { sem_custom,          "custom" },
                { sem_invalid,         "invalid" }
            };
            return names;
        }

        static const map<string, Semantic>& semantics_from_strings() {
            static auto r = reverse_map(semantics_to_strings());
            return r;
        }
        
        string to_string() const {
            ostringstream oss;
            oss << "g3d"
                << ":" << associations_to_strings().at(association)
                << ":" << semantics_to_strings().at(semantic)
                << ":" << data_types_to_strings().at(data_type)
                << ":" << data_arity;
            return oss.str();
        };

        template<typename Out>
        static void split(const string &s, char delim, Out result) {
            stringstream ss(s);
            string item;
            while (getline(ss, item, delim)) 
                *(result++) = item;
        }

        static vector<string> split(const string &s, char delim) {
            vector<string> elems;
            split(s, delim, back_inserter(elems));
            return elems;
        }

		static Association association_from_string(const string& s) {
			auto d = associations_from_strings();
			if (d.find(s) == d.end()) throw runtime_error("unknown association");
			return d.at(s);
		}

		static DataType data_type_from_string(const string& s) {
			auto d = data_types_from_strings();
			if (d.find(s) == d.end()) throw runtime_error("unknown data-type");
			return d.at(s);
		}

		static Semantic semantic_from_string(const string& s) {
			auto d = semantics_from_strings();
			if (d.find(s) == d.end()) return sem_unknown;
			return d.at(s);
		}

		static AttributeDescriptor from_string(const string& s) {
            AttributeDescriptor desc;
            auto tokens = split(s, ':');
            auto token = tokens.begin();
            auto end = tokens.end();
            if (token == end) throw runtime_error("Insufficient tokens");
            if (*token++ != "g3d") throw runtime_error("Expected g3d"); if (token == end) throw runtime_error("Insufficient tokens");
            desc.association = association_from_string(*token++); if (token == end) throw runtime_error("Insufficient tokens");
            desc.semantic = semantic_from_string(*token++); if (token == end) throw runtime_error("Insufficient tokens");
            desc.data_type = data_type_from_string(*token++); if (token == end) throw runtime_error("Insufficient tokens");
            desc.data_arity = stoi(*token++);
            if (token != end) throw runtime_error("Too many tokens");
            return desc;
        }
    };

    /// Manage the data buffer and meta-information of an attribute 
    struct Attribute {
        Attribute(const string& desc, const void* begin, const void* end)
            : descriptor(AttributeDescriptor::from_string(desc))
			, _begin((uint8_t*)begin)
			, _end((uint8_t*)end)
        { 
            if (!begin || !end) throw runtime_error("Null parameters");
            if (byte_size() % data_element_size() != 0) throw runtime_error("Data buffer byte size does not divide evenly by size of elements");        
        }
        size_t byte_size() const {
            return _end - _begin;
        }
        size_t data_element_size() const {
            return descriptor.data_type_size() * descriptor.data_arity;
        }
        size_t num_elements() const {
            return byte_size() / data_element_size();
        }
		bfast::Buffer to_buffer() {
			return bfast::Buffer{ descriptor.to_string(), bfast::ByteRange { _begin, _end } };
		}
		static Attribute from_buffer(bfast::Buffer buffer) {
			return Attribute(buffer.name, buffer.data.begin(), buffer.data.end());
		}
        AttributeDescriptor descriptor;
        uint8_t* _begin;
        uint8_t* _end;
    };

   // A G3d data structure, is a set of attributes. 
    // If you pass a pointer to data when adding an attribute it will not make a copy instead that data will be referenced by the G3dBuilder, 
    // If on the other hand you pass a nullptr, you will be responsible for copying the data into the attribute afterwards
    struct G3d    
    {
		vector<Attribute> attributes; 

		G3d()
		{ }
			
		G3d(const G3d& other)
			: attributes(other.attributes)
		{ }

		G3d& operator=(const G3d& x)
		{
			attributes = x.attributes;
			return *this;
		}

		bfast::Bfast to_bfast() {
			bfast::Bfast r;
			for (auto attr : attributes)
				r.buffers.push_back(attr.to_buffer());
			return r;
		}

        void write_file(string path) {
            bfast::Bfast b;
			auto meta = "{ filetype: \"g3d\" }";
            b.add("meta", meta);
			for (auto attr : attributes)
				b.buffers.push_back(attr.to_buffer());
			b.write_file(path);
        }

		static G3d read_file(string path)
		{
			auto bfast = bfast::Bfast::read_file(path);
			G3d r;
			for (auto b : bfast.buffers)
				r.add_attribute(b.name, b.data.begin(), b.data.end());
			return r;
		}

		const string Position = "g3d:vertex:position:float32:3";
		const string Indices = "g3d:corner:index:int32:1";
		const string UV = "g3d:vertex:uv:float32:2";
		const string UVW = "g3d:vertex:uv:float32:3";
		const string VertexNormal = "g3d:vertex:normal:float32:3";
		const string FaceNormal = "g3d:vertex:normal:float32:3";
		const string ObjectFaceSize = "g3d:all:facesize:int32:1";
		const string GroupFaceSize = "g3d:group:facesize:int32:1";
		const string FaceSizes = "g3d:face:facesize:int32:1";
		const string FaceIndices = "g3d:face:indexoffset:int32:1";
		const string VertexColor = "g3d:vertex:color:float32:3";
		const string VertexColorWithAlpha = "g3d:vertex:color:float32:4";
		const string Bitangent = "g3d:vertex:bitangent:float32:3";
		const string Tangent3 = "g3d:vertex:tangent:float32:3";
		const string Tangent4 = "g3d:vertex:tangent:float32:4";
		const string GroupIndices = "g3d:group:indexoffset:int32:1";
		const string MaterialIds = "g3d:face:materialid:int32:1";

		void add_attribute(const string& name, const void* begin, const void* end) {
			attributes.push_back(Attribute(name, begin, end));
		}

		void add_attribute(const string& name, void* begin, size_t size) {
			add_attribute(name, begin, (uint8_t*)begin + size);
		}
	};
}