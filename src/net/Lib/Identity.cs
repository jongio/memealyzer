using Azure.Identity;

namespace Lib
{
    public static class Identity
    {
        public static ChainedTokenCredential GetCredentialChain()	
        {
            return new ChainedTokenCredential(new EnvironmentCredential(), new AzureCliCredential(), new ManagedIdentityCredential());
        }
    }
}