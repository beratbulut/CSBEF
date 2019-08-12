using System;

namespace CSBEF.Core.Models.HubModels
{
    public class NotificationModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime AddDate { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public DateTime? ViewDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public bool ViewStatus { get; set; }
        public bool ReadStatus { get; set; }
    }
}