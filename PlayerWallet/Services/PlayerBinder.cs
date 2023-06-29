using PlayerWallet.Models;
using PlayerWallet.Services.ModelsDTO;
using static PlayerWallet.Services.ModelsDTO.Enums.TransactionEnum;

namespace PlayerWallet.Services
{
    public class PlayerBinder
    {
        internal static PlayerID GetPlayerID(Guid playerID)
        {
            return new PlayerID()
            {
                PlayerId = playerID,
            };
        }

        internal static BalanceViewModel GetBalanceViewModel(decimal balance)
        {
            return new BalanceViewModel()
            {
                Balance = balance,
            };
        }

        internal static TransactionResultModel GetTransactionResultModel(string state)
        {
            return new TransactionResultModel()
            {
                TransactionResult = state
            };
        }

        internal static PlayerTransactionListModel GetPlayerTransactionListModel(PlayerTransaction transaction)
        {
            return new PlayerTransactionListModel()
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                State = transaction.State,
                CreatedDate = transaction.CreatedDate
            };
        }

        internal static Wallet CreateWallet()
        {
            return new Wallet()
            {
                Balance = 0
            };
        }

        internal static PlayerTransaction CreateTransaction(TransactionCreateModel createTransaction, int walletId, string state)
        {
            return new PlayerTransaction()
            {
                Id = createTransaction.TransactionId,
                WalletId = walletId,
                Amount = createTransaction.Amount,
                TransactionType = createTransaction.Type,
                CreatedDate = DateTime.UtcNow,
                State = state
            };
        }

        internal static Player CreatePlayerWithWallet(PlayerWalletCreateModel createPlayer)
        {
            return new Player()
            {
                Id = new Guid(),
                Name = createPlayer.Name,
                Surname = createPlayer.Surname,
                Email = createPlayer.Email,
                Wallet = CreateWallet(),
            };
        }

        internal static void UpdateWalletBalance(Wallet wallet, TransactionCreateModel creditTransaction)
        {
            if(creditTransaction.Type == TransactionType.stake.ToString())
            {
                wallet.Balance -= creditTransaction.Amount;
            }
            else
            {
                wallet.Balance += creditTransaction.Amount;
            }
        }
    }
}
