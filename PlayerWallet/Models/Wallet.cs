namespace PlayerWallet.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }

        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public List<PlayerTransaction> Transactions { get; set; }
    }
}
