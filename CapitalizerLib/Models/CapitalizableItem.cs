namespace CapitalizerLib.Models
{
    public enum CapitalizableType { File, Folder }

    public class CapitalizableItem
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public string Path { get; set; }
        public CapitalizableType Type { get; set; }
    }
}
