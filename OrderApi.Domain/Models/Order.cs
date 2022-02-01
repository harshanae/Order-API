using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domain.Models
{
    public class Order: BaseClasses.BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public int CustomerId { get; set; }

        [ForeignKey("ShippingMethodId")]
        public virtual ShippingMethod ShippingMethod { get; set; }
        public int ShippingMethodId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal Total { get; set; }

        [Range(typeof(DateTime), "1/1/1900", "6/6/2079")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public IList<Order_Product> OrderProducts { get; set; }
    }
}
