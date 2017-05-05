using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dSearch.Helpers
{
    public static class FileSearchHelpers
    {

        /// <summary>
        /// Получение списка логических дисков
        /// </summary>
        public static List<Drive> GetLogicalDrives()
        {
            List<Drive> drives = new List<Drive>();

            try
            {
                var dr = System.IO.DriveInfo.GetDrives().Where(x => x.DriveType == System.IO.DriveType.Fixed).ToList();
                foreach (var item in dr)
                {
                    drives.Add(new Drive (true, item.Name, decimal.Round(item.TotalSize / 1073741824m, 2, MidpointRounding.AwayFromZero)));
                }
            }
            catch (System.IO.IOException)
            {
                System.Console.WriteLine("An I/O error occurs.");
            }
            catch (System.Security.SecurityException)
            {
                System.Console.WriteLine("The caller does not have the required permission.");
            }

            return drives;
        }
    }

    public class Drive : INotifyPropertyChanged
    {
        public bool isIndexed { get; set; }
        public string Name { get; set; }
        public decimal Size { get; set; }

        public Drive(bool isIndexed, String Name, decimal Size)
        {
            this.isIndexed = isIndexed;
            this.Name = Name;
            this.Size = Size;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
