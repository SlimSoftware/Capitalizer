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
using Microsoft.UI.Xaml.Controls.Primitives;

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

        private AppWindow GetAppWindow()
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            return AppWindow.GetFromWindowId(windowId);
        }

        private void SetAlwaysOnTop(bool value)
        {
            var appWindow = GetAppWindow();
            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsAlwaysOnTop = value;         
        }

        /// <summary>
        /// Sets the window icon to be the Capitalizer icon
        /// </summary>
        private void SetIcon()
        {
            var appWindow = GetAppWindow();
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

        /// <summary>
        /// Adds the given StorageFiles to the CapitalizableItems list
        /// </summary>
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
                    ShowAddedItemsInfoBar(CapitalizableType.File, items.Count);
                }
                else
                {
                    Utilities.HideInfoBar(succesInfoBar);

                    if (storageFiles.Count > 1)
                    {                      
                        Utilities.ShowInfoBar(errorInfoBar, "These files are already added.");
                    } 
                    else
                    {
                        Utilities.ShowInfoBar(errorInfoBar, "This file is already added.");
                    }               
                }       
            }
        }

        /// <summary>
        /// Adds a given StorageFolder as items to the CapitalizableItems list.
        /// </summary>
        /// <param name="folder">The StorageFolder to add</param>
        /// <param name="addMethodChoice">The add method of the folder (add folder itself or its contents)</param>
        /// <param name="showAddedInfoBar">Whether or not to show an InfoBar message that the items have been added</param>
        /// <returns></returns>
        private async Task AddFolderAsync(StorageFolder folder, AddFolderMethod? addMethod = null, bool showAddedInfoBar = true)
        {
            if (folder != null)
            {
                ContentDialogResult? addMethodChoice = null;

                if (addMethod == null && Settings.AddFolderMethod == null)
                {
                    addMethodChoice = await addFolderMethodChoiceDialog.ShowAsync();

                    if (addMethodChoice == ContentDialogResult.Primary)
                    {
                        addMethod = AddFolderMethod.AddFolder;
                    }
                    else if (addMethodChoice == ContentDialogResult.Secondary)
                    {
                        addMethod = AddFolderMethod.AddContents;
                    }
                    else
                    {
                        return;
                    }
                }

                if (addMethod == AddFolderMethod.AddFolder || Settings.AddFolderMethod == AddFolderMethod.AddFolder)
                {
                    var folderItem = ItemHelper.FolderToItem(folder, SelectedCapitalizeMode);

                    if (!IsItemAlreadyAdded(folder.Path))
                    {
                        CapitalizableItems.Add(folderItem);
                    }

                    if (showAddedInfoBar)
                    {
                        ShowAddedItemsInfoBar(CapitalizableType.Folder, 1);
                    }
                }
                else if (addMethod == AddFolderMethod.AddContents || Settings.AddFolderMethod == AddFolderMethod.AddContents)
                {
                    var fileItems = await ItemHelper.FolderToItemsAsync(folder, SelectedCapitalizeMode);

                    foreach (var item in fileItems)
                    {
                        if (!IsItemAlreadyAdded(item.Path))
                        {
                            CapitalizableItems.Add(item);
                        }
                    }

                    if (showAddedInfoBar)
                    {
                        ShowAddedItemsInfoBar(CapitalizableType.File, fileItems.Count);
                    }
                }

                SaveAddFolderMethod(addMethodChoice);                    
                SortItems();
            }
        }

        /// <summary>
        /// Saves the add folder method choice to the settings if the user chooses to do so
        /// </summary>
        void SaveAddFolderMethod(ContentDialogResult? addMethodChoice)
        {
            if (rememberAddFolderMethodCheckBox.IsChecked == true)
            {
                if (addMethodChoice == ContentDialogResult.Primary)
                {
                    Settings.AddFolderMethod = AddFolderMethod.AddFolder;
                }
                else if (addMethodChoice == ContentDialogResult.Secondary)
                {
                    Settings.AddFolderMethod = AddFolderMethod.AddContents;
                }
            }
        }

        /// <summary>
        /// Shows a InfoBar with a message how many items have been added to the list.
        /// </summary>
        /// <param name="capitalizableType">The type of the items added</param>
        /// <param name="itemCount">The number of items added</param>
        private void ShowAddedItemsInfoBar(CapitalizableType capitalizableType, int itemCount)
        {
            Utilities.HideInfoBar(errorInfoBar);

            if (capitalizableType == CapitalizableType.File)
            {
                if (itemCount > 1)
                {
                    Utilities.ShowInfoBar(succesInfoBar, $"Added {itemCount} files.");
                } 
                else if (itemCount == 1)
                {
                    Utilities.ShowInfoBar(succesInfoBar, "Added 1 file.");
                }
                else
                {
                    Utilities.ShowInfoBar(errorInfoBar, "Added no files.\nWhen adding a folder, make sure it contains files.");
                }
            }
            else
            {
                if (itemCount > 1)
                {
                    Utilities.ShowInfoBar(succesInfoBar, $"Added {itemCount} folders.");
                }
                else if (itemCount == 1)
                {
                    Utilities.ShowInfoBar(succesInfoBar, "Added 1 folder.");
                }
                else
                {
                    Utilities.ShowInfoBar(errorInfoBar, "Could not add folder(s)");
                }
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
            Utilities.HideInfoBar(errorInfoBar);
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
                Utilities.HideInfoBar(errorInfoBar);
                Utilities.ShowInfoBar(succesInfoBar, $"Succesfully renamed {CapitalizableItems.Count} items(s).");
            }
            else
            {
                Utilities.HideInfoBar(succesInfoBar);
                Utilities.ShowInfoBar(errorInfoBar, $"Failed to rename {failedRenameCount} items(s). They have been marked in the list. " +
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
                        AddFolderMethod? addFolderMethod = Settings.AddFolderMethod;
                        if (addFolderMethod == null)
                        {
                            var addMethodChoice = await addFolderMethodChoiceDialog.ShowAsync();
                            if (addMethodChoice == ContentDialogResult.Primary)
                            {
                                addFolderMethod = AddFolderMethod.AddFolder;
                            }
                            else if (addMethodChoice == ContentDialogResult.Secondary)
                            {
                                addFolderMethod = AddFolderMethod.AddContents;
                            }
                            else
                            {
                                return;
                            }

                            SaveAddFolderMethod(addMethodChoice);
                        }
                        
                        int oldItemCount = CapitalizableItems.Count;          

                        foreach (StorageFolder folder in folders)
                        {
                            await AddFolderAsync(folder, addFolderMethod, false);
                        }

                        int addedItemCount = CapitalizableItems.Count - oldItemCount;
                        if (addFolderMethod == AddFolderMethod.AddFolder)
                        {
                            ShowAddedItemsInfoBar(CapitalizableType.Folder, addedItemCount);
                        }
                        else
                        {
                            ShowAddedItemsInfoBar(CapitalizableType.File, addedItemCount);                        
                        }           
                    }
                }
            }
        }

        private async void AboutAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string version = CapitalizerLib.Utilities.GetFriendlyVersion(Package.Current.Id.Version);
            versionText.Text = $"{Package.Current.DisplayName} v{version}";

            await aboutDialog.ShowAsync();
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

        private void AlwaysOnTopToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButton = (ToggleButton)sender;
            SetAlwaysOnTop(toggleButton.IsChecked == true);
        }
    }
}
