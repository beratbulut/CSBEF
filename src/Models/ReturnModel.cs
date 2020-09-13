using System;
using CSBEF.enums;
using CSBEF.Models.Interfaces;

namespace CSBEF.Models
{
    public class ReturnModel<TResult> : IReturnModel<TResult>
    {
        public IErrorResult ErrorInfo { get; set; }
        public TResult Result { get; set; }

        public ReturnModel()
        {
            ErrorInfo = new ErrorResult();
        }

        public IReturnModel<TResult> SendError<T>(T error, bool stopAction = false)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return SendError(GlobalError.TechnicalError);

            ErrorInfo.Code = error.ToString();
            ErrorInfo.Message = Enum.GetName(typeof(T), error);
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError<T>(T error, Exception ex, bool stopAction = false)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return SendError(GlobalError.TechnicalError);

            ErrorInfo.Code = error.ToString();
            ErrorInfo.Message = Enum.GetName(typeof(T), error);
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError<T>(T error, Exception ex, TResult result, bool stopAction = false)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return SendError(GlobalError.TechnicalError);

            ErrorInfo.Code = error.ToString();
            ErrorInfo.Message = Enum.GetName(typeof(T), error);
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;
            Result = result;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError(string message, bool stopAction = false)
        {
            ErrorInfo.Code = "";
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError(string message, Exception ex, bool stopAction = false)
        {
            ErrorInfo.Code = "";
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError(string message, Exception ex, TResult result, bool stopAction = false)
        {
            ErrorInfo.Code = "";
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;
            Result = result;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError(string message, string code, bool stopAction = false)
        {
            ErrorInfo.Code = code;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError(string message, string code, Exception ex, bool stopAction = false)
        {
            ErrorInfo.Code = code;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendError(string message, string code, Exception ex, TResult result, bool stopAction = false)
        {
            ErrorInfo.Code = code;
            ErrorInfo.Message = message;
            ErrorInfo.Status = true;
            ErrorInfo.StopAction = stopAction;
            Result = result;

            if (stopAction)
            {
                throw new Exception(ErrorInfo.Message + " (" + ErrorInfo.Code + ")");
            }

            return this;
        }

        public IReturnModel<TResult> SendResult(TResult result)
        {
            Result = result;

            return this;
        }

    }
}