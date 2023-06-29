using Microsoft.AspNetCore.Mvc;
using PlayerWallet.Services;
using PlayerWallet.Services.ModelsDTO;

namespace PlayerWallet.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PlayerWalletController : ControllerBase
    {

        private readonly ILogger<PlayerWalletController> _logger;
        private readonly PlayerService _playerService;

        public PlayerWalletController(ILogger<PlayerWalletController> logger, PlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayerList()
        {
            var result = await _playerService.GetPlayerIdList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPlayerBalance([FromBody] PlayerIdInputModel model)
        {
            var result = await _playerService.GetPlayerBalance(model.PlayerId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPlayerWallet([FromBody] PlayerWalletCreateModel model)
        {
            var result = await _playerService.RegisterPlayerWallet(model);
            if (result == null)
            {
                ModelState.TryAddModelError("PlayerWallet", "email_already_registered");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> CreditTransaction([FromBody] TransactionCreateModel model)
        {
            var result = _playerService.CreditTransaction(model);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetTransactionList([FromBody] PlayerIdWithFiltersModel model)
        {
            var result = await _playerService.GetTransactionList(model);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}