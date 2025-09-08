namespace MediaAPI.Models.Stremio
{
    public class Extra
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Options { get; set; } = [];
        public bool IsRequired { get; set; }
    }
}