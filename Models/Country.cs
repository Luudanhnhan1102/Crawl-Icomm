using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    [Table("country")]
    public partial class Country
    {
        [Key]
        public int Id { get; set; }
        [Column("country_name_vi")]
        [StringLength(500)]
        public string country_name_vi { get; set; }
        [Column("country_name_en")]
        [StringLength(500)]
        public string country_name_en { get; set; }
        [Column("country_name_local")]
        [StringLength(500)]
        public string country_name_local { get; set; }
        [Required]
        [Column("country_code")]
        [StringLength(500)]
        public string country_code { get; set; }
        [Column("lang_code")]
        [StringLength(50)]
        public string LangCode { get; set; }
    }
}
