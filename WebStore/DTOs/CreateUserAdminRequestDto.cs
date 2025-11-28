namespace WebStore.DTOs
{
    public class CreateUserAdminRequestDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "simple";
        public bool IsClient { get; set; } = false;
    }
}
