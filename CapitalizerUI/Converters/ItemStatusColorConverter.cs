using CapitalizerLib.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace CapitalizerUI.Converters
{
    internal class ItemStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                CapitalizableStatus itemStatus = (CapitalizableStatus)value;

                if (itemStatus == CapitalizableStatus.Pending)
                {
                    return new SolidColorBrush(Colors.Gray);
                }
                else if (itemStatus == CapitalizableStatus.Succes)
                {
                    return new SolidColorBrush(Colors.DodgerBlue);
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
            catch
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
