using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Org.Ktu.T120B197.DBs.Entities;

public partial class Author
{
    [Key]
    [Column("id", TypeName = "int(11)")]
    public int id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("name")]
    public string name { get; set; }

    [Required]
    [StringLength(50)]
    [Column("country")]
    public string country { get; set; }

    [Column("birth_year", TypeName = "int(11)")]
    public int birth_year { get; set; }

    [InverseProperty("Author")]
    public virtual ICollection<Book> Book { get; set; } = new List<Book>();
}
