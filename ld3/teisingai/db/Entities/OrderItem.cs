using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Org.Ktu.T120B197.DBs.Entities;

[Index("book_id", Name = "ix_item_book")]
[Index("order_id", Name = "ix_item_order")]
public partial class OrderItem
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int id { get; set; }

    [Column(TypeName = "int(11)")]
    public int order_id { get; set; }

    [Column(TypeName = "int(11)")]
    public int book_id { get; set; }

    [Column(TypeName = "int(11)")]
    public int quantity { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal unit_price { get; set; }

    [ForeignKey("book_id")]
    [InverseProperty("OrderItem")]
    public virtual Book Book { get; set; }

    [ForeignKey("order_id")]
    [InverseProperty("OrderItem")]
    public virtual Orders Order { get; set; }
}
