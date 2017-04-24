using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spotlight.Helpers
{
    public static class FileSearchHelpers
    {
        public static List<string> GetLogicalDrives()
        {
            List<string> drives = new List<string>();

            try
            {
                // drives = System.Environment.GetLogicalDrives().ToList<string>();
                var dr = System.IO.DriveInfo.GetDrives().Where(x => x.DriveType == System.IO.DriveType.Fixed).ToList();
                foreach (var item in dr)
                {
                    drives.Add(item.Name);
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
}
