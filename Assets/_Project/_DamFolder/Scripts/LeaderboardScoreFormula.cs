using UnityEngine;

namespace B70.Leaderboard
{
    public static class LeaderboardScoreFormula
    {
        // ── Weights ────────────────────────────────────────────────────────
        private const float W_HAPPY = 1.2f;
        private const float W_EDUCATION = 1.5f;
        private const float W_LEVEL = 80f;
        private const float W_STUDENT = 0.4f;

        // ── Exponential shaping ────────────────────────────────────────────
        // Happy & Education: normalized [0,1] then raised to power < 1 so
        // approaching 100% yields diminishing extra reward, but 100% = big bonus.
        private const float EXP_HAPPY = 1.8f;
        private const float EXP_EDUCATION = 1.8f;

        // Level: quadratic growth — each new level is worth more than the last.
        private const float EXP_LEVEL = 2.0f;

        // Student: square-root so early students matter more, late ones still count.
        private const float EXP_STUDENT = 0.6f;

        // ── Bonus ──────────────────────────────────────────────────────────
        // Flat bonus awarded when BOTH Happy AND Education reach 100%.
        private const float BONUS_PERFECT_STATS = 500f;

        /// <summary>
        /// Calculate score from raw resource values.
        /// </summary>
        /// <param name="happy">0–100</param>
        /// <param name="education">0–100</param>
        /// <param name="level">1+</param>
        /// <param name="students">0+</param>
        public static float Calculate(float happy, float education, int level, int students)
        {
            float hNorm = Mathf.Clamp01(happy / 100f);
            float eNorm = Mathf.Clamp01(education / 100f);

            float scoreHappy = W_HAPPY * Mathf.Pow(hNorm, EXP_HAPPY);
            float scoreEducation = W_EDUCATION * Mathf.Pow(eNorm, EXP_EDUCATION);
            float scoreLevel = W_LEVEL * Mathf.Pow(level, EXP_LEVEL);
            float scoreStudent = W_STUDENT * Mathf.Pow(students, EXP_STUDENT);

            float bonus = (happy >= 100f && education >= 100f) ? BONUS_PERFECT_STATS : 0f;

            float total = scoreHappy + scoreEducation + scoreLevel + scoreStudent + bonus;
            return Mathf.Max(0f, total);
        }

        /// <summary>
        /// Convenience overload that reads directly from SceneManager.
        /// </summary>
        public static float CalculateFromScene()
        {
            var sm = SceneManager.instance;
            return Calculate(
                sm.numberOfHappyInStorage,
                sm.numberOfEducationInStorage,
                sm.currentLevel,
                sm.numberOfStudentInStorage
            );
        }
    }
}