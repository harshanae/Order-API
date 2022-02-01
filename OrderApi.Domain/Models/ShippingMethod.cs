using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Models
{
    public class ShippingMethod: BaseClasses.BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string ShippingMethodName { get; set; }
    }
}
