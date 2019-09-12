namespace Vim.G3d
{
    public class Header
    {
        public string G3DVersion;
        public string FileName;
        public string Description;
        public string Copyright;
        public string Author;
        public string Units;
        public string Axis;
        public string DateCreated;
        public string Geolocation;
        public string Generator;
        public string Elevation;
        public string BoundingBox;
        public string Origin;

        public Header(string data = null)
        {
            // TODO: parse the header data. 
        }

        public override string ToString() =>
            $@"{{
                ""G3D"": ""{G3DVersion}"",
                ""fileName"": ""{FileName}"",
                ""description"": ""{Description}"",
                ""copyright"": ""{Copyright}"",
                ""author"": ""{Author}"",
                ""dateCreated"": ""{DateCreated}"",
                ""units"": ""{Units}"", 
                ""axis"": ""{Axis}"",
                ""geolocation"": ""{Geolocation}"",
                ""elevation"": ""{Elevation}"", 
                ""boundingbox"": ""{BoundingBox}"", 
                ""origin"": ""{Origin}"",
                ""generator"": ""vim"", 
                }}    
            }}";
    }
}
