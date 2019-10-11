using CSBEF.Core.Models;
using System.Collections.Generic;

namespace CSBEF.Core.Concretes
{
    public static class GlobalConfiguration
    {
        public static IList<ModuleInfo> Modules { get; }
        public static string DbProvider { get; set; }
        public static string DbConnectionString { get; set; }
        public static string sAppPath { get; set; }
        public static string sWwwRootPath { get; set; }

        static GlobalConfiguration()
        {
            Modules = new List<ModuleInfo>();
        }
    }
}