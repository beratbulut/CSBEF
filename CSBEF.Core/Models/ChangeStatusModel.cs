using System.ComponentModel.DataAnnotations;

namespace CSBEF.Core.Models
{
    public class ChangeStatusModel : HashControlModel
    {
        [Required(ErrorMessage = "ModelValidationError_IdRequired")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "ModelValidationError_IdIsZero")]
        public int Id { get; set; }
        public bool Status { get; set; }
    }
}
