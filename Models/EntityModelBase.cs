using System;
using System.ComponentModel.DataAnnotations;
using CSBEF.Core.Interfaces;

namespace CSBEF.Core.Models {
    public abstract class EntityModelBase : IEntityModelBase {
        #region Public Properties

        [Key]
        public int Id { get; set; }

        public bool Status { get; set; }
        public DateTime AddingDate { get; set; }
        public DateTime UpdatingDate { get; set; }
        public int AddingUserId { get; set; }
        public int UpdatingUserId { get; set; }

        #endregion Public Properties
    }
}