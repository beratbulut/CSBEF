namespace CSBEF.Models.Interfaces
{
    public interface IErrorResult
    {
        string Message { get; set; }

        string Code { get; set; }

        bool Status { get; set; }

        bool StopAction { get; set; }
    }
}