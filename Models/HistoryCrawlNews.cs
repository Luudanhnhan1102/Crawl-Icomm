using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icomm.NewsCrawl.Website.Models
{
    [Table("history_crawl_news")]
    public partial class HistoryCrawlNews
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("url")]
        public string Url { get; set; }
        [Column("domain")]
        public string Domain { get; set; }
        [Column("crawl_date", TypeName = "datetime")]
        public DateTime CrawlDate { get; set; }
        [Column("crawl_source")]
        public string CrawlSource { get; set; }
        [Column("keyword_search")]
        public string KeywordSearch { get; set; }
    }
}
