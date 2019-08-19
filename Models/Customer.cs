using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceClient.Models
{
    public class Customer :TableEntity
    {
        public Customer(string customerId, string insuranceType)
        {
            this.RowKey = customerId;
            this.PartitionKey = insuranceType;
        }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Amount { get; set; }
        public string premium { get; set; }
        public DateTime appDate { get; set; }
        public DateTime endDate { get; set; }

        public string imageUrl { get; set; }
    }
}
