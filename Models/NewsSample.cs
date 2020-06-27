using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class NewsSample
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Domain { get; set; }
        [Required]
        public string Url { get; set; }
        [Column(TypeName = "ntext")]
        public string Keywords { get; set; }
        [Column(TypeName = "ntext")]
        public string Title { get; set; }
        [Column(TypeName = "ntext")]
        public string Intro { get; set; }
        [Column(TypeName = "ntext")]
        public string Content { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NewsTime { get; set; }
        public string ActorName { get; set; }
        [Column("PId")]
        public string Pid { get; set; }
        [Column("_CreateDate", TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column("_LastUpdate", TypeName = "datetime")]
        public DateTime LastUpdate { get; set; }
    }
}
