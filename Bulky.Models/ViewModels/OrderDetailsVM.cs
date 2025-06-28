using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookedIn.Models.ViewModels
{
   public class OrderDetailsVM
    {
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        public IEnumerable<OrderDetail> OrderDetailList { get; set; }
    }
}
