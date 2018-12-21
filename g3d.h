#pragma once

#include <vector>
#include <sstream>
#include <map>

#define G3D_VERSION { 0, 9, 0, "2018-12-20" }

namespace g3d
{
    using namespace std;

    // The different types of data types that can be used as elements. 
    enum DataType 
    {
        dt_uint8,
        dt_uint16,
        dt_uint32,
        dt_uint64,
        dt_uint128,
        dt_int8,
        dt_int16,
        dt_int32,
        dt_int64,
        dt_int128,
        dt_float16,
        dt_float32,
        dt_float64,
        dt_float128,
        dt_invalid,
    };

    // What element each attribute is associated with 
    enum Association 
    {
        assoc_vertex,
        assoc_face,
        assoc_corner,
        assoc_edge,
        assoc_object,
        assoc_none,
        assoc_invalid,
    };

    // The type of the attribute
    enum AttributeType 
    {
        attr_custom, 
        attr_coordinate,
        attr_index,
        attr_faceindex,
        attr_facesize,
        attr_normal,
        attr_binormal,
        attr_tangent,
        attr_materialid,
        attr_polygroup,
        attr_uv,
        attr_color,
        attr_smoothing,
        attr_crease,
        attr_hole,
        attr_invisibility, 
        attr_selection,
        attr_pervertex,
        attr_mapchannel_data,
        attr_mapchannel_index,
        attr_invalid,
    };

    // Contains all the information necessary to parse an attribute data channel and associate it with geometry 
    // 8 * (sizeof(int32_t) = 4) = 32 byte structures
    struct AttributeDescriptor
    {
        int32_t _association;            // Indicates the part of the geometry that this attribute is assoicated with 
        int32_t _attribute_type;         // n integer values 
        int32_t _attribute_type_index;   // each attribute type should have it's own index ( you can have uv0, uv1, etc. )
        int32_t _data_arity;             // how many values associated with each element (e.g. UVs might be 2, geometry might be 3)
        int32_t _data_type;              // the type of individual values (e.g. int32, float64)
        int32_t _pad0, _pad1, _pad2;     // ignored, used to bring the alignment up to a power of two.

        void validate() const {
            if (_association < 0 || _association >= assoc_invalid) throw runtime_error("association out of range");
            if (_attribute_type < 0 || _attribute_type >= attr_invalid) throw runtime_error("attribute type out of range");
            if (_data_arity <= 0) throw runtime_error("data arity must be greater than zero");
            if (_data_type < 0 || _data_type >= dt_invalid) throw runtime_error("data type out of range");
        }

        /// The type of individual data values. There are n of these per element where n is the arity.
        DataType data_type() const { 
            return (DataType)_data_type; 
        }

        /// The number of primitive values 
        int data_arity() const {
            return _data_arity;
        }
        
        //
        Association association() const { 
            return (Association)_association; 
        }
        
        AttributeType attribute_type() const { 
            return (AttributeType)_attribute_type; 
        }

        int32_t attribute_type_index() const {
            return _attribute_type_index;
        }

        int32_t data_arity() const {
            return _data_arity;
        }

        int32_t data_type_size() const {
            return data_type_size(data_type());
        }

        static int32_t data_type_size(DataType dt) {
            switch (dt) {
                case dt_uint8:      return 1;
                case dt_uint16:     return 2;
                case dt_uint32:     return 4;
                case dt_uint64:     return 8;
                case dt_uint128:    return 16;
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

        static map<string, int32_t> reverse_map(const map<int32_t, string>& m) {
            map<string, int32_t> r;
            for (const auto& kv : m)
                r[kv.second] = kv.first;
            return r;
        }

        static const map<int32_t, string>& data_types_to_strings() {
            static map<int32_t, string> names = {
                { dt_uint8,     "uint8" },
                { dt_uint16,    "uint16" },
                { dt_uint32,    "uint32" }, 
                { dt_uint64,    "uint64" }, 
                { dt_uint128,   "uint128" },
                { dt_int8,      "int8" },
                { dt_int16,     "int16" },
                { dt_int32,     "int32" },
                { dt_int64,     "int64" },
                { dt_int128,    "int128" },
                { dt_float16,   "float16" },
                { dt_float32,   "float32" },
                { dt_float64,   "float64" },
                { dt_float128,  "float128" },
                { dt_invalid,   "invalid" },
            };
            return names;
        }

        static const map<string, int32_t>& data_types_from_strings() {
            static auto r = reverse_map(data_types_to_strings());
            return r;
        }

        static const map<int32_t, string>& associations_to_strings() {
            static map<int32_t, string> names = {
                { assoc_vertex,     "vertex" },
                { assoc_face,       "face" },
                { assoc_corner,     "corner" },
                { assoc_edge,       "edge" },
                { assoc_object,     "object" },
                { assoc_none,       "none" },
                { assoc_invalid,    "invalid" },
            };
            return names;
        }

        static const map<string, int32_t>& associations_from_strings() {
            static auto r = reverse_map(associations_to_strings());
            return r;
        }

        static map<int32_t, string> attribute_types_to_strings() {
            static map<int32_t, string> names = {
                { attr_custom,          "custom" },
                { attr_coordinate,      "coordinate" },
                { attr_index,           "index" },
                { attr_faceindex,       "faceindex" },
                { attr_facesize,        "facesize" },
                { attr_normal,          "normal" },
                { attr_binormal,        "binormal" },
                { attr_tangent,         "tangent" },
                { attr_materialid,      "materialid" },
                { attr_polygroup,       "polygroup" },
                { attr_uv,              "uv", },
                { attr_color,           "color" },
                { attr_smoothing,       "smoothing" },
                { attr_crease,          "crease" },
                { attr_hole,            "hole" },
                { attr_invisibility,    "invisibility" },
                { attr_selection,       "selection" },
                { attr_pervertex,       "pervertex" },
                { attr_invalid,         "invalid" }
            };
            return names;
        }

        static const map<string, int32_t>& attribute_types_from_strings() {
            static auto r = reverse_map(attribute_types_to_strings());
            return r;
        }
        
        static string data_type_to_string(int32_t dt) {
            return data_types_to_strings().at(dt);
        }

        static int32_t data_type_from_string(string s) {
            return data_types_from_strings().at(s);
        }

        static string association_to_string(int32_t assoc) {
            return associations_to_strings().at(assoc);
        }

        static int32_t association_from_string(string s) {
            return associations_from_strings().at(s);
        }

        static string attribute_type_to_string(int32_t attr) {
            return attribute_types_to_strings().at(attr);
        }

        static int32_t attribute_type_from_string(string s) {
            return attribute_types_from_strings().at(s);
        }

        string association_string() const {
            return association_to_string(association());
        }

        string data_type_string() const {
            return data_type_to_string(data_type());
        }

        string attribute_type_string() const {
            return attribute_type_to_string(data_type());
        }

        const string& to_string() const {
            ostringstream oss;
            oss << "g3d"
                << ":" << association_string()
                << ":" << attribute_type_string()
                << ":" << attribute_type_index()
                << ":" << data_type_string()
                << ":" << data_arity();
            return oss.str();
        };

        template<typename Out>
        static void split(const string &s, char delim, Out result) {
            stringstream ss(s);
            string item;
            while (getline(ss, item, delim)) {
                *(result++) = item;
            }
        }

        static vector<string> split(const string &s, char delim) {
            vector<string> elems;
            split(s, delim, back_inserter(elems));
            return elems;
        }

        static AttributeDescriptor from_string(const string& s) {
            AttributeDescriptor desc;
            auto tokens = split(s, ':');
            auto token = tokens.begin();
            auto end = tokens.end();
            if (token == end) throw runtime_error("Insufficient tokens");
            if (*token++ != "g3d") throw runtime_error("Expected g3d"); if (token == end) throw runtime_error("Insufficient tokens");
            desc._association = association_from_string(*token++); if (token == end) throw runtime_error("Insufficient tokens");
            desc._attribute_type = attribute_type_from_string(*token++); if (token == end) throw runtime_error("Insufficient tokens");
            desc._attribute_type_index = stoi(*token++); if (token == end) throw runtime_error("Insufficient tokens");
            desc._data_type = data_type_from_string(*token++); if (token == end) throw runtime_error("Insufficient tokens");
            desc._data_arity = stoi(*token++); 
            desc.validate();
            if (token != end) throw runtime_error("Too many tokens");
            auto tmp = desc.to_string();
            if (tmp != s) throw runtime_error("Internal error: parsed attribute descriptor does not match generated attribute descriptor");
            return desc;
        }
    };

    /// Manage the data buffer and meta-information of an attribute 
    struct Attribute {
        Attribute(const AttributeDescriptor& desc, void* begin, void* end)
            : descriptor(desc), _begin((uint8_t*)begin), _end((uint8_t*)end)
        { 
            if (!begin || !end) throw runtime_error("Null parameters");
            if (byte_size() % data_element_size() != 0) throw runtime_error("Data buffer is not the correct alignment");        
        }
        size_t byte_size() const {
            return _end - _begin;
        }
        size_t data_element_size() const {
            return descriptor.data_type_size() * descriptor.data_arity();
        }
        size_t num_elements() const {
            return byte_size() / data_element_size();
        }
        AttributeDescriptor descriptor;
        uint8_t* _begin;
        uint8_t* _end;
    };

    // An abstract class for collections of attributes 
    struct AttributeBuilderBase {
       virtual Attribute* GetAttribute() = 0;
       virtual ~AttributeBuilderBase() {} 
    };

    // A typed 
    template<typename T>
    struct AttributeBuilderTypedBase : AttributeBuilderBase {
        T* begin() {
            return (T*)GetAttribute()->_begin;
        }
        T* end() {
            return (T*)GetAttribute()->_end;
        }
    };

    // A class for creating an attribute that reference external data
    template<typename T>
    struct AttributeBuilderRef : AttributeBuilderTypedBase<T> {
        AttributeBuilderRef(AttributeDescriptor descriptor, T* begin, T* end)
            : attribute(descriptor, begin, end)
        {
        }
        virtual Attribute* GetAttribute() {
            return &attribute;
        }
        Attribute attribute;
    };

    // A class for creating an attribute that owns the data, managing it in an internal vector 
    template<typename T>
    struct AttributeBuilderOwn : AttributeBuilderTypedBase<T> {
        AttributeBuilderOwn(AttributeDescriptor descriptor, size_t size, T* data = nullptr)
            : vector(size), attribute(descriptor, buffer.data(), buffer.data() + size)
        { 
            if (data)
                memcpy_s(begin(), size, data(), size);
        }
        virtual Attribute* GetAttribute() {
            return &attribute;
        }
        vector<T> buffer;
        Attribute attribute;
    };

    // A G3d data structure, which is just a set of attributes. 
    // If you pass a pointer to data when adding an attribute it will not make a copy instead that data will be referenced by the G3dBuilder, 
    // If on the other hand you pass a nullptr, you will be responsible for copying the data into the attribute. 
    struct G3d    
    {
        map<string, AttributeBuilderBase*> builders;
        int _vertex_count;
        int _face_count;
        int _corner_count;
        int _polygon_size;

        G3d(int vertex_count, int face_count, int corner_count, int polygon_size = 3) 
        {
            
        }

        ~G3d() {
            for (auto kv : builders)
                delete kv.second;   
        }

        template<typename T>
        AttributeBuilderTypedBase<T>* add_attribute(const AttributeDescriptor& desc, size_t size, T* data = nullptr) {
            // TODO: check that the map channels are not already present ... 
            auto name = desc.to_string();
            if (builders.find(name) != builder.end())
                throw runtime_error("Attribute descriptor already exists");
            if (data) {
                auto r = new AttributeBuilderRef<T>(desc, size, data);
                builders.push_back(r);
                return r;
            }
            else {
                auto r = new AttributeBuilderVector<T>(desc, size, data);
                builders.push_back(r);
                return r;
            }
        }

        template<typename T>
        AttributeBuilderTypedBase<T>* add_attribute(const string& desc, size_t size, T* data = nullptr) {
            return add_attribute(AttributeDescriptor::from_string(desc), size, data);
        }
        
        AttributeBuilderTypedBase<float>* add_vertices(int size, float* data = nullptr) {
            return add_attribute("g3d:vertex:coordinate:0:float32:3", size, data);
        }

        AttributeBuilderTypedBase<float>* add_vertices_as_float4(int size, float* data = nullptr) {
            return add_attribute("g3d:vertex:coordinate:0:float32:4", size, data);
        }

        AttributeBuilderTypedBase<int32_t>* add_indexes(int size, int32_t* data = nullptr) {
            return add_attribute("g3d:corner:index:0:int32:1", size, data);
        }

        AttributeBuilderTypedBase<float>* add_uvs(int size, float* data = nullptr) {
            return add_attribute("g3d:vertex:uv:0:float32:2", size, data);
        }

        AttributeBuilderTypedBase<float>* add_uv2s(int size, float* data = nullptr) {
            return add_attribute("g3d:vertex:uv:1:float32:2", size, data);
        }

        AttributeBuilderTypedBase<float>* add_vertex_normals(int size, float* data = nullptr) {
            return add_attribute("g3d:vertex:uv:0:float32:3", size, data);
        }

        AttributeBuilderTypedBase<float>* add_material_ids(int size, float* data = nullptr) {
            return add_attribute("g3d:face:material_id:0:int32:1", size, data);
        }

        AttributeBuilderTypedBase<float>* add_map_channel_data(int id, int size, float* data = nullptr) {
            auto desc= AttributeDescriptor::from_string("g3d:none:map_channel_data:0:float32:3");
            desc._attribute_type_index = id;
            return add_attribute(desc, size, data);
        }

        AttributeBuilderTypedBase<int>* add_map_channel_index(int id, int size, int* data = nullptr) {
            auto desc = AttributeDescriptor::from_string("g3d:corner:map_channel_index:0:int32:1");
            desc._attribute_type_index = id;
            return add_attribute(desc, size, data);
        }

        void add_map_channel(int id, float* texture_vertices, size_t num_texture_vertices, int* texture_indices, size_t num_texture_faces) {
            add_map_channel_data(id, num_texture_vertices * 3, texture_vertices);
            add_map_channel_index(id, num_texture_faces * 3, texture_indices);
        }
    };
}