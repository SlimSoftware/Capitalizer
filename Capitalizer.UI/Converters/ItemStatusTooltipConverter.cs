using Capitalizer.Core.Enums;
using Microsoft.UI.Xaml.Data;
using System;

namespace Capitalizer.UI.Converters
{
    public partial class ItemStatusTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                CapitalizableStatus itemStatus = (CapitalizableStatus)value;

                if (itemStatus == CapitalizableStatus.Pending)
                {
                    return "Rename pending";
                }
                else if (itemStatus == CapitalizableStatus.Succes)
                {
                    return "Rename succesful/no change";
                }
                else
                {
                    return "Renaming error";
                }
            }
            catch
            {
                return "Renaming error";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
