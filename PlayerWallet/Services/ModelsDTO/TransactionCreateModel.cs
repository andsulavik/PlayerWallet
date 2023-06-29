namespace PlayerWallet.Services.ModelsDTO
{
    public class TransactionCreateModel : PlayerIdInputModel
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = null!;
    }
}
