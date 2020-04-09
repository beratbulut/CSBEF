﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CSBEF.Core.Models.HelperModels {
    public class ChangePictureModel {
        [Required (ErrorMessage = "ModelValidationError_IdRequired")]
        [Range (minimum: 1, maximum: int.MaxValue, ErrorMessage = "ModelValidationError_IdIsZero")]
        public int Id { get; set; }

        [Required (ErrorMessage = "ModelValidationError_PictureTypeRequired")]
        public string PictureType { get; set; }

        [Required (ErrorMessage = "ModelValidationError_PictureRequired")]
        public IFormFile Picture { get; set; }
    }
}