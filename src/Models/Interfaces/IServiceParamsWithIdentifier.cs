namespace CSBEF.Models.Interfaces
{
    public interface IServiceParamsWithIdentifier<TParam>
    {
        TParam Param { get; set; }
        int UserId { get; set; }
        int TokenId { get; set; }
    }
}