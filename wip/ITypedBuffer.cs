using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vim
{
    // Interface for typed buffers 
    public interface ITypedBuffer : IBuffer
    {
        Type Type { get; }
    }

    // Typed buffer class 
    public class TypedBuffer<T> : ITypedBuffer
    {
        public TypedBuffer(IBuffer buffer)
            => Buffer = buffer;

        public Type Type => typeof(T);
        public IBuffer Buffer { get; }
        public Span<byte> Bytes => Buffer.Bytes;
    }

    // Typed buffer class 
    public class TypedBuffer : ITypedBuffer
    {
        public TypedBuffer(IBuffer buffer, Type type)
        {
            Buffer = buffer;
            Type = type;
        }

        public Type Type { get; }
        public IBuffer Buffer { get; }
        public Span<byte> Bytes => Buffer.Bytes;
    }

    public static class TypedBufferExtensions
    {
        public static Span<T> Cast<T>(this IBuffer b) where T: struct
            => MemoryMarshal.Cast<byte, T>(b.Bytes);

        public static Span<T> SafeCast<T>(this ITypedBuffer b) where T: struct
        {
            // TODO: math types should be smarter than that (e.g. Vector3 etc.) 
            // TODO: allow signed to/from unsigned casts 
            if (b.Type != typeof(T))
                throw new Exception($"Cannot safely cast from buffer of type {b.Type} to {typeof(T)}");
            return b.Cast<T>();
        }            
    }
}
