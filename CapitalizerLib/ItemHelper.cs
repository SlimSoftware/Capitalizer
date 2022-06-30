using CapitalizerLib.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace CapitalizerLib
{
    public class ItemHelper
    {
        public static List<CapitalizableItem> FilesToItems(IReadOnlyList<StorageFile> files)
        {
            var items = new List<CapitalizableItem>();

            foreach (var file in files)
            {
                var item = new CapitalizableItem()
                {
                    OldName = file.Name,
                    NewName = ProcessFileName(file.Name),
                    Path = file.Path,
                    Type = CapitalizableType.File
                };

                items.Add(item);
            }

            return items;
        }

        private static string ProcessFileName(string fileName)
        {
            string newName = fileName.ToLower();

            if (newName != fileName)
            {
                return newName;
            }
            else
            {
                return null;
            }
        }
    }
}
