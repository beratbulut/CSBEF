using System;

namespace CSBEF.Models.Interfaces
{
    public interface IDtoModelBase
    {
        Guid Id { get; set; }
        bool Status { get; set; }
        DateTime AddingDate { get; set; }
        DateTime UpdatingDate { get; set; }
        int AddingUserId { get; set; }
        int UpdatingUserId { get; set; }
    }
}