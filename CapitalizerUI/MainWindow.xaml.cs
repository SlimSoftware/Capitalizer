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

        private async void AddFilesAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            var files = await picker.PickMultipleFilesAsync();
            if (files != null)
            {
                var items = ItemHelper.FilesToItems(files, SelectedCapitalizeMode);
                foreach (var item in items)
                {
                    CapitalizableItems.Add(item);
                }
            }
        }

        private async void AddFolderAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                var items = await ItemHelper.FolderToItemsAsync(folder, SelectedCapitalizeMode);
                foreach (var item in items)
                {
                    CapitalizableItems.Add(item);
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
    }
}
