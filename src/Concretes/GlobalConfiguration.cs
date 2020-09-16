using System.Collections.Generic;
using CSBEF.Models;
using CSBEF.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Concretes
{
    public static class GlobalConfiguration
    {
        public static List<ModuleInfo> Modules { get; } = new List<ModuleInfo>();

        public static IApiStartOptionsModel ApiStartOptions { get; set; } = new ApiStartOptionsModel();

        public static IMvcBuilder MvcBuilder { get; set; }
    }
}