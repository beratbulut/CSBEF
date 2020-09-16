using System;

namespace CSBEF.Interfaces
{
    public interface IReturnModel<TResult>
    {
        IErrorResult ErrorInfo { get; set; }
        TResult Result { get; set; }

        IReturnModel<TResult> SendError<T>(T errorInfo, bool stopAction = false) where T : struct, IConvertible;

        IReturnModel<TResult> SendError<T>(T errorInfo, TResult result, bool stopAction = false) where T : struct, IConvertible;

        IReturnModel<TResult> SendError(string message, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, TResult result, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, string code, bool stopAction = false);

        IReturnModel<TResult> SendError(string message, string code, TResult result, bool stopAction = false);

        IReturnModel<TResult> SendResult(TResult result);
    }
}