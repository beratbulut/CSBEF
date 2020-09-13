using System;

namespace CSBEF.Models.Interfaces
{
    public interface IReturnModel<TResult>
    {
        IErrorResult ErrorInfo { get; set; }
        TResult Result { get; set; }

        IReturnModel<TResult> SendError<T>(T errorInfo, bool stopAction = false) where T : struct, IConvertible;

        IReturnModel<TResult> SendError<T>(T errorInfo, Exception ex, bool stopAction = false) where T : struct, IConvertible;

        IReturnModel<TResult> SendError<T>(T errorInfo, Exception ex, TResult result, bool stopAction = false) where T : struct, IConvertible;

        IReturnModel<TResult> SendError(string message, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, Exception ex, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, Exception ex, TResult result, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, string code, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, string code, Exception ex, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, string code, Exception ex, TResult result, bool stopAction = false);

        IReturnModel<TResult> SendResult(TResult result);
    }
}