namespace CSBEF.Core.Interfaces {
    public interface IAfterEventParameterModel<TDataToBeSent, TActionParameter> {
        TDataToBeSent DataToBeSent { get; set; }
        TActionParameter ActionParameter { get; set; }
        string ModuleName { get; set; }
        string ServiceName { get; set; }
        string ActionName { get; set; }
    }
}