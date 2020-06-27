using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class ApiCrawlLogs
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Domain { get; set; }
        [Required]
        [StringLength(50)]
        public string Category { get; set; }
        public int? SourceType { get; set; }
        public int Status { get; set; }
        public int? TotalPost { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }
        public int? TotalSuccess { get; set; }
        public int? TotalExits { get; set; }
    }
}
