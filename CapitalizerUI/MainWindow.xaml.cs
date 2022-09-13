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

        private void AddFiles(IReadOnlyList<StorageFile> storageFiles)
        {
            if (storageFiles != null)
            {
                var items = ItemHelper.FilesToItems(storageFiles, SelectedCapitalizeMode);
                foreach (var item in items)
                {
                    CapitalizableItems.Add(item);
                }
            }
        }
        private async Task AddFolderAsync(StorageFolder folder)
        {
            if (folder != null)
            {
                var items = await ItemHelper.FolderToItemsAsync(folder, SelectedCapitalizeMode);
                foreach (var item in items)
                {
                    CapitalizableItems.Add(item);
                }
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

            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Title = "Add folder or contents?";
            dialog.PrimaryButtonText = "Add folder";
            dialog.SecondaryButtonText = "Add file contents";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "Do you want to add the folder itself or add the files that the folder contains?";

            var folder = await picker.PickSingleFolderAsync();
            var result = await dialog.ShowAsync();
            
            if (folder != null)
            {
                if (result == ContentDialogResult.Primary)
                {
                    var folderItem = ItemHelper.FolderToItem(folder, SelectedCapitalizeMode);
                    CapitalizableItems.Add(folderItem);
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    var fileItems = await ItemHelper.FolderToItemsAsync(folder, SelectedCapitalizeMode);
                    foreach (var item in fileItems)
                    {
                        CapitalizableItems.Add(item);
                    }
                }
            }
        }

        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = capitalizeItemsDataGrid.SelectedItems;
            for (int i = 0; i < selectedItems.Count; i++)
            {
                CapitalizableItems.RemoveAt(i);
            }
        }

        private void DeleteAllAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            CapitalizableItems.Clear();
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

            foreach (CapitalizableItem item in CapitalizableItems)
            {
                item.Capitalize(SelectedCapitalizeMode);
            }
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
                    item.RenameFailed = true;
                    failedRenameCount++;
                }
            }

            if (failedRenameCount == 0)
            {
                if (renameFailedInfoBar.Visibility == Visibility.Visible)
                {
                    renameFailedInfoBar.Visibility = Visibility.Collapsed;
                }

                renameSuccesInfoBar.Message = $"Succesfully renamed {CapitalizableItems.Count} file(s).";
                renameSuccesInfoBar.IsOpen = true;
                renameSuccesInfoBar.Visibility = Visibility.Visible;
            }
            else
            {
                if (renameSuccesInfoBar.Visibility == Visibility.Visible)
                {
                    renameSuccesInfoBar.Visibility = Visibility.Collapsed;
                }

                renameFailedInfoBar.Message = $"Failed to rename {failedRenameCount} file(s). They have been marked in the list. " +
                    $"Please check if these files still exist and if they are writeable.";
                renameFailedInfoBar.IsOpen = true;
                renameFailedInfoBar.Visibility = Visibility.Visible;
            }
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
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
    }
}
