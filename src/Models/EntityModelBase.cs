using System;
using System.ComponentModel.DataAnnotations;
using CSBEF.Models.Interfaces;

namespace CSBEF.Models
{
    public abstract class EntityModelBase : IEntityModelBase
    {
        [Key]
        public Guid Id { get; set; }

        public bool Status { get; set; }

        public DateTime AddingDate { get; set; }

        public DateTime UpdatingDate { get; set; }

        public int AddingUserId { get; set; }

        public int UpdatingUserId { get; set; }
    }
}