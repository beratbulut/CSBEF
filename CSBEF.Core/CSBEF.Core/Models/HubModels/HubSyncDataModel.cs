namespace CSBEF.Core.Models.HubModels
{
    public class HubSyncDataModel<T>
    {
        public string Key { get; set; }
        public string ProcessType { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string ExtraInfo1 { get; set; }
        public string ExtraInfo2 { get; set; }
        public string ExtraInfo3 { get; set; }
        public T Data { get; set; }
    }
}