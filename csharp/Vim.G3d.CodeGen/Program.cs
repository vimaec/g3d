#if !NETSTANDARD
namespace Vim.G3d.CodeGen
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var file = args[0];
            G3dAttributeCollectionGenerator.WriteDocument(file);
        }
    }
}
#endif