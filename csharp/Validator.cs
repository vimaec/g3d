using System.Collections.Generic;

namespace Vim.G3D
{
    /// <summary>
    /// Validates a G3D object (attribute set) using an optional schema 
    /// </summary>
    public class Validator
    {
        public Validator(G3D g3d, Schema schema = null)
        {
            // TODO: run through and check for absolute errors or warnings
            // Missing channels? Extra channels? Indexed channels that are not monotonically increasing?
            // Channel counts that don't add up? 
            // No position channel.
            // Certain semantics that have unexpected types or associations. 
            // Unrecognized semantic 
        }

        public List<string> Warnings = new List<string>();
        public List<string> Errors = new List<string>();
        public bool Success => Errors.Count == 0;
        public bool StrictSuccess => Success && Warnings.Count == 0;
    }
}
