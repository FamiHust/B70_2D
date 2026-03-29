# Quick Start: Map Shop Setup

## 5-Minute Setup

### Step 1: Prepare GameObject (2 min)

```
1. Select or create a GameObject on your map (e.g., "BuilderHut", "Market", "Library")
2. Add a Collider component (Inspector > Add Component > Collider)
   - Choose Box Collider or Sphere Collider
   - Make sure "Is Trigger" is UNCHECKED
3. Position and size it so it's visible and clickable
```

### Step 2: Add MapShopAreaScript (1 min)

```
1. Inspector > Add Component > MapShopAreaScript
2. Fill in the settings:
   - Area Name: "BuilderHut Shop" (or your name)
   - Item Ids:
     - Click "+" to add items
     - Add values: 3635, 2496, 6677 (example: D4, C1, LIBRARY)
```

### Step 3: Update Prefab (2 min)

```
1. Find and open SubCategoryItem prefab
   - Path: Assets/_Project/UI/... (find in project)
2. Select SubCategoryItem
3. Inspector > Add Component > MapShopItemScript
4. Verify it already has these components:
   - Text for Name
   - Text for Price
   - Image for icon
```

### Step 4: Test It!

```
1. Play the game
2. Click on your GameObject with MapShopAreaScript
3. Shop window should appear with your items!
4. Click item to buy (needs resources)
```

## Common Item IDs

| Name            | ID   | Type        |
| --------------- | ---- | ----------- |
| C1              | 2496 | SERVICE     |
| C4              | 3265 | RESOURCES   |
| D4              | 3635 | SERVICE     |
| C7              | 3336 | STUDENT     |
| B8              | 5342 | STUDENT     |
| LIBRARY         | 6677 | RESOURCES   |
| WALL            | 7666 | DECORATIONS |
| GIAI_PHONG_GATE | 2949 | DECORATIONS |
| TDN_GATE        | 1251 | DECORATIONS |
| TREE3           | 5341 | DECORATIONS |

## Troubleshooting Checklist

- [ ] GameObject has a Collider (not Trigger)
- [ ] MapShopAreaScript is attached to GameObject
- [ ] Item Ids list is not empty
- [ ] SubCategoryItem prefab has MapShopItemScript
- [ ] Item Ids are valid (exist in Items.cs)
- [ ] You have enough resources to buy items

## Example Setup

```csharp
// If you want to setup multiple items in code:
MapShopAreaScript shopArea = myGameObject.GetComponent<MapShopAreaScript>();
shopArea.areaName = "Quality Buildings";
shopArea.itemIds.Add(2496);  // C1
shopArea.itemIds.Add(3635);  // D4
shopArea.itemIds.Add(6677);  // LIBRARY
```

Done! Your map shop should work now.
