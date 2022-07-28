using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CapitalizerLib.Models
{
    public enum CapitalizableType { File, Folder }

    public class CapitalizableItem : INotifyPropertyChanged
    {
        public string OldName { get; set; }

        private string newName;
        public string NewName 
        { 
            get { return newName; } 
            set
            {
                newName = value;
                OnPropertyChanged();
            } 
        }

        public string Path { get; set; }
        public CapitalizableType Type { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
