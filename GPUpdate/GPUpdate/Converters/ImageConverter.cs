using System;
using System.Globalization;
using Xamarin.Forms;

namespace GPUpdate.Converters
{
    internal class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imagePath = value;

            if (imagePath == null) imagePath = "Icons/add.png";

            return imagePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}