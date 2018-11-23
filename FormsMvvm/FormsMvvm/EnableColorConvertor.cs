using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace FormsMvvm
{
    public class EnableColorConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Trace.WriteLine($"{GetType().Name} Convert {value} {targetType} {parameter} {culture}");
            if ((bool)value)
            {
                return Color.White;
            }
            else
            {
                return Color.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Trace.WriteLine($"{GetType().Name} ConvertBack {value} {targetType} {parameter} {culture}");
            return false;
        }
    }
}
