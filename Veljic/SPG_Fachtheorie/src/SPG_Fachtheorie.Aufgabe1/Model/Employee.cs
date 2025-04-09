namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Employee : User
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Employee() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Employee(Company company, string firstName, string lastName, Email email) : base(firstName, lastName, email)
        {
            Company = company;
        }
        public Company Company { get; set; }
    }

}