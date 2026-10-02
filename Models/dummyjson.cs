namespace PruebaChatbot.Models
{
    public class CartsRspuestas
    {
        public List<Carts> carts { get; set; }
    }

    public class Carts
    {
        public int id { get; set; }
        public List<Productos> products { get; set; }
    }

    public class Productos
    {
        public string title { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }

    public class Usuario
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
