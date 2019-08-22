using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace Vim.G3d.Tests
{
    public static class Tests
    {
        public static string InputDataPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "..", // yes 5, count em, 5  
            "data", "assimp", "test");

        [Test]
        public static void TestAssimp()
        {
            Console.WriteLine(InputDataPath);
            var file = Path.Combine(InputDataPath, "models", "STL", "wuson.stl");
            using (var context = new Assimp.AssimpContext())
            {
                var scene = context.ImportFile(file);
            }
        }
    }
}
