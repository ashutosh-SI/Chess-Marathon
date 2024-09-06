using ChessAPI.models;

namespace ChessAPI.DAO
{
    public interface IChessDAO
    {
        // Adds a new match to the database
        void AddMatch(Match match);

        // Retrieves all players from a specific country sorted by their current world ranking
        List<Player> GetPlayersByCountry(string country);

        // Retrieves each player's performance in the matches they have played
        List<PlayerPerformance> GetPlayerPerformances();

        // Retrieves a list of players who have won more matches than the average number of matches won by all players
        List<PlayerPerformance> GetTopPlayersAboveAverageWins();
    }
}
