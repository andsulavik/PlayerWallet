using Microsoft.EntityFrameworkCore;
using PlayerWallet.Models;
using PlayerWallet.Services.ModelsDTO;
using System.Runtime.Caching;
using static PlayerWallet.Services.ModelsDTO.Enums.TransactionEnum;

namespace PlayerWallet.Services
{
    public class PlayerService
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;
        private static object lockObject = new object();

        public PlayerService()
        {
            using (var context = new ApiDbContext())
            {
                var players = new List<Player>
                {
                    new Player
                    {
                        Id = new Guid("7fe72f6b-befe-4b98-9ae5-719f495e2e1f"),
                        Name = "Andrej",
                        Surname = "Sulavii",
                        Email = "and.sulavik@gmail.com",
                        Wallet = new Wallet()
                        {
                            Balance = 9090,
                            Transactions = new List<PlayerTransaction>()
                            {
                                new PlayerTransaction {Id = new Guid("ff69bc62-083e-4ff1-8bad-c476d3d7d516") ,Amount = 120, TransactionType = "stake", State = "accepted", CreatedDate = DateTime.UtcNow },
                                new PlayerTransaction {Id = new Guid("901a7adb-42ae-4811-b856-f55827641266") ,Amount = 456.2m, TransactionType = "deposit", State = "accepted", CreatedDate = DateTime.UtcNow },
                                new PlayerTransaction {Id = new Guid("5b7ced5a-6497-4b57-a02d-9b51e8051303") ,Amount = 43.25m, TransactionType = "win", State = "accepted", CreatedDate = DateTime.UtcNow },
                                new PlayerTransaction {Id = new Guid("554a7045-5bb1-4e6e-aadf-1c7226e72654") ,Amount = 120.0m, TransactionType = "stake" , State = "rejected", CreatedDate = DateTime.UtcNow},
                            }
                        }
                    },
                    new Player
                    {
                        Id = new Guid("9de3753a-7dae-4e67-83f8-856fceb324e9"),
                        Name = "Stefan",
                        Surname = "Abrahamovic",
                        Email = "s.abram@gmail.com",
                        Wallet = new Wallet()
                        {
                            Balance = 120,
                            Transactions = new List<PlayerTransaction>()
                            {
                                new PlayerTransaction {Id = new Guid("0b0d2513-2f62-4cec-97c3-a13ec8c56a8d") ,Amount = 13.25m, TransactionType = "stake", State = "accepted", CreatedDate = DateTime.UtcNow },
                                new PlayerTransaction {Id = new Guid("ad9c5fa5-984b-436e-8dd8-7d7a224141ec") ,Amount = 12.21m, TransactionType = "stake", State = "accepted", CreatedDate = DateTime.UtcNow },
                            }
                        }
                    },
                    new Player
                    {
                        Id = new Guid("db1805bf-a527-44e5-8008-836fbb4fb83e"),
                        Name = "Patrick",
                        Surname = "Zeleny",
                        Email = "p.zel@gmail.com",
                        Wallet = new Wallet()
                        {
                            Balance = 0
                        }
                    }
                };
                context.Players.AddRange(players);
                context.SaveChanges();
            }
        }

        public async Task<List<Guid>> GetPlayerIdList()
        {
            using (var context = new ApiDbContext())
            {
                return await context.Players.AsNoTracking()
                                            .Select(x => x.Id)
                                            .ToListAsync();
            }
        }

        public async Task<BalanceViewModel?> GetPlayerBalance(Guid playerId)
        {
            using (var context = new ApiDbContext())
            {
                var cacheKey = playerId.ToString();
                var cachedValue = Cache.Get(cacheKey);

                if (cachedValue != null && cachedValue is decimal balance)
                {
                    return PlayerBinder.GetBalanceViewModel(balance);
                }

                var playerWallet = await context.Wallets.AsNoTracking()
                                                        .Where(x => x.PlayerId == playerId)
                                                        .SingleOrDefaultAsync();
                if (playerWallet == null)
                {
                    return null;
                }

                Cache.Add(cacheKey, playerWallet.Balance, new CacheItemPolicy { });

                return PlayerBinder.GetBalanceViewModel(playerWallet.Balance);
            }
        }

        public async Task<PlayerID?> RegisterPlayerWallet(PlayerWalletCreateModel createPlayerWallet)
        {
            using (var context = new ApiDbContext())
            {
                if (await context.Players.AnyAsync(x => x.Email == createPlayerWallet.Email))
                {
                    return null;
                }

                var player = PlayerBinder.CreatePlayerWithWallet(createPlayerWallet);
                context.Players.Add(player);

                await context.SaveChangesAsync();

                return PlayerBinder.GetPlayerID(player.Id);
            }
        }

        public TransactionResultModel? CreditTransaction(TransactionCreateModel creditTransaction)
        {
            var state = TransactionState.accepted.ToString();

            lock (lockObject)
            {
                using (var context = new ApiDbContext())
                {
                    var wallet = context.Wallets.Include(x => x.Transactions)
                                                .SingleOrDefault(x => x.PlayerId == creditTransaction.PlayerId);
                    if (wallet == null)
                    {
                        return null;
                    }

                    var existingTransaction = wallet.Transactions.SingleOrDefault(x => x.Id == creditTransaction.TransactionId);
                    if (existingTransaction != null)
                    {
                        return PlayerBinder.GetTransactionResultModel(existingTransaction.State);
                    }

                    if (creditTransaction.Type == TransactionType.stake.ToString())
                    {
                        if (wallet.Balance - creditTransaction.Amount < 0)
                        {
                            state = TransactionState.rejected.ToString();
                        }
                    }

                    var newTransaction = PlayerBinder.CreateTransaction(creditTransaction, wallet.Id, state);
                    wallet.Transactions.Add(newTransaction);

                    if (state != TransactionState.rejected.ToString())
                    {
                        UpdateWalletBalance(wallet, creditTransaction);
                    }

                    context.SaveChanges();

                    return PlayerBinder.GetTransactionResultModel(state);
                }
            }
        }

        public async Task<List<PlayerTransactionListModel>> GetTransactionList(PlayerIdWithFiltersModel getTransactions)
        {
            using (var context = new ApiDbContext())
            {
                return await context.Transactions.AsNoTracking()
                                                 .Where(x => x.Wallet.PlayerId == getTransactions.PlayerId && (!getTransactions.TransactionTypes.Any() || getTransactions.TransactionTypes.Contains(x.TransactionType)))
                                                 .Include(x => x.Wallet)
                                                 .Select(x => PlayerBinder.GetPlayerTransactionListModel(x))
                                                 .ToListAsync();
            }
        }

        public void UpdateCacheIfExisting(string playerId, decimal newBalance)
        {
            var cacheKey = playerId;
            var cachedValue = Cache.Get(cacheKey);

            if (cachedValue != null)
            {
                Cache.Set(cacheKey, newBalance, new CacheItemPolicy { });
            }
        }

        public void UpdateWalletBalance(Wallet wallet, TransactionCreateModel creditTransaction)
        {
            using (var context = new ApiDbContext())
            {
                PlayerBinder.UpdateWalletBalance(wallet, creditTransaction);
                UpdateCacheIfExisting(wallet.PlayerId.ToString(), wallet.Balance);
                context.Wallets.Update(wallet);
            }
        }
    }
}
