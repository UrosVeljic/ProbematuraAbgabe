using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }

}