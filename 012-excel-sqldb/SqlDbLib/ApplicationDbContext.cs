using System;
using Microsoft.EntityFrameworkCore;

namespace SqlDbLib;
public class ApplicationDbContext : DbContext {
	//Add a default constructor if scaffolding is needed
	public ApplicationDbContext () { }

	//Add the complex constructor for allowing Dependency Injection
	public ApplicationDbContext (DbContextOptions options) : base (options) {
		//intentionally empty. 
	}

	public DbSet<Entities.TrialBalance> TrialBalance { get; set; }
	protected override void OnModelCreating (ModelBuilder builder) {
		builder.Entity<Entities.TrialBalance> (p => {
			p.HasKey (k => k.Id);
			p.Property (prop => prop.Description).IsUnicode (true).UseCollation ("Latin1_General_100_BIN2_UTF8").HasMaxLength (100);

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