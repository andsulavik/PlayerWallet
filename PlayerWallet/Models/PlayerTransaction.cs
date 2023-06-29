namespace PlayerWallet.Models
{
    public class PlayerTransaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = null!;
        public string State { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        public int WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;

    }
}
