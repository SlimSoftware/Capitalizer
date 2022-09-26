using CapitalizerLib.Models;
using Microsoft.UI.Xaml.Data;
using System;

namespace CapitalizerUI
{
    internal class ItemTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                CapitalizableType itemType = (CapitalizableType)value;

                if (itemType == CapitalizableType.Folder)
                {
                    return "\uF12B";
                }
                else
                {
                    return "\uE7C3";
                }
            } 
            catch
            {
                return "\uE7C3";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string iconGlyph = (string)value;

            if (iconGlyph == "\uF12B")
            {
                return CapitalizableType.Folder;
            } 
            else
            {
                return CapitalizableType.File;
            }
        }
    }
}
