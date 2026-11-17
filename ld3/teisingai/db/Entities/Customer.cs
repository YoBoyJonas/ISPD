using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Org.Ktu.T120B197.DBs.Entities;

[Index("email", Name = "uq_customer_email", IsUnique = true)]
public partial class Customer
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    public string name { get; set; }

    [Required]
    [StringLength(120)]
    public string email { get; set; }

    [Required]
    [StringLength(50)]
    public string city { get; set; }

    public DateOnly registered_at { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
}
