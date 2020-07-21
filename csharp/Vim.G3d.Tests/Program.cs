using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vim.G3d.Tests;

namespace Vim.G3d.CppCLR.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Running big file test");
            G3dTests.BigFileTest();
            Console.WriteLine("Finished running big file test");
        }
    }
}
