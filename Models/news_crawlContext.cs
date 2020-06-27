using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Icomm.NewsCrawl.Website.Models
{
    public partial class news_crawlContext : DbContext
    {
        //public news_crawlContext()
        //{
        //}

        public news_crawlContext(DbContextOptions<news_crawlContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiCrawlConfig> ApiCrawlConfig { get; set; }
        public virtual DbSet<ApiCrawlLogs> ApiCrawlLogs { get; set; }
        public virtual DbSet<ApiDomain> ApiDomain { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<ConfigBase> ConfigBase { get; set; }
        public virtual DbSet<ConfigDomainCategories> ConfigDomainCategories { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Domain> Domain { get; set; }
        public virtual DbSet<Domain2DomainConfig> Domain2DomainConfig { get; set; }
        public virtual DbSet<DomainConfig> DomainConfig { get; set; }
        public virtual DbSet<DomainRssUrl> DomainRssUrl { get; set; }
        public virtual DbSet<DomainSearch> DomainSearch { get; set; }
        public virtual DbSet<EventregistryAccount> EventregistryAccount { get; set; }
        public virtual DbSet<HistoryAutoUpdateDomain> HistoryAutoUpdateDomain { get; set; }
        public virtual DbSet<HistoryCrawlNews> HistoryCrawlNews { get; set; }
        public virtual DbSet<KeywordsDomain> KeywordsDomain { get; set; }
        public virtual DbSet<NewsSample> NewsSample { get; set; }
        public virtual DbSet<SessionSmcc> SessionSmcc { get; set; }
        public virtual DbSet<start_url> start_url { get; set; }
        public virtual DbSet<url_crawl_list> UrlCrawlList { get; set; }
        public virtual DbSet<UrlCrawlNewsDetail> UrlCrawlNewsDetail { get; set; }
        public virtual DbSet<url_not_config> url_not_config { get; set; }
        public virtual DbSet<url_not_push_to_list> url_not_push_to_list { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiCrawlConfig>(entity =>
            {
                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RequestMethod).HasDefaultValueSql("(N'GET')");

                entity.Property(e => e.Version).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ApiCrawlLogs>(entity =>
            {
                entity.HasIndex(e => new { e.Domain, e.CreatedTime })
                    .HasName("NonClusteredIndex-20200518-173013");

                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ApiDomain>(entity =>
            {
                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.Property(e => e.DateTimeFormat).IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ConfigBase>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_RssConfigBase");

                entity.Property(e => e.Key).IsUnicode(false);

                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TypeCode).IsUnicode(false);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.LangCode).IsUnicode(false);
            });

            modelBuilder.Entity<Domain>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.Property(e => e.Domain1).IsUnicode(false);

                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.UpdateTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Domain2DomainConfig>(entity =>
            {
                entity.HasIndex(e => e.Domain)
                    .HasName("idx_unique")
                    .IsUnique();

                entity.Property(e => e.DomainGetConfig).IsUnicode(false);
            });

            modelBuilder.Entity<DomainConfig>(entity =>
            {
                entity.HasIndex(e => new { e.Domain, e.Key })
                    .HasName("idx_unique")
                    .IsUnique();

                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Domain).IsUnicode(false);

                entity.Property(e => e.Key).IsUnicode(false);
            });

            modelBuilder.Entity<DomainRssUrl>(entity =>
            {
                entity.HasIndex(e => e.RssUrl)
                    .HasName("idx_unique")
                    .IsUnique();

                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Domain).IsUnicode(false);

                entity.Property(e => e.RssUrl).IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<DomainSearch>(entity =>
            {
                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HistoryAutoUpdateDomain>(entity =>
            {
                entity.Property(e => e.UpdateTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HistoryCrawlNews>(entity =>
            {
                entity.Property(e => e.CrawlDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<KeywordsDomain>(entity =>
            {
                entity.Property(e => e.Domain).IsUnicode(false);
            });

            modelBuilder.Entity<NewsSample>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastUpdate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SessionSmcc>(entity =>
            {
                entity.Property(e => e.Session).IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<start_url>(entity =>
            {
                entity.HasIndex(e => e.domain)
                    .HasName("U_domain")
                    .IsUnique();
            });

            modelBuilder.Entity<url_crawl_list>(entity =>
            {
                entity.HasIndex(e => e.urlhashv1)
                    .HasName("IX_url_crawl_list_1")
                    .IsUnique();

                entity.HasIndex(e => new { e.Id, e.url, e.interval, e.urlhashv1, e.domain, e.schedule_time, e.status })
                    .HasName("IX_url_crawl_list");

                entity.Property(e => e.domain).IsUnicode(false);

                entity.Property(e => e.interval).HasDefaultValueSql("((7200))");

                entity.Property(e => e.MaxPage).HasDefaultValueSql("((5))");

                entity.Property(e => e.MinPage).HasDefaultValueSql("((2))");

                entity.Property(e => e.module)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('philippines')");

                entity.Property(e => e.schedule_time).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.status).HasDefaultValueSql("((1))");

                entity.Property(e => e.url).IsUnicode(false);

                entity.Property(e => e.urlhashv1).IsUnicode(false);
            });

            modelBuilder.Entity<UrlCrawlNewsDetail>(entity =>
            {
                entity.HasIndex(e => e.PublishTime)
                    .HasName("IX_publish_time");

                entity.HasIndex(e => new { e.Urlhashv1, e.Type })
                    .HasName("idx_urlhashv1")
                    .IsUnique();

                entity.Property(e => e.Domain).IsUnicode(false);

                entity.Property(e => e.Message).IsUnicode(false);

                entity.Property(e => e.Module).IsUnicode(false);

                entity.Property(e => e.Url).IsUnicode(false);

                entity.Property(e => e.Urlhashv1).IsUnicode(false);
            });

            //modelBuilder.Entity<url_not_config>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToView("url_not_config");
            //});

            //modelBuilder.Entity<url_not_push_to_list>(entity =>
            //{
            //    entity.HasNoKey();

            //    entity.ToView("url_not_push_to_list");
            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
