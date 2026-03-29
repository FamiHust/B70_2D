# Map Shop System - Hướng Dẫn Sử Dụng

## Tổng Quan

Hệ thống này cho phép bạn đặt các GameObject trên map với danh sách item có sẵn. Khi người chơi click vào những GameObject này, sẽ hiển thị shop window với các item đó để mua.

## Cấu Trúc Thực Hiện

### 1. **MapShopAreaScript** - Script chính

- Attach vào GameObject trên map
- Chứa danh sách Item IDs (`itemIds`)
- Khi click, call ShopWindowScript để hiển thị items

### 2. **ShopWindowScript** - Modified

- Thêm method `RenderMapShop(string areaName, List<int> itemIds)`
- Hiển thị custom item list thay vì categories
- Hỗ trợ singleton instance để dễ access

### 3. **MapShopItemScript** - Script UI item

- Render individual items trong Map Shop
- Xử lý click và mua item
- Tương tự SubCategoryItemScript nhưng dùng itemId trực tiếp

### 4. **CameraManager** - Modified

- Thêm raycast detection cho MapShopArea
- Prioritize MapShop click trước BaseItem click

## Setup Hướng Dẫn

### Bước 1: Chuẩn Bị GameObject trên Map

```csharp
1. Tạo hoặc chọn GameObject trên map
2. Thêm Collider component (Box Collider, Sphere Collider, etc.)
   - Đảm bảo Collider không phải trigger (uncheck "Is Trigger")
3. Attach MapShopAreaScript vào GameObject
```

### Bước 2: Setup Item List trong MapShopAreaScript

```csharp
// Trong Inspector:
- Area Name: Đặt tên cho shop area (ví dụ: "Builder Hut Shop")
- Item Ids: Thêm item IDs mà bạn muốn bán

// Hoặc lập trình:
MapShopAreaScript mapShop = GetComponent<MapShopAreaScript>();
mapShop.areaName = "My Shop";
mapShop.itemIds.Add(3635);  // D4
mapShop.itemIds.Add(2496);  // C1
mapShop.itemIds.Add(6677);  // LIBRARY
```

### Bước 3: Update SubCategoryItem Prefab

1. Mở SubCategoryItem prefab
2. Thêm **MapShopItemScript** component vào script list
   - MapShopItemScript được thêm sau SubCategoryItemScript
3. Đảm bảo nó có references đến:
   - Name (Text component)
   - PriceText (Text component)
   - Image (Image component)

### Bước 4: Test

1. Chạy game
2. Click vào GameObject có MapShopAreaScript
3. Shop window hiển thị với danh sách items từ `itemIds`
4. Click vào item để mua

## Item IDs Reference

Các Item IDs phổ biến:

- 2496: C1
- 3265: C4
- 3635: D4
- 3336: C7
- 5342: B8
- 6677: LIBRARY
- 7666: WALL
- 2949: GIAI_PHONG_GATE
- 1251: TDN_GATE
- 5341: TREE3

Để tìm item ID: Xem ShopWindowScript.GetItemIdFromSubCategory() hoặc Items.cs

## API Public Methods

### MapShopAreaScript

```csharp
// Get items list
List<int> itemIds = mapShop.GetItemIds();

// Add/Remove items
mapShop.AddItem(3635);      // Add D4
mapShop.RemoveItem(3635);   // Remove D4
mapShop.ClearItems();       // Clear all

// Manual trigger
mapShop.OnMapShopClicked(); // Manually open shop
```

### ShopWindowScript

```csharp
// Open Map Shop
ShopWindowScript shop = ShopWindowScript.instance;
shop.RenderMapShop("My Shop", new List<int> { 2496, 3635 });
shop.Open();

// Close
shop.Close();

// Check mode
bool isMapShop = shop.IsMapShopMode();
string areaName = shop.GetCurrentMapShopName();
```

## Troubleshooting

### Shop không mở khi click

- Kiểm tra Collider có "Is Trigger" bị enable không (phải uncheck)
- Kiểm tra Collider layer có bị block bởi raycast mask không
- Kiểm tra MapShopAreaScript component có được attach không
- Kiểm tra itemIds list không empty

### Item không hiển thị đúng

- Kiểm tra item ID có tồn tại không (xem Items.cs)
- Kiểm trace SubCategoryItem prefab có MapShopItemScript không
- Kiểm tra sprite/data có được setup trong ItemData không

### Price hiển thị sai

- Kiểm tra ItemData configuration.price
- Kiểm tra ItemData configuration.resourceType (gold, diamond, etc.)

## Advanced Usage

### Dynamic Item List

```csharp
public class DynamicShopManager : MonoBehaviour
{
    public void SetupDynamicShop(MapShopAreaScript mapShop, int[] itemIds)
    {
        mapShop.ClearItems();
        foreach (int id in itemIds)
        {
            mapShop.AddItem(id);
        }
    }
}
```

### Multiple Map Shops

```csharp
// Setup 3 shops khác nhau trên map
MapShopAreaScript shop1 = builder_hut.GetComponent<MapShopAreaScript>();
shop1.areaName = "Builder Hut";
shop1.itemIds.Add(3635); // D4 only

MapShopAreaScript shop2 = library.GetComponent<MapShopAreaScript>();
shop2.areaName = "Library";
shop2.itemIds.Add(6677); // LIBRARY only

MapShopAreaScript shop3 = market.GetComponent<MapShopAreaScript>();
shop3.areaName = "Market";
shop3.itemIds.AddRange(new int[] { 2496, 3265, 3635 }); // Multiple items
```

## Notes

- Hệ thống vẫn giữ nguyên normal shop functionality
- Click vào item thường (BaseItem) vẫn hoạt động bình thường
- MapShop click được prioritize trước BaseItem click (tức MapShop sẽ được check trước)
- Mỗi click vào item sẽ close shop window như bình thường

## File Đã Modified/Tạo

1. ✅ `MapShopAreaScript.cs` - CREATED
2. ✅ `MapShopItemScript.cs` - CREATED
3. ✅ `ShopWindowScript.cs` - MODIFIED (thêm singleton, RenderMapShop, Open methods)
4. ✅ `CameraManager.cs` - MODIFIED (thêm \_TryGetRaycastHitMapShop, update UpdateBaseItemTap)
