using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Code_Folders.Models
{
    class CodeFolder
    {
        public string Path { get; set; }
    }

    public class PathToFolderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string folderPath = (string)value;
            return Path.GetFileName(folderPath);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
