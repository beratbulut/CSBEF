using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSBEF.Core.Enums;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CSBEF.Core.Concretes {
    public class FileUploader {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IReturnModel<bool>> _logger;
        private readonly List<string> _allowTypes;
        private readonly int _minSize;
        private readonly int _maxSize;
        private readonly string _path;

        public FileUploader (
            IConfiguration configuration,
            ILogger<IReturnModel<bool>> logger,
            List<string> allowTypes = null,
            int minSize = 0,
            int maxSize = 0,
            string path = ""
        ) {
            _configuration = configuration ??
                throw new ArgumentNullException (nameof (configuration));
            _logger = logger ??
                throw new ArgumentNullException (nameof (logger));

            if (allowTypes == null) {
                var conf_allowDefaultFileTypes = _configuration["AppSettings:FileUploader:allowDefaultFileTypes"];
                if (conf_allowDefaultFileTypes != null) {
                    var splitTypes = conf_allowDefaultFileTypes.Split (';').ToList ();
                    allowTypes = splitTypes;
                } else {
                    allowTypes = new List<string> {
                        "image/gif",
                        "image/jpeg",
                        "image/svg+xml"
                    };
                }
            }

            if (string.IsNullOrWhiteSpace (path))
                path = _configuration["AppSettings:FileUploader:defaultUploadPath"];

            _allowTypes = allowTypes;
            _minSize = minSize;
            _maxSize = maxSize;
            _path = path;
        }

        public IReturnModel<string> Upload (IFormFile file) {
            if (file == null)
                throw new ArgumentNullException (nameof (file));

            IReturnModel<string> rtn = new ReturnModel<string> (_logger);
            bool cnt = true;
            string newFilePath = string.Empty;

            try {
                if (!Directory.Exists (_path)) {
                    rtn = rtn.SendError (FileUploaderErrorsEnum.UploadUploadPathNotFound);
                    cnt = false;
                }

                if (cnt && _minSize > 0 && file.Length < _minSize) {
                    rtn = rtn.SendError (FileUploaderErrorsEnum.UploadMinSize);
                    cnt = false;
                }

                if (cnt && _minSize > 0 && file.Length > _maxSize) {
                    rtn = rtn.SendError (FileUploaderErrorsEnum.UploadMaxSize);
                    cnt = false;
                }

                if (cnt && !_allowTypes.Any (i => i == file.ContentType)) {
                    rtn = rtn.SendError (FileUploaderErrorsEnum.UploadContentType);
                    cnt = false;
                }

                if (cnt) {
                    var setting_guidDontUse = _configuration["AppSettings:FileUploader:fileName:guidDontUse"] != null && _configuration["AppSettings:FileUploader:fileName:guidDontUse"].ToBool2 ();
                    var setting_prepend = _configuration["AppSettings:FileUploader:fileName:prepend"] ?? string.Empty;
                    var setting_append = _configuration["AppSettings:FileUploader:fileName:append"] ?? string.Empty;
                    var setting_importantExt = _configuration["AppSettings:FileUploader:fileName:importantExt"] ?? string.Empty;

                    newFilePath = setting_prepend + (setting_guidDontUse ? file.FileName : Guid.NewGuid ().ToString ()) + setting_append + (!setting_guidDontUse ? Path.GetExtension (file.FileName) : string.Empty) + (!string.IsNullOrWhiteSpace (setting_importantExt) ? setting_importantExt : string.Empty);
                }

                if (cnt) {
                    using var stream = new FileStream (Path.Combine (_path, newFilePath), FileMode.Create);
                    file.CopyTo (stream);
                }

                if (cnt) {
                    rtn.Result = newFilePath;
                }
            } catch (CustomException ex) {
                rtn = rtn.SendError (GlobalError.TechnicalError, ex);
                rtn.Result = string.Empty;
            }

            return rtn;
        }
    }
}