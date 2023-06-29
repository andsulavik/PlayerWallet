using PlayerWallet.Services;
using PlayerWallet.Services.ModelsDTO;
using static PlayerWallet.Services.ModelsDTO.Enums.TransactionEnum;

namespace Player.nUnitTests
{
    public class PlayerServiceTests
    {
        private static PlayerService _playerService = new PlayerService();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetPlayerBalance_NullTest()
        {
            //Arrange
            var playerId = new Guid();

            //Act
            var playerBalance = _playerService.GetPlayerBalance(playerId).Result;

            //Assert
            Assert.IsNull(playerBalance);
        }

        [Test]
        public void GetPlayerBalance_EqualValueTest()
        {
            //Arrange
            var playerId = new Guid("7fe72f6b-befe-4b98-9ae5-719f495e2e1f");

            //Act
            var playerBalance = _playerService.GetPlayerBalance(playerId).Result;

            //Assert
            Assert.That(playerBalance?.Balance, Is.EqualTo(9090));
        }

        [Test]
        public void CreditTransaction_RejectedTest()
        {
            //Arrange
            var creditTransaction = new TransactionCreateModel()
            {
                PlayerId = new Guid("7fe72f6b-befe-4b98-9ae5-719f495e2e1f"),
                TransactionId = new Guid(),
                Amount = 100000,
                Type = TransactionType.stake.ToString()
            };

            //Act
            var transactionResult = _playerService.CreditTransaction(creditTransaction);

            //Assert
            Assert.That(TransactionState.rejected.ToString(), Is.EqualTo(transactionResult?.TransactionResult));
        }
    }
}