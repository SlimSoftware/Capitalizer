using CapitalizerLib.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace CapitalizerLib
{
    public static class ItemHelper
    {
        public static List<CapitalizableItem> FilesToItems(IReadOnlyList<StorageFile> files, CapitalizeMode mode)
        {
            var items = new List<CapitalizableItem>();

            foreach (var file in files)
            {
                var item = new CapitalizableItem()
                {
                    OldName = file.Name,
                    NewName = CapitalizeHelper.Capitalize(file.Name, mode),
                    Path = file.Path,
                    Type = CapitalizableType.File
                };

                items.Add(item);
            }

            return items;
        }

        public async static Task<List<CapitalizableItem>> FolderToItemsAsync(StorageFolder folder, CapitalizeMode mode)
        {
            var files = await folder.GetFilesAsync();
            var items = FilesToItems(files, mode);
            return items;
        }
    }
}
