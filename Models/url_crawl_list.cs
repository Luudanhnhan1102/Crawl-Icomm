using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    [Table("url_crawl_list")]
    public partial class url_crawl_list
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("url")]
        [StringLength(250)]
        public string url { get; set; }
        [Column("interval")]
        public int interval { get; set; }
        [Column("schedule_time", TypeName = "datetime")]
        public DateTime schedule_time { get; set; }
        [Column("status")]
        public int status { get; set; }
        [Required]
        [Column("module")]
        [StringLength(50)]
        public string module { get; set; }
        [Column("domain")]
        [StringLength(50)]
        public string domain { get; set; }
        [Column("urlhashv1")]
        [StringLength(64)]
        public string urlhashv1 { get; set; }
        public int? SourceType { get; set; }
        public int MaxPage { get; set; }
        public int MinPage { get; set; }
        [StringLength(500)]
        public string PaternNextPage { get; set; }
    }
}
