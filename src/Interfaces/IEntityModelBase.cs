using System;

namespace CSBEF.Interfaces
{
    public interface IEntityModelBase
    {
        Guid Id { get; set; }

        bool Status { get; set; }

        DateTime AddingDate { get; set; }

        DateTime UpdatingDate { get; set; }

        int AddingUserId { get; set; }

        int UpdatingUserId { get; set; }
    }
}