namespace Api.Models.IOptionModel
{
    public class JwtOption
    {
        public string Key { get; set; }
        public int ExpiresInDays { get; set; }
        public string RefreshTokenExpiresInDays { get; set; }
        public string Issuer { get; set; }
        public string ClientUrl { get; set; }
        
    }
}
