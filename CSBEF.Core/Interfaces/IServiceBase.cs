using System;
using System.Collections.Generic;
using AutoMapper;
using CSBEF.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSBEF.Core.Interfaces {
    public interface IServiceBase<TPoco, TDTO> : IModuleService, IDisposable
    where TPoco : class, IEntityModelBase, new ()
    where TDTO : class, IDTOModelBase, new () {
        IConfiguration Configuration { get; set; }
        IWebHostEnvironment HostingEnvironment { get; set; }
        ILogger<IReturnModel<bool>> Logger { get; set; }
        IMapper Mapper { get; set; }
        IEventService EventService { get; set; }
        IHubSyncDataService HubSyncDataService { get; set; }
        IRepositoryBase<TPoco> Repository { get; set; }
        IReturnModel<TDTO> First (GenericFilterModel<TDTO> filter);
        IReturnModel<TDTO> First (ActionFilterModel filter);
        IReturnModel<TDTO> FirstOrDefault (GenericFilterModel<TDTO> filter);
        IReturnModel<TDTO> FirstOrDefault (ActionFilterModel filter);
        IReturnModel<bool> Any (GenericFilterModel<TDTO> filter);
        IReturnModel<bool> Any (ActionFilterModel filter);
        IReturnModel<IList<TDTO>> List (GenericFilterModel<TDTO> filter);
        IReturnModel<IList<TDTO>> List (ActionFilterModel filter);
        IReturnModel<int> Count (ActionFilterModel filter);
    }
}