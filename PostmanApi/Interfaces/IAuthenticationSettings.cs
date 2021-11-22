namespace PostmanApi.Interfaces
{
    public interface IAuthenticationSettings
    {
        string Issuer { get; set; }
        string Audience { get; set; }
        string Secret { get; set; }
        int ExpirationDays { get; set; }
    }
}
