using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class url_not_push_to_list
    {
        [Column("ID")]
        public int ID { get; set; }
        [Required]
        [Column("url")]
        [StringLength(250)]
        public string url { get; set; }
        [Required]
        [Column("domain")]
        [StringLength(50)]
        public string domain { get; set; }
        public int? SourceType { get; set; }
    }
}
