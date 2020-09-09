using CSBEF.Models.Interfaces;

namespace CSBEF.Models
{
    public class ErrorResult : IErrorResult
    {
        #region Public Properties

        public string Message { get; set; }
        public string Code { get; set; }
        public bool Status { get; set; }
        public bool StopAction { get; set; }

        #endregion Public Properties
    }
}