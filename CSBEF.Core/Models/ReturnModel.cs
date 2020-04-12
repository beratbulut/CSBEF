using System;
using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSBEF.Core.Models {
    public class ReturnModel<TResult> : IReturnModel<TResult> {
        #region Dependencies

        private readonly ILogger<IReturnModel<bool>> _logger;

        #endregion Dependencies

        #region Public Properties

        public IErrorResult ErrorInfo { get; set; }
        public TResult Result { get; set; }

        #endregion Public Properties

        #region Construction

        public ReturnModel (ILogger<IReturnModel<bool>> logger) {
            _logger = logger;
            ErrorInfo = new ErrorResult ();
        }

        #endregion Construction

        #region Public Actions

        public IReturnModel<TResult> SendError<T> (T error)
        where T : struct,
        IConvertible {
            if (!typeof (T).IsEnum)
                return SendError (GlobalError.TechnicalError);

            ErrorInfo.Code = error.ToString ();
            ErrorInfo.Message = Enum.GetName (typeof (T), error);
            ErrorInfo.Status = true;

            _logger.LogError ("(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError<T> (T error, Exception ex)
        where T : struct,
        IConvertible {
            if (!typeof (T).IsEnum)
                return SendError (GlobalError.TechnicalError);

            ErrorInfo.Code = error.ToString ();
            ErrorInfo.Message = Enum.GetName (typeof (T), error);
            ErrorInfo.Status = true;

            _logger.LogError (ex, "(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError<T> (T error, Exception ex, TResult result)
        where T : struct,
        IConvertible {
            if (!typeof (T).IsEnum)
                return SendError (GlobalError.TechnicalError);

            ErrorInfo.Code = error.ToString ();
            ErrorInfo.Message = Enum.GetName (typeof (T), error);
            ErrorInfo.Status = true;
            Result = result;

            _logger.LogError (ex, "(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError (string message) {
            ErrorInfo.Code = string.Empty;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;

            _logger.LogError ("(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError (string message, Exception ex) {
            ErrorInfo.Code = string.Empty;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;

            _logger.LogError (ex, "(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError (string message, Exception ex, TResult result) {
            ErrorInfo.Code = string.Empty;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            Result = result;

            _logger.LogError (ex, "(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError (string message, string code) {
            ErrorInfo.Code = code;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;

            _logger.LogError ("(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError (string message, string code, Exception ex) {
            ErrorInfo.Code = code;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;

            _logger.LogError (ex, "(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendError (string message, string code, Exception ex, TResult result) {
            ErrorInfo.Code = code;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            Result = result;

            _logger.LogError (ex, "(" + ErrorInfo.Code + ")" + ErrorInfo.Message);

            return this;
        }

        public IReturnModel<TResult> SendResult (TResult result) {
            Result = result;

            return this;
        }

        #endregion Public Actions
    }
}