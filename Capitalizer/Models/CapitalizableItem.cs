namespace Capitalizer.Models
{
    enum CapitalizableType { File, Folder }

    internal class CapitalizableItem
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public string Path { get; set; }
        public CapitalizableType Type { get; set; }
    }
}
