using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class Config
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Domain { get; set; }
        [StringLength(250)]
        public string XpathTitle { get; set; }
        [StringLength(250)]
        public string XpathTime { get; set; }
        public string XpathContentHtml { get; set; }
        [StringLength(250)]
        public string DateTimeFormat { get; set; }
        [StringLength(250)]
        public string RegexTime { get; set; }
        [StringLength(250)]
        public string RegexDate { get; set; }
        public string RegexIdFromUrl { get; set; }
        [StringLength(250)]
        public string XpathSnippet { get; set; }
        public int? Status { get; set; }
    }
}
