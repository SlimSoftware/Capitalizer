using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

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

        private bool renameFailed = false;
        public bool RenameFailed 
        { 
            get { return renameFailed; } 
            set
            {
                renameFailed = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Renames the file of this item to its new name
        /// </summary>
        public async Task Rename()
        {
            var file = await StorageFile.GetFileFromPathAsync(Path);
            if (file.IsAvailable)
            {
                await file.RenameAsync(NewName, NameCollisionOption.FailIfExists);
            }
            else
            {
                throw new Exception("Could not find to rename");
            }

            Path = file.Path;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
