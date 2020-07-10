using System;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace demo
{
    class Program
    {
        static async Task Main(string adminKey, string cognitiveKey, string connectionString, string endpoint)
        {
            var credential = new AzureKeyCredential(adminKey);
            var endpointUri = new Uri(endpoint);
            var indexClient = new SearchIndexClient(endpointUri, credential);
            var indexerClient = new SearchIndexerClient(endpointUri, credential);

            #region Create the index
            var index = new SearchIndex("demo")
            {
                Fields =
                {
                    new SimpleField("id", SearchFieldDataType.String) { IsKey = true },
                    new SearchableField("title") { AnalyzerName = LexicalAnalyzerName.EnLucene },
                    new SimpleField("url", SearchFieldDataType.String),
                    new SimpleField("blobUri", SearchFieldDataType.String),
                    new SearchableField("text") { AnalyzerName = LexicalAnalyzerName.EnLucene },
                    new SimpleField("sentiment", SearchFieldDataType.String) { IsFacetable = true, IsFilterable = true },
                    new SimpleField("status", SearchFieldDataType.String) { IsFacetable = true, IsFilterable = true },
                    new SimpleField("createdDate", SearchFieldDataType.DateTimeOffset) { IsFacetable = true, IsSortable = true },
                },
                CorsOptions = new CorsOptions(new[] { "*" })
                {
                    MaxAgeInSeconds = 60,
                },
            };

            await indexClient.CreateIndexAsync(index);
            #endregion

            #region Create the data source
            var dataSource = new SearchIndexerDataSourceConnection(
                "demostg",
                SearchIndexerDataSourceType.AzureBlob,
                connectionString,
                new SearchIndexerDataContainer("images"));

            await indexerClient.CreateDataSourceConnectionAsync(dataSource);
            #endregion

            #region Create the skillset to OCR scan and analyze sentiment the text
            var skillset = new SearchIndexerSkillset(
                "demoskillset",
                new SearchIndexerSkill[]
                {
                    // Scan text in the image (OCR).
                    new OcrSkill(
                        inputs: new[]
                        {
                            new InputFieldMappingEntry("image") { Source = "/document/normalized_images/*" },
                        },
                        outputs: new[]
                        {
                            new OutputFieldMappingEntry("text") { TargetName = "text" },
                        }
                    )
                    {
                        Context = "/document/normalized_images/*",
                        DefaultLanguageCode = OcrSkillLanguage.En,
                        ShouldDetectOrientation = false,
                    },

                    // Merge scanned text into a single document field.
                    new MergeSkill(
                        inputs: new[]
                        {
                            new InputFieldMappingEntry("text") { Source = "/document/content" },
                            new InputFieldMappingEntry("itemsToInsert") { Source = "/document/normalized_images/*/text" },
                            new InputFieldMappingEntry("offsets") { Source = "/document/normalized_images/*/contentOffset" },
                        },
                        outputs: new[]
                        {
                            new OutputFieldMappingEntry("mergedText") { TargetName = "merged_text" },
                        }
                    )
                    {
                        Context = "/document",
                    },

                    // Analyze sentiment of merged text (assume OCR text < 5,000 chars).
                    new SentimentSkill(
                        inputs: new[]
                        {
                            new InputFieldMappingEntry("text") { Source = "/document/merged_text" },
                        },
                        outputs: new[]
                        {
                            new OutputFieldMappingEntry("score") { TargetName = "sentiment_score" },
                        }
                    )
                    {
                        Context = "/document",
                        DefaultLanguageCode = SentimentSkillLanguage.En,
                    },

                    // Set textual sentiment based on sentiment score.
                    new ConditionalSkill(
                        inputs: new[]
                        {
                            new InputFieldMappingEntry("condition") { Source = "= $(/document/sentiment_score) >= 0.5" },
                            new InputFieldMappingEntry("whenTrue") { Source = "= 'positive'" },
                            new InputFieldMappingEntry("whenFalse") { Source = "= 'negative'" },
                        },
                        outputs: new[]
                        {
                            new OutputFieldMappingEntry("output") { TargetName = "sentiment" },
                        }
                    )
                    {
                        Context = "/document",
                    },
                }
            )
            {
                CognitiveServicesAccount = new CognitiveServicesAccountKey(cognitiveKey),
            };

            await indexerClient.CreateSkillsetAsync(skillset);
            #endregion

            #region Create the indexer
            var indexer = new SearchIndexer(
                "demoindexer",
                dataSource.Name,
                index.Name
            )
            {
                FieldMappings =
                {
                    new FieldMapping("metadata_storage_path")
                    {
                        TargetFieldName = "id",
                        MappingFunction = new FieldMappingFunction("base64Encode"),
                    },
                    new FieldMapping("metadata_storage_name") { TargetFieldName = "title" },
                    new FieldMapping("metadata_storage_path") { TargetFieldName = "blobUri"},
                    new FieldMapping("metadata_storage_last_modified") { TargetFieldName = "createdDate" },
                },
                OutputFieldMappings =
                {
                    new FieldMapping("/document/merged_text") { TargetFieldName = "text" },
                    new FieldMapping("/document/sentiment") { TargetFieldName = "sentiment" },
                },
                Parameters = new IndexingParameters
                {
                    Configuration =
                    {
                        ["dataToExtract"] = "contentAndMetadata",
                        ["parsingMode"] = "default",
                        ["imageAction"] = "generateNormalizedImages",
                        ["indexedFileNameExtensions"] = ".jpg,.png",
                    }
                },
                SkillsetName = skillset.Name,
            };

            await indexerClient.CreateIndexerAsync(indexer);
            #endregion
        }
    }
}
