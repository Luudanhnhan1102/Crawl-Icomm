using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    [Table("session_smcc")]
    public partial class SessionSmcc
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("session")]
        public string Session { get; set; }
        [Column("status")]
        public int Status { get; set; }
    }
}
