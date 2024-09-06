namespace ChessAPI.models
{ 
        public class PlayerPerformance
        {
            public int PlayerId { get; set; }
            public string FullName { get; set; }
            public int TotalMatchesPlayed { get; set; }
            public int MatchesWon { get; set; }
            public double WinPercentage { get; set; }
        }
    }


