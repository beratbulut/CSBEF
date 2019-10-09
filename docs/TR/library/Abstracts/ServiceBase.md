# ServiceBase.cs Abstract Sınıfı

Bu sınıf, modüller içerisinde servis sınıfları üretmek için kullanılmaktadır. **Generic Service Pattern** kapsamında kullanılan bir base sınıftır.

## Kod Yapısı

```
using AutoMapper;
using CSBEF.Core.Enums;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace CSBEF.Core.Abstracts
{
    public class ServiceBase : IServiceBase
    {
    }

    public abstract class ServiceBase<TPoco, TDTO> : IServiceBase<TPoco, TDTO>
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
        }

        #endregion Construction

        #region Public Actions

        public virtual async Task<IReturnModel<TDTO>> FirstAsync(GenericFilterModel<TDTO> filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

                    getData = await query.FirstAsync();
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
                        ActionName = "FirstAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.After")
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

        public virtual async Task<IReturnModel<TDTO>> FirstAsync(ActionFilterModel filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.Before").EventHandler<bool, ActionFilterModel>(filter);
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

                    getData = await query.FirstAsync();
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
                        ActionName = "FirstAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.After")
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

        public virtual async Task<IReturnModel<TDTO>> FirstOrDefaultAsync(GenericFilterModel<TDTO> filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

                    getData = await query.FirstOrDefaultAsync();
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
                        ActionName = "FirstOrDefaultAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.After")
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

        public virtual async Task<IReturnModel<TDTO>> FirstOrDefaultAsync(ActionFilterModel filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.Before").EventHandler<bool, ActionFilterModel>(filter);
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

                    getData = await query.FirstOrDefaultAsync();
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
                        ActionName = "FirstOrDefaultAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.After")
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

        public virtual async Task<IReturnModel<bool>> AnyAsync(GenericFilterModel<TDTO> filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

                    rtn.Result = await query.AnyAsync();
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
                        ActionName = "AnyAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.After")
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

        public virtual async Task<IReturnModel<bool>> AnyAsync(ActionFilterModel filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.Before").EventHandler<bool, ActionFilterModel>(filter);
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

                    rtn.Result = await query.AnyAsync();
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
                        ActionName = "AnyAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.After")
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

        public virtual async Task<IReturnModel<IList<TDTO>>> ListAsync(GenericFilterModel<TDTO> filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.ListAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

                    rtn.Result = _mapper.Map<List<TDTO>>(await query.ToListAsync());
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
                        ActionName = "ListAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.ListAsync.After")
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

        public virtual async Task<IReturnModel<IList<TDTO>>> ListAsync(ActionFilterModel filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.Before").EventHandler<bool, ActionFilterModel>(filter);
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

                    rtn.Result = _mapper.Map<List<TDTO>>(await query.ToListAsync());
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
                        ActionName = "ListAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.After")
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

        public virtual async Task<IReturnModel<int>> CountAsync(ActionFilterModel filter)
        {
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.CountAsync").EventHandler<bool, ActionFilterModel>(filter);
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

                    rtn.Result = await query.CountAsync();
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
                        ActionName = "CountAsync"
                    };
                    afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.CountAsync")
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
```

## Kullanım Şekli
Bu base sınıf, modüllerde bulunacak olan servis sınıfları tarafından kullanılmaktadır. Modüllerde servislerin olması zorunlu değildir ancak yaygın olarak kullanılan bir şeydir.

Bu base sınıf, **Generic Service Pattern** için hazırlanmış bir base sınıftır. Bu nedenle içerisinde, bir tabloyu temel almış olan bir servis sınıfı için gereken temel işlemleri bulundurmaktadır.

Servis sınıflarının 2 kullanım tarzı vardır. Bunlardan biri, herhangi bir POCO ve DTO modelini hedef alarak işlemler yapan servis sınıflarıdır. Bu servis sınıflarının tüm sahip olduğu işlemler, temsil ettiği tablo üzerinde yapılan işlemlerdir ve kısaca veri işleme servisi de denebilir. Bu servisler için bir Repository instance'ı otomatik olarak alınmaktadır. Bu alınacak instance, **Service Provider** üzerinden Dependency olarak alınır. Yani kısacası bir tabloyu baz alan veri işleme servisleri için gerekli olan tüm temel araçlar, bu base sınıfının bu kullanım tipinde sağlanmaktadır.

Bir diğer kullanım tipi ise boş servistir. Bu servislerin base'inde herhangi bir generic yapı bulunmamaktadır. Çünkü bu servisler veri işleme servisleri değil, farklı işlemler için kullanılmış servislerdir.

Bu kullanım tipine ne tarz servisler örnek verilebilir? Örneğin herhangi bir tabloyu hedef almayan ama birden fazla tablo üzerinde iş yapan bir servis olabilir. Bu servis özünde diğer kullanım tipindeki servisler gibi bir veri işleme servisidir ancak diğerlerine farkla bu servisin merkezinde tek bir tablo yoktur.

Bir diğer örnek ise veri işleme servisi olmayan servislerdir. Örneğin dış kaynaklar üzerinde işlem yapan servisler olabilir. Bunun en güzel örneği ise; dış kaynağın sağladığı farklı bir API üzerinden SMS atan bir servis veya ödeme işlemleri için bankalarla haberleşen bir servis veya dış kaynaklara veri gönderen ve raporlayan bir servis olabilir.

## Generic Olmayan Base Kullanımı

Yukardaki başlıkta anlarılan iki kullanım tipinden biri olan generic olmayan base kullanımı, bir başka tabiriyle boş servis tipidir. Yani, herhangi bir tabloyu baz almayan ve veri işleme dışında işlemler yapan servislerdir.

```
public class ServiceBase : IServiceBase
{
}
```

Yukarıdaki base sınıf bu konuda kullanılacak olan base'dir. Örnek kullanımı;

```
public class SMSService : ServiceBase, ISMSService {
    ....
}
```

Bu servislerin kullanım şekline bir örnek;

```
private ISMSService _smsService = null;
...
public MessageService(ISMSService smsService) {
    _smsService = smsService;
}
...
public async Task<bool> SendMessage(string msgContent, string phoneNumber = "", bool sms = false){
    ...
    if(sms)
        await _smsService.SendAsync(msgContent, phoneNumber);
    ...
}
...
```

## Generic Olan Base Kullanımı
Bir veri tabanı tablosuna odaklı veri işleyen servislerdir.
Yaygın kullanım olarak bizler her bir tablo için modül içerisinde bir servis bulundurmaktayız. Bu sayede, servislerin temsil ettiği tablolar için CRUD işlemlere (eğer özel olarak gerektiren bir durum yoksa) vakit harcamamakta, generic metotları kullanmatayız. Böyle bir durumda bu base sınıfının generic versiyonu kullanılmaktadır.

```
public abstract class ServiceBase<TPoco, TDTO> : IServiceBase<TPoco, TDTO>
        where TPoco : class, IEntityModelBase, new()
        where TDTO : class, IDTOModelBase, new() 
{
    ...
}
```

Yukarıdaki base generic yapıdaki base'dir. Görüleceği gibi bunu kullanarak oluşturulacak servisler için bir POCO ve bir de DTO modeli belirtilmelidir. Bu sayede generic metotların çalışabilmesi için gerekli yapılar oluşturulabilmektedir.

## Dependency Instance'lar
Generic metotlar için base sınıf içerisinde bazı dependency instance'ları alınmaktadır. Bunlar;

```
protected IConfiguration _configuration;
protected IWebHostEnvironment _hostingEnvironment;
public IRepositoryBase<TPoco> Repository { get; set; }
protected ILogger<ILog> _logger;
protected IMapper _mapper;
protected IEventService _eventService;
```


- **_configuration:** Servis içerisinde ihtiyaç duyulması durumunda kullanılması için alınan instance'tır.
- **_hostingEnvironment:** Servis içerisinde ihtiyaç duyulması durumunda kullanılması için alınan instance'tır.
- **Repository:** Generic metotlarda kullanılan bir gereksinimdir. Ayrıca servis içerisinde geliştirilecek diğer metotlarda da kullanılabilir. Erişim seviyesi **public** şeklindedir. Bu sayede, başka bir servis içerisinde bu servis'in instance'ına erişilirse ve bu instance üzerinden ilgili **Repository** instance'ına erişilmek istenirse, bunu sağlamak için erişim seviyesi **public** şeklindedir. Önerdiğimiz **Repository** instance erişimi bu şekilde değildir. Bunun için diğer servisin yapıcı metodu içerisinde gerekli olan **Repository** instance'ı almaktır. Fakat geliştiriciler arasında ihtiyaç duyabilen olabilir düşüncesiyle bu şekilde ayarlanmıştır.
- **_logger:** Gerek generic, gerekse de servis metotlarında kullanılması için gerekli olan instance'tır. Modülün içerisinde bulunduğu API uygulamasında tek bir **Logger** kullanılmaktadır. Buradaki instance ile de modül bu **Logger** instance'ına erişebilmektedir ve kullanabilmektedir.
- **_mapper:** Gerek generic, gerekse de servis metotlarında kullanılması için gerekli olan instance'tır. İçinde bulunulan API projesi için CSBEF tarafından tek bir **AutoMapper** instance'ı oluşturulur ve yapılandırılır. Modüller ise bu instance'a bu değişkenle erişebilir. Söz konusu instance, dependency injection ile elde edilir.
- **_eventService:** Servisler arası iletişim ve işlem paylaşımı, iki şekilde gerçekleşmektedir. Bunlardan biri, Service Provider üzerinden inject edilen instance'a erişmek, diğeri ise **Event Draven** yöntemidir. **Event Draven** yönteminde haberleşme Event'lar üzerinden gerçekleştirilir. Bu konuda bir standart getirilmiştir. Bu kullanım standartı gereğince her bir servis metodu için bir **Before** bir de **After** Event'ları olmalıdır. Bu sayede bir metot tetiklendiğinde, işini yapmadan önce kendisine ait **Before** Event'ını tetikler. İşini yaptıktan sonra ve Return etmeden önce de **After** Event'ını tetikler. Tüm servisler bir birinin Event tanımlarına abone olabilmektedir. Böylece bir metot başka bir metodun **Before** veya **After** Event'ına abone olabilir, buna göre işlem yapabilir. Örneğin resmi modül olan **UserActionLog** modülü, içerisinde bulunduğu API projesindeki tüm modüllerin tüm Event'larına kendini ekler ve bunların herhangi biri tetiklendiğinde bu işlemlerle ilgili Log kayıtları tutar. İşte bu değişken, projedeki tüm Event tanımlarını servise taşır ve servisin bu Event tanımlarına yenisini eklemesini veya bu tanımlara erişmesini sağlar.

## Bilgi Değişkenleri

```
#region IModuleService

public string ModuleName { get; set; }
public string ServiceName { get; set; }

#endregion IModuleService
```

Yukarıdaki kodlar, base içerisinde bulunmaktadır. Bu değişkenler özellikle generic metotlar içerisindeki **Before** ve **After** Event'larını tetiklemek için kullanılan bilgilerdir. Bu bilgiler, bu base'den türetilen servis sınıflarından aktarılmaktadır.

## Construction

```
#region Construction

public ServiceBase(
    IWebHostEnvironment hostingEnvironment,
    IConfiguration configuration,
    ILogger<ILog> logger,
    IMapper mapper,
    IRepositoryBase<TPoco> repository,
    IEventService eventService,
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
}

#endregion Construction
```

Yukarıdaki kod bloğunda gerekli olan instance'lar bu base'den türetilen servis sınıfından aktarılır. 

Örnek kullanım;

```
...

public UserService : ServiceBase<User, UserDTO>, IUserService {
    ...

    public UserService(
        IWebHostEnvironment hostingEnvironment,
        IConfiguration configuration,
        ILogger<ILog> logger,
        IMapper mapper,
        IRepositoryBase<TPoco> repository,
        IEventService eventService
    ) base(
        hostingEnvironment,
        configuration,
        logger,
        mapper,
        repository,
        eventService,
        "UserManagement", // ModuleName
        "UserService" // ServiceName
    ) {

    }

    ...
}
```

Yukarıdaki örnek kod, bu base sınıfından türetilen bir servis sınıfı örneğidir. Görüleceği üzere gerekli olan tüm instance'lar, Container üzerinden elde edilmekte ve base'e aktarılmaktadır. Sadece **ModuleName** ve **ServiceName** değişkenleri için gereken string bilgiler, servislere göre özel olarak belirtilmektedir.

## FirstAsync

```
public virtual async Task<IReturnModel<TDTO>> FirstAsync(GenericFilterModel<TDTO> filter)
{
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

        beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

            getData = await query.FirstAsync();
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
                ActionName = "FirstAsync"
            };
            afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.After")
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
```

Bu metot, servisin temel aldığı tablodaki bir veriyi döndürmektedir. Veriye ulaşmak için filtre kullanılabilir.

Örnek kullanım;

```
...
var getUser = await _userService.FirstAsync(new GenericFilterModel().SetOneWherePredicate(i => i.Id == 1));
if(getUser.Error.Status){
    rtn.Error = getUser.Error;
    cnt = false;
}

rtn.Result = getUser.Result.Email;
...
```

## FirstOrDefaultAsync

```
public virtual async Task<IReturnModel<TDTO>> FirstOrDefaultAsync(GenericFilterModel<TDTO> filter)
{
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

        beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

            getData = await query.FirstOrDefaultAsync();
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
                ActionName = "FirstOrDefaultAsync"
            };
            afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.After")
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
```

İlgili tablodaki bir veriye erişmek için kullanılır. Eğer herhangi bir veri bulunamazsa geriye **null** değeri döndürülür.

Örnek kullanım:

```
var getUser = await _userService.FirstAsync(new GenericFilterModel().SetOneWherePredicate(i => i.Id == 1));
if(getUser.Error.Status){
    rtn.Error = getUser.Error;
    cnt = false;
}

rtn.Result = getUser.Result != null ? getUser.Result.Email : "";
...
```

## AnyAsync

```
public virtual async Task<IReturnModel<bool>> AnyAsync(GenericFilterModel<TDTO> filter)
{
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

        beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

            rtn.Result = await query.AnyAsync();
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
                ActionName = "AnyAsync"
            };
            afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.After")
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
```

Verilen filtreye uygun herhangi bir kayıt olup olmadığı konusunda boolean dönüş yapan metoddur. Eğer kayıt varsa sonuç **true**, yoksa **false** dönüşü yapılmaktadır.

Örnek kullanım;

```
var getUser = await _userService.AnyAsync(new GenericFilterModel().SetOneWherePredicate(i => i.Id == 1));
if(getUser.Error.Status){
    rtn.Error = getUser.Error;
    cnt = false;
}

rtn.Result = getUser.Result != null ? "Kayıt var" : "Kayıt yok";
...
```

## ListAsync

```
public virtual async Task<IReturnModel<IList<TDTO>>> ListAsync(GenericFilterModel<TDTO> filter)
{
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

        beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.ListAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter);
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

            rtn.Result = _mapper.Map<List<TDTO>>(await query.ToListAsync());
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
                ActionName = "ListAsync"
            };
            afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.ListAsync.After")
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
```

Birden fazla kaydın listesine erişmek için kullanılan generic metottur.

Örnek kullanımı;

```
...
var getUsers = await _userService.ListAsync(new GenericFilterModel().SetOneWherePredicate(i => i.Status == true));
if(getUser.Error.Status){
    rtn.Error = getUser.Error;
    cnt = false;
}

rtn.Result = getUsers.Result;
...
```

## CountAsync

```
public virtual async Task<IReturnModel<int>> CountAsync(ActionFilterModel filter)
{
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

        beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.CountAsync").EventHandler<bool, ActionFilterModel>(filter);
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

            rtn.Result = await query.CountAsync();
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
                ActionName = "CountAsync"
            };
            afterEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.CountAsync")
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
```

Verilen filteye uygun bulunan kayıtların toplam sayısını döndürür.

Örnek kullanımı;

```
...
var getUsersCount = await _userService.CountAsync(new GenericFilterModel().SetOneWherePredicate(i => i.Status == true));
if(getUser.Error.Status){
    rtn.Error = getUser.Error;
    cnt = false;
}

rtn.Result = getUsers.Result;
...
```

## GenericFilterModel Alternatifi

Sınıfın kodlarına baktığınızda **CountAsync** hariç diğer tüm metotlarda 2 alternatif kullanım olduğunu göreceksiniz. Bu alternatiflerden biri **GenericFilterModel** tipinde parametre alanıdır. 

Bu alternatif kullanım, servisten servise erişimde kullanılmaktadır. Bu filtre tipi oldukça geniş bir filtreleme olanağı sağlamaktadır.

## ActionFilterModel Alternatifi

Generic metotlar için bulunan bir diğer filtre alternatifi ise Controller'dan servise doğru olan iletişimlerde kullanılmaktadır. İçerisindeki filtreler, Request olarak gelebilecek şekilde ayarlanmıştır.

Controller'lara **get** tipinde yapılan sorgularda **ActionFilterModel** tipine denk gelecek şekilde parametre alınır. Örnek sorgu;

```
https://www.demoapi.com/api/UserManagement/User/List?Where=Status = true&Order="Name asc, Surname asc"&Page=1&PageSize=10
```

Yukarıdaki örnek sorguda yer alan **Where** ve **Order** kısımlarında **System.Linq.Dynamic.Core** kullanılarak dönüştürülmektedir. Bu nedenle detaylı kullanım bilgisi için https://github.com/StefH/System.Linq.Dynamic.Core adresine bakılabilir.