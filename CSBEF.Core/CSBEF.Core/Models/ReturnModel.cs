using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace CSBEF.Core.Models
{
    public class ReturnModel<TResult> : IReturnModel<TResult>
    {
        #region Dependencies

        private ILogger<ILog> _logger;

        #endregion Dependencies

        #region Public Properties

        public IErrorResult Error { get; set; }
        public TResult Result { get; set; }

        #endregion Public Properties

        #region Construction

        public ReturnModel(ILogger<ILog> logger)
        {
            _logger = logger;
            Error = new ErrorResult();
        }

        #endregion Construction

        #region Public Actions

        public IReturnModel<TResult> SendError<TerrorsEnum>(TerrorsEnum error)
            where TerrorsEnum : struct, IConvertible
        {
            if (!typeof(TerrorsEnum).IsEnum)
                return SendError(GlobalErrors.TechnicalError);

            Error.Code = error.ToString();
            Error.Message = Enum.GetName(typeof(TerrorsEnum), error);
            Error.Status = true;

            _logger.LogError("(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError<TerrorsEnum>(TerrorsEnum error, Exception ex)
            where TerrorsEnum : struct, IConvertible
        {
            if (!typeof(TerrorsEnum).IsEnum)
                return SendError(GlobalErrors.TechnicalError);

            Error.Code = error.ToString();
            Error.Message = Enum.GetName(typeof(TerrorsEnum), error);
            Error.Status = true;

            _logger.LogError(ex, "(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError<TerrorsEnum>(TerrorsEnum error, Exception ex, TResult result)
            where TerrorsEnum : struct, IConvertible
        {
            if (!typeof(TerrorsEnum).IsEnum)
                return SendError(GlobalErrors.TechnicalError);

            Error.Code = error.ToString();
            Error.Message = Enum.GetName(typeof(TerrorsEnum), error);
            Error.Status = true;
            Result = result;

            _logger.LogError(ex, "(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError(string message)
        {
            Error.Code = "";
            Error.Message = message;
            Error.Status = true;

            _logger.LogError("(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError(string message, Exception ex)
        {
            Error.Code = "";
            Error.Message = message;
            Error.Status = true;

            _logger.LogError(ex, "(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError(string message, Exception ex, TResult result)
        {
            Error.Code = "";
            Error.Message = message;
            Error.Status = true;
            Result = result;

            _logger.LogError(ex, "(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError(string message, string code)
        {
            Error.Code = code;
            Error.Message = message;
            Error.Status = true;

            _logger.LogError("(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError(string message, string code, Exception ex)
        {
            Error.Code = code;
            Error.Message = message;
            Error.Status = true;

            _logger.LogError(ex, "(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendError(string message, string code, Exception ex, TResult result)
        {
            Error.Code = code;
            Error.Message = message;
            Error.Status = true;
            Result = result;

            _logger.LogError(ex, "(" + Error.Code + ")" + Error.Message);

            return this;
        }

        public IReturnModel<TResult> SendResult(TResult result)
        {
            Result = result;

            return this;
        }

        #endregion Public Actions
    }
}