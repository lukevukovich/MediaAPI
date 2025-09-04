namespace MdbListApi.Models
{
    public class MdbList
    {
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public List<MdbItem> Items { get; set; } = [];
    }
}