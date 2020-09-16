using Microsoft.Extensions.Logging;

namespace CSBEF.Models
{
    public class WriteLoggerForService<T>
    {
        public string ModuleName { get; set; }
        public string ServiceName { get; set; }
        public string ActionName { get; set; }
        public T ErrorEnum { get; set; }
        public string ErrorDescription { get; set; }
        public ILogger LoggerInstance { get; set; }
        public LogLevel LogLevel { get; set; }
        public object Args { get; set; }
    }
}