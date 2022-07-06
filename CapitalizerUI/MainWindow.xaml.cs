using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage.Pickers;
using CapitalizerLib.Models;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace CapitalizerUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        ObservableCollection<CapitalizableItem> CapitalizableItems { get; set; } = 
            new ObservableCollection<CapitalizableItem>();

        public MainWindow()
        {
            this.InitializeComponent();
            Title = Package.Current.DisplayName;
            SetIcon();
            capitalizeItemsDataGrid.ItemsSource = CapitalizableItems;
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
                var items = CapitalizerLib.ItemHelper.FilesToItems(files);
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
    }
}
