using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazamSetup
{
    internal static class Helpers
    {
        internal static bool IsNullOrEmpty(this string value)
        {
            return (value == null || value == "");
        }
    }
}
