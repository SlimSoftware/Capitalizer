using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage.Pickers;
using CapitalizerLib.Models;
using CapitalizerLib;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.Generic;
using Windows.Storage;
using System.Threading.Tasks;
using System.Linq;

namespace CapitalizerUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<CapitalizableItem> CapitalizableItems { get; set; } = 
            new ObservableCollection<CapitalizableItem>();

        public CapitalizeMode SelectedCapitalizeMode = CapitalizeMode.EveryWord;

        public MainWindow()
        {
            this.InitializeComponent();
            Title = Package.Current.DisplayName;
            SetIcon();
            capitalizeItemsDataGrid.ItemsSource = CapitalizableItems;

            findStringTextBox.Text = Settings.FindString;
            replaceWithStringTextBox.Text = Settings.ReplaceWithString;
        }

        /// <summary>
        /// Sets the window icon to be the Capitalizer icon
        /// </summary>
        private void SetIcon()
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets\\capitalizer.ico"));
        }

        /// <summary>
        /// Sorts the current CapitalizableItems in the DataGrid
        /// </summary>
        private void SortItems()
        {
            CapitalizableItems = new ObservableCollection<CapitalizableItem>(ItemHelper.SortItems(CapitalizableItems));

            if (capitalizeItemsDataGrid?.ItemsSource != null)
            {
                capitalizeItemsDataGrid.ItemsSource = CapitalizableItems;
            }
        }

        /// <summary>
        /// Returns true if a CapitalizableItem with the given path is already added to the CapitalizableItems list
        /// </summary>
        private bool IsItemAlreadyAdded(string itemPath)
        {
            if (CapitalizableItems.FirstOrDefault(i => i.Path == itemPath) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a list that does not contain StorageItems that have already been added to the CapitalizableItems list
        /// </summary>
        private IReadOnlyList<T> FilterExistingItems<T>(IReadOnlyList<T> storageItems) where T : IStorageItem
        {
            var filteredItems = storageItems.ToList();

            foreach (T item in storageItems)
            {
                if (IsItemAlreadyAdded(item.Path)) 
                {
                    filteredItems.Remove(item);
                }
            }

            return filteredItems.AsReadOnly();
        }

        private void AddFiles(IReadOnlyList<StorageFile> storageFiles)
        {
            if (storageFiles != null)
            {
                var filteredStorageFiles = FilterExistingItems<StorageFile>(storageFiles);
                var items = ItemHelper.FilesToItems(filteredStorageFiles, SelectedCapitalizeMode);

                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        CapitalizableItems.Add(item);
                    }

                    SortItems();

                    HideInfoBar(errorInfoBar);
                    ShowInfoBar(succesInfoBar, $"Added {items.Count} file(s).");
                }
                else
                {
                    HideInfoBar(succesInfoBar);

                    if (storageFiles.Count > 1)
                    {                      
                        ShowInfoBar(errorInfoBar, "These files are already added.");
                    } 
                    else
                    {
                        ShowInfoBar(errorInfoBar, "This file is already added.");
                    }               
                }       
            }
        }

        private async Task AddFolderAsync(StorageFolder folder)
        {
            if (folder != null)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.XamlRoot = this.Content.XamlRoot;
                dialog.Title = "Add folder or contents?";
                dialog.PrimaryButtonText = "Add folder";
                dialog.SecondaryButtonText = "Add folder contents";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = "Do you want to add the folder itself or the files that it contains?";          
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    var folderItem = ItemHelper.FolderToItem(folder, SelectedCapitalizeMode);

                    if (!IsItemAlreadyAdded(folder.Path))
                    {
                        CapitalizableItems.Add(folderItem);
                    }

                    HideInfoBar(errorInfoBar);
                    ShowInfoBar(succesInfoBar, $"Added 1 folder.");
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    var fileItems = await ItemHelper.FolderToItemsAsync(folder, SelectedCapitalizeMode);

                    foreach (var item in fileItems)
                    {
                        if (!IsItemAlreadyAdded(item.Path))
                        {
                            CapitalizableItems.Add(item);
                        }
                    }

                    HideInfoBar(errorInfoBar);
                    ShowInfoBar(succesInfoBar, $"Added {fileItems.Count} file(s).");
                }

                SortItems();
            }
        }

        private void CapitalizeAllItems()
        {
            foreach (CapitalizableItem item in CapitalizableItems)
            {
                item.Capitalize(SelectedCapitalizeMode);
            }

            SortItems();
        }

        /// <summary>
        /// Shows a InfoBar with the given message
        /// </summary>
        /// <param name="infoBar">The InfoBar instance to show</param>
        /// <param name="message">The message that will appear on the InfoBar</param>
        private void ShowInfoBar(InfoBar infoBar, string message)
        {
            infoBar.Message = message;
            infoBar.IsOpen = true;
            infoBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the given InfoBar if it is currently visible
        /// </summary>
        private void HideInfoBar(InfoBar infoBar)
        {
            if (infoBar.Visibility == Visibility.Visible)
            {
                infoBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void AddFilesAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            var files = await picker.PickMultipleFilesAsync();
            AddFiles(files);
        }

        private async void AddFolderAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            var folder = await picker.PickSingleFolderAsync();
            await AddFolderAsync(folder);
        }           

        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = capitalizeItemsDataGrid.SelectedItems;
            for (int i = 0; i < selectedItems.Count; i++)
            {
                var selectedItem = (CapitalizableItem)selectedItems[i];
                CapitalizableItems.Remove(selectedItem);
            }
        }

        private void DeleteAllAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            CapitalizableItems.Clear();
            capitalizeItemsDataGrid.ItemsSource = CapitalizableItems;
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCapitalizeMode = (CapitalizeMode)modeComboBox.SelectedIndex;

            if (findReplacePanel != null)
            {
                if (SelectedCapitalizeMode == CapitalizeMode.FindReplace)
                {
                    findReplacePanel.Visibility = Visibility.Visible;
                }
                else
                {
                    findReplacePanel.Visibility = Visibility.Collapsed;
                }
            }

            CapitalizeAllItems();
        }

        private async void RenameAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            int failedRenameCount = 0;

            foreach (CapitalizableItem item in CapitalizableItems)
            {
                try
                {
                    await item.Rename();
                    
                } 
                catch
                {
                    item.Status = CapitalizableStatus.Failed;
                    failedRenameCount++;
                }
            }

            if (failedRenameCount == 0)
            {
                HideInfoBar(errorInfoBar);
                ShowInfoBar(succesInfoBar, $"Succesfully renamed {CapitalizableItems.Count} items(s).");
            }
            else
            {
                HideInfoBar(succesInfoBar);
                ShowInfoBar(errorInfoBar, $"Failed to rename {failedRenameCount} items(s). They have been marked in the list. " +
                    $"Please check if these still exist in this location and if they are writeable.");
            }
        }

        private void MainGrid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void MainGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    List<StorageFile> files = new();
                    List<StorageFolder> folders = new();

                    foreach (var item in items)
                    {
                        if (item is StorageFile)
                        {
                            files.Add(item as StorageFile);
                        }
                        if (item is StorageFolder)
                        {
                            folders.Add(item as StorageFolder);
                        }
                    }

                    if (files.Count > 0)
                    {
                        AddFiles(files.AsReadOnly());
                    }
                    if (folders.Count > 0)
                    {
                        foreach (StorageFolder folder in folders)
                        {
                            await AddFolderAsync(folder);
                        }
                    }
                }
            }
        }

        private async void AboutAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string version = Utilities.GetFriendlyVersion(Package.Current.Id.Version);
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Title = "About";
            dialog.Content = $"Capitalizer v{version}";
            dialog.CloseButtonText = "Close";
            dialog.DefaultButton = ContentDialogButton.Close;
            await dialog.ShowAsync();
        }

        private void FindStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string newText = (sender as TextBox).Text;
            Settings.FindString = newText;
            CapitalizeAllItems();
        }

        private void ReplaceWithStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        { 
            string newText = (sender as TextBox).Text;
            Settings.ReplaceWithString = newText;
            CapitalizeAllItems();
        }
    }
}
