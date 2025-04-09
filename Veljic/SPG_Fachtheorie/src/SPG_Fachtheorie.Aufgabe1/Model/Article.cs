using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Article
    {
        public Article(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        [Key]
        public int Number { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

}