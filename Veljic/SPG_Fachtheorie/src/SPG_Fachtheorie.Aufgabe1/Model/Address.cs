namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Address
    {
        public Address(string street, string city, string zip)
        {
            Street = street;
            City = city;
            Zip = zip;
        }

        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }

    }

}