using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class ConfigBase
    {
        [Key]
        [StringLength(250)]
        public string Key { get; set; }
        [StringLength(250)]
        public string TypeCode { get; set; }
        [Column("_CreatedTime", TypeName = "datetime")]
        public DateTime CreatedTime { get; set; }
    }
}
