using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace CapitalizerLib.Models
{
    public enum CapitalizableType { File, Folder };
    public enum CapitalizableStatus { Pending, Succes, Failed };

    public class CapitalizableItem : INotifyPropertyChanged
    {
        public string OldName { get; set; }

        private string newName;
        public string NewName 
        { 
            get 
            {
                if (newName != null)
                    return newName;
                else
                    return "(no change)";
            } 
            set
            {
                newName = value;
                OnPropertyChanged();
            } 
        }

        private string path;
        public string Path 
        {
            get { return path; }
            set 
            {
                path = value;
                OnPropertyChanged();
            }
        }

        public CapitalizableType Type { get; set; }

        private CapitalizableStatus status = CapitalizableStatus.Pending;
        public CapitalizableStatus Status 
        { 
            get { return status; } 
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Renames the file of this item to its new name
        /// </summary>
        public async Task Rename()
        {
            if (NewName != null)
            {
                var file = await StorageFile.GetFileFromPathAsync(Path);
                if (file.IsAvailable)
                {
                    await file.RenameAsync(NewName, NameCollisionOption.GenerateUniqueName);
                    Status = CapitalizableStatus.Succes;
                }
                else
                {
                    Status = CapitalizableStatus.Failed;
                    throw new Exception("Could not find item to rename");
                }

                Path = file.Path;
            }
        }

        /// <summary>
        /// Capitalizes the old filename and stores the result in the NewName property
        /// </summary>
        public void Capitalize(CapitalizeMode mode)
        {
            string newName = CapitalizeHelper.Capitalize(OldName, mode);
            if (newName != NewName)
            {
                NewName = newName;

                if (Status != CapitalizableStatus.Pending)
                {
                    Status = CapitalizableStatus.Pending;
                }
            }
            
            if (OldName == NewName)
            {
                NewName = null;
                Status = CapitalizableStatus.Succes;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
