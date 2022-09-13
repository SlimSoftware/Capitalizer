using CapitalizerLib.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace CapitalizerLib
{
    public static class ItemHelper
    {
        /// <summary>
        /// Gets the CapitalizableItems from the given StorageFiles
        /// </summary>
        public static List<CapitalizableItem> FilesToItems(IReadOnlyList<StorageFile> files, CapitalizeMode mode)
        {
            var items = new List<CapitalizableItem>();

            foreach (var file in files)
            {
                var item = new CapitalizableItem()
                {
                    OldName = file.Name,
                    Path = file.Path,
                    Type = CapitalizableType.File
                };

                item.Capitalize(mode);
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Get the CapitalizableItems from a given StorageFolder.
        /// </summary>
        public async static Task<List<CapitalizableItem>> FolderToItemsAsync(StorageFolder folder, CapitalizeMode mode)
        {
            var files = await folder.GetFilesAsync();
            var items = FilesToItems(files, mode);
            return items;
        }

        /// <summary>
        /// Gets the CapitalizableItems from the given StorageFolder
        /// </summary>
        public static CapitalizableItem FolderToItem(StorageFolder folder, CapitalizeMode mode)
        {
            var item = new CapitalizableItem()
            {
                OldName = folder.Name,
                Path = folder.Path,
                Type = CapitalizableType.Folder
            };
            item.Capitalize(mode);

            return item;
        }
    }
}
