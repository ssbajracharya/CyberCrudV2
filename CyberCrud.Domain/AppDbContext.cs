using Microsoft.EntityFrameworkCore;

namespace CyberCrud.Domain;

public class AppDbContext : DbContext
{
    // Constructor that takes DbContextOptions and passes it to the base class constructor
    // DbContextOptions is a class that contains configuration information for the DbContext, such as the database provider and connection string
    // Options are passed through dependency injection in console applications or web applications, allowing for flexible configuration of the DbContext without hardcoding connection details in the class itself
    // Generic type parameter AppDbContext specifies the type of the DbContext being configured.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // Empty constructor body, but we need the constructor to pass the options to the base class constructor
        // We can do validation or other setup here if needed
    }

    // Define a DbSet for the VulnerabilityRecord entity
    // Each DbSet represents a table in the database, and the type parameter specifies the entity type that will be stored in that table
    // One DbSet => One table in the database
    // The name of the DbSet property (VulnerabilityRecords) will be used as the name of the table in the database (Unless overridden)
    public DbSet<VulnerabilityRecord> VulnerabilityRecords { get; set; }
}