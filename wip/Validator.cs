using System.Collections.Generic;

namespace Vim.G3D
{
    /// <summary>
    /// Validates a G3D object (attribute set) using an optional schema 
    /// </summary>
    public class Validator
    {
        public Validator(G3D g3d, Schema schema)
        {
            foreach (var attribute in g3d.)
        }

        public List<string> Warnings = new List<string>();
        public List<string> Errors = new List<string>();
        public bool Success => Errors.Count == 0;
        public bool StrictSuccess => Success && Warnings.Count == 0;
    }
}
