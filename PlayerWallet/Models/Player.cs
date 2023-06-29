namespace PlayerWallet.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Wallet Wallet { get; set; } = null!;
    }
}
