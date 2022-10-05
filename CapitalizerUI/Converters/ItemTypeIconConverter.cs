using CapitalizerLib.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace CapitalizerUI.Converters
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
                    return Symbol.Folder;
                }
                else
                {
                    return Symbol.Page;
                }
            }
            catch
            {
                return Symbol.Page;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Symbol symbol = (Symbol)value;

            if (symbol == Symbol.Folder)
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
