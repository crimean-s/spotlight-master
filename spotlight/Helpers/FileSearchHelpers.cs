using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spotlight.Helpers
{
    public static class FileSearchHelpers
    {
        public static List<Drive> GetLogicalDrives()
        {
            List<Drive> drives = new List<Drive>();

            try
            {
                // drives = System.Environment.GetLogicalDrives().ToList<string>();
                var dr = System.IO.DriveInfo.GetDrives().Where(x => x.DriveType == System.IO.DriveType.Fixed).ToList();
                foreach (var item in dr)
                {
                    drives.Add(new Drive (true, item.Name, item.TotalSize));
                }


            }
            catch (System.IO.IOException)
            {
                System.Console.WriteLine("An I/O error occurs.");
            }
            catch (System.Security.SecurityException)
            {
                System.Console.WriteLine("The caller does not have the " +
                    "required permission.");
            }

            return drives;
        }
    }

    public class Drive
    {
        public bool isIndexed { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }

        public Drive(bool isIndexed, String Name, long Size)
        {
            this.isIndexed = isIndexed;
            this.Name = Name;
            this.Size = Size;
        }
    }
}
