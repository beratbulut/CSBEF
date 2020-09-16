using System;
using CSBEF.Interfaces;

namespace CSBEF.Models
{
    public class DtoModelBase : IDtoModelBase
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public DateTime AddingDate { get; set; }
        public DateTime UpdatingDate { get; set; }
        public int AddingUserId { get; set; }
        public int UpdatingUserId { get; set; }
    }
}