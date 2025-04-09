using Bogus;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    //[Collection("Sequential")]
    public class Aufgabe1Test
    {
        private InvoiceContext GetEmptyDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder()
                .UseSqlite(connection)
                .Options;

            var db = new InvoiceContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        private void GenerateFixtures(InvoiceContext db)
        {
            var artikelA = new Article("Velo 6er", 6m);
            var artikelB = new Article("Malboro Rot", 5.5m);
            var artikelC = new Article("Shisha Tabak", 15m);

            var company = new Company("Tabak Trafik", new Address("Pilgrambrücke 4", "Wien", "1050"));
            var employee = new Employee(company, "Uros", "Veljic", new Email("veljicros@gmail.com"));

            var customer = new Customer(
                new() { new Address("Spengergasse", "Wien", "1050") },
                CustomerType.B2C,
                null,
                "Robert",
                "Koffler",
                "roberto@gmail.com");

            var invoiceDate = new DateTime(2025, 8, 26, 0, 0, 0); // Konstantes Datum
            var invoice = new Invoice(1, invoiceDate, customer, employee);

            var invoiceItemA = new InvoiceItem(invoice, artikelA, 5);
            var invoiceItemB = new InvoiceItem(invoice, artikelB, 1);
            var invoiceItemC = new InvoiceItem(invoice, artikelC, 1);

            invoice.InvoiceItems.AddRange([invoiceItemA, invoiceItemB, invoiceItemC]);
            db.Invoices.Add(invoice);
            db.SaveChanges();
            db.ChangeTracker.Clear();
        }


        /// <summary>
        /// "Dummy Test". Läuft dann durch, wenn EF Core keine Exception liefert.
        /// Deswegen auch kein Assert.
        /// Sollte vor dem Schreiben der weiteren Tests geprüft werden.
        /// </summary>
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
            GenerateFixtures(db);
        }

        /// <summary>
        /// Prüft, ob der rich type Email in User korrekt gespeichert werden kann.
        /// </summary>
        [Fact]
        public void PersistRichTypesSuccessTest()
        {
            using var db = GetEmptyDbContext();
            GenerateFixtures(db);
            Assert.Equal("veljicros@gmail.com", db.Employees.First().Email);
        }
        /// <summary>
        /// Prüft, ob das enum CustomerType korrekt (mit B oder C) gespeichert werden kann.
        /// Hinweis: Erstelle einen Datensatz und lese diesen zurück. Ist der enum Wert korrekt?
        /// </summary>
        [Fact]
        public void PersistEnumSuccessTest()
        {


            using var db = GetEmptyDbContext();
            GenerateFixtures(db);
            Assert.Equal(CustomerType.B2C, db.Customers.First().Type);


        }

        /// <summary>
        /// Prüft, ob das Property Address in Company als value object korrekt gespeichert werden kann.
        /// </summary>
        [Fact]
        public void PersistValueObjectInCompanySuccessTest()
        {
            using var db = GetEmptyDbContext();
            GenerateFixtures(db);

            var company = db.Companies.First();

            Assert.NotNull(company.Address);
            Assert.Equal("1050", company.Address.Zip);
        }


        /// <summary>
        /// Prüft, ob ein Eintrag zur Liste von Adressen in Customer hinzugefügt und wieder
        /// gelesen werden kann.
        /// </summary>
        [Fact]
        public void PersistValueObjectInCustomerSuccessTest()
        {
            using var db = GetEmptyDbContext();
            GenerateFixtures(db);
            Assert.Equal("Spengergasse", db.Customers.First().Addresses.First().Street);
            Assert.Equal("Wien", db.Customers.First().Addresses.First().City);
            Assert.Equal("1050", db.Customers.First().Addresses.First().Zip);
        }

        /// <summary>
        /// Das Entity InvoiceItem soll in der Datenbank gespeichert werden.
        /// Es referenziert auf alle anderen Entities, deswegen reicht dieser eine Test,
        /// um die korrekte Speicherung aller Entities zu prüfen.
        /// </summary>
        [Fact]
        public void PersistInvoiceItemSuccessTest()
        {
            using var db = GetEmptyDbContext();
            GenerateFixtures(db);
            Assert.Equal(3, db.InvoiceItems.Count());



        }

        /// <summary>
        /// Unittest für die Methode GetArticleWithSalesInfo in InvoiceContext.
        /// </summary>
        [Fact]
        public void GetArticleWithSalesInfoSuccessTest()
        {
            using var db = GetEmptyDbContext();
            GenerateFixtures(db);

            var articles = db.GetArticleWithSalesInfo(
                new DateTime(2025, 8, 24, 12, 0, 0),
                new DateTime(2025, 8, 26, 12, 0, 0));

            Assert.Equal(3, articles.Count());

            var names = articles.Select(a => a.Articlename).ToList();
            Assert.Contains("Velo 6er", names);
            Assert.Contains("Malboro Rot", names);
            Assert.Contains("Shisha Tabak", names);
        }


        /// <summary>
        /// Unittest für die Methode GetEmployeeWithSales in InvoiceContext.
        /// </summary>
        [Fact]
        public void GetEmployeeWithSalesSuccessTest()
        {
            using var db = GetEmptyDbContext();
            GenerateFixtures(db);
            var sales = db.GetEmployeeWithSales(db.Employees.First().Id);
            Assert.True(sales.Count() >= 1);
        }

    }
}