using InsuranceClient.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceClient.helpers
{
    public class StorageHelpers
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudTableClient tableClient;
        private CloudQueueClient queueClient;



        public string ConnectionString
        {
            set
            {
                this.storageAccount = CloudStorageAccount.Parse(value);
                this.blobClient = storageAccount.CreateCloudBlobClient();
                this.tableClient = storageAccount.CreateCloudTableClient();
                this.queueClient = storageAccount.CreateCloudQueueClient();

            }
        }

        public async Task<string> UploadCustomerImageAsynch(string containerName, string imagepath)
        {
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            var image = Path.GetFileName(imagepath);
            var blob = container.GetBlockBlobReference(image);
            await blob.DeleteIfExistsAsync();
            await blob.UploadFromFileAsync(imagepath);
            return blob.Uri.AbsoluteUri;

        }

        public async Task<Customer> InsertCustAsync(string tableName, Customer cust)
        {
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            TableOperation insertOperation = TableOperation.Insert(cust);
            var result = await table.ExecuteAsync(insertOperation);
            return result.Result as Customer;
        }

        public async Task AddMessageAsync(String queueName, Customer customer)
        {
            var queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            var messageBody = JsonConvert.SerializeObject(customer);
            CloudQueueMessage message = new CloudQueueMessage(messageBody);
            await queue.AddMessageAsync(message, TimeSpan.FromDays(3), TimeSpan.Zero, null, null)
;        }

    }
}
