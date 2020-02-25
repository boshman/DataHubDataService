using System;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DataHubDataService.Models;

namespace DataHubFileService
{
    public class DbAccess
    {
        public static List<DataHubFile> GetFileList(string agencyID, string sourcePath)
        {
            List<DataHubFile> fileList = new List<DataHubFile>();

            using (var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1))
            {
                var qRequest = new QueryRequest
                {
                    TableName = "Files",
                    KeyConditionExpression = "agency_id = :aid and begins_with(file_path, :fp)",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {{":aid", new AttributeValue { N =  agencyID }}, { ":fp", new AttributeValue { S = sourcePath } } }
                };

                var qResponse = client.QueryAsync(qRequest).GetAwaiter().GetResult();

                foreach (Dictionary<string, AttributeValue> item in qResponse.Items)
                {
                    fileList.Add(new DataHubFile { AgencyID = Convert.ToInt32(item.GetValueOrDefault("agency_id").N), Path = item.GetValueOrDefault("file_path").S, UploadedBy = item.GetValueOrDefault("uploaded_by").S });
                }
            }

            return fileList;
        }

        public static void UploadFile(int agencyID, string filePath, string uploadedBy)
        {
            using (var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1))
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

                var response = client.PutItemAsync(request).GetAwaiter().GetResult();
            }
        }
    }
}