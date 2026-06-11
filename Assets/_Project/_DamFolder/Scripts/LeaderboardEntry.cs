using System;

namespace B70.Leaderboard
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string displayName;
        public bool isPlayer;
        public float score;
        public int rank;

        public LeaderboardEntry(string displayName, bool isPlayer)
        {
            this.displayName = displayName;
            this.isPlayer = isPlayer;
            this.score = 0f;
            this.rank = 0;
        }
    }
}