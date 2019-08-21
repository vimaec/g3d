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
            var file = Path.Combine(InputDataPath, "models-nonbsd", "OBJ", "rifle.obj");
            using (var context = new Assimp.AssimpContext())
            {
                context.ImportFile(file);
            }
        }
    }
}
