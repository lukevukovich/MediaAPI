namespace MediaAPI.Models.MdbList;

public class MdbList
{
    public string Name { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public int Results => Items.Count;
    public List<MdbItem> Items { get; set; } = [];
}