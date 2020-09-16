using System;
using System.Linq;
using AutoMapper;
using CSBEF.Helpers;
using CSBEF.Models;
using CSBEF.Models.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Collections.Generic;
using CSBEF.enums;

namespace CSBEF.Concretes
{
    public class ServiceBase : IServiceBase
    {
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class ServiceBase<TPoco, TDTO> : IServiceBase<TPoco, TDTO>
        where TPoco : class, IEntityModelBase, new()
        where TDTO : class, IDtoModelBase, new()
    {
        protected IConfiguration configuration;
        protected IWebHostEnvironment hostingEnvironment;
        public IRepositoryBaseWithCud<TPoco> Repository { get; set; }
        protected ILogger<IServiceBase<TPoco, TDTO>> logger;
        protected IMapper mapper;
        protected IEventService eventService;
        public string ModuleName { get; set; }
        public string ServiceName { get; set; }
        private readonly IDynamicServiceAction dynamicServiceAction;

        private readonly UnitOfWork worker;

        public ServiceBase(
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration,
            ILogger<IServiceBase<TPoco, TDTO>> logger,
            IMapper mapper,
            IEventService eventService,
            string moduleName,
            string serviceName,
            IDynamicServiceAction dynamicServiceAction,
            UnitOfWork worker
        )
        {
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.logger = logger;
            this.mapper = mapper;
            ModuleName = moduleName;
            ServiceName = serviceName;
            this.eventService = eventService;
            this.dynamicServiceAction = dynamicServiceAction;
            this.worker = worker;

            this.Repository = this.worker.GenerateRepositoryWithCud<TPoco>();
        }

        public virtual async Task<IReturnModel<TDTO>> FirstAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default)
        {
            var actionName = "FirstAsync";

            async Task<IReturnModel<TDTO>> invoker(GenericFilterModel<TDTO> args, IReturnModel<TDTO> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (args.WherePredicates.Any())
                {
                    foreach (var wherePre in args.WherePredicates)
                    {
                        var convertExpression = ExpressionFuncConverter<TPoco>.Convert(wherePre);
                        query = query.Where(convertExpression);
                    }
                }

                if (args.OrderPredicates.Any())
                {
                    foreach (var op in args.OrderPredicates)
                    {
                        var prop = typeof(TPoco).GetProperty(op.PropertyName);
                        if (op.Descending)
                            query = query.OrderByDescending(x => prop.GetValue(x, null));
                        else
                            query = query.OrderBy(x => prop.GetValue(x, null));
                    }
                }

                rtn.Result = mapper.Map<TDTO>(await query.FirstAsync(cancellationToken).ConfigureAwait(false));

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<TDTO, GenericFilterModel<TDTO>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<TDTO>> FirstAsync(ActionFilterModel args, CancellationToken cancellationToken = default)
        {
            var actionName = "FirstAsync";

            async Task<IReturnModel<TDTO>> invoker(ActionFilterModel args, IReturnModel<TDTO> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (!string.IsNullOrWhiteSpace(args.Where))
                {
                    query = query.Where(args.Where);
                }

                if (!string.IsNullOrWhiteSpace(args.Order))
                {
                    query = query.OrderBy(args.Order);
                }

                rtn.Result = mapper.Map<TDTO>(await query.FirstAsync(cancellationToken).ConfigureAwait(false));

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<TDTO, ActionFilterModel>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<TDTO>> FirstOrDefaultAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default)
        {
            var actionName = "FirstOrDefaultAsync";

            async Task<IReturnModel<TDTO>> invoker(GenericFilterModel<TDTO> args, IReturnModel<TDTO> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (args.WherePredicates.Any())
                {
                    foreach (var wherePre in args.WherePredicates)
                    {
                        var convertExpression = ExpressionFuncConverter<TPoco>.Convert(wherePre);
                        query = query.Where(convertExpression);
                    }
                }

                if (args.OrderPredicates.Any())
                {
                    foreach (var op in args.OrderPredicates)
                    {
                        var prop = typeof(TPoco).GetProperty(op.PropertyName);
                        if (op.Descending)
                            query = query.OrderByDescending(x => prop.GetValue(x, null));
                        else
                            query = query.OrderBy(x => prop.GetValue(x, null));
                    }
                }

                rtn.Result = mapper.Map<TDTO>(await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false));

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<TDTO, GenericFilterModel<TDTO>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<TDTO>> FirstOrDefaultAsync(ActionFilterModel args, CancellationToken cancellationToken = default)
        {
            var actionName = "FirstOrDefaultAsync";

            async Task<IReturnModel<TDTO>> invoker(ActionFilterModel args, IReturnModel<TDTO> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (!string.IsNullOrWhiteSpace(args.Where))
                {
                    query = query.Where(args.Where);
                }

                if (!string.IsNullOrWhiteSpace(args.Order))
                {
                    query = query.OrderBy(args.Order);
                }

                rtn.Result = mapper.Map<TDTO>(await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false));

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<TDTO, ActionFilterModel>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<bool>> AnyAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default)
        {
            var actionName = "AnyAsync";

            async Task<IReturnModel<bool>> invoker(GenericFilterModel<TDTO> args, IReturnModel<bool> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (args.WherePredicates.Any())
                {
                    foreach (var wherePre in args.WherePredicates)
                    {
                        query = query.Where(ExpressionFuncConverter<TPoco>.Convert(wherePre));
                    }
                }

                if (args.OrderPredicates.Any())
                {
                    foreach (var op in args.OrderPredicates)
                    {
                        var prop = typeof(TPoco).GetProperty(op.PropertyName);
                        if (op.Descending)
                            query = query.OrderByDescending(x => prop.GetValue(x, null));
                        else
                            query = query.OrderBy(x => prop.GetValue(x, null));
                    }
                }

                rtn.Result = await query.AnyAsync(cancellationToken).ConfigureAwait(false);

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<bool, GenericFilterModel<TDTO>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<bool>> AnyAsync(ActionFilterModel args, CancellationToken cancellationToken = default)
        {
            var actionName = "AnyAsync";

            async Task<IReturnModel<bool>> invoker(ActionFilterModel args, IReturnModel<bool> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (!string.IsNullOrWhiteSpace(args.Where))
                {
                    query = query.Where(args.Where);
                }

                if (!string.IsNullOrWhiteSpace(args.Order))
                {
                    query = query.OrderBy(args.Order);
                }

                rtn.Result = await query.AnyAsync(cancellationToken).ConfigureAwait(false);

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<bool, ActionFilterModel>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<IList<TDTO>>> ListAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default)
        {
            var actionName = "ListAsync";

            async Task<IReturnModel<IList<TDTO>>> invoker(GenericFilterModel<TDTO> args, IReturnModel<IList<TDTO>> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (args.WherePredicates.Any())
                {
                    foreach (var wherePre in args.WherePredicates)
                    {
                        query = query.Where(ExpressionFuncConverter<TPoco>.Convert(wherePre));
                    }
                }

                if (args.OrderPredicates.Any())
                {
                    foreach (var op in args.OrderPredicates)
                    {
                        var prop = typeof(TPoco).GetProperty(op.PropertyName);
                        if (op.Descending)
                            query = query.OrderByDescending(x => prop.GetValue(x, null));
                        else
                            query = query.OrderBy(x => prop.GetValue(x, null));
                    }
                }

                if (args.Page.ToInt(0) == 0)
                    args.Page = 1;

                if (args.PageSize.ToInt(0) == 0)
                    args.PageSize = 10;

                query = query.Skip((args.Page - 1) * args.PageSize).Take(args.PageSize);

                rtn.Result = mapper.Map<List<TDTO>>(await query.ToListAsync(cancellationToken).ConfigureAwait(false));

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<IList<TDTO>, GenericFilterModel<TDTO>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<IList<TDTO>>> ListAsync(ActionFilterModel args, CancellationToken cancellationToken = default)
        {
            var actionName = "ListAsync";

            async Task<IReturnModel<IList<TDTO>>> invoker(ActionFilterModel args, IReturnModel<IList<TDTO>> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (!string.IsNullOrWhiteSpace(args.Where))
                {
                    query = query.Where(args.Where);
                }

                if (!string.IsNullOrWhiteSpace(args.Order))
                {
                    query = query.OrderBy(args.Order);
                }

                if (args.Page.ToInt(0) == 0)
                    args.Page = 1;

                if (args.PageSize.ToInt(0) == 0)
                    args.PageSize = 10;

                query = query.Skip((args.Page - 1) * args.PageSize).Take(args.PageSize);

                rtn.Result = mapper.Map<List<TDTO>>(await query.ToListAsync(cancellationToken).ConfigureAwait(false));

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<IList<TDTO>, ActionFilterModel>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<int>> CountAsync(GenericFilterModel<TDTO> args, CancellationToken cancellationToken = default)
        {
            var actionName = "CountAsync";

            async Task<IReturnModel<int>> invoker(GenericFilterModel<TDTO> args, IReturnModel<int> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (args.WherePredicates.Any())
                {
                    foreach (var wherePre in args.WherePredicates)
                    {
                        query = query.Where(ExpressionFuncConverter<TPoco>.Convert(wherePre));
                    }
                }

                if (args.OrderPredicates.Any())
                {
                    foreach (var op in args.OrderPredicates)
                    {
                        var prop = typeof(TPoco).GetProperty(op.PropertyName);
                        if (op.Descending)
                            query = query.OrderByDescending(x => prop.GetValue(x, null));
                        else
                            query = query.OrderBy(x => prop.GetValue(x, null));
                    }
                }

                rtn.Result = await query.CountAsync(cancellationToken).ConfigureAwait(false);

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<int, GenericFilterModel<TDTO>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<int>> CountAsync(ActionFilterModel args, CancellationToken cancellationToken = default)
        {
            var actionName = "CountAsync";

            async Task<IReturnModel<int>> invoker(ActionFilterModel args, IReturnModel<int> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var query = Repository.GetAll();

                if (!string.IsNullOrWhiteSpace(args.Where))
                {
                    query = query.Where(args.Where);
                }

                if (!string.IsNullOrWhiteSpace(args.Order))
                {
                    query = query.OrderBy(args.Order);
                }

                rtn.Result = await query.CountAsync(cancellationToken).ConfigureAwait(false);

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithValidation<int, ActionFilterModel>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<int>> AddAsync(ServiceParamsWithIdentifier<TDTO> args, bool useSave = true, CancellationToken cancellationToken = default)
        {
            var actionName = "AddAsync";

            async Task<IReturnModel<int>> invoker(ServiceParamsWithIdentifier<TDTO> args, IReturnModel<int> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var convertPoco = mapper.Map<TPoco>(args.Param);
                convertPoco.Status = true;
                convertPoco.AddingDate = DateTime.Now;
                convertPoco.UpdatingDate = DateTime.Now;
                convertPoco.AddingUserId = args.UserId;
                convertPoco.UpdatingUserId = args.UserId;
                rtn.Result = await Repository.AddAsync(convertPoco, useSave, cancellationToken).ConfigureAwait(false);

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithServiceParamsWithIdentifier<int, ServiceParamsWithIdentifier<TDTO>, TDTO>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<int>> UpdateAsync(ServiceParamsWithIdentifier<TDTO> args, bool useSave = true, CancellationToken cancellationToken = default)
        {
            var actionName = "UpdateAsync";

            async Task<IReturnModel<int>> invoker(ServiceParamsWithIdentifier<TDTO> args, IReturnModel<int> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var convertPoco = mapper.Map<TPoco>(args.Param);
                convertPoco.UpdatingDate = DateTime.Now;
                convertPoco.UpdatingUserId = args.UserId;
                rtn.Result = await Repository.UpdateAsync(convertPoco, useSave, cancellationToken).ConfigureAwait(false);

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithServiceParamsWithIdentifier<int, ServiceParamsWithIdentifier<TDTO>, TDTO>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<bool>> ChangeStatusAsync(ServiceParamsWithIdentifier<ChangeStatusModel> args, bool useSave = true, CancellationToken cancellationToken = default)
        {
            var actionName = "ChangeStatusAsync";

            async Task<IReturnModel<bool>> invoker(ServiceParamsWithIdentifier<ChangeStatusModel> args, IReturnModel<bool> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var getData = await Repository.FindAsync(i => i.Id == args.Param.Id, true, cancellationToken).ConfigureAwait(false);
                if (getData == null)
                {
                    rtn = rtn.SendError(GlobalErrors.DataNotFound);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.DataNotFound, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                getData.Status = args.Param.Status;
                var effectedRows = await Repository.UpdateAsync(getData, useSave, cancellationToken).ConfigureAwait(false);
                if (effectedRows <= 0)
                {
                    rtn = rtn.SendError(GlobalErrors.DbNoEffectedRows);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.DbNoEffectedRows, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                rtn.Result = true;

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunActionWithServiceParamsWithIdentifier<bool, ServiceParamsWithIdentifier<ChangeStatusModel>, ChangeStatusModel>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<int>> AddRangeAsync(ServiceParamsWithIdentifier<List<TDTO>> args, bool withTransaction = true, CancellationToken cancellationToken = default)
        {
            var actionName = "AddRangeAsync";

            async Task<IReturnModel<int>> invoker(ServiceParamsWithIdentifier<List<TDTO>> args, IReturnModel<int> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var effectedRows = 0;

                if (args.Param != null)
                {
                    if (args.Param.Any())
                    {
                        if (withTransaction)
                        {
                            await worker.TransactionHelper.CreateTransactionAsync(cancellationToken).ConfigureAwait(false);
                        }

                        foreach (var item in args.Param)
                        {
                            var serviceParamsWithIdentifier = new ServiceParamsWithIdentifier<TDTO>(item, args.UserId, args.TokenId);
                            var exec = await AddAsync(serviceParamsWithIdentifier, false, cancellationToken).ConfigureAwait(false);
                            if (exec.ErrorInfo.Status)
                            {
                                if (withTransaction)
                                {
                                    await worker.TransactionHelper.RollbackAsync(cancellationToken).ConfigureAwait(false);
                                }
                                rtn.ErrorInfo = exec.ErrorInfo;
                                Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, rtn.ErrorInfo.Code, rtn.ErrorInfo.Message, logger, LogLevel.Error, args);
                                return rtn;
                            }

                            effectedRows += exec.Result;
                        }

                        await Repository.SaveAsync(cancellationToken).ConfigureAwait(false);

                        if (withTransaction)
                        {
                            await worker.TransactionHelper.CommitAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                }

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunAction<int, ServiceParamsWithIdentifier<List<TDTO>>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<int>> UpdateRangeAsync(ServiceParamsWithIdentifier<List<TDTO>> args, bool withTransaction = true, CancellationToken cancellationToken = default)
        {
            var actionName = "UpdateRangeAsync";

            async Task<IReturnModel<int>> invoker(ServiceParamsWithIdentifier<List<TDTO>> args, IReturnModel<int> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var effectedRows = 0;

                if (args.Param != null)
                {
                    if (args.Param.Any())
                    {
                        if (withTransaction)
                        {
                            await worker.TransactionHelper.CreateTransactionAsync(cancellationToken).ConfigureAwait(false);
                        }

                        foreach (var item in args.Param)
                        {
                            var serviceParamsWithIdentifier = new ServiceParamsWithIdentifier<TDTO>(item, args.UserId, args.TokenId);
                            var exec = await UpdateAsync(serviceParamsWithIdentifier, false, cancellationToken).ConfigureAwait(false);
                            if (exec.ErrorInfo.Status)
                            {
                                if (withTransaction)
                                {
                                    await worker.TransactionHelper.RollbackAsync(cancellationToken).ConfigureAwait(false);
                                }
                                rtn.ErrorInfo = exec.ErrorInfo;
                                Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, rtn.ErrorInfo.Code, rtn.ErrorInfo.Message, logger, LogLevel.Error, args);
                                return rtn;
                            }

                            effectedRows += exec.Result;
                        }

                        await Repository.SaveAsync(cancellationToken).ConfigureAwait(false);

                        if (withTransaction)
                        {
                            await worker.TransactionHelper.CommitAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                }

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunAction<int, ServiceParamsWithIdentifier<List<TDTO>>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }

        public virtual async Task<IReturnModel<int>> ChangeStatusRangeAsync(ServiceParamsWithIdentifier<List<ChangeStatusModel>> args, bool withTransaction = true, CancellationToken cancellationToken = default)
        {
            var actionName = "ChangeStatusRangeAsync";

            async Task<IReturnModel<int>> invoker(ServiceParamsWithIdentifier<List<ChangeStatusModel>> args, IReturnModel<int> rtn)
            {
                if (args == null)
                {
                    rtn = rtn.SendError(GlobalErrors.ArgsIsNull);
                    Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, GlobalErrors.ArgsIsNull, string.Empty, logger, LogLevel.Error, args);
                    return rtn;
                }

                var effectedRows = 0;

                if (args.Param != null)
                {
                    if (args.Param.Any())
                    {
                        if (withTransaction)
                        {
                            await worker.TransactionHelper.CreateTransactionAsync(cancellationToken).ConfigureAwait(false);
                        }

                        foreach (var item in args.Param)
                        {
                            var serviceParamsWithIdentifier = new ServiceParamsWithIdentifier<ChangeStatusModel>(item, args.UserId, args.TokenId);
                            var exec = await ChangeStatusAsync(serviceParamsWithIdentifier, false, cancellationToken).ConfigureAwait(false);
                            if (exec.ErrorInfo.Status)
                            {
                                if (withTransaction)
                                {
                                    await worker.TransactionHelper.RollbackAsync(cancellationToken).ConfigureAwait(false);
                                }
                                rtn.ErrorInfo = exec.ErrorInfo;
                                Tools.WriteLoggerForService(ModuleName, ServiceName, actionName, rtn.ErrorInfo.Code, rtn.ErrorInfo.Message, logger, LogLevel.Error, args);
                                return rtn;
                            }

                            effectedRows++;
                        }

                        await Repository.SaveAsync(cancellationToken).ConfigureAwait(false);

                        if (withTransaction)
                        {
                            await worker.TransactionHelper.CommitAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }
                }

                return rtn;
            }

            var rtn = await dynamicServiceAction.RunAction<int, ServiceParamsWithIdentifier<List<ChangeStatusModel>>>(args, actionName, ServiceName, ModuleName, invoker).ConfigureAwait(false);
            return rtn;
        }


        private bool disposed;

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
    }
}