﻿using CapitalizerLib.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace CapitalizerUI.Converters
{
    internal class ItemTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                CapitalizableType itemType = (CapitalizableType)value;

                if (itemType == CapitalizableType.Folder)
                {
                    return new SolidColorBrush(Colors.Goldenrod);
                }
                else
                {
                    return new SolidColorBrush(Colors.Gray);
                }
            }
            catch
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
