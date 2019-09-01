namespace CSBEF.Core.Models.HubModels
{
    public class HubSyncDataModel<Tdata>
    {
        public string Key { get; set; }
        public string ProcessType { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string ExtraInfo1 { get; set; }
        public string ExtraInfo2 { get; set; }
        public string ExtraInfo3 { get; set; }
        public Tdata Data { get; set; }
    }
}
