using ChessAPI.models;
using Npgsql;

namespace ChessAPI.DAO
{
    public class ChessDAOImpl : IChessDAO
    {
        private readonly string _connectionString;

        public ChessDAOImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChessDatabase");
        }

        public void AddMatch(Match match)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO chess.Matches (player1_id, player2_id, match_date, match_level, winner_id) VALUES (@Player1Id, @Player2Id, @MatchDate, @MatchLevel, @WinnerId)", connection))
                {
                    command.Parameters.AddWithValue("@Player1Id", match.Player1Id);
                    command.Parameters.AddWithValue("@Player2Id", match.Player2Id);
                    command.Parameters.AddWithValue("@MatchDate", match.MatchDate);
                    command.Parameters.AddWithValue("@MatchLevel", match.MatchLevel);
                    command.Parameters.AddWithValue("@WinnerId", match.WinnerId ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Player> GetPlayersByCountry(string country)
        {
            var players = new List<Player>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM chess.Players WHERE country = @Country ORDER BY current_world_ranking", connection))
                {
                    command.Parameters.AddWithValue("@Country", country);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            players.Add(new Player
                            {
                                PlayerId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Country = reader.GetString(3),
                                CurrentWorldRanking = reader.GetInt32(4),
                                TotalMatchesPlayed = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return players;
        }

        public List<PlayerPerformance> GetPlayerPerformances()
        {
            var performances = new List<PlayerPerformance>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(
                    "SELECT p.player_id, p.first_name, p.last_name, COUNT(m.match_id) AS total_matches, " +
                    "COUNT(CASE WHEN m.winner_id = p.player_id THEN 1 END) AS matches_won, " +
                    "(CAST(COUNT(CASE WHEN m.winner_id = p.player_id THEN 1 END) AS FLOAT) / COUNT(m.match_id)) * 100 AS win_percentage " +
                    "FROM chess.Players p LEFT JOIN Matches m ON p.player_id = m.player1_id OR p.player_id = m.player2_id " +
                    "GROUP BY p.player_id, p.first_name, p.last_name", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            performances.Add(new PlayerPerformance
                            {
                                PlayerId = reader.GetInt32(0),
                                FullName = $"{reader.GetString(1)} {reader.GetString(2)}",
                                TotalMatchesPlayed = reader.GetInt32(3),
                                MatchesWon = reader.GetInt32(4),
                                WinPercentage = reader.GetDouble(5)
                            });
                        }
                    }
                }
            }
            return performances;
        }

        public List<PlayerPerformance> GetTopPlayersAboveAverageWins()
        {
            var topPlayers = new List<PlayerPerformance>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(
                    "WITH PlayerWinStats AS (" +
                    "SELECT p.player_id, p.first_name, p.last_name, COUNT(m.match_id) AS total_matches, " +
                    "COUNT(CASE WHEN m.winner_id = p.player_id THEN 1 END) AS matches_won, " +
                    "(CAST(COUNT(CASE WHEN m.winner_id = p.player_id THEN 1 END) AS FLOAT) / COUNT(m.match_id)) * 100 AS win_percentage " +
                    "FROM chess.Players p LEFT JOIN Matches m ON p.player_id = m.player1_id OR p.player_id = m.player2_id " +
                    "GROUP BY p.player_id, p.first_name, p.last_name) " +
                    "SELECT * FROM PlayerWinStats WHERE matches_won > (SELECT AVG(matches_won) FROM PlayerWinStats)", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            topPlayers.Add(new PlayerPerformance
                            {
                                PlayerId = reader.GetInt32(0),
                                FullName = $"{reader.GetString(1)} {reader.GetString(2)}",
                                TotalMatchesPlayed = reader.GetInt32(3),
                                MatchesWon = reader.GetInt32(4),
                                WinPercentage = reader.GetDouble(5)
                            });
                        }
                    }
                }
            }
            return topPlayers;
        }
    }
}
