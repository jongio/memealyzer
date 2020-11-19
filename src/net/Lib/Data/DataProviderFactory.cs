using Lib.Data.Providers;

namespace Lib.Data
{
    public static class DataProviderFactory
    {
        public static IDataProvider Get(string type)
        {
            switch (type)
            {
                case "COSMOS_SQL":
                    return new CosmosDataProvider();
                case "STORAGE_TABLE":
                    return new TableDataProvider();
                default:
                    return null;
            }
        }
    }
}