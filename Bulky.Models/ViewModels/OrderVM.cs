using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.Models.ViewModels
{
    public class OrderVM
    {
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        public List<ShoppingCart> ShoppingCartList { get; set; }
    }
}
