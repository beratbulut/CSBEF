using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models {
    public class ErrorResult : IErrorResult {
        #region Public Properties

        public string Message { get; set; }
        public string Code { get; set; }
        public bool Status { get; set; }

        #endregion Public Properties
    }
}