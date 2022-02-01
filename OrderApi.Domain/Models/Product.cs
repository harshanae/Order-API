using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Models
{
    public class Product: BaseClasses.BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Items { get; set; }
        public IList<Order_Product> OrderProducts { get; set; }
    }
}
