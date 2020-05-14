using System;

namespace CSBEF.Core.Interfaces
{
    public interface IDTOModelBase
    {
        #region Properties

        int Id { get; set; }
        bool Status { get; set; }
        DateTime AddingDate { get; set; }
        DateTime UpdatingDate { get; set; }
        int AddingUserId { get; set; }
        int UpdatingUserId { get; set; }

        #endregion Properties
    }
}