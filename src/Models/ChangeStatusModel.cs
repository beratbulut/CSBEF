using System;
using System.ComponentModel.DataAnnotations;

namespace CSBEF.Models
{
    public class ChangeStatusModel
    {
        [Required(ErrorMessage = "ModelValidationError_IdRequired")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "ModelValidationError_IdIsZero")]
        public Guid Id { get; set; }
        public bool Status { get; set; }
    }
}