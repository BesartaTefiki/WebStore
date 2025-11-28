namespace WebStore.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;  

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

     
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
    }
}
