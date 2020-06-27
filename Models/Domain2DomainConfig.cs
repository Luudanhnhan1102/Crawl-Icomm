using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class Domain2DomainConfig
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Domain { get; set; }
        [Required]
        [StringLength(250)]
        public string DomainGetConfig { get; set; }
    }
}
