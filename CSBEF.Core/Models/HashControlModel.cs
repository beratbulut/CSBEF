using System.ComponentModel.DataAnnotations;

namespace CSBEF.Core.Models {
    public class HashControlModel {
        [Required (ErrorMessage = "ModelValidationError_HashRequired")]
        public string Hash { get; set; }
    }
}