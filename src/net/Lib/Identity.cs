using Azure.Identity;

namespace Lib
{
    public static class Identity
    {
        public static ChainedTokenCredential GetCredentialChain()
        {
            return new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential()
            );
        }
    }
}