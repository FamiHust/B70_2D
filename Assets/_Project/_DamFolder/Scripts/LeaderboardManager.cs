using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace B70.Leaderboard
{
    /// <summary>
    /// Manages one leaderboard session (resets on game-over via ResetSession()).
    /// Hook: SceneManager.CompleteSemesterCoroutine should call
    ///       LeaderboardManager.instance.OnSemesterCompleted() at its end.
    ///
    /// UI API (read-only):
    ///   List<LeaderboardEntry> GetRankedEntries()
    ///   LeaderboardEntry       GetPlayerEntry()
    ///   int                    GetPlayerRank()
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager instance { get; private set; }

        // ── Bot configuration ──────────────────────────────────────────────
        // Each bot has a base score it starts with and a growth rate per semester.
        // Growth is exponential: botScore += baseGrowth * (1 + aggressionMultiplier)^semester
        // This means early semesters are slow, later semesters bots ramp hard.
        private static readonly BotConfig[] BOT_CONFIGS = new BotConfig[]
        {
            new BotConfig("UETệ", baseScore:  80f,  growthPerSem:  55f, aggression: 0.08f),
            new BotConfig("PTIT", baseScore:  60f,  growthPerSem:  70f, aggression: 0.10f),
            new BotConfig("Bắc Đại", baseScore: 120f,  growthPerSem:  45f, aggression: 0.06f),
            new BotConfig("NEU", baseScore:  40f,  growthPerSem:  90f, aggression: 0.12f),
            new BotConfig("Harvard", baseScore: 200f,  growthPerSem:  35f, aggression: 0.05f),
            new BotConfig("HUCE", baseScore:  30f,  growthPerSem: 110f, aggression: 0.15f),
            new BotConfig("Thanh Hoa", baseScore: 150f,  growthPerSem:  60f, aggression: 0.09f),
        };

        // ── Runtime state ──────────────────────────────────────────────────
        private LeaderboardEntry _playerEntry;
        private List<BotEntry> _botEntries = new List<BotEntry>();
        private List<LeaderboardEntry> _ranked = new List<LeaderboardEntry>();
        private int _semesterCount = 0;
        private bool _sessionActive = false;

        // ── Unity lifecycle ────────────────────────────────────────────────
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        // ── Public API ─────────────────────────────────────────────────────

        /// <summary>
        /// Call once when the player starts a new game (EnterNormalMode).
        /// </summary>
        public void StartSession()
        {
            _semesterCount = 0;
            _sessionActive = true;

            _playerEntry = new LeaderboardEntry("Player", isPlayer: true);

            _botEntries.Clear();
            foreach (var cfg in BOT_CONFIGS)
            {
                _botEntries.Add(new BotEntry(cfg));
            }

            RefreshRankings();
            Debug.Log("[Leaderboard] Session started.");
        }

        /// <summary>
        /// Called by SceneManager at the END of CompleteSemesterCoroutine.
        /// Updates player score from live resources, grows bot scores, rebuilds rankings.
        /// </summary>
        public void OnSemesterCompleted()
        {
            if (!_sessionActive) return;

            _semesterCount++;

            // Player score from real resources
            _playerEntry.score = LeaderboardScoreFormula.CalculateFromScene();

            // Bot growth
            foreach (var bot in _botEntries)
            {
                bot.GrowScore(_semesterCount);
            }

            RefreshRankings();

            Debug.Log($"[Leaderboard] Semester {_semesterCount} — Player score: {_playerEntry.score:F0} | Rank: {_playerEntry.rank}");
        }

        /// <summary>
        /// Resets the session (call on game-over or restart).
        /// </summary>
        public void ResetSession()
        {
            _sessionActive = false;
            _semesterCount = 0;
            _playerEntry = null;
            _botEntries.Clear();
            _ranked.Clear();
            Debug.Log("[Leaderboard] Session reset.");
        }

        /// <summary>
        /// Returns a ranked snapshot (rank 1 = highest score).
        /// Safe to call every frame from UI — returns cached list, no recalculation.
        /// </summary>
        public List<LeaderboardEntry> GetRankedEntries()
        {
            return _ranked;
        }

        /// <summary>
        /// Returns the player's entry with current rank.
        /// Returns null if session has not started.
        /// </summary>
        public LeaderboardEntry GetPlayerEntry()
        {
            return _playerEntry;
        }

        /// <summary>
        /// Returns 1-based rank of the player. Returns -1 if session not active.
        /// </summary>
        public int GetPlayerRank()
        {
            return _playerEntry?.rank ?? -1;
        }

        /// <summary>
        /// Returns total number of participants (bots + player).
        /// </summary>
        public int GetTotalParticipants()
        {
            return _botEntries.Count + 1;
        }

        // ── Private helpers ────────────────────────────────────────────────

        private void RefreshRankings()
        {
            _ranked.Clear();

            if (_playerEntry != null)
                _ranked.Add(_playerEntry);

            foreach (var bot in _botEntries)
                _ranked.Add(bot.entry);

            // Sort descending by score
            _ranked = _ranked.OrderByDescending(e => e.score).ToList();

            // Assign ranks
            for (int i = 0; i < _ranked.Count; i++)
                _ranked[i].rank = i + 1;
        }

        // ── Inner types ────────────────────────────────────────────────────

        private class BotConfig
        {
            public string name;
            public float baseScore;
            public float growthPerSem;
            public float aggression;   // compounds per semester

            public BotConfig(string name, float baseScore, float growthPerSem, float aggression)
            {
                this.name = name;
                this.baseScore = baseScore;
                this.growthPerSem = growthPerSem;
                this.aggression = aggression;
            }
        }

        private class BotEntry
        {
            public LeaderboardEntry entry;
            private BotConfig _cfg;

            public BotEntry(BotConfig cfg)
            {
                _cfg = cfg;
                entry = new LeaderboardEntry(cfg.name, isPlayer: false);
                entry.score = cfg.baseScore;
            }

            /// <summary>
            /// Exponential growth: each semester compounds by aggression factor.
            /// Small random jitter ±10% keeps bots from feeling robotic.
            /// </summary>
            public void GrowScore(int semesterNumber)
            {
                float compound = Mathf.Pow(1f + _cfg.aggression, semesterNumber);
                float growth = _cfg.growthPerSem * compound;
                float jitter = Random.Range(0.9f, 1.1f);
                entry.score += growth * jitter;
            }
        }
    }
}