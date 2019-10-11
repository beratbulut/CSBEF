using CSBEF.Core.Enums;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSBEF.Core.Concretes
{
    public class FileUploader
    {
        private IConfiguration _configuration;
        private ILogger<ILog> _logger;
        private List<string> _allowTypes;
        private int _minSize;
        private int _maxSize;
        private string _path;

        public FileUploader(
            IConfiguration configuration,
            ILogger<ILog> logger,
            List<string> allowTypes = null,
            int minSize = 0,
            int maxSize = 0,
            string path = ""
        )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (allowTypes == null)
            {
                var conf_allowDefaultFileTypes = _configuration["AppSettings:FileUploader:allowDefaultFileTypes"];
                if (conf_allowDefaultFileTypes != null)
                {
                    var splitTypes = conf_allowDefaultFileTypes.Split(';').ToList();
                    allowTypes = splitTypes;
                }
                else
                {
                    allowTypes = new List<string>
                    {
                        "image/gif",
                        "image/jpeg",
                        "image/svg+xml"
                    };
                }
            }

            if (string.IsNullOrWhiteSpace(path))
                path = _configuration["AppSettings:FileUploader:defaultUploadPath"];

            _allowTypes = allowTypes;
            _minSize = minSize;
            _maxSize = maxSize;
            _path = path;
        }

        public async Task<IReturnModel<string>> Upload(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            IReturnModel<string> rtn = new ReturnModel<string>(_logger);
            bool cnt = true;
            string newFilePath = "";

            try
            {
                if (!Directory.Exists(_path))
                {
                    rtn = rtn.SendError(FileUploaderErrorsEnum.Upload_UploadPathNotFound);
                    cnt = false;
                }

                if (cnt && _minSize > 0 && file.Length < _minSize)
                {
                    rtn = rtn.SendError(FileUploaderErrorsEnum.Upload_MinSize);
                    cnt = false;
                }

                if (cnt && _minSize > 0 && file.Length > _maxSize)
                {
                    rtn = rtn.SendError(FileUploaderErrorsEnum.Upload_MaxSize);
                    cnt = false;
                }

                if (cnt && !_allowTypes.Any(i => i == file.ContentType))
                {
                    rtn = rtn.SendError(FileUploaderErrorsEnum.Upload_ContentType);
                    cnt = false;
                }

                if (cnt)
                {
                    newFilePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
                }

                if (cnt)
                {
                    using (var stream = new FileStream(Path.Combine(_path, newFilePath), FileMode.Create))
                    {
                        await file.CopyToAsync(stream).ConfigureAwait(false);
                    }
                }

                if (cnt)
                {
                    rtn.Result = newFilePath;
                }
            }
            catch (Exception ex)
            {
                rtn = rtn.SendError(GlobalErrors.TechnicalError, ex);
                rtn.Result = "";
            }

            return rtn;
        }
    }
}