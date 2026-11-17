using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Org.Ktu.T120B197.DBs.Entities;

[Index("customer_id", Name = "ix_orders_customer")]
public partial class Orders
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int id { get; set; }

    [Column(TypeName = "int(11)")]
    public int customer_id { get; set; }

    public DateOnly order_date { get; set; }

    [Required]
    [StringLength(20)]
    public string status { get; set; }

    [ForeignKey("customer_id")]
    [InverseProperty("Orders")]
    public virtual Customer Customer { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();
}
