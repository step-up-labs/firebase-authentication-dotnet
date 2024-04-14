using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Firebase.Auth.UI.Helpers
{
    public class RuntimeHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        public static bool IsMSIX
        {
            get
            {
                var length = 0;

                return GetCurrentPackageFullName(ref length, null) != 15700L;
            }
        }
    }

}
