namespace Lib.Data
{
    public class DataProviderFactory
    {
        public IDataProvider GetDataProvider(string type)
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