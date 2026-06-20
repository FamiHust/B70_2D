using System;
using UnityEngine;

namespace B70.Balance
{
    /// <summary>
    /// Các tham số cân bằng có thể chỉnh được bởi designer.
    /// Nên lưu trong ScriptableObject hoặc PlayerPrefs để dễ tinh chỉnh.
    /// </summary>
    [Serializable]
    public class BalanceParameters
    {
        // ── Kinh tế / Sinh viên ────────────────────────────────────────────
        public float alpha = 10f;       // Gold thu được mỗi tick thời gian (passive income từ công trình).
        public float beta = 0f;         // Gold thưởng phẳng cuối học kỳ.
        public float gamma = 2000f;     // Hệ số quy mô tân sinh viên nhập học tối đa.
        public float mu = 1f;           // Cường độ đỉnh của hệ số nhân happiness.
        public float delta = 2000f;     // Hệ số quy mô dropout do burnout (education quá cao).
        public float epsilon = 2000f;   // Hệ số quy mô dropout do happiness thấp.
        public float zeta = 500f;       // Hệ số giảm dropout khi happiness cao (âm → kéo dropout xuống).
        public float theta = 20f;       // Hệ số cơ sở tỉ lệ tốt nghiệp đỉnh.

        // ── Hình dạng đường cong (tunable) ────────────────────────────────
        public float p1 = 0.70f;        // Tỉ lệ nhập học mục tiêu tại giai đoạn 1 (education = t1).
        public float p2 = 0.85f;        // Tỉ lệ nhập học mục tiêu tại giai đoạn 2 (education = t2).
        public float intakeExp = 2f;    // Số mũ của đường cong nhập học (làm dốc / phẳng đường).
        public float burnoutExp = 4f;   // Số mũ kiểm soát vách đá dropout khi burnout.

        // ── Các mốc toàn cục ──────────────────────────────────────────────
        public float t1 = 30f;          // Mốc education đầu tiên (điểm uốn cong nhập học giai đoạn 1→2).
        public float t2 = 70f;          // Mốc education thứ hai (điểm uốn cong nhập học + ngưỡng burnout).
        public float neutralH = 50f;    // Ngưỡng happiness trung lập: dưới → phạt dropout, trên → thưởng.
    }

    /// <summary>
    /// Trạng thái runtime thay đổi trong quá trình mô phỏng.
    /// Ánh xạ trực tiếp sang các biến trong SceneManager.
    /// </summary>
    [Serializable]
    public class BalanceState
    {
        public float gold = 0f;           // numberOfGoldInStorage
        public float students = 0f;       // numberOfStudentInStorage  (S_total)
        public float studentCap = 4000f;  // studentStorageCapacity
        public float happiness = 30f;     // numberOfHappyInStorage    (H, [0,100])
        public float education = 30f;     // numberOfEducationInStorage (E, [0,100])
    }

    /// <summary>
    /// Kết quả chi tiết của một lần cập nhật cuối học kỳ.
    /// Hữu ích cho biểu đồ, log và công cụ cân bằng.
    /// </summary>
    public struct SemesterBreakdown
    {
        public float freshmen;            // Số tân sinh viên nhập học kỳ này.
        public float dropouts;            // Số sinh viên bỏ học kỳ này.
        public float graduated;           // Số sinh viên tốt nghiệp kỳ này.
        public float deltaStudents;       // Thay đổi net số sinh viên (freshmen - dropouts - graduated).
        public float semesterGoldIncome;  // Gold thu được từ học phí cuối kỳ.
        public float diningIncome;        // Gold thu được từ nhà ăn.
        public float graduationRate;      // Tỉ lệ tốt nghiệp thực tế [0,1].
    }

    /// <summary>
    /// Thư viện công thức tĩnh — phiên bản B70 (University Tycoon 2D).
    ///
    /// Thay đổi so với mẫu gốc (UniversityTycoon.Balance):
    ///   • "academic"  → "education"  (khớp với biến numberOfEducationInStorage).
    ///   • Namespace   → B70.Balance.
    ///   • Thêm helpers bridge với SceneManager (StateFromSceneManager / ApplyStateToSceneManager).
    ///   • Comment toàn bộ bằng tiếng Việt.
    /// </summary>
    public static class UniversityBalanceFormulas
    {
        // ══════════════════════════════════════════════════════════════════
        //  TIỆN ÍCH
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Giới hạn happiness / education trong khoảng thiết kế [0,100].
        /// </summary>
        public static float ClampScore(float value)
        {
            return Mathf.Clamp(value, 0f, 100f);
        }

        // ══════════════════════════════════════════════════════════════════
        //  GOLD
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Gold từ một game tick thời gian thực:
        ///   G(tick) += alpha * deltaTime
        /// alpha     : tốc độ gold passive từ công trình đang hoạt động.
        /// deltaTime : độ dài tick (giây).
        /// </summary>
        public static float GoldFromGameTick(float alpha, float deltaTime)
        {
            return alpha * deltaTime;
        }

        /// <summary>
        /// Gold từ học phí cuối kỳ:
        ///   G(semester) += beta + S_total
        /// beta     : khoản thưởng phẳng mỗi kỳ.
        /// students : tổng sinh viên hiện tại (S_total).
        /// </summary>
        public static float GoldFromSemester(float beta, float students)
        {
            return beta + students;
        }

        // ══════════════════════════════════════════════════════════════════
        //  NHẬP HỌC (FRESHMEN)
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Tân sinh viên từ Education — phương trình 3 pha cố định:
        ///   E ≤ 30        : gamma * 0.70 * (E/30)²
        ///   30 &lt; E ≤ 70  : gamma * 0.70 + gamma * 0.15 * ((E-30)/40)²
        ///   E &gt; 70        : gamma * 1.00 - gamma * 0.15 * ((100-E)/30)²
        ///
        /// Ý nghĩa: tăng trưởng mạnh giai đoạn đầu, ổn định giữa kỳ,
        ///          nhẹ chậm lại khi Education gần tối đa.
        /// </summary>
        public static float FreshmenFromEducationFixed(float education, float gamma)
        {
            float e = ClampScore(education);

            if (e <= 30f)
                return gamma * 0.70f * Mathf.Pow(e / 30f, 2f);

            if (e <= 70f)
                return gamma * 0.70f + gamma * 0.15f * Mathf.Pow((e - 30f) / 40f, 2f);

            return gamma * 1.00f - gamma * 0.15f * Mathf.Pow((100f - e) / 30f, 2f);
        }

        /// <summary>
        /// Tân sinh viên từ Education — phương trình 3 pha có thể tham số hoá:
        ///   E ≤ t1             : gamma * p1 * (E/t1)^intakeExp
        ///   t1 &lt; E ≤ t2       : gamma*p1 + gamma*(p2-p1) * ((E-t1)/(t2-t1))^intakeExp
        ///   E &gt; t2            : gamma*1.0 - gamma*(1-p2) * ((100-E)/(100-t2))^intakeExp
        ///
        /// Ý nghĩa: hình dạng giống công thức cố định nhưng designer có thể điều chỉnh.
        /// </summary>
        public static float FreshmenFromEducationParameterized(float education, BalanceParameters p)
        {
            float e = ClampScore(education);

            if (e <= p.t1)
                return p.gamma * p.p1 * Mathf.Pow(e / p.t1, p.intakeExp);

            if (e <= p.t2)
                return p.gamma * p.p1 + p.gamma * (p.p2 - p.p1) * Mathf.Pow((e - p.t1) / (p.t2 - p.t1), p.intakeExp);

            return p.gamma * 1.0f - p.gamma * (1.0f - p.p2) * Mathf.Pow((100f - e) / (100f - p.t2), p.intakeExp);
        }

        /// <summary>
        /// Hệ số nhân nhập học từ Happiness:
        ///   f_H(H) = c * H * exp(-k*H)
        ///   c = mu * e / neutralH,   k = 1 / neutralH
        ///
        /// Đỉnh tại H = neutralH; suy giảm về 2 phía.
        /// mu       : cường độ đỉnh (1 = chuẩn).
        /// neutralH : mức happiness cho đỉnh hệ số nhân (mặc định 50).
        /// </summary>
        public static float FreshmenHappinessMultiplier(float happiness, float mu, float neutralH = 50f)
        {
            float h = ClampScore(happiness);
            float c = (mu * Mathf.Exp(1f)) / neutralH;
            float k = 1f / neutralH;
            return c * h * Mathf.Exp(-k * h);
        }

        /// <summary>
        /// Tổng tân sinh viên nhập học kỳ này:
        ///   S_freshmen = f_education(E) * f_happiness(H)
        ///
        /// Kết hợp sức hút từ chất lượng đào tạo và mức độ hài lòng của sinh viên.
        /// useParameterized = true → dùng công thức tham số hoá thay vì công thức cố định.
        /// </summary>
        public static float TotalFreshmen(float education, float happiness, BalanceParameters p, bool useParameterized = false)
        {
            float fromE = useParameterized
                ? FreshmenFromEducationParameterized(education, p)
                : FreshmenFromEducationFixed(education, p.gamma);

            float fromH = FreshmenHappinessMultiplier(happiness, p.mu, p.neutralH);
            return Mathf.Max(0f, fromE * fromH);
        }

        // ══════════════════════════════════════════════════════════════════
        //  DROPOUT (BỎ HỌC)
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Dropout do áp lực học tập — burnout khi Education quá cao:
        ///   f_out_E(E) = delta * (E/100)^burnoutExp
        ///
        /// Gần như 0 ở Education thấp, tăng dốc khi Education tiếp cận 100.
        /// </summary>
        public static float DropoutFromEducation(float education, float delta, float burnoutExp = 4f)
        {
            float e = ClampScore(education);
            return delta * Mathf.Pow(e / 100f, burnoutExp);
        }

        /// <summary>
        /// Dropout điều chỉnh bởi Happiness (piecewise):
        ///   H &lt; neutralH : epsilon * (1 - H/neutralH)     → phạt: tăng dropout
        ///   H ≥ neutralH : -zeta * ((H-neutralH)/neutralH) → thưởng: giảm dropout
        ///
        /// Happiness thấp → học sinh chán nản → bỏ học nhiều hơn.
        /// Happiness cao  → gắn kết → ít bỏ học hơn (trả về số âm → cộng vào giảm tổng dropout).
        /// </summary>
        public static float DropoutFromHappiness(float happiness, float epsilon, float zeta, float neutralH = 50f)
        {
            float h = ClampScore(happiness);
            if (h < neutralH)
                return epsilon * (1f - h / neutralH);

            return -zeta * ((h - neutralH) / neutralH);
        }

        /// <summary>
        /// Tổng dropout mỗi kỳ:
        ///   S_out = f_out_E(E) + f_out_H(H)
        ///
        /// Tổng hợp áp lực học tập và sức khoẻ tinh thần.
        /// Kết quả được clamp về 0 (không bao giờ âm).
        /// </summary>
        public static float TotalDropouts(float education, float happiness, BalanceParameters p)
        {
            float outE = DropoutFromEducation(education, p.delta, p.burnoutExp);
            float outH = DropoutFromHappiness(happiness, p.epsilon, p.zeta, p.neutralH);
            return Mathf.Max(0f, outE + outH);
        }

        // ══════════════════════════════════════════════════════════════════
        //  TỐT NGHIỆP (GRADUATION)
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Tỉ lệ tốt nghiệp cơ sở (piecewise):
        ///   E ≤ t2 : theta - 20 * ((t2-E)/t2)²
        ///   E &gt; t2 : theta - 10 * ((E-t2)/(100-t2))²
        ///
        /// Đỉnh tại E = t2; giảm nhẹ khi Education quá cao (quá tải học thuật).
        /// theta : hệ số cơ sở (mặc định 20 = tỉ lệ tốt nghiệp đỉnh 20%).
        /// </summary>
        public static float GraduationBase(float education, float theta, float t2 = 70f)
        {
            float e = ClampScore(education);

            if (e <= t2)
                return theta - 20f * Mathf.Pow((t2 - e) / t2, 2f);

            return theta - 10f * Mathf.Pow((e - t2) / (100f - t2), 2f);
        }

        /// <summary>
        /// Tỉ lệ tốt nghiệp thực tế — có tính cơ chế phục hồi từ Happiness:
        ///   penalty   = max(0, theta - baseRate)
        ///   recovery  = max(0, (H - neutralH) / neutralH)
        ///   gradPct   = baseRate + penalty * recovery
        ///   gradRate  = clamp(gradPct, 0, 100) / 100
        ///
        /// Happiness cao giúp sinh viên vượt qua áp lực và tốt nghiệp đúng hạn.
        /// </summary>
        public static float GraduationRate(float education, float happiness, BalanceParameters p)
        {
            float baseRate = GraduationBase(education, p.theta, p.t2);
            float penalty  = Mathf.Max(0f, p.theta - baseRate);
            float recovery = Mathf.Max(0f, (ClampScore(happiness) - p.neutralH) / p.neutralH);
            float gradPercent = baseRate + penalty * recovery;
            return Mathf.Clamp(gradPercent, 0f, 100f) / 100f;
        }

        /// <summary>
        /// Số sinh viên tốt nghiệp kỳ này:
        ///   S_graduated = S_total * gradRate
        /// </summary>
        public static float GraduatedStudents(float studentsTotal, float graduationRate)
        {
            return Mathf.Max(0f, studentsTotal * graduationRate);
        }

        // ══════════════════════════════════════════════════════════════════
        //  THAY ĐỔI NET SINH VIÊN
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Thay đổi net số sinh viên trong kỳ:
        ///   ΔS = S_freshmen - S_out - S_graduated
        ///   S_total_next = clamp(S_total + ΔS, 0, studentCap)
        /// </summary>
        public static float NetStudentDelta(float freshmen, float dropouts, float graduated)
        {
            return freshmen - dropouts - graduated;
        }

        // ══════════════════════════════════════════════════════════════════
        //  ÁP DỤNG TICK
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Áp dụng một game tick thời gian thực:
        ///   • Cộng gold passive từ alpha và deltaTime.
        /// Gọi trong Update() hoặc coroutine vòng lặp.
        /// </summary>
        public static void ApplyGameTick(ref BalanceState s, BalanceParameters p, float deltaTime)
        {
            s.gold += GoldFromGameTick(p.alpha, deltaTime);
        }

        /// <summary>
        /// Áp dụng một tick cuối học kỳ:
        ///   1. Tính tân sinh viên, dropout và sinh viên tốt nghiệp.
        ///   2. Cập nhật tổng số sinh viên (clamp [0, studentCap]).
        ///   3. Cộng gold học phí: G += beta + S_total.
        ///
        /// Trả về SemesterBreakdown để log / hiển thị UI chi tiết.
        /// Thích hợp gọi trong SceneManager.CompleteSemester().
        ///
        /// useParameterized = true → dùng công thức education tham số hoá.
        /// </summary>
        public static SemesterBreakdown ApplySemesterTick(ref BalanceState s, BalanceParameters p, bool useParameterized = false)
        {
            float freshmen  = TotalFreshmen(s.education, s.happiness, p, useParameterized);
            float rawDropouts = TotalDropouts(s.education, s.happiness, p);
            float gradRate  = GraduationRate(s.education, s.happiness, p);
            float graduated = GraduatedStudents(s.students, gradRate);

            // Capping dropouts to not exceed the current students minus graduated students
            float dropouts  = Mathf.Clamp(rawDropouts, 0f, Mathf.Max(0f, s.students - graduated));

            float deltaS = NetStudentDelta(freshmen, dropouts, graduated);
            s.students = Mathf.Clamp(s.students + deltaS, 0f, s.studentCap);

            float semesterGold = GoldFromSemester(p.beta, s.students);
            s.gold += semesterGold;

            return new SemesterBreakdown
            {
                freshmen           = freshmen,
                dropouts           = dropouts,
                graduated          = graduated,
                deltaStudents      = deltaS,
                semesterGoldIncome = semesterGold,
                diningIncome       = 0f, // TODO: calculate dining income
                graduationRate     = gradRate
            };
        }

        // ══════════════════════════════════════════════════════════════════
        //  BRIDGE VỚI SCENEMANAGER
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Tạo BalanceState từ các biến runtime hiện tại của SceneManager.
        /// Tiện để truyền vào ApplySemesterTick mà không cần map tay.
        ///
        /// Ví dụ sử dụng:
        ///   var state = UniversityBalanceFormulas.StateFromSceneManager();
        ///   var bd    = UniversityBalanceFormulas.ApplySemesterTick(ref state, myParams);
        ///   UniversityBalanceFormulas.ApplyStateToSceneManager(state);
        ///   Debug.Log($"Freshmen: {bd.freshmen}, Dropouts: {bd.dropouts}, Gold: {bd.semesterGoldIncome}");
        /// </summary>
        public static BalanceState StateFromSceneManager()
        {
            var sm = SceneManager.instance;
            float happyPct = sm.happyStorageCapacity > 0 
                ? Mathf.Clamp((float)sm.numberOfHappyInStorage / sm.happyStorageCapacity * 100f, 0f, 100f) 
                : 0f;
            float eduPct = sm.educationStorageCapacity > 0 
                ? Mathf.Clamp((float)sm.numberOfEducationInStorage / sm.educationStorageCapacity * 100f, 0f, 100f) 
                : 0f;
            return new BalanceState
            {
                gold       = sm.numberOfGoldInStorage,
                students   = sm.numberOfStudentInStorage,
                studentCap = sm.studentStorageCapacity,
                happiness  = happyPct,
                education  = eduPct
            };
        }

        /// <summary>
        /// Ghi BalanceState đã xử lý ngược lại vào SceneManager,
        /// sau đó lưu PlayerPrefs và refresh các UI liên quan.
        /// </summary>
        public static void ApplyStateToSceneManager(in BalanceState s)
        {
            var sm = SceneManager.instance;
            sm.numberOfGoldInStorage      = Mathf.RoundToInt(s.gold);
            sm.numberOfStudentInStorage   = Mathf.RoundToInt(s.students);
            
            float happyValue = s.happiness / 100f * sm.happyStorageCapacity;
            float eduValue = s.education / 100f * sm.educationStorageCapacity;
            sm.numberOfHappyInStorage     = Mathf.Clamp(Mathf.RoundToInt(happyValue), 0, sm.happyStorageCapacity);
            sm.numberOfEducationInStorage = Mathf.Clamp(Mathf.RoundToInt(eduValue), 0, sm.educationStorageCapacity);

            sm.SaveResources();
            sm.RefreshResourceUIs("gold");
            sm.RefreshResourceUIs("student");
            sm.RefreshResourceUIs("happy");
            sm.RefreshResourceUIs("education");
        }
    }
}
