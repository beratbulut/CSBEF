using System;
using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models {
    public abstract class DTOModelBase : IDTOModelBase {
        #region Public Properties

        public int Id { get; set; }
        public bool Status { get; set; }
        public DateTime AddingDate { get; set; }
        public DateTime UpdatingDate { get; set; }
        public int AddingUserId { get; set; }
        public int UpdatingUserId { get; set; }

        #endregion Public Properties
    }
}