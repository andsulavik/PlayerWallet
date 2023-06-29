namespace PlayerWallet.Services.ModelsDTO
{
    public class PlayerIdWithFiltersModel : PlayerIdInputModel
    {
        public List<string> TransactionTypes { get; set; }
    }
}
