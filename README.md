# G3d

[<img src="https://img.shields.io/nuget/v/Vim.G3d.svg">](https://www.nuget.org/packages/Vim.G3d)

**G3d** is a simple, efficient, generic binary format for storing and transmitting geometry. The G3d format
is designed to be used either as a serialization format or as an in-memory data structure.

G3d is maintained by [VIMaec LLC](https://vimaec.com) and is licensed under the terms of the [MIT License](LICENSE.txt).

# G3d Format 

## BFAST Container

The underlying binary layout of a G3d file conforms to the [BFAST serialization format](https://github.com/vimaec/bfast), which is a simple and efficient binary format for serializing collections of byte arrays. BFAST provides an interface that allows named arrays of binary data to be serialized and deserialized quickly and easily.

## Meta Header

The first bfast buffer of a G3d contains a struct called the [MetaHeader](csharp/Vim.G3d/MetaHeader.cs).

## Attributes

A G3d contains a structured collection of attributes. Examples of attributes include mesh vertices, mesh indices, instance transforms, instanced meshes, mesh submeshes, materials, etc.

Every [Attribute](csharp/Vim.G3d/IAttribute.cs) in a G3d has an [AttributeDescriptor](csharp/Vim.G3d/IAttributeDescriptor.cs) which uniquely identifies it and describes it.

```
g3d:<association>:<semantic>:<index>:<data_type>:<data_arity>
```

- `<association>`: Designates the object to which the attribute is conceptually associated.
- `<semantic>`: Designates the meaning of the data.
- `<index>`: Designates the index in case the same `<association>:<semantic>` combination occurs more than once among the collection of attribute descriptors.
- `<data_type>`: Designates the data type of the values contained in the buffer. Possible values are:
    - `int8`
    - `int16`
    - `int32`
    - `int64`
    - `uint8`
    - `uint16`
    - `uint32`
    - `uint64`
    - `float32`
    - `float64`
- `<data_arity>`: Designates the number of values which compose one semantic element.

## VIM File G3d Attributes

A [VIM file](https://github.com/vimaec/vim) contains a particular collection of G3d attributes which define its geometric scene:

* `g3d:all:facesize:0:int32:1`
  A single 32-bit integer value, usually "3", indicating that all mesh faces are triangles and are composed of 3 corners.

* `g3d:vertex:position:0:float32:3`
  An array of 32-bit single-precision floating point values, arranged in slices of 3 to represent the (X, Y, Z) vertices of all the meshes in the VIM. We refer to this as the "vertex buffer".

* `g3d:corner:index:0:int32:1`
  An array of 32-bit integers representing the combined index buffer of all the meshes in the VIM. The values in this index buffer are relative to the beginning of the vertex buffer. Meshes in a VIM are composed of triangular faces, whose corners are defined by 3 indices.

* `g3d:submesh:indexoffset:0:int32:1`
  An array of 32-bit integers representing the index offset of the index buffer of a given submesh.

* `g3d:submesh:material:0:int32:1`
  An array of 32-bit integers representing the index of the material associated with a given submesh.

* `g3d:mesh:submeshoffset:0:int32:1`
  An array of 32-bit integers representing the index offset of a submesh in a given mesh.

* `g3d:material:color:0:float32:4`
  An array of 32-bit single-precision floating point values in the domain [0.0f, 1.0f], arranged in slices of 4 to represent the (R, G, B, A) diffuse color of a given material.

* `g3d:material:glossiness:0:float32:1`
  An array of 32-bit single-precision floating point values in the domain [0.0f, 1.0f] representing the glossiness of a given material.

* `g3d:material:smoothness:0:float32:1`
  An array of 32-bit single-precision floating point values in the domain [0.0f, 1.0f] representing the smoothness of a given material.

* `g3d:instance:transform:0:float32:16`
  An array of 32-bit single-precision floating point values, arranged in slices of 16 to represent the 4x4 row-major transformation matrix associated with a given instance.

* `g3d:instance:flags:0:uint16:1`
  (Optional) An array of 16-bit unsigned integers representing the flags of a given instance. The first bit of each flag designates whether the instance should be initially hidden (1) or not (0) when rendered.

* `g3d:instance:parent:0:int32:1`
  An array of 32-bit integers representing the index of the parent instance associated with a given instance.

* `g3d:instance:mesh:0:int32:1`
  An array of 32-bit integers representing the index of a mesh associated with a given instance.

* `g3d:shapevertex:position:0:float32:3`
  (Optional) An array of 32-bit single-precision floating point values, arranged in slices of 3 to represent the (X, Y, Z) positions of all the world-space shapes in the VIM. We refer to this as the "shape vertex buffer"

* `g3d:shape:vertexoffset:0:int32:1`
  (Optional) An array of 32-bit integers representing the index offset of the vertices in a given shape.

* `g3d:shape:color:0:float32:4`
  (Optional) An array of 32-bit single-precision floating point values, arranged in slices of 4 to represent the (R, G, B, A) color of a given shape.

* `g3d:shape:width:0:float32:1`
  (Optional) An array of 32-bit single-precision floating-point values represents a given shape's width.

Additional attributes are possible but are ignored and may or may not be written out by any tool that inputs and outputs VIM files.

Conceptually, the geometric objects in a VIM file are related in the following manner:

- **Instance**:
  - Has a 4x4 row-major matrix representing its world-space transform.
  - Has a set of **Flag**s.
  - May have a parent **Instance**.
  - May have a **Mesh**.
- **Mesh**
  - Is composed of 0 or more **Submesh**es.
- **Submesh**
  - Has a **Material**
  - References a slice of values in the index buffer to define the geometry of its triangular faces in local space.
- **Material**
  - Has a glossiness value in the domain [0f, 1f].
  - Has a smoothness value in the domain [0f, 1f].
  - Has an RGBA diffuse color whose components are in the domain [0f, 1f].
- **Shape**
  - Has an RGBA color whose components are in the domain [0f, 1f].
  - Has a width.
  - References a slice of vertices in the shape vertex buffer to define the sequence of world-space vertices which compose its linear segments.


## C# Project Structure

- [G3d.sln](csharp/G3d.sln) - Visual Studio C# G3d solution
- [Vim.G3d.csproj](csharp/Vim.G3d/Vim.G3d.csproj) - Basic building blocks to read and write G3d objects.
- [Vim.G3d.Attributes.csproj](csharp/Vim.G3d.Attributes/Vim.G3d.Attributes.csproj) - Defines the structured attribute collection of G3ds found inside VIM files (see [Attributes.cs](csharp/Vim.G3d.Attributes/Attributes.cs) and the code-generated sibling file [Attributes.g.cs](csharp/Vim.G3d.Attributes/Attributes.g.cs))
- [Vim.G3d.CodeGen](csharp/Vim.G3d.CodeGen/Vim.G3d.CodeGen.csproj) - The code-generator which emits [Attributes.g.cs](csharp/Vim.G3d.Attributes/Attributes.g.cs).
- [Vim.G3d.Tests](csharp/Vim.G3d.Tests/Vim.G3d.Tests.csproj) - An NUnit project which validates the C# G3d implementation.
- [Vim.G3d.CppCLR.Tests](csharp/Vim.G3d.CppCLR.Tests/Vim.G3d.CppCLR.Tests.csproj) - An NUnit project which validates the C++ G3d implementation. Must be executed after Vim.G3d.Tests is run; it uses g3d files generated from the C# unit tests.

## C++ Project Structure

The C++ implementation is still a work in progress.
