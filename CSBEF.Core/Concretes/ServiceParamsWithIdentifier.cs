namespace CSBEF.Core.Concretes
{
    public class ServiceParamsWithIdentifier<TParam>
    {
        public TParam Param { get; set; }
        public int UserId { get; set; }
        public int TokenId { get; set; }

        public ServiceParamsWithIdentifier(TParam param, int userId, int tokenId)
        {
            Param = param;
            UserId = userId;
            TokenId = tokenId;
        }
    }
}