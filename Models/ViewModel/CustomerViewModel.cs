using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceClient.Models.ViewModel
{
    public class CustomerViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Amount { get; set; }
        public string premium { get; set; }

        public string insuranceType { get; set; }
        [DataType(DataType.Date)]
        public DateTime appDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }

        public IFormFile image { get; set; }
    }
}
