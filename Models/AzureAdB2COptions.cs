namespace MultiAuthSchemas.Controllers
{
    public class AzureAdB2COptions
    {
        public string Instance {get; set;}
        public string TenantId {get; set;}
        public string ClientId {get; set;}
        public string ClientSecret {get; set;}
    }
}