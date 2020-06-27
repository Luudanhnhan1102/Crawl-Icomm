using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    [Table("url_crawl_news_detail")]
    public partial class UrlCrawlNewsDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("url")]
        public string Url { get; set; }
        [Required]
        [Column("module")]
        [StringLength(50)]
        public string Module { get; set; }
        [Required]
        [Column("domain")]
        public string Domain { get; set; }
        [Column("status")]
        public int Status { get; set; }
        [Column("type")]
        public int Type { get; set; }
        [Column("publish_time", TypeName = "datetime")]
        public DateTime? PublishTime { get; set; }
        [Column("processed_time", TypeName = "datetime")]
        public DateTime? ProcessedTime { get; set; }
        [Column("duration")]
        public double? Duration { get; set; }
        [Column("message")]
        public string Message { get; set; }
        [Column("urlhashv1")]
        [StringLength(64)]
        public string Urlhashv1 { get; set; }
        public int? SourceType { get; set; }
    }
}
