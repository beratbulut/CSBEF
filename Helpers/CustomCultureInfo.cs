using System.Globalization;
using System.Threading;

namespace CSBEF.Core.Helpers {
    public class CustomCultureInfo {
        public CustomCultureInfo () {
            CultureInfo = Thread.CurrentThread.CurrentCulture;
            IgnoreCase = false;
        }

        public CultureInfo CultureInfo { get; set; }
        public bool IgnoreCase { get; set; }
    }
}