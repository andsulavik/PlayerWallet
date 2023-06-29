namespace PlayerWallet.Services.ModelsDTO.Enums
{
    public class TransactionEnum
    {
        public enum TransactionType
        {
            deposit,
            stake,
            win
        }

        public enum TransactionState
        {
            rejected,
            accepted
        }
    }
}
