﻿using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using dSearch.ListItem;

namespace dSearch
{
    public class FileInformation : SearchItem
    {
        public FileInformation(string fileLocation)
        {
            FileLocation = fileLocation;
        }

        public string FileLocation { get; }

        private string displayName;
        public string DisplayName
        {
            //get { return displayName ?? (displayName = Path.GetFileName(FileLocation)); } // показываем расширение
            get { return displayName ?? (displayName = Path.GetFileNameWithoutExtension(FileLocation)); } // не показываем расширение файла
        }

        private string extension;
        public string Extension
        {
            get { return extension ?? (extension = Path.GetExtension(FileLocation)); }
        }

        private ImageSource icon;
        public ImageSource Icon
        {
            get { return icon ?? (icon = GetAssociatedIcon(FileLocation)); }
        }

        private static ImageSource GetAssociatedIcon(string fileName)
        {
            return ToImageSource(
                System.Drawing.Icon.ExtractAssociatedIcon(fileName)
            );
        }

        private static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        public string SelectedName { get; set; }

        public override string ToString()
        {
            return $"{DisplayName}";
        }
    }
}