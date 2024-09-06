using ChessAPI.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChessAPI.models;

namespace ChessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChessController : ControllerBase
    {
        private readonly IChessDAO _chessDAO;

        public ChessController(IChessDAO chessDAO)
        {
            _chessDAO = chessDAO;
        }

        // 1. Add a new match
        [HttpPost("add-match")]
        public IActionResult AddMatch([FromBody] Match match)
        {
            if (match == null || match.Player1Id <= 0 || match.Player2Id <= 0)
            {
                return BadRequest("Invalid match data.");
            }

            _chessDAO.AddMatch(match);
            return Ok("Match added successfully.");
        }

        // 2. Retrieve all players from a specific country sorted by their current world ranking
        [HttpGet("players-by-country")]
        public IActionResult GetPlayersByCountry([FromQuery] string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return BadRequest("Country parameter is required.");
            }

            var players = _chessDAO.GetPlayersByCountry(country);
            return Ok(players);
        }

        // 3. Retrieve each player's performance in the matches they have played
        [HttpGet("player-performances")]
        public IActionResult GetPlayerPerformances()
        {
            var performances = _chessDAO.GetPlayerPerformances();
            return Ok(performances);
        }

        // 4. Retrieve a list of players who have won more matches than the average number of matches won by all players
        [HttpGet("top-players-above-average-wins")]
        public IActionResult GetTopPlayersAboveAverageWins()
        {
            var topPlayers = _chessDAO.GetTopPlayersAboveAverageWins();
            return Ok(topPlayers);
        }
    }
}
