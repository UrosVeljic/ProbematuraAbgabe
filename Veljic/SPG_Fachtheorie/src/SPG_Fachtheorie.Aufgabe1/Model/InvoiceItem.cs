namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class InvoiceItem : Entity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected InvoiceItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public InvoiceItem(Invoice invoice, Article article, int quantity)
        {
            Invoice = invoice;
            Article = article;
            Quantity = quantity;
        }

        public Invoice Invoice { get; set; }
        public Article Article { get; set; }
        public int Quantity { get; set; }
        public decimal Price => Article.Price * Quantity;
    }

}