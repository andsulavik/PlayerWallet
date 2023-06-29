namespace PlayerWallet.Services.ModelsDTO
{
    public class PlayerTransactionListModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = null!;
        public string State { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
