using NUnit.Framework;
using System;
using System.IO;

namespace Vim.G3d.Tests
{
    public static class Tests
    {
        public static string InputDataPath = Path.Combine(TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "..",
            "data", "assimp", "test");

        [Test]
        public static void TestAssimp()
        {
            var file = Path.Combine(InputDataPath, "models", "STL", "wuson.stl");
            using (var context = new Assimp.AssimpContext())
            {
                context.ImportFile(file);
            }
        }

        public static void Main()
        {
            TestAssimp();
            Console.WriteLine("Test");
        }
    }
}
