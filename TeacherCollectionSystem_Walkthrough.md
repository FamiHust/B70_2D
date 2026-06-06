# Hệ thống Teacher Collection & Assignment (Bộ sưu tập Giảng viên)

Tài liệu này tổng hợp toàn bộ các tính năng, logic và sự thay đổi đã được triển khai cho hệ thống thẻ Giảng viên trong game.

## 1. Cấu trúc Dữ liệu (Data)

*   **`TeacherData` (ScriptableObject):**
    *   Lưu thông tin cơ bản: Tên, Level, Seniority, Avatar.
    *   Chỉ số Buff: `influenceGold`, `influenceEducation`, `influenceHappy`.
    *   Mô tả Buff: `descGold`, `descEducation`, `descHappy` (để giải thích rõ tác dụng từng loại buff cho người chơi).
*   **`TeacherCollection` (ScriptableObject):**
    *   Đóng vai trò là cơ sở dữ liệu gốc chứa toàn bộ các thẻ giảng viên có trong game.

## 2. Quản lý Kho thẻ (Inventory)

*   **`UIManager.cs`:**
    *   Lưu trữ `playerTeachers` (danh sách các `TeacherData` mà người chơi đang sở hữu).
    *   Cung cấp các hàm hỗ trợ mở `CollectionWindow` ở chế độ Xem (View Mode) hoặc chế độ Gán (Assign Mode).

## 3. Giao diện (UI Windows)

*   **`CardSelectionWindow` (Quay thẻ mới):**
    *   Lọc ngẫu nhiên thẻ từ hệ thống nhưng **loại bỏ các thẻ người chơi đã sở hữu**.
    *   Sau khi quay và nhận thẻ, data thẻ sẽ được đẩy vào kho `playerTeachers`.
*   **`CollectionWindow` (Kho chứa thẻ):**
    *   Thay vì tự động Instantiate thẻ mới, hệ thống tự động tìm và **tái sử dụng các slot rỗng (Prefab đã xếp sẵn)** bên trong `cardZone`.
    *   Đã tích hợp `ScrollRect` (vertical) giúp người chơi có thể lướt danh sách lên/xuống nếu kho chứa quá lớn.
    *   **View Mode:** Khi mở từ màn hình chính, thẻ không thể bấm để gán, chỉ xem thông tin.
    *   **Assign Mode:** Khi mở từ `TeacherButton` trong nhà, bấm vào thẻ sẽ tự động gán giảng viên đó cho tòa nhà đang chọn và đóng cửa sổ.
*   **`GameOverlayWindow` (Màn hình chính):**
    *   Thêm `CollectionButton` để mở nhanh kho thẻ.
    *   Thêm `CollectionHint` (dấu chấm đỏ hoặc thông báo) bật lên khi có thẻ mới thu thập được.
*   **`ItemOptionsWindow` (Menu của Tòa nhà):**
    *   Bổ sung thêm `TeacherButton` dùng để mở cửa sổ gán giảng viên.
    *   Thứ tự Animation xuất hiện của các nút đã được chuẩn hóa: `Info > Upgrade > Boost > Teacher > Remove`. Lỗi Animator tham chiếu nhầm giữa `InfoButton` và `RemoveButton` đã được khắc phục.

## 4. UI Thẻ Giảng Viên (`TeacherCardCtrl.cs`)

*   Mỗi Component thẻ được tái cấu trúc để hỗ trợ 2 trạng thái:
    *   **Trạng thái có Data:** Hiển thị Avatar, các Text thông số và thông tin Buff. `LockImage` bị ẩn đi. Nút bấm được kích hoạt (nếu ở chế độ Assign Mode).
    *   **Trạng thái Slot Rỗng (`data == null`):** Ẩn Avatar, xóa sạch các dòng Text, vô hiệu hóa khả năng click, và **hiển thị `LockImage`** để biểu diễn slot đang trống/chưa mở khóa.

## 5. Tích hợp Logic Core (Sản xuất)

*   **`BaseItemScript.cs`:** Thêm biến `assignedTeacher` để lưu lại giảng viên đang được gán cho tòa nhà đó.
*   **`ProductionScript.cs`:** 
    *   Logic thu thập tài nguyên và sản xuất đã tự động đọc `assignedTeacher`.
    *   Các thông số tính toán cuối cùng (Gold, Education, Happy) đều được nhân/cộng thêm với hệ số buff (`influenceGold`, `influenceEducation`, `influenceHappy`) từ giảng viên.

---
**Hướng dẫn Setup trên Unity Editor đối với những tính năng mới:**
1. Hãy bọc `cardZone` của `CollectionWindow` bằng Component `Scroll Rect` (chỉ chọn Vertical) rồi kéo vào tham chiếu `scrollView`.
2. Tạo các thẻ `TeacherCard` trống làm slot xếp sẵn vào trong `cardZone`.
3. Thêm một Component Image (ổ khóa) vào Prefab của `TeacherCard` và kéo vào biến `lockImage` của `TeacherCardCtrl`.
4. Đảm bảo tất cả các Animator Trigger của `ItemOptionsWindow` được set đúng tham chiếu trong Inspector.
