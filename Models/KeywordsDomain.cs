using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    [Table("keywords_domain")]
    public partial class KeywordsDomain
    {
        [Required]
        [StringLength(250)]
        public string Domain { get; set; }
        [Required]
        [Column("keywords")]
        [StringLength(250)]
        public string Keywords { get; set; }
        [Key]
        public int Id { get; set; }
    }
}
