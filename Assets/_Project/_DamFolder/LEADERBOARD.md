# Leaderboard System — B70 University Tycoon

## Tổng quan

Hệ thống leaderboard cho một session chơi. Điểm được tính dựa trên 4 chỉ số:
Happy, Education, Level và Students. 7 bot sẽ tăng điểm theo từng semester
để tạo áp lực cạnh tranh cho player.

**Leaderboard reset khi game over / bắt đầu ván mới.**

---

## Cấu trúc file

```
Assets/_Project/Scripts/Leaderboard/
├── LeaderboardEntry.cs          # Data model cho mỗi người chơi / bot
├── LeaderboardScoreFormula.cs   # Công thức tính điểm (pure static)
└── LeaderboardManager.cs        # Manager chính, singleton MonoBehaviour
```

---

## Setup trong Unity

1. Mở scene chính
2. Tìm GameObject đang chứa `SceneManager` hoặc `UIManager`
3. Kéo `LeaderboardManager.cs` vào Inspector của GameObject đó
4. Không cần assign reference gì thêm — tất cả data đã hardcode

> **Lưu ý:** `LeaderboardManager` là singleton. Chỉ attach vào **một** GameObject duy nhất.

---

## API dành cho UI dev

### Lấy danh sách xếp hạng

```csharp
List<LeaderboardEntry> entries = LeaderboardManager.instance.GetRankedEntries();

foreach (var entry in entries)
{
    Debug.Log($"#{entry.rank} {entry.displayName} — {entry.score:F0} pts");

    if (entry.isPlayer)
    {
        // highlight player row
    }
}
```

### Lấy thông tin của player

```csharp
LeaderboardEntry player = LeaderboardManager.instance.GetPlayerEntry();
int rank    = player.rank;
float score = player.score;
```

### Lấy rank nhanh

```csharp
int rank  = LeaderboardManager.instance.GetPlayerRank();        // 1-based, -1 nếu chưa start
int total = LeaderboardManager.instance.GetTotalParticipants(); // luôn là 8 (7 bot + player)
```

### Reset khi game over

```csharp
LeaderboardManager.instance.ResetSession();
```

---

## LeaderboardEntry — Data model

| Property      | Type   | Mô tả                         |
|---------------|--------|-------------------------------|
| `displayName` | string | Tên hiển thị                  |
| `isPlayer`    | bool   | `true` nếu là người chơi thật |
| `score`       | float  | Điểm hiện tại                 |
| `rank`        | int    | Hạng hiện tại (1 = cao nhất)  |

---

## Công thức tính điểm

```
Score = W_HAPPY     × (Happy/100)^1.8
      + W_EDUCATION × (Education/100)^1.8
      + W_LEVEL     × Level^2.0
      + W_STUDENT   × Students^0.6
      + BONUS (nếu Happy = 100 VÀ Education = 100)
```

| Hệ số         | Giá trị | Ghi chú                             |
|---------------|---------|-------------------------------------|
| W_HAPPY       | 1.2     |                                     |
| W_EDUCATION   | 1.5     | Education được thưởng cao hơn Happy |
| W_LEVEL       | 80      | Level tăng điểm mạnh nhất           |
| W_STUDENT     | 0.4     |                                     |
| BONUS_PERFECT | 500     | Chỉ khi cả 2 chỉ số đạt 100%       |

**Đặc điểm:**
- Happy và Education bị cap ở 100%, dùng luỹ thừa 1.8 → tiến gần 100% thì khó hơn
- Level dùng luỹ thừa 2.0 → mỗi level mới có giá trị lớn hơn level trước
- Students dùng luỹ thừa 0.6 → sinh viên đầu tiên có giá trị cao hơn sinh viên thứ 1000

Nếu muốn preview điểm mà không cần SceneManager:

```csharp
float score = LeaderboardScoreFormula.Calculate(
    happy:     75f,
    education: 80f,
    level:     3,
    students:  500
);
```

---

## Bot system

7 bot với tính cách khác nhau. Điểm tăng theo công thức:

```
growth   = growthPerSem × (1 + aggression)^semesterNumber × jitter(0.9–1.1)
botScore += growth
```

## Khi nào điểm được cập nhật

Điểm chỉ cập nhật khi **semester kết thúc** — không cập nhật real-time.

Flow:

```
SceneManager.CompleteSemester()
    └── CompleteSemesterCoroutine()
            └── LeaderboardManager.instance.OnSemesterCompleted()
                    ├── Tính điểm player từ SceneManager (live data)
                    ├── Tăng điểm tất cả bot
                    └── Sort lại bảng xếp hạng
```

---

## Test trước khi làm UI

Dùng `LeaderboardTester.cs` (chỉ dùng trong development):

| Phím | Chức năng                                     |
|------|-----------------------------------------------|
| F1   | Bắt đầu session mới                           |
| F2   | Giả lập 1 semester kết thúc (dùng fake stats) |
| F3   | In bảng xếp hạng ra Console                   |
| F4   | Reset session                                  |
| F5   | Test formula với fake stats, không cần session |

Fake stats có thể chỉnh trong Inspector của `LeaderboardTester`.

> **Xóa `LeaderboardTester.cs` trước khi push lên production.**

---

## Checklist

- [ ] Gọi `GetRankedEntries()` để render danh sách
- [ ] Dùng `entry.isPlayer` để highlight dòng của player
- [ ] Dùng `entry.rank` và `entry.score` để hiển thị
- [ ] Gọi `ResetSession()` khi game over
- [ ] Không gọi `StartSession()` hay `OnSemesterCompleted()` từ UI — đã có SceneManager lo

---

## Namespace

Tất cả class trong `B70.Leaderboard`. Thêm dòng sau vào đầu file nếu cần:

```csharp
using B70.Leaderboard;
```
