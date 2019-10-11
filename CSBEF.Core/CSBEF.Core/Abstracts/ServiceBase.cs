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
    public abstract class ServiceBase : IServiceBase
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter).ConfigureAwait(false);
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

                    getData = await query.FirstAsync().ConfigureAwait(false);
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
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.Before").EventHandler<bool, ActionFilterModel>(filter).ConfigureAwait(false);
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

                    getData = await query.FirstAsync().ConfigureAwait(false);
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
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter).ConfigureAwait(false);
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

                    getData = await query.FirstOrDefaultAsync().ConfigureAwait(false);
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
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, GenericFilterModel<TDTO>>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstOrDefaultAsync.Before").EventHandler<bool, ActionFilterModel>(filter).ConfigureAwait(false);
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

                    getData = await query.FirstOrDefaultAsync().ConfigureAwait(false);
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
                        .EventHandler<TDTO, IAfterEventParameterModel<IReturnModel<TDTO>, ActionFilterModel>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter).ConfigureAwait(false);
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

                    rtn.Result = await query.AnyAsync().ConfigureAwait(false);
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
                        .EventHandler<bool, IAfterEventParameterModel<IReturnModel<bool>, GenericFilterModel<TDTO>>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.AnyAsync.Before").EventHandler<bool, ActionFilterModel>(filter).ConfigureAwait(false);
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

                    rtn.Result = await query.AnyAsync().ConfigureAwait(false);
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
                        .EventHandler<bool, IAfterEventParameterModel<IReturnModel<bool>, ActionFilterModel>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.ListAsync.Before").EventHandler<bool, GenericFilterModel<TDTO>>(filter).ConfigureAwait(false);
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

                    rtn.Result = _mapper.Map<List<TDTO>>(await query.ToListAsync().ConfigureAwait(false));
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
                        .EventHandler<IList<TDTO>, IAfterEventParameterModel<IReturnModel<IList<TDTO>>, GenericFilterModel<TDTO>>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.Before").EventHandler<bool, ActionFilterModel>(filter).ConfigureAwait(false);
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

                    rtn.Result = _mapper.Map<List<TDTO>>(await query.ToListAsync().ConfigureAwait(false));
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
                        .EventHandler<IList<TDTO>, IAfterEventParameterModel<IReturnModel<IList<TDTO>>, ActionFilterModel>>(afterEventParameterModel).ConfigureAwait(false);
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

                beforeEventHandler = await _eventService.GetEvent(ModuleName, $"{ServiceName}.FirstAsync.CountAsync").EventHandler<bool, ActionFilterModel>(filter).ConfigureAwait(false);
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

                    rtn.Result = await query.CountAsync().ConfigureAwait(false);
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
                        .EventHandler<int, IAfterEventParameterModel<IReturnModel<int>, ActionFilterModel>>(afterEventParameterModel).ConfigureAwait(false);
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