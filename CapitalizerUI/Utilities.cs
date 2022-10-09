using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace CapitalizerUI
{
    static internal class Utilities
    {
        /// <summary>
        /// Shows a InfoBar with the given message
        /// </summary>
        /// <param name="infoBar">The InfoBar instance to show</param>
        /// <param name="message">The message that will appear on the InfoBar</param>
        public static void ShowInfoBar(InfoBar infoBar, string message)
        {
            infoBar.Message = message;
            infoBar.IsOpen = true;
            infoBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the given InfoBar if it is currently visible
        /// </summary>
        public static void HideInfoBar(InfoBar infoBar)
        {
            if (infoBar.Visibility == Visibility.Visible)
            {
                infoBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
