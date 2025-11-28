namespace WebStore.Models
{
    public class Client
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<User> Users { get; set; } = new List<User>(); 
    }
}
