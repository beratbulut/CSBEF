namespace CSBEF.Core.Models {
    public class ActionFilterModel : HashControlModel {
        public string Where { get; set; }
        public string Order { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}