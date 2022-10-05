using CapitalizerLib.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace CapitalizerUI.Converters
{
    internal class ItemStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                CapitalizableStatus itemStatus = (CapitalizableStatus)value;

                if (itemStatus == CapitalizableStatus.Pending)
                {
                    return Symbol.Clock.ToString();
                }
                else if (itemStatus == CapitalizableStatus.Succes)
                {
                    return Symbol.Accept.ToString();
                }
                else
                {
                    return Symbol.Clear.ToString();
                }
            }
            catch
            {
                return Symbol.Clear.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Symbol symbol = (Symbol)value;

            if (symbol == Symbol.Clock)
            {
                return CapitalizableStatus.Pending;
            }
            else if (symbol == Symbol.Accept)
            {
                return CapitalizableStatus.Succes;
            }
            else
            {
                return CapitalizableStatus.Failed;
            }
        }
    }
}
