using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class ApiCrawlConfig
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Domain { get; set; }
        [Required]
        [StringLength(50)]
        public string BaseUrl { get; set; }
        [StringLength(200)]
        public string UrlPath { get; set; }
        [StringLength(10)]
        public string RequestMethod { get; set; }
        public string RequestParam { get; set; }
        public string RequestHeader { get; set; }
        public string ConfigMapping { get; set; }
        public string ParameterConfig { get; set; }
        public int? Type { get; set; }
        public int Version { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }
    }
}
