using System;

namespace CSBEF.Core.Interfaces {
    public interface IReturnModel<TResult> {
        #region Properties

        IErrorResult ErrorInfo { get; set; }
        TResult Result { get; set; }

        #endregion Properties

        #region Actions

        IReturnModel<TResult> SendError<T> (T errorInfo) where T : struct,
        IConvertible;

        IReturnModel<TResult> SendError<T> (T errorInfo, Exception ex) where T : struct,
        IConvertible;

        IReturnModel<TResult> SendError<T> (T errorInfo, Exception ex, TResult result) where T : struct,
        IConvertible;

        IReturnModel<TResult> SendError (string message);

        IReturnModel<TResult> SendError (string message, Exception ex);

        IReturnModel<TResult> SendError (string message, Exception ex, TResult result);

        IReturnModel<TResult> SendError (string message, string code);

        IReturnModel<TResult> SendError (string message, string code, Exception ex);

        IReturnModel<TResult> SendError (string message, string code, Exception ex, TResult result);

        IReturnModel<TResult> SendResult (TResult result);

        #endregion Actions
    }
}