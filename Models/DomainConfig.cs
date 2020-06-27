using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class DomainConfig
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Domain { get; set; }
        [Required]
        [StringLength(250)]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
        [Column("_CreatedTime", TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }
    }
}
