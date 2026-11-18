using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Org.Ktu.T120B197.DBs.Entities;

[Index("author_id", Name = "ix_book_author")]
[Index("isbn", Name = "uq_book_isbn", IsUnique = true)]
public partial class Book
{
    [Key]
    [Column("id", TypeName = "int(11)")]
    public int id { get; set; }

    [Required]
    [StringLength(150)]
    [Column("title")]
    public string title { get; set; }

    [Required]
    [StringLength(13)]
    [Column("isbn", TypeName = "char(13)")]
    public string isbn { get; set; }

    [Required]
    [StringLength(30)]
    [Column("genre")]
    public string genre { get; set; }

    [Column("price", TypeName = "decimal(6,2)")]
    public decimal price { get; set; }

    [Column("pages", TypeName = "int(11)")]
    public int pages { get; set; }

    [Column("publish_year", TypeName = "int(11)")]
    public int publish_year { get; set; }

    [Column("author_id", TypeName = "int(11)")]
    public int author_id { get; set; }

    [ForeignKey("author_id")]
    [InverseProperty("Book")]
    public virtual Author Author { get; set; }

    [InverseProperty("Book")]
    public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();
}
