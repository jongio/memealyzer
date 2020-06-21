using Azure.Identity;

namespace Lib
{
    public static class Identity
    {
        public static ChainedTokenCredential GetCredentialChain()
        {
            return new ChainedTokenCredential(new AzureCliCredential(), new ManagedIdentityCredential());
        }
    }
}
