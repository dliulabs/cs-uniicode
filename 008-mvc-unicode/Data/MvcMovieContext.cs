using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Data {
    public class MvcMovieContext : DbContext {
        public MvcMovieContext (DbContextOptions<MvcMovieContext> options) : base (options) { }

        public DbSet<Movie> Movie { get; set; }

        protected override void OnModelCreating (ModelBuilder builder) {
            builder.Entity<Movie> (p => {
                p.Property (prop => prop.Title).IsUnicode (true).UseCollation ("Latin1_General_100_BIN2_UTF8");

            });
        }

        protected override void ConfigureConventions (ModelConfigurationBuilder configurationBuilder) {
            configurationBuilder
                .Properties<string> ()
                //.HaveColumnType("varchar") // not needed, see below
                .AreUnicode (false)
                .HaveMaxLength (100);
        }
    }
}