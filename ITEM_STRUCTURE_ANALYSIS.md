# B70_2D Item ID Structure Analysis

## Summary

The game uses a **category-based item system** with specific Item IDs mapped to physical buildings/items. The system is organized through:

1. Category enums (SERVICE, RESOURCES, STUDENT, DECORATIONS)
2. SubCategory enums (specific buildings like C1, D4, LIBRARY, etc.)
3. Item ID integers that map to the actual game database entries

---

## 1. CATEGORY DEFINITIONS

### Location: `ShopWindowScript.cs`

File: `c:\B70_2D\Assets\_Project\Scripts\ui\windows\shop_window\ShopWindowScript.cs`

### Current Categories (Active):

```
- SERVICE      (Services category)
- RESOURCES    (Resource buildings)
- STUDENT      (Student-related items)
- DECORATIONS  (Decorative items)
```

### Commented Out Categories (Disabled):

```
- ARMY
- DEFENCE
```

### Category Rendering Order (in `RenderCategories()`):

```csharp
Category[] categories = new Category[] {
    Category.SERVICE,
    Category.RESOURCES,
    Category.STUDENT,
    Category.DECORATIONS
};
```

---

## 2. SUBCATEGORY DEFINITIONS

### Location: `ShopWindowScript.cs`

The `SubCategory` enum contains all the physical items/buildings that can be purchased.

### SubCategories by Category:

#### SERVICE Category Items:

- `C1` → Item ID: **2496**
- `D4` → Item ID: **3635**

#### RESOURCES Category Items:

- `C4` → Item ID: **3265**
- `LIBRARY` → Item ID: **6677**

#### STUDENT Category Items:

- `C7` → Item ID: **3336**
- `B8` → Item ID: **5342**

#### DECORATIONS Category Items:

- `GIAI_PHONG_GATE` → Item ID: **2949**
- `TDN_GATE` → Item ID: **1251**
- `WALL` → Item ID: **7666**
- `TREE3` → Item ID: **5341**

### Commented Out SubCategories (Disabled):

```
BARRACK (ID: 8833)
BOAT (ID: 6871)
CAMP (ID: 2728)
CANNON (ID: 1712)
ELIXIR_COLLECTOR (ID: unspecified)
ELIXIR_STORAGE (ID: unspecified)
GOLD_STORAGE (ID: 9074)
TOWER (ID: 4764)
```

---

## 3. ITEM ID MAPPING FUNCTIONS

### Location: `ShopWindowScript.cs`

```csharp
public int GetItemIdFromSubCategory(SubCategory subCategory)
{
    switch (subCategory) {
        case SubCategory.D4: return 3635;
        case SubCategory.C4: return 3265;
        case SubCategory.C1: return 2496;
        case SubCategory.LIBRARY: return 6677;
        case SubCategory.WALL: return 7666;
        case SubCategory.GIAI_PHONG_GATE: return 2949;
        case SubCategory.TDN_GATE: return 1251;
        case SubCategory.TREE3: return 5341;
        case SubCategory.C7: return 3336;
        case SubCategory.B8: return 5342;
        default: return 0;
    }
}
```

### Duplicate Mapping in `SubCategoryItemScript.cs`:

- Same function `GetItemId()` exists in `SubCategoryItemScript.cs`
- Provides redundant mapping (same logic repeated)

---

## 4. SUBCATEGORY RENDERING IN SHOP WINDOW

### Location: `ShopWindowScript.cs::RenderSubCategories(Category category)`

This function determines which SubCategories are available for each Category:

```csharp
case Category.SERVICE:
    subItems = new SubCategory[] { SubCategory.C1, SubCategory.D4 };
    break;
case Category.RESOURCES:
    subItems = new SubCategory[] { SubCategory.C4, SubCategory.LIBRARY };
    break;
case Category.STUDENT:
    subItems = new SubCategory[] { SubCategory.C7, SubCategory.B8 };
    break;
case Category.DECORATIONS:
    subItems = new SubCategory[] {
        SubCategory.GIAI_PHONG_GATE,
        SubCategory.TDN_GATE,
        SubCategory.WALL,
        SubCategory.TREE3
    };
    break;
```

---

## 5. ITEM DATA STRUCTURE

### ItemsCollection.ItemData Properties:

Based on code usage in UI scripts:

```
ItemData {
    name: string              // Display name of the item
    configuration: {
        price: int            // Cost to purchase
        resourceType: string  // Type of resource consumed (e.g., "gold", "elixir")
    }
}
```

### Accessing Item Data:

```csharp
Items.GetItem(itemId)  // Static method that returns ItemsCollection.ItemData
```

### Item Display Rules:

1. **Multiple Purchase Items** (can buy multiple times):
   - `WALL` (ID: 7666)
   - `TREE3` (ID: 5341)

2. **Single Purchase Items** (can only build once):
   - All others: Checked with `SceneManager.instance.IsItemBuiltInScene(itemId)`

---

## 6. KEY FILES INVOLVED

### Shop Window Files:

1. **ShopWindowScript.cs** - Main shop logic, Category/SubCategory enums
   - Location: `c:\B70_2D\Assets\_Project\Scripts\ui\windows\shop_window\`
   - Contains: Category enum, SubCategory enum, GetItemIdFromSubCategory()

2. **CategoryItemScript.cs** - Category display logic
   - Location: `c:\B70_2D\Assets\_Project\Scripts\ui\windows\shop_window\`
   - Maps categories to display sprites and names

3. **SubCategoryItemScript.cs** - SubCategory/Item display logic
   - Location: `c:\B70_2D\Assets\_Project\Scripts\ui\windows\shop_window\`
   - Contains: Item display names, sprites, duplicate ID mapping

### UI Manager Files:

4. **UIManager.cs** - Window management
   - Location: `c:\B70_2D\Assets\_Project\Scripts\ui\`
   - Shows/hides shop and item windows

5. **ItemWindowScript.cs** - Map shop item display
   - Location: `c:\B70_2D\Assets\_Project\Scripts\ui\windows\`
   - Displays items from map shop areas

### Map System Files:

6. **MapShopAreaScript.cs** - Defines shop areas on map
   - Location: `c:\B70_2D\Assets\_Project\Scripts\map\`
   - Stores `List<int> itemIds` for each shop area

### Base Classes:

7. **WindowScript.cs** - Base class for all windows
   - Location: `c:\B70_2D\Assets\_Project\Scripts\ui\windows\`

---

## 7. ADDITIONAL SYSTEMS REFERENCED

### Systems That Need Updating:

1. **Items.cs** - Central item database (location TBD)
   - Static method: `GetItem(int itemId)`
   - Returns: `ItemsCollection.ItemData`

2. **ItemsCollection.cs** - Item data definitions (location TBD)
   - Class: `ItemsCollection.ItemData`
   - Contains: item configuration

3. **SceneManager.cs** - Scene management (location TBD)
   - Method: `IsItemBuiltInScene(int itemId)`
   - Method: `ConsumeResource(string resourceType, int amount)`
   - Method: `AddItem(int itemId, bool ?, bool ?)`

4. **DataBaseManager.cs** - Persistent data (location TBD)
   - Method: `UpdateItemData(BaseItemScript item)`

---

## 8. CURRENT ITEM ID REFERENCE TABLE

| SubCategory     | Category    | Item ID | Display Name | Type                    |
| --------------- | ----------- | ------- | ------------ | ----------------------- |
| C1              | SERVICE     | 2496    | C1           | Service Building        |
| D4              | SERVICE     | 3635    | D4           | Service Building        |
| C4              | RESOURCES   | 3265    | C4           | Resource Building       |
| LIBRARY         | RESOURCES   | 6677    | LIBRARY      | Resource Building       |
| C7              | STUDENT     | 3336    | C7           | Student Building        |
| B8              | STUDENT     | 5342    | B8           | Student Building        |
| GIAI_PHONG_GATE | DECORATIONS | 2949    | GP GATE      | Gate                    |
| TDN_GATE        | DECORATIONS | 1251    | TDN GATE     | Gate                    |
| WALL            | DECORATIONS | 7666    | WALL         | Decoration (Repeatable) |
| TREE3           | DECORATIONS | 5341    | TREE3        | Decoration (Repeatable) |

---

## 9. HOW TO ADD NEW ITEMS

### Steps to Add a New Item:

1. **Add SubCategory enum entry** in `ShopWindowScript.cs`:

   ```csharp
   public enum SubCategory {
       // ... existing entries
       NEW_ITEM_NAME,  // Add here
   }
   ```

2. **Add mapping in GetItemIdFromSubCategory()** in `ShopWindowScript.cs`:

   ```csharp
   case SubCategory.NEW_ITEM_NAME: return ITEM_ID_NUMBER;
   ```

3. **Add same mapping in GetItemId()** in `SubCategoryItemScript.cs`:

   ```csharp
   case ShopWindowScript.SubCategory.NEW_ITEM_NAME:
       itemId = ITEM_ID_NUMBER;
       break;
   ```

4. **Add to category in RenderSubCategories()** in `ShopWindowScript.cs`:

   ```csharp
   case Category.CATEGORY_NAME:
       subItems = new SubCategory[] {
           SubCategory.EXISTING_ITEM,
           SubCategory.NEW_ITEM_NAME  // Add here
       };
       break;
   ```

5. **Add display case in SetSubCategory()** in `SubCategoryItemScript.cs`:

   ```csharp
   case ShopWindowScript.SubCategory.NEW_ITEM_NAME:
       this.Name.text = "DISPLAY_NAME";
       this.Image.sprite = this.NewItemSprite;  // Also add sprite reference
       break;
   ```

6. **Add sprite reference** in `SubCategoryItemScript.cs`:

   ```csharp
   public Sprite NewItemSprite;
   ```

7. **Update Items database** (in Items.cs - location TBD):
   - Add ItemData entry for the new Item ID
   - Define: name, price, resourceType

8. **Update CategoryItemScript.cs** if adding new Categories:
   - Add sprite reference
   - Add switch case in SetCategory()

---

## 10. FILES TO BE LOCATED

⚠️ **Still need to find these critical files:**

1. **Items.cs** - Contains `Items.GetItem(int itemId)` method
   - Likely locations:
     - `Scripts/managers/Items.cs`
     - `Scripts/core/Items.cs`
     - `Scripts/data/Items.cs`
     - `Scripts/Items.cs`

2. **ItemsCollection.cs** - Contains `ItemsCollection.ItemData` class
   - Likely locations:
     - `Scripts/data/ItemsCollection.cs`
     - `Scripts/ItemsCollection.cs`
     - `Scripts/items/ItemsCollection.cs`

3. **SceneManager.cs** - Contains scene management methods
   - Likely locations:
     - `Scripts/managers/SceneManager.cs`
     - `Scripts/Scene/SceneManager.cs`

4. **DataBaseManager.cs** - Persistent data management
   - Likely locations:
     - `Scripts/managers/DataBaseManager.cs`
     - `Scripts/Database/DataBaseManager.cs`

5. **Utilities.cs** - Contains `Utilities.CreateInstance()` method

---

## 11. MODIFICATION IMPACT ANALYSIS

### Files That WILL NEED UPDATE When Adding New Items:

1. ✅ **ShopWindowScript.cs** - Add SubCategory enum, Add GetItemIdFromSubCategory case, Add RenderSubCategories case
2. ✅ **SubCategoryItemScript.cs** - Add GetItemId case, Add SetSubCategory case, Add sprite reference
3. ✅ **CategoryItemScript.cs** - May need update if adding new categories
4. ✅ **Items.cs** (TBD) - Add new ItemData entry
5. ✅ **MapShopAreaScript.cs** (optional) - If adding items to map shops

### Places to Check for References:

- Any scene files that spawn items directly by ID
- Configuration/data files that list available items
- Any other scripts that reference specific item IDs
- MapShopAreaScript instances in scenes (in inspector data)

---

## 12. NEXT STEPS RECOMMENDATIONS

1. **Locate and examine Items.cs and ItemsCollection.cs** - These contain the actual item database
2. **Look for JSON or data files** that might contain item configurations
3. **Check scene prefabs** to see if item definitions are stored in prefabs or scene hierarchy
4. **Review SceneManager and DataBaseManager** for additional ID references
5. **Search codebase** for usages of the disabled categories (ARMY, DEFENCE) to see if there are other references to clean up
