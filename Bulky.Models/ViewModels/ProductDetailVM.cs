using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.Models.ViewModels
{

    public class ProductDetailVM
    {
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1, 1000, ErrorMessage = "Please Enter Quantity betwen 1 and 1000")]
        public int Count { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
