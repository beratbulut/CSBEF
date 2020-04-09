namespace CSBEF.Core.Interfaces {
    public interface IErrorResult {
        #region Properties

        string Message { get; set; }
        string Code { get; set; }
        bool Status { get; set; }

        #endregion Properties
    }
}