using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DataHubDataService.Models;

namespace DataHubFileService
{
    public class DbAccess
    {

        public static AmazonDynamoDBClient GetAmazonDynamoDBClient()
        {
            AmazonDynamoDBClient client;

            string? accessKeyID = System.Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            string? secretyAccessKey = System.Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

            if (accessKeyID != null)
            {
                client = new AmazonDynamoDBClient(accessKeyID, secretyAccessKey, RegionEndpoint.USEast1);
            }
            else
            {
                client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            }

            return client;
        }
        public static async Task<List<DataHubFile>> GetFileList(string agencyID, string sourcePath)
        {
            List<DataHubFile> fileList = new List<DataHubFile>();

            using (var client = GetAmazonDynamoDBClient())
            {
                var qRequest = new QueryRequest
                {
                    TableName = "Files",
                    KeyConditionExpression = "agency_id = :aid and begins_with(file_path, :fp)",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> { { ":aid", new AttributeValue { N = agencyID } }, { ":fp", new AttributeValue { S = sourcePath } } }
                };


                var qResponse = await client.QueryAsync(qRequest);

                foreach (Dictionary<string, AttributeValue> item in qResponse.Items)
                {
                    fileList.Add(new DataHubFile { AgencyID = Convert.ToInt32(item.GetValueOrDefault("agency_id").N), Path = item.GetValueOrDefault("file_path").S, UploadedBy = item.GetValueOrDefault("uploaded_by").S });
                }
            }

            return fileList;
        }

        public static async Task<string> UploadFile(int agencyID, string filePath, string uploadedBy)
        {
            string result = string.Empty;
            using (var client = GetAmazonDynamoDBClient())
            {
                var request = new PutItemRequest
                {
                    TableName = "Files",
                    Item = new Dictionary<string, AttributeValue> {{ "agency_id", new AttributeValue { N = agencyID.ToString() }}
                        , { "file_path", new AttributeValue { S = filePath }}
                        , { "uploaded_by", new AttributeValue { S = uploadedBy }}
                        , { "upload_date", new AttributeValue { S = DateTime.Now.ToString("O") }}
                        , { "published", new AttributeValue { BOOL = true }}}
                };

                var response = await client.PutItemAsync(request);
                //result = response.HttpStatusCode.ToString();
            }
            return result;
        }

        public static async Task<string> DeleteFile(string agencyID, string filePath)
        {
            string result = string.Empty;

            using (var client = GetAmazonDynamoDBClient())
            {
                var request = new DeleteItemRequest
                {
                    TableName = "Files",
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "agency_id", new AttributeValue { N = agencyID.ToString() } },
                        { "file_path", new AttributeValue { S = filePath } }
                    }
                };

                var response = await client.DeleteItemAsync(request);
            }

            return result;
        }
    }
}