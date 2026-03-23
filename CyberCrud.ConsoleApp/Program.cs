using CyberCrud.Data;
using CyberCrud.Domain;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

// ConsoleApp  ->  depends on  ->  Data  ->  depends on  ->  Domain
namespace CyberCrud.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Define the connection string for the database. This is a local SQL Server database instance.
            var connectionString = "data source=(localdb)\\MSSQLLocalDB;initial catalog=CyberCrudDbV2;integrated security=True;encrypt=False;TrustServerCertificate=True";

            // Create DbContextOptions for the AppDbContext using the connection string. 
            // This configures the DbContext to use SQL Server with the specified connection string.
            // DbContextOptions is a class that holds configuration options for the DbContext,
            // such as the database provider and connection string.
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            // Create an instance of the AppDbContext using the configured options.
            // The DbContext is responsible for managing the database connection and providing access to the database tables
            using var context = new AppDbContext(options);


            if (context.Database.EnsureCreated())
                Console.WriteLine("Database created successfully.");
            else
                Console.WriteLine("Database already exists.");

            // Create an instance of the VulnerabilityRepository, passing the DbContext to it.
            // The repository will use the DbContext to perform CRUD operations on the VulnerabilityRecords table in the database.
            var repository = new VulnerabilityRepository(context);

            // Seeding the database with some initial data if it's empty
            if (repository.GetAll().Any() == false)
            {
                repository.Create(new VulnerabilityRecord
                {
                    Title = "SQL Injection in Login Form",
                    Severity = "High",
                    AffectedSystem = "Web Application",
                    Status = "Open",
                    DateDiscovered = DateTime.Now.AddDays(-10)
                });
                repository.Create(new VulnerabilityRecord
                {
                    Title = "Cross-Site Scripting (XSS) in Comment Section",
                    Severity = "Medium",
                    AffectedSystem = "Web Application",
                    Status = "In Progress",
                    DateDiscovered = DateTime.Now.AddDays(-5)
                });

            }


            // Runing the display menu in a loop until the user chooses to exit
            bool running = true;

            while (running)
            {
                ShowMenu();
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": CreateVulnerability(repository); break;
                    case "2": ViewAllVulnerabilities(repository); break;
                    case "3": ViewVulnerabilityById(repository); break;
                    case "4": UpdateVulnerability(repository); break;
                    case "5": DeleteVulnerability(repository); break;
                    case "6": running = false; break;
                    default: Console.WriteLine("INVALID CHOICE!! Please Try Again"); break;
                }
            }

        } // End of Main method


        public static void ShowMenu()
        {
            Console.WriteLine("\n=== Cyber Vulnerability Tracker ===");
            Console.WriteLine("1. Create Vulnerability");
            Console.WriteLine("2. View All Vulnerabilities");
            Console.WriteLine("3. View Vulnerability by ID");
            Console.WriteLine("4. Update Vulnerability");
            Console.WriteLine("5. Delete Vulnerability");
            Console.WriteLine("6. Exit");
            Console.Write("\nEnter choice: ");
        }


        public static void CreateVulnerability(VulnerabilityRepository repository)
        {
            Console.Write("Title: ");
            var title = Console.ReadLine() ?? string.Empty;
            Console.Write("Severity (Low/Medium/High/Critical): ");
            var severity = Console.ReadLine() ?? string.Empty;
            Console.Write("Affected System: ");
            var affectedSystem = Console.ReadLine() ?? string.Empty;
            Console.Write("Status (Open/In Progress/Resolved): ");
            var status = Console.ReadLine() ?? string.Empty;

            var record = new VulnerabilityRecord
            {
                Title = title,
                Severity = severity,
                AffectedSystem = affectedSystem,
                Status = status,
                DateDiscovered = DateTime.Now
            };

            repository.Create(record);
            Console.WriteLine("Vulnerability created successfully.");
        }

        public static void ViewAllVulnerabilities(VulnerabilityRepository repository)
        {
            var records = repository.GetAll();
            if (!records.Any())
            {
                Console.WriteLine("No vulnerabilities found.");
                return;
            }

            Console.WriteLine("\n=== All Vulnerabilities ===");
            foreach (var record in records)
                Print(record);
        }

        public static void ViewVulnerabilityById(VulnerabilityRepository repository)
        {
            Console.Write("Enter Vulnerability ID:");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine("Invalid ID entered.");
                return;
            }

            var record = repository.GetById(id);
            if (record == null)
            {
                System.Console.WriteLine($"No vulnerability found with ID {id}.");
                return;
            }
            Print(record);
        }


        public static void UpdateVulnerability(VulnerabilityRepository repository)
        {
            Console.Write("Enter ID to update: ");
            var input = System.Console.ReadLine();
            if (!int.TryParse(input, out int id))
            {
                System.Console.WriteLine("Invalid ID.");
                return;
            }
            var existingRecord = repository.GetById(id);
            if (existingRecord == null)
            {
                System.Console.WriteLine($"No vulnerability found with ID {id}.");
                return;
            }
            Console.Write($"Title ({existingRecord.Title}): ");
            var title = Console.ReadLine();
            if (!string.IsNullOrEmpty(title))
                existingRecord.Title = title;

            Console.Write($"Severity ({existingRecord.Severity}): ");
            var severity = Console.ReadLine();
            if (!string.IsNullOrEmpty(severity))
                existingRecord.Severity = severity;

            Console.Write($"Affected System ({existingRecord.AffectedSystem}): ");
            var affectedSystem = Console.ReadLine();
            if (!string.IsNullOrEmpty(affectedSystem))
                existingRecord.AffectedSystem = affectedSystem;

            Console.Write($"Status ({existingRecord.Status}): ");
            var status = Console.ReadLine();
            if (!string.IsNullOrEmpty(status))
                existingRecord.Status = status;

            repository.Update(existingRecord);

            if (repository.Update(existingRecord))
                Console.WriteLine($"Record with ID {id} updated successfully!");
            else
                Console.WriteLine("Record update failed");
        }

        public static void DeleteVulnerability(VulnerabilityRepository repository)
        {
            Console.Write("Enter Vulnerability ID to delete:");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine("Invalid ID entered.");
                return;
            }
            
            var existingRecord = repository.GetById(id);
            if(existingRecord == null)
            {
                System.Console.WriteLine($"No vulnerability found with ID {id}.");
                return;
            }
            Print(existingRecord);
            if (repository.Delete(id))
                Console.WriteLine($"Record above with ID{id} deleted successfully!");
            else
                Console.WriteLine("Record deletion failed");
        }


        // ------------------------ Helper Method to Print a Vulnerability Record ----------------------------------
        static void Print(VulnerabilityRecord record)
        {
            System.Console.WriteLine("---------------------------------------");
            System.Console.WriteLine($"ID:               {record.Id}");
            System.Console.WriteLine($"Title:            {record.Title}");
            System.Console.WriteLine($"Severity:         {record.Severity}");
            System.Console.WriteLine($"Affected System:  {record.AffectedSystem}");
            System.Console.WriteLine($"Status:           {record.Status}");
            System.Console.WriteLine($"Discovered:       {record.DateDiscovered:MM-dd-yyyy}");
            System.Console.WriteLine("---------------------------------------");
        }



    }// End of Program class
}// End of namespace

