using Bogus;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace SPG_Fachtheorie.Aufgabe1.Infrastructure
{
    public class InvoiceContext : DbContext
    {





        public record ArticleWithSalesDto(int Articlenumber, string Articlename, DateTime PurchaseDate, string Firstname, string Lastname);
        public record EmployeeWithSalesDto(int InvoiceNumber, DateTime InvoiceDate, string Firstname, string Lastname, decimal Price);



        public DbSet<User> Users => Set<User>();
        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Company> Companies => Set<Company>();

        public DbSet<Article> Articles => Set<Article>();

        public DbSet<Invoice> Invoices => Set<Invoice>();

        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

        private static readonly string[] _customerType = new[] { "B", "C" };


        // TODO: Add your DbSets
        public InvoiceContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().OwnsOne(c => c.Address);
            modelBuilder.Entity<Customer>().OwnsMany(c => c.Addresses, c =>
            {
                c.HasKey("Id");
                c.Property<int>("Id");
            });
            modelBuilder.Entity<User>().Property(u => u.Email)
                .HasConversion(objValue => objValue.Value, dbValue => new Email(dbValue));
            modelBuilder.Entity<Customer>().Property(c => c.Type)
                .HasConversion(
                    objValue => _customerType[(int)objValue],
                    dbValue => (CustomerType)Array.IndexOf(_customerType, dbValue)
                );




        }

        /// <summary>
        /// Listen Sie alle Artikel auf, die innerhalb eines bestimmten Zeitraumes gekauft wurden.
        /// Geben Sie Artikelnummer, Artikelname, Kaufdatum und den Vor- und Zunamen des Kunden aus.
        /// </summary>
        public List<ArticleWithSalesDto> GetArticleWithSalesInfo(DateTime from, DateTime to)
        {
            return InvoiceItems
                .Include(i => i.Article)
                .Include(i => i.Invoice)
                    .ThenInclude(inv => inv.Customer)
                .Where(i => i.Invoice.Date >= from && i.Invoice.Date <= to)
                .Select(i => new ArticleWithSalesDto(
                    i.Article.Number,
                    i.Article.Name,
                    i.Invoice.Date,
                    i.Invoice.Customer.FirstName,
                    i.Invoice.Customer.LastName
                ))
                .ToList();
        }



        /// <summary>
        /// Listen Sie alle Verkäufe auf, die ein bestimmter Mitarbeiter getätigt hat.
        /// Geben Sie die Rechnungsnummer, das Rechnungsdatum, den Vor- und Zunamen des Kunden
        /// und den Gesamtbetrag aus.
        /// </summary>
        public List<EmployeeWithSalesDto> GetEmployeeWithSales(int employeeId)
        {
            return Invoices
                    .Include(i => i.Employee)
            .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Article)
            .Where(i => i.Employee.Id == employeeId)
            .ToList()
            .Select(invoice => new EmployeeWithSalesDto(
                invoice.Number,
                invoice.Date,
                invoice.Employee.FirstName,
                invoice.Employee.LastName,
                invoice.InvoiceItems.Sum(item => item.Price)
            ))
            .ToList();

        }
    }
}