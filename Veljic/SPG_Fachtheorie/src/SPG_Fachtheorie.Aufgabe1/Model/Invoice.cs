using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Invoice : Entity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Invoice() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Invoice(int number, DateTime date, Customer customer, Employee employee)
        {
            Number = number;
            Date = date;
            Customer = customer;
            Employee = employee;
        }

        public int Number { get; set; }
        public DateTime Date { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public List<InvoiceItem> InvoiceItems { get; } = new();
    }

}