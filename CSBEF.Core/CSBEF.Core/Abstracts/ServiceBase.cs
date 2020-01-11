using AutoMapper;
using CSBEF.Core.Concretes;
using CSBEF.Core.Enums;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using CSBEF.Core.Models.HubModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace CSBEF.Core.Abstracts
{
    public class ServiceBase : IServiceBase
    {
    }

    public class ServiceBase<TPoco, TDTO> : IServiceBase<TPoco, TDTO>
        where TPoco : class, IEntityModelBase, new()
        where TDTO : class, IDTOModelBase, new()
    {
        #region Dependencies

        protected IConfiguration _configuration;
        protected IWebHostEnvironment _hostingEnvironment;
        public IRepositoryBase<TPoco> Repository { get; set; }
        protected ILogger<ILog> _logger;
        protected IMapper _mapper;
        protected IEventService _eventService;
        private readonly IHubSyncDataService _hubSyncDataService;

        #endregion Dependencies

        #region IModuleService

        public string ModuleName { get; set; }
        public string ServiceName { get; set; }

        #endregion IModuleService

        #region Construction

        public ServiceBase(
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration,
            ILogger<ILog> logger,
            IMapper mapper,
            IRepositoryBase<TPoco> repository,
            IEventService eventService,
            IHubSyncDataService hubSyncDataService,
            string moduleName,
            string serviceName
        )
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            Repository = repository;
            ModuleName = moduleName;
            ServiceName = serviceName;
            _eventService = eventService;
            _hubSyncDataService = hubSyncDataService;
        }

        #endregion Construction

        #region Public Actions

        public virtual IReturnModel<TDTO> First(GenericFilterModel<TDTO> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                Expression<Func<TPoco, bool>> convertExpression = null;
                PropertyInfo prop = null;
                TPoco getData = null;
                TDTO convertModel = null;
                AfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>> afterEventParameterModel = null;
                IReturnModel<TDTO> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.First.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (filter.WherePredicates.Any())
                    {
                        foreach (var wherePre in filter.WherePredicates)
                        {
                            convertExpression = ExpressionFuncConverter<TPoco>.Convert(wherePre);
                            query = query.Where(convertExpression);
                        }
                    }

                    if (filter.OrderPredicates.Any())
                    {
                        foreach (var op in filter.OrderPredicates)
                        {
                            prop = typeof(TPoco).GetProperty(op.PropertyName);
                            if (op.Descending)
                                query = query.OrderByDescending(x => prop.GetValue(x, null));
                            else
                                query = query.OrderBy(x => prop.GetValue(x, null));
                        }
                    }

                    getData = query.First();
                    convertModel = _mapper.Map<TDTO>(getData);
                    rtn.Result = convertModel;
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "First"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.First.After")
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                convertExpression = null;
                prop = null;
                getData = null;
                convertModel = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> First(ActionFilterModel filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                TPoco getData = null;
                TDTO convertModel = null;
                AfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel> afterEventParameterModel = null;
                IReturnModel<TDTO> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.First.Before").EventHandler<bool, ActionFilterModel>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (!string.IsNullOrWhiteSpace(filter.Where))
                    {
                        query = query.Where(filter.Where);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Order))
                    {
                        query = query.OrderBy(filter.Order);
                    }

                    getData = query.First();
                    convertModel = _mapper.Map<TDTO>(getData);
                    rtn.Result = convertModel;
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "First"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.First.After")
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                getData = null;
                convertModel = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> FirstOrDefault(GenericFilterModel<TDTO> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                Expression<Func<TPoco, bool>> convertExpression = null;
                PropertyInfo prop = null;
                TPoco getData = null;
                TDTO convertModel = null;
                AfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>> afterEventParameterModel = null;
                IReturnModel<TDTO> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefault.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (filter.WherePredicates.Any())
                    {
                        foreach (var wherePre in filter.WherePredicates)
                        {
                            convertExpression = ExpressionFuncConverter<TPoco>.Convert(wherePre);
                            query = query.Where(convertExpression);
                        }
                    }

                    if (filter.OrderPredicates.Any())
                    {
                        foreach (var op in filter.OrderPredicates)
                        {
                            prop = typeof(TPoco).GetProperty(op.PropertyName);
                            if (op.Descending)
                                query = query.OrderByDescending(x => prop.GetValue(x, null));
                            else
                                query = query.OrderBy(x => prop.GetValue(x, null));
                        }
                    }

                    getData = query.FirstOrDefault();
                    if (getData != null)
                        convertModel = _mapper.Map<TDTO>(getData);
                    rtn.Result = convertModel;
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "FirstOrDefault"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefault.After")
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                convertExpression = null;
                prop = null;
                getData = null;
                convertModel = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> FirstOrDefault(ActionFilterModel filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                TPoco getData = null;
                TDTO convertModel = null;
                AfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel> afterEventParameterModel = null;
                IReturnModel<TDTO> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefault.Before").EventHandler<bool, ActionFilterModel>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (!string.IsNullOrWhiteSpace(filter.Where))
                    {
                        query = query.Where(filter.Where);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Order))
                    {
                        query = query.OrderBy(filter.Order);
                    }

                    getData = query.FirstOrDefault();
                    convertModel = _mapper.Map<TDTO>(getData);
                    rtn.Result = convertModel;
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "FirstOrDefault"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefault.After")
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                getData = null;
                convertModel = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<bool> Any(GenericFilterModel<TDTO> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<bool> rtn = new ReturnModel<bool>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                Expression<Func<TPoco, bool>> convertExpression = null;
                PropertyInfo prop = null;
                AfterEventParameterModel<IReturnModel<bool>, GenericFilterModel<TDTO>> afterEventParameterModel = null;
                IReturnModel<bool> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.Any.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (filter.WherePredicates.Any())
                    {
                        foreach (var wherePre in filter.WherePredicates)
                        {
                            convertExpression = ExpressionFuncConverter<TPoco>.Convert(wherePre);
                            query = query.Where(convertExpression);
                        }
                    }

                    if (filter.OrderPredicates.Any())
                    {
                        foreach (var op in filter.OrderPredicates)
                        {
                            prop = typeof(TPoco).GetProperty(op.PropertyName);
                            if (op.Descending)
                                query = query.OrderByDescending(x => prop.GetValue(x, null));
                            else
                                query = query.OrderBy(x => prop.GetValue(x, null));
                        }
                    }

                    rtn.Result = query.Any();
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<bool>, GenericFilterModel<TDTO>>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "Any"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.Any.After")
                        .EventHandler<bool, IAfterEventParameterModel<IReturnModel<bool>, GenericFilterModel<TDTO>>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                convertExpression = null;
                prop = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<bool> Any(ActionFilterModel filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<bool> rtn = new ReturnModel<bool>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                AfterEventParameterModel<IReturnModel<bool>, ActionFilterModel> afterEventParameterModel = null;
                IReturnModel<bool> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.Any.Before").EventHandler<bool, ActionFilterModel>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (!string.IsNullOrWhiteSpace(filter.Where))
                    {
                        query = query.Where(filter.Where);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Order))
                    {
                        query = query.OrderBy(filter.Order);
                    }

                    rtn.Result = query.Any();
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<bool>, ActionFilterModel>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "Any"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.Any.After")
                        .EventHandler<bool, IAfterEventParameterModel<IReturnModel<bool>, ActionFilterModel>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<IList<TDTO>> List(GenericFilterModel<TDTO> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<IList<TDTO>> rtn = new ReturnModel<IList<TDTO>>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                AfterEventParameterModel<IReturnModel<IList<TDTO>>, GenericFilterModel<TDTO>> afterEventParameterModel = null;
                IReturnModel<IList<TDTO>> afterEventHandler = null;
                IQueryable<TPoco> query = null;
                Expression<Func<TPoco, bool>> convertExpression = null;
                PropertyInfo prop = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.List.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (filter.WherePredicates.Any())
                    {
                        foreach (var wherePre in filter.WherePredicates)
                        {
                            convertExpression = ExpressionFuncConverter<TPoco>.Convert(wherePre);
                            query = query.Where(convertExpression);
                        }
                    }

                    if (filter.OrderPredicates.Any())
                    {
                        foreach (var op in filter.OrderPredicates)
                        {
                            prop = typeof(TPoco).GetProperty(op.PropertyName);
                            if (op.Descending)
                                query = query.OrderByDescending(x => prop.GetValue(x, null));
                            else
                                query = query.OrderBy(x => prop.GetValue(x, null));
                        }
                    }

                    if (filter.Page.ToInt(0) == 0)
                        filter.Page = 1;

                    if (filter.PageSize.ToInt(0) == 0)
                        filter.PageSize = 10;

                    query = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

                    rtn.Result = _mapper.Map<List<TDTO>>(query.ToList());
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<IList<TDTO>>, GenericFilterModel<TDTO>>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "List"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.List.After")
                        .EventHandler<IList<TDTO>, IAfterEventParameterModel<IReturnModel<IList<TDTO>>, GenericFilterModel<TDTO>>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                afterEventParameterModel = null;
                afterEventHandler = null;
                query = null;
                convertExpression = null;
                prop = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<IList<TDTO>> List(ActionFilterModel filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<IList<TDTO>> rtn = new ReturnModel<IList<TDTO>>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                AfterEventParameterModel<IReturnModel<IList<TDTO>>, ActionFilterModel> afterEventParameterModel = null;
                IReturnModel<IList<TDTO>> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.List.Before").EventHandler<bool, ActionFilterModel>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (!string.IsNullOrWhiteSpace(filter.Where))
                    {
                        query = query.Where(filter.Where);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Order))
                    {
                        query = query.OrderBy(filter.Order);
                    }

                    if (filter.Page.ToInt(0) == 0)
                        filter.Page = 1;

                    if (filter.PageSize.ToInt(0) == 0)
                        filter.PageSize = 10;

                    query = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

                    rtn.Result = _mapper.Map<List<TDTO>>(query.ToList());
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<IList<TDTO>>, ActionFilterModel>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "List"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.List.After")
                        .EventHandler<IList<TDTO>, IAfterEventParameterModel<IReturnModel<IList<TDTO>>, ActionFilterModel>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<int> Count(ActionFilterModel filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            IReturnModel<int> rtn = new ReturnModel<int>(_logger);

            try
            {
                #region Variables

                bool cnt = true;
                IReturnModel<bool> beforeEventHandler = null;
                IQueryable<TPoco> query;
                AfterEventParameterModel<IReturnModel<int>, ActionFilterModel> afterEventParameterModel = null;
                IReturnModel<int> afterEventHandler = null;

                #endregion Variables

                #region Before Event Handler

                beforeEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.Count.Before").EventHandler<bool, ActionFilterModel>(filter);
                if (beforeEventHandler != null)
                {
                    if (beforeEventHandler.Error.Status)
                    {
                        rtn.Error = beforeEventHandler.Error;
                        cnt = false;
                    }
                }

                #endregion Before Event Handler

                #region Action Body

                if (cnt)
                {
                    query = Repository.GetAll();

                    if (!string.IsNullOrWhiteSpace(filter.Where))
                    {
                        query = query.Where(filter.Where);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.Order))
                    {
                        query = query.OrderBy(filter.Order);
                    }

                    rtn.Result = query.Count();
                }

                #endregion Action Body

                #region After Event Handler

                if (cnt)
                {
                    afterEventParameterModel = new AfterEventParameterModel<IReturnModel<int>, ActionFilterModel>
                    {
                        DataToBeSent = rtn,
                        ActionParameter = filter,
                        ModuleName = ModuleName,
                        ServiceName = ServiceName,
                        ActionName = "Count"
                    };
                    afterEventHandler = _eventService.GetEvent(ModuleName, $"{ServiceName}.Count.After")
                        .EventHandler<int, IAfterEventParameterModel<IReturnModel<int>, ActionFilterModel>>(afterEventParameterModel);
                    if (afterEventHandler != null)
                    {
                        if (afterEventHandler.Error.Status)
                        {
                            rtn.Error = afterEventHandler.Error;
                            cnt = false;
                        }
                        else
                        {
                            rtn.Result = afterEventHandler.Result;
                        }
                    }
                }

                #endregion After Event Handler

                #region Clear Memory

                filter = null;
                beforeEventHandler = null;
                query = null;
                afterEventParameterModel = null;
                afterEventHandler = null;

                #endregion Clear Memory
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> BaseAdd(ServiceParamsWithIdentifier<TDTO> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                var convertPoco = _mapper.Map<TPoco>(data.Param);
                convertPoco.AddingDate = DateTime.Now;
                convertPoco.UpdatingDate = DateTime.Now;
                convertPoco.AddingUserId = data.UserId;
                convertPoco.UpdatingUserId = data.UserId;
                var savedModel = Repository.Add(convertPoco);
                Repository.Save();
                rtn.Result = _mapper.Map<TDTO>(savedModel);
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> BaseUpdate(ServiceParamsWithIdentifier<TDTO> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                var getData = Repository.Find(i => i.Id == data.Param.Id);
                if (getData == null)
                {
                    rtn = rtn.SendError(GlobalErrors.DataNotFound);
                    return rtn;
                }

                getData = _mapper.Map<TPoco>(data.Param);
                getData.AddingDate = DateTime.Now;
                getData.UpdatingDate = DateTime.Now;
                getData.AddingUserId = data.UserId;
                getData.UpdatingUserId = data.UserId;
                getData = Repository.Update(getData);
                Repository.Save();
                rtn.Result = _mapper.Map<TDTO>(getData);
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> BaseChangeStatus(ServiceParamsWithIdentifier<ChangeStatusModel> data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                var getData = Repository.Find(i => i.Id == data.Param.Id);
                if (getData == null)
                {
                    rtn = rtn.SendError(GlobalErrors.DataNotFound);
                    return rtn;
                }

                getData.Status = data.Param.Status;
                getData = Repository.Update(getData);
                Repository.Save();
                rtn.Result = _mapper.Map<TDTO>(getData);
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> BaseAddWithSocket(ServiceParamsWithIdentifier<TDTO> data, string socketUpdateKey, string socketUpdatedDataName)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(socketUpdateKey))
                throw new ArgumentNullException(nameof(socketUpdateKey));

            if (string.IsNullOrWhiteSpace(socketUpdatedDataName))
                throw new ArgumentNullException(nameof(socketUpdatedDataName));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                var save = BaseAdd(data);
                if (save.Error.Status)
                {
                    rtn.Error = save.Error;
                }
                else
                {
                    rtn.Result = save.Result;

                    _hubSyncDataService.OnSync(new HubSyncDataModel<TDTO>
                    {
                        Key = socketUpdateKey,
                        ProcessType = "add",
                        Id = rtn.Result.Id,
                        UserId = data.UserId,
                        Name = socketUpdatedDataName,
                        Data = rtn.Result
                    });
                }
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> BaseUpdateWithSocket(ServiceParamsWithIdentifier<TDTO> data, string socketUpdateKey, string socketUpdatedDataName)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(socketUpdateKey))
                throw new ArgumentNullException(nameof(socketUpdateKey));

            if (string.IsNullOrWhiteSpace(socketUpdatedDataName))
                throw new ArgumentNullException(nameof(socketUpdatedDataName));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                var save = BaseUpdate(data);
                if (save.Error.Status)
                {
                    rtn.Error = save.Error;
                }
                else
                {
                    rtn.Result = save.Result;

                    _hubSyncDataService.OnSync(new HubSyncDataModel<TDTO>
                    {
                        Key = socketUpdateKey,
                        ProcessType = "update",
                        Id = rtn.Result.Id,
                        UserId = data.UserId,
                        Name = socketUpdatedDataName,
                        Data = rtn.Result
                    });
                }
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        public virtual IReturnModel<TDTO> BaseChangeStatusWithSocket(ServiceParamsWithIdentifier<ChangeStatusModel> data, string socketUpdateKey, string socketUpdatedDataName)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(socketUpdateKey))
                throw new ArgumentNullException(nameof(socketUpdateKey));

            if (string.IsNullOrWhiteSpace(socketUpdatedDataName))
                throw new ArgumentNullException(nameof(socketUpdatedDataName));

            IReturnModel<TDTO> rtn = new ReturnModel<TDTO>(_logger);

            try
            {
                var save = BaseChangeStatus(data);
                if (save.Error.Status)
                {
                    rtn.Error = save.Error;
                }
                else
                {
                    rtn.Result = save.Result;

                    _hubSyncDataService.OnSync(new HubSyncDataModel<bool>
                    {
                        Key = socketUpdateKey,
                        ProcessType = "remove",
                        Id = rtn.Result.Id,
                        UserId = data.UserId,
                        Name = socketUpdatedDataName,
                        Data = rtn.Result.Status
                    });
                }
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
            }

            return rtn;
        }

        #endregion Public Actions

        #region Dispose

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Repository.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Dispose
    }
}