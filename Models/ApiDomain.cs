using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class ApiDomain
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Domain { get; set; }
        public int Status { get; set; }
        public string CategoryIds { get; set; }
        public int? Interval { get; set; }
        public int? Priority { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastCrawlTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CrawlNextTime { get; set; }
        public int Proxy { get; set; }
        public int? SourceType { get; set; }
        public int? LastCrawlStatus { get; set; }
    }
}
