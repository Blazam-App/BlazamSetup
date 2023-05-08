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
        /// <summary>
        /// Hash code that doesn't change with application
        /// restarts
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static int GetAppHashCode(this string input)
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                foreach (char c in input)
                {
                    hash = hash * 23 + c;
                }
                return hash;
            }
        }

    }
}
