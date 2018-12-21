# G3D

G3D is a simple, efficient, generic binary format for storing and transmitting 3D meshes inspired by Assimp, FBX, OBJ, and 3DS.

To see the official specification take a look at the [g3d.h](https://github.com/ara3d/g3d/blob/master/g3d.h) header file. 

# Overview 

## Design Goals

G3D was designed to be:
1. sufficiently generic to be able to transfer meshes between most formats and 3D tools without loss 
2. minimize the amount of data processing that serializers and deserializers must perform 
3. facilitate writing new compliant serializers / deserializers in different languages or contexts 

## Use Case

The G3D was developed by Ara 3D to allow geometry data to be transferred as efficiently as possibly and without loss between: 3ds Max, Unity, Unreal, FBX, 
OBJ and various tools written in different languages. It serves as the core geometry representation for different Ara 3D tools and libraries.

# Structure

## Attributes 

G3D is organized as a collection of attribute channels. Each attributes describe what part of the mesh they are associated with:
* point
* face
* corner
* edge
* whole object 

Attributes also have a "type" which is used to identify what role the attribute has when parsing. These map roughly to FBX layer elements.

Attributes are groups of one or more data values. The number of data values per associate element is called the "arity". 
The individual data values can be integers, unsigned integers, or floating point values of various widths from 1 to 128 bytes.
There is no 1 byte floating point, but there is a 2 byte floating point, which requires additional libraries (e.g. OpenEXR) in 
order to parse. 

Every attribute descriptor maps to a unique string representation similar to a URN: 
    
    `g3d:<association>:<attribute_type>:<attribute_type_index>:<data_type>:<data_arity>`

This string representation makes it easier to define attribute descriptors and to understand them when debugging. 

## About Map Channels and Indirect Referencing 

In 3ds Max, FBX, and OBJ files it is possible to associate data with UVs, Normals, and Vertex Colors directly with face corners (aka polygon vertices)
instead of the more common approach of just associating it directly with vertices. 

In 3ds Max this is done using map channels, and in FBX it is done using "indirect referencing". This means that this data has an index buffer
which is the same length as the geometry index buffer, but is used for accessing the relevant data. 

In G3D data this can be accomplished by directly associating data with face corners. This has the disdavantage of causing the data to be potentially 
repeated but has the advantage of not requiring indirect memory addressing. The other option is to use a pair of map_channel_data and 
map_channel_index attributes. According to 3ds Max a map channel the map_channel_index is associated with corners, and consists of integers. The map_channel_data 
has no association and consists of triplets of single precision floating point values. 
    
## BFAST - Binary Format for Array Streaming and Transmission

The underlying binary format of the G3D file conforms to the [BFAST serialization format](https://github.com/ara3d/bfast), which is a simple and efficient binary
format for serializing collections of byte arrays. BFAST provides an interface that allows arrays of binary data to be serialized
and deserialized quickly and easily.

Each array in the BFAST container has the following purpose:
* Array 0: meta-information strng in JSON format
* Array 1: array of N attribute descriptors (each is 32 bytes)
* Array 2 to n + 2: an array for each attribute 

# FAQ

## Why not use a 3D Scene file like FBX, Collada, Alembic, USD, Assimp, or glTF?

Existing 3D scene file formats store more than just geometry and as a result make writing efficient and fully conformant serializers 
and deserializers a very daunting task. A conformant G3D parser is very easy to write in any language.

## Why not use a 3D geometry file like OBJ, PLY, or STL?

Many of the older geometry file formats are limited in the type of data attributes that can be stored, causing a loss of data when round-tripping. 
They also require additional data processing when serializing and deserializing to get the data to or from a format that 
most 3D tools and rendering engines require. The G3D format can accept natively most data from 3ds Max, FBX, Assimp, and other tools with 
minimal processing. 

# Recommended reading:

* http://assimp.sourceforge.net/lib_html/structai_mesh.html
* http://help.autodesk.com/view/FBX/2017/ENU/?guid=__files_GUID_5EDC0280_E000_4B0B_88DF_5D215A589D5E_htm
* https://help.autodesk.com/cloudhelp/2017/ENU/Max-SDK/cpp_ref/class_mesh.html
* https://help.autodesk.com/view/3DSMAX/2016/ENU/?guid=__files_GUID_CBBA20AD_F7D5_46BC_9F5E_5EDA109F9CF4_htm
* http://paulbourke.net/dataformats/
* http://paulbourke.net/dataformats/obj/
* http://paulbourke.net/dataformats/ply/
* http://paulbourke.net/dataformats/3ds/
* https://github.com/KhronosGroup/gltf
* http://help.autodesk.com/view/FBX/2017/ENU/?guid=__cpp_ref_class_fbx_layer_element_html
