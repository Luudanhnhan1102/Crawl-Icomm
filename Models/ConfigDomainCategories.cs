using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class ConfigDomainCategories
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Domain { get; set; }
        [StringLength(250)]
        public string DomainCate { get; set; }
        [StringLength(250)]
        public string Xpath1 { get; set; }
        [StringLength(250)]
        public string Xpath2 { get; set; }
        [StringLength(250)]
        public string Xpath3 { get; set; }
        [StringLength(250)]
        public string Xpath4 { get; set; }
        [StringLength(250)]
        public string Xpath5 { get; set; }
    }
}
