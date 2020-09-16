using CSBEF.Interfaces;

namespace CSBEF.Models
{
    public class AfterEventParameterModel<TDataToBeSent, TActionParameter> : IAfterEventParameterModel<TDataToBeSent, TActionParameter>
    {
        public TDataToBeSent DataToBeSent { get; set; }
        public TActionParameter ActionParameter { get; set; }
        public string ModuleName { get; set; }
        public string ServiceName { get; set; }
        public string ActionName { get; set; }
    }
}