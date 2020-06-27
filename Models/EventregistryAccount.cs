using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    [Table("eventregistry_account")]
    public partial class EventregistryAccount
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("maill")]
        [StringLength(500)]
        public string Maill { get; set; }
        [Column("pass")]
        [StringLength(500)]
        public string Pass { get; set; }
        [Column("token")]
        [StringLength(500)]
        public string Token { get; set; }
        [Column("total_requests")]
        public int TotalRequests { get; set; }
        [Column("limit_request")]
        public int LimitRequest { get; set; }
    }
}
