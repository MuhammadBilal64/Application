using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Precision(18, 2)]
        public decimal OrderTotal { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever] 
        public ApplicationUser ApplicationUser { get; set; }
        [Display(Name="Order Status")]
        public string OrderStatus { get; set; } = "Pending";
        [Display(Name = "Payment Status")]

        public string PaymentStatus { get; set; } = "Pending";
        public string? SessionId { get; set; }  
        public string? PaymentIntentId { get; set; }

    }
}
