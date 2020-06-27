using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class HistoryAutoUpdateDomain
    {
        [Key]
        public long Id { get; set; }
        public int DomainId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateTime { get; set; }
        public string Content { get; set; }
    }
}
