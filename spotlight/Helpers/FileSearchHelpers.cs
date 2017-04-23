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
                drives = System.IO.Directory.GetLogicalDrives().ToList<string>();
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
