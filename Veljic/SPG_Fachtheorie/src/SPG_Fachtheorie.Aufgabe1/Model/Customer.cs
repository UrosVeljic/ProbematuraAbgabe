using System.Collections.Generic;

namespace SPG_Fachtheorie.Aufgabe1.Model
{

    public class Customer : User
    {
        protected Customer() { }

        public Customer(List<Address> addresses, CustomerType type, string? note, string firstName, string lastName, Email email) : base(firstName, lastName, email)
        {
            Addresses = addresses;
            Type = type;
            Note = note;
        }

        public List<Address> Addresses { get; } = new();
        public CustomerType Type { get; set; }
        public string? Note { get; set; }


    }

}