using System;
using UnityEngine;

namespace UniversityTycoon.Balance
{
    /// <summary>
    /// Tunable parameters used by the balance formulas.
    /// Keep these in ScriptableObject or save data so designers can tweak values.
    /// </summary>
    [Serializable]
    public class BalanceParameters
    {
        // Core economy / student parameters.
        public float alpha = 10f;      // Gold income per game tick unit.
        public float beta = 0f;        // Flat semester gold bonus.
        public float gamma = 2000f;    // Base freshmen intake scale.
        public float mu = 1f;          // Peak strength of happiness multiplier.
        public float delta = 2000f;    // Max burnout dropout scale.
        public float epsilon = 2000f;  // Low-happiness dropout scale.
        public float zeta = 500f;      // High-happiness attraction (negative dropout) scale.
        public float theta = 20f;      // Baseline graduation peak coefficient.

        // Curve shaping parameters (editable version from design doc).
        public float p1 = 0.70f;       // Target percentage at first A phase.
        public float p2 = 0.85f;       // Target percentage at second A phase.
        public float intakeExp = 2f;   // Exponent for freshmen A-curve phases.
        public float burnoutExp = 4f;  // Exponent controlling burnout dropout cliff.

        // Global breakpoints.
        public float t1 = 30f;         // First academic inflection point.
        public float t2 = 70f;         // Second academic inflection / burnout threshold.
        public float neutralH = 50f;   // Happiness threshold where penalty switches to bonus.
    }

    /// <summary>
    /// Runtime state variables that change during simulation.
    /// </summary>
    [Serializable]
    public class BalanceState
    {
        public float gold = 0f;      // G
        public float students = 0f;       // S_total
        public float studentCap = 4000f;  // Current max student capacity
        public float happiness = 30f;     // H, clamped to [0,100]
        public float academic = 30f;      // A, clamped to [0,100]
    }

    /// <summary>
    /// Diagnostics for one semester update.
    /// Useful for charts, logs, and balancing tools.
    /// </summary>
    public struct SemesterBreakdown
    {
        public float freshmen;
        public float dropouts;
        public float graduated;
        public float deltaStudents;
        public float semesterGoldIncome;
        public float graduationRate;
    }

    /// <summary>
    /// Static formula library mapped from Design v1 equations.
    /// </summary>
    public static class UniversityBalanceFormulas
    {
        /// <summary>
        /// Keeps H and A within the designed range [0,100].
        /// </summary>
        public static float ClampScore(float value)
        {
            return Mathf.Clamp(value, 0f, 100f);
        }

        /// <summary>
        /// Game tick gold update from design:
        /// G(tick) += alpha * deltaTime
        /// alpha: passive gold rate from facilities.
        /// deltaTime: tick duration.
        /// </summary>
        public static float GoldFromGameTick(float alpha, float deltaTime)
        {
            return alpha * deltaTime;
        }

        /// <summary>
        /// Semester gold update from design:
        /// G(semester) += beta + S_total
        /// beta: flat semester gold term.
        /// students: S_total at semester end.
        /// </summary>
        public static float GoldFromSemester(float beta, float students)
        {
            return beta + students;
        }

        /// <summary>
        /// Freshmen intake from Academic Excellence (fixed 3-phase equation in doc):
        /// - A <= 30: gamma * 0.70 * (A/30)^2
        /// - 30 < A <= 70: gamma * 0.70 + gamma * 0.15 * ((A-30)/40)^2
        /// - A > 70: gamma * 1.00 - gamma * 0.15 * ((100-A)/30)^2
        /// Purpose: strong growth early, softer growth mid, slight taper near max A.
        /// </summary>
        public static float FreshmenFromAcademicFixed(float academic, float gamma)
        {
            float a = ClampScore(academic);

            if (a <= 30f)
            {
                return gamma * 0.70f * Mathf.Pow(a / 30f, 2f);
            }

            if (a <= 70f)
            {
                return gamma * 0.70f + gamma * 0.15f * Mathf.Pow((a - 30f) / 40f, 2f);
            }

            return gamma * 1.00f - gamma * 0.15f * Mathf.Pow((100f - a) / 30f, 2f);
        }

        /// <summary>
        /// Freshmen intake from Academic Excellence (parameterized variant):
        /// - A <= t1: gamma * p1 * (A/t1)^intakeExp
        /// - t1 < A <= t2: gamma * p1 + gamma * (p2-p1) * ((A-t1)/(t2-t1))^intakeExp
        /// - A > t2: gamma * 1.00 - gamma * (1-p2) * ((100-A)/(100-t2))^intakeExp
        /// Purpose: same shape as fixed formula but fully tunable by designers.
        /// </summary>
        public static float FreshmenFromAcademicParameterized(float academic, BalanceParameters p)
        {
            float a = ClampScore(academic);

            if (a <= p.t1)
            {
                return p.gamma * p.p1 * Mathf.Pow(a / p.t1, p.intakeExp);
            }

            if (a <= p.t2)
            {
                return p.gamma * p.p1 + p.gamma * (p.p2 - p.p1) * Mathf.Pow((a - p.t1) / (p.t2 - p.t1), p.intakeExp);
            }

            return p.gamma * 1.0f - p.gamma * (1.0f - p.p2) * Mathf.Pow((100f - a) / (100f - p.t2), p.intakeExp);
        }

        /// <summary>
        /// Happiness multiplier for freshmen:
        /// f_freshmen_H(H) = c * H * exp(-kH)
        /// with fixed peak at neutralH:
        /// c = mu * e / neutralH, k = 1/neutralH
        /// Purpose: intake bonus peaks around neutral happiness and tapers at extremes.
        /// </summary>
        public static float FreshmenHappinessMultiplier(float happiness, float mu, float neutralH = 50f)
        {
            float h = ClampScore(happiness);
            float c = (mu * Mathf.Exp(1f)) / neutralH;
            float k = 1f / neutralH;
            return c * h * Mathf.Exp(-k * h);
        }

        /// <summary>
        /// Total freshmen intake:
        /// S_freshmen = f_freshmen_A(A) * f_freshmen_H(H)
        /// Purpose: combines academic attractiveness and happiness-driven conversion.
        /// </summary>
        public static float TotalFreshmen(float academic, float happiness, BalanceParameters p, bool useParameterizedAcademicCurve = false)
        {
            float fromA = useParameterizedAcademicCurve
                ? FreshmenFromAcademicParameterized(academic, p)
                : FreshmenFromAcademicFixed(academic, p.gamma);

            float fromH = FreshmenHappinessMultiplier(happiness, p.mu, p.neutralH);
            return Mathf.Max(0f, fromA * fromH);
        }

        /// <summary>
        /// Dropout from academic burnout:
        /// f_out_A(A) = delta * (A/100)^burnoutExp
        /// Purpose: minimal dropout at low A pressure, steep increase at high A pressure.
        /// </summary>
        public static float DropoutFromAcademic(float academic, float delta, float burnoutExp = 4f)
        {
            float a = ClampScore(academic);
            return delta * Mathf.Pow(a / 100f, burnoutExp);
        }

        /// <summary>
        /// Dropout modifier from happiness (piecewise):
        /// - H < neutralH: epsilon * (1 - H/neutralH)
        /// - H >= neutralH: -zeta * ((H-neutralH)/neutralH)
        /// Purpose: low happiness increases dropouts, high happiness reduces net dropouts.
        /// </summary>
        public static float DropoutFromHappiness(float happiness, float epsilon, float zeta, float neutralH = 50f)
        {
            float h = ClampScore(happiness);
            if (h < neutralH)
            {
                return epsilon * (1f - h / neutralH);
            }

            return -zeta * ((h - neutralH) / neutralH);
        }

        /// <summary>
        /// Total dropout:
        /// S_out = f_out_A(A) + f_out_H(H)
        /// Purpose: aggregate academic pressure and wellbeing effects.
        /// </summary>
        public static float TotalDropouts(float academic, float happiness, BalanceParameters p)
        {
            float outA = DropoutFromAcademic(academic, p.delta, p.burnoutExp);
            float outH = DropoutFromHappiness(happiness, p.epsilon, p.zeta, p.neutralH);
            return Mathf.Max(0f, outA + outH);
        }

        /// <summary>
        /// Graduation base (piecewise):
        /// - A <= t2: theta - 20 * ((t2-A)/t2)^2
        /// - A > t2: theta - 10 * ((A-t2)/(100-t2))^2
        /// Purpose: graduation quality peaks around t2 and soft-clips after overload.
        /// </summary>
        public static float GraduationBase(float academic, float theta, float t2 = 70f)
        {
            float a = ClampScore(academic);

            if (a <= t2)
            {
                return theta - 20f * Mathf.Pow((t2 - a) / t2, 2f);
            }

            return theta - 10f * Mathf.Pow((a - t2) / (100f - t2), 2f);
        }

        /// <summary>
        /// Graduation rate with burnout shield:
        /// penalty = max(0, theta - base)
        /// recovery = max(0, (H-neutralH)/neutralH)
        /// gradPct = base + penalty * recovery
        /// gradRate = gradPct / 100
        /// Purpose: high happiness recovers part of burnout penalty.
        /// </summary>
        public static float GraduationRate(float academic, float happiness, BalanceParameters p)
        {
            float baseRate = GraduationBase(academic, p.theta, p.t2);
            float penalty = Mathf.Max(0f, p.theta - baseRate);
            float recovery = Mathf.Max(0f, (ClampScore(happiness) - p.neutralH) / p.neutralH);
            float gradPercent = baseRate + penalty * recovery;
            return Mathf.Clamp(gradPercent, 0f, 100f) / 100f;
        }

        /// <summary>
        /// Graduated students:
        /// S_graduated = S_total * gradRate
        /// </summary>
        public static float GraduatedStudents(float studentsTotal, float graduationRate)
        {
            return Mathf.Max(0f, studentsTotal * graduationRate);
        }

        /// <summary>
        /// Net semester student change:
        /// deltaS = S_freshmen - S_out - S_graduated
        /// S_total_next = S_total + deltaS
        /// </summary>
        public static float NetStudentDelta(float freshmen, float dropouts, float graduated)
        {
            return freshmen - dropouts - graduated;
        }

        /// <summary>
        /// Applies one game tick:
        /// - adds passive gold from alpha and deltaTime.
        /// </summary>
        public static void ApplyGameTick(ref BalanceState s, BalanceParameters p, float deltaTime)
        {
            s.gold += GoldFromGameTick(p.alpha, deltaTime);
        }

        /// <summary>
        /// Applies one semester tick:
        /// - computes freshmen, dropouts, and graduates.
        /// - updates student count with cap clamp.
        /// - updates gold with semester formula G += beta + S_total.
        /// </summary>
        public static SemesterBreakdown ApplySemesterTick(ref BalanceState s, BalanceParameters p, bool useParameterizedAcademicCurve = false)
        {
            float freshmen = TotalFreshmen(s.academic, s.happiness, p, useParameterizedAcademicCurve);
            float dropouts = TotalDropouts(s.academic, s.happiness, p);
            float gradRate = GraduationRate(s.academic, s.happiness, p);
            float graduated = GraduatedStudents(s.students, gradRate);

            float deltaS = NetStudentDelta(freshmen, dropouts, graduated);
            s.students = Mathf.Clamp(s.students + deltaS, 0f, s.studentCap);

            float semesterGold = GoldFromSemester(p.beta, s.students);
            s.gold += semesterGold;

            return new SemesterBreakdown
            {
                freshmen = freshmen,
                dropouts = dropouts,
                graduated = graduated,
                deltaStudents = deltaS,
                semesterGoldIncome = semesterGold,
                graduationRate = gradRate
            };
        }
    }
}
