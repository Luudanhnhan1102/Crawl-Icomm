using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class DomainSearch
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Domain { get; set; }
        [Required]
        [StringLength(500)]
        public string SearchUrl { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }
    }
}
