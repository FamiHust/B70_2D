# B70_2D Game Architecture Summary

## Overview

This is a 2D isometric city-building game with a shop system for purchasing and placing buildings/items. The architecture separates data, game logic, UI, and rendering into distinct layers.

---

## 1. BaseItemScript - Core Game Object

**Location:** `isometric\_base_item\BaseItemScript.cs`

### Purpose

BaseItemScript is the central MonoBehaviour that represents any placeable item in the game (buildings, units, decorations, walls, etc.).

### Key Properties

```csharp
public int instanceId;                           // Unique instance identifier
public Common.State state;                       // IDLE, WALK, ATTACK, DESTROYED
public Common.Direction direction;               // 8-directional (TOP, TOP_LEFT, etc.)
public ItemsCollection.ItemData itemData;        // Template data for this item type
public float healthPoints;                       // Current health
public int level;                                // Building level
public bool isDestroyed;                         // Destruction state
public bool ownedItem;                           // Is it owned by player or enemy?
public List<BaseItemScript> connectedItems;      // Connected buildings (e.g., builder huts with workers)
```

### Key Sub-Systems (Composition)

- **Renderer** - Handles sprite rendering for different states
- **Walker** - Movement logic
- **Attacker** - Offensive capabilities
- **Defender** - Defensive range and HP management
- **Production** - Resource generation (e.g., gold collectors)
- **Training** - Unit training (e.g., barracks)
- **UI** - Health bars, selection indicators, notifications
- **Particles** - Effect rendering

### Key Methods

- `SetItemData(itemId, posX, posZ, level)` - Initializes the item with template data
- `SetState(Common.State state)` - Changes item state
- `OnDestroy()` - Cleanup, removes connected items via SceneManager

### Events

- `OnItemDestroy` - Fired when item is destroyed
- `OnConstructionComplete` - Fired when construction finishes

---

## 2. Item Data Structure

### ItemsCollection.ItemData

**Location:** `_libs\ItemsCollection.cs`

Serializable data class that defines an item template:

```csharp
public class ItemData
{
    public int id;                              // Unique item type ID
    public string name;                         // Display name
    public string description;                  // Item description
    public Texture2D thumb;                     // Shop thumbnail

    // Grid positioning for isometric placement
    public int gridWidth;                       // Width in grid cells
    public int gridHeight;                      // Height in grid cells
    public float arrowOffsetX, arrowOffsetZ;    // Arrow UI offsets
    public float gridOffsetX, gridOffsetZ;      // Grid positioning
    public float uiOffsetX, uiOffsetZ;          // UI element position
    public int defaultPosX, defaultPosZ;        // Default placement position

    // Sprite textures for different states
    public List<Texture2D> idleSpriteTextures;      // Idle animation frames
    public List<Texture2D> walkSpriteTextures;      // Walk animation frames
    public List<Texture2D> attackSpriteTextures;    // Attack animation frames
    public List<Texture2D> destroyedSpriteTextures; // Destroyed state frames

    // Configuration properties
    public Configuration configuration;
}

public class Configuration
{
    public float buildTime;                 // Construction duration
    public bool isCharacter;                // Is unit vs building
    public float speed;                     // Movement speed
    public float attackRange;               // Attack range (0 if melee)
    public float defenceRange;              // Defense range
    public float healthPoints;              // Max HP
    public float hitPoints;                 // (Unclear - possibly current HP?)
    public float productionRate;            // Resource generation rate
    public string product;                  // What resource is produced
    public int productPrice;                // Price of produced resource
    public int price;                       // Purchase/build cost
    public string resourceType;             // Resource type ("gold", "diamond", etc.)
    public int studentCapacityIncrease;     // Student storage increase
}
```

### Items.cs - Static Item Loader

**Location:** `_libs\Items.cs`

```csharp
public class Items : MonoBehaviour
{
    public static Dictionary<int, ItemsCollection.ItemData> items;

    public static void LoadItems()
    {
        // Loads from Resources.Load("ItemsCollection")
        // Populates Dictionary<itemId, ItemData>
    }

    public static ItemsCollection.ItemData GetItem(int itemId)
    {
        // Returns item template by ID
    }
}
```

---

## 3. SubCategoryItem Management & Shop Display

### SubCategoryItemScript

**Location:** `ui\windows\shop_window\SubCategoryItemScript.cs`

Represents a single purchasable item in the shop UI.

**Responsibilities:**

- Display item name, price, and thumbnail
- Handle click-to-purchase interaction
- Validate resource availability
- Trigger item creation

**Key Methods:**

```csharp
public void SetSubCategory(ShopWindowScript.SubCategory subCategory)
{
    // Displays item data:
    // - Sets Name text
    // - Sets PriceText from itemData.configuration.price
    // - Sets Image sprite
    // - Caches _subCategory enum value
}

public void OnClick()
{
    // Purchase flow:
    1. Maps SubCategory → itemId
    2. Gets ItemData via Items.GetItem(itemId)
    3. Gets price from itemData.configuration.price
    4. Gets resource type from itemData.configuration.resourceType
    5. Calls SceneManager.ConsumeResource(resourceType, price)
    6. If consumer succeeds:
       - Calls SceneManager.AddItem(itemId, false, true)
       - Updates database via DataBaseManager
       - Focuses camera on new item
       - Closes shop window
}
```

**Subcategory to ItemID Mapping:**

- D4 → 3635 (Service)
- C1 → 2496 (Service)
- C4 → 3265 (Resources)
- LIBRARY → 6677 (Resources)
- C7 → 3336 (Student)
- B8 → 5342 (Student)
- GIAI_PHONG_GATE → 2949 (Decorations)
- TDN_GATE → 1251 (Decorations)
- WALL → 7666 (Decorations)
- TREE3 → 5341 (Decorations)

---

## 4. Shop System Display Flow

### ShopWindowScript

**Location:** `ui\windows\shop_window\ShopWindowScript.cs`

Manages the shop UI hierarchy.

**Categories:**

- SERVICE
- RESOURCES
- STUDENT
- DECORATIONS

**Display Flow:**

```
ShopWindowScript (Main Container)
├── CategoryList (Horizontal scroll)
│   └── CategoryItemScript instances (prefabs instantiated)
│       └── OnClickCategory() → calls RenderSubCategories()
├── ItemsList (Horizontal scroll)
│   └── SubCategoryItemScript instances (dynamically created)
│       └── OnClick() → Purchase flow
└── BackButton
```

**Key Methods:**

```csharp
public void Init()
{
    RenderCategories();           // Show: SERVICE, RESOURCES, STUDENT, DECORATIONS
    RenderSubCategories(Category.SERVICE);  // Default to SERVICE
}

public void RenderCategories()
{
    // Creates CategoryItemScript instances for each category
    // Adjusts horizontal scroll layout based on content
}

public void RenderSubCategories(Category category)
{
    // Gets SubCategory[] for selected category
    // Filters items:
    //   - WALL and TREE3 can be bought multiple times
    //   - Other items: only show if NOT already built in scene
    //     (checked via SceneManager.IsItemBuiltInScene(itemId))
    // Creates SubCategoryItemScript prefabs for each available item
    // Updates scroll layout
}

public bool IsItemBuiltInScene(int itemId)
{
    // Iterates _itemInstances, returns true if itemId matches any instance
}
```

---

## 5. SceneManager - Core Game Logic

**Location:** `isometric\SceneManager.cs`

Singleton that manages:

- All game objects in the scene (Dictionary<instanceId, BaseItemScript>)
- World resources and storage
- Item lifecycle (creation, destruction)
- Resource consumption
- Game state transitions

### Key Properties

```csharp
private Dictionary<int, BaseItemScript> _itemInstances;  // All placed items
private ShopLayoutData _shopLayout;                       // Saved building positions

// Resources and storage capacities
public int numberOfGoldInStorage;
public int numberOfDiamondsInStorage;
public int numberOfHappyInStorage;
public int numberOfStudentInStorage;
public int numberOfEducationInStorage;
public int goldStorageCapacity = 1000;
public int diamondStorageCapacity = 100;
// ... etc
```

### Key Methods

#### Adding Items

```csharp
public BaseItemScript AddItem(int itemId, bool immediate, bool ownedItem, int level = 1)
{
    // Determines position for item:
    ItemsCollection.ItemData itemData = Items.GetItem(itemId);

    // Priority:
    1. ShopLayout (saved positions)
    2. Item's defaultPosX, defaultPosZ
    3. Random free position via GroundManager.GetRandomFreePosition()

    // Calls AddItem(itemId, instanceId, posX, posZ, immediate, ownedItem, level)
}

public BaseItemScript AddItem(int itemId, int instanceId, int posX, int posZ,
                              bool immediate, bool ownedItem, int level = 1)
{
    // Creates BaseItemScript instance
    // Gets unique instanceId if needed
    // Adds to _itemInstances Dictionary
    // Calls instance.SetItemData()
    // Sets initial state to IDLE
    // Assigns to builders or immediately completes based on 'immediate' flag
}
```

#### Resource Management

```csharp
public bool ConsumeResource(string resourceType, int count)
{
    // Validates if player has enough resources
    // If yes:
    //   - Deducts from storage
    //   - Saves to PlayerPrefs
    //   - Updates UI via RefreshResourceUIs()
    //   - Returns true
    // If no:
    //   - Returns false

    // Supported resources: "gold", "diamond", "student"
}

public void RefreshResourceUIs(string resourceType)
{
    // Updates GameOverlayWindowScript to show new resource count
    // Updates TrainTroopsWindowScript if open
}
```

#### Item Queries

```csharp
public bool IsItemBuiltInScene(int itemId)
{
    // Checks if any instance has this itemId
    // Used to prevent duplicate purchases (except WALL, TREE3)
}

public void RemoveItem(int instanceId)
{
    // Removes from _itemInstances
    // Fired when item is destroyed
}
```

### Event Subscriptions

```csharp
void Awake()
{
    CameraManager.instance.OnItemTap += OnItemTap;
    CameraManager.instance.OnItemDragStart += OnItemDragStart;
    CameraManager.instance.OnItemDrag += OnItemDrag;
    CameraManager.instance.OnItemDragStop += OnItemDragStop;
    CameraManager.instance.OnTapGround += OnTapGround;
}
```

---

## 6. Complete Shop Purchase Flow

```
┌─────────────────────────────────────────────────────────────┐
│ 1. Player opens shop, ShopWindow displays categories        │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 2. Player clicks category                                   │
│    ShopWindowScript.OnClickCategory()                       │
│    → RenderSubCategories(category)                          │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 3. SubCategoryItemScripts created for available items       │
│    Filtering: IsItemBuiltInScene checks                     │
│    - Some items like WALL can be bought multiple times      │
│    - Most items only show if not already built              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 4. Player clicks SubCategoryItem                            │
│    SubCategoryItemScript.OnClick()                          │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 5. Get item data and price                                  │
│    itemData = Items.GetItem(itemId)                         │
│    price = itemData.configuration.price                     │
│    resourceType = itemData.configuration.resourceType       │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 6. Consume resources                                        │
│    SceneManager.ConsumeResource(resourceType, price)        │
│    ✗ Not enough? → Show error, return                       │
│    ✓ Success? → Continue                                    │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 7. Create item in scene                                     │
│    SceneManager.AddItem(itemId, false, true)               │
│    - Determines position (layout/default/random)            │
│    - Creates BaseItemScript instance                        │
│    - Assigns to builder if not immediate                    │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 8. Update database & utilities                              │
│    DataBaseManager.UpdateItemData(item)                     │
│    CameraManager.FocusOnItem(item)                          │
│    RefreshResourceUIs()                                     │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ 9. Close shop window                                        │
│    ShopWindowScript.Close()                                 │
└─────────────────────────────────────────────────────────────┘
```

---

## 7. Data Flow Architecture

```
┌──────────────────────────────────────────────────────────────┐
│ Persistence Layer (PlayerPrefs)                              │
│ - numberOfGoldInStorage                                      │
│ - numberOfDiamondsInStorage                                  │
│ - numberOfHappyInStorage                                     │
│ - numberOfStudentInStorage                                   │
│ - numberOfEducationInStorage                                 │
└──────────────────┬───────────────────────────────────────────┘
                   │
                   ▼
┌──────────────────────────────────────────────────────────────┐
│ SceneManager (Singleton)                                     │
│ - Resource storage (int counters)                            │
│ - Item instances (Dictionary)                                │
│ - Game state management                                      │
└──────────────────┬───────────────────────────────────────────┘
                   │
       ┌───────────┼────────────┐
       ▼           ▼            ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ BaseItemScript│ │   Items.cs   │ │DataBaseManager│
│ (instances)  │ │  (static)    │ │              │
│              │ │              │ │              │
│ -health      │ │ Dictionary  │ │ Persistence  │
│ -state       │ │   ItemData  │ │ Updates      │
│ -subsystems  │ │             │ │              │
└──────────────┘ └──────────────┘ └──────────────┘
       ▲           ▲                    ▲
       │           │                    │
       └───────────┼────────────────────┘
                   │
           ┌───────┴────────┐
           ▼                ▼
       ┌──────────┐   ┌──────────────────┐
       │ ItemData │   │ SubCategoryItem   │
       │ Template │   │ Script            │
       └──────────┘   │ (UI)              │
           ▲          │                   │
           │          │ Purchase click    │
           │          │ → GetItem()       │
           │          │ → ConsumeResource │
           │          │ → AddItem()       │
           │          └───────┬──────────┘
           │                  │
           └──────────────────┘
              ItemsCollection.cs
          (ScriptableObject asset)
```

---

## 8. File Structure Summary

```
Assets/_Project/Scripts/
├── isometric/
│   ├── SceneManager.cs ⭐ Core game logic, item/resource management
│   ├── CameraManager.cs
│   ├── DataBaseManager.cs
│   ├── GroundManager.cs
│   ├── Main.cs
│   ├── _base_item/
│   │   ├── BaseItemScript.cs ⭐ Core game object
│   │   ├── BaseItemRendererScript.cs
│   │   ├── AttackerScript.cs
│   │   ├── DefenderScript.cs
│   │   ├── ProductionScript.cs
│   │   ├── TrainingScript.cs
│   │   ├── WalkerScript.cs
│   │   └── _ui/
│   │       ├── BaseItemSelectionUIScript.cs
│   │       ├── BaseItemEnergyBarUIScript.cs
│   │       ├── BaseItemCollectNotificationUIScript.cs
│   │       └── BaseItemProgressUIScript.cs
│   └── ... (other managers)
├── ui/
│   ├── UIManager.cs
│   └── windows/
│       └── shop_window/
│           ├── ShopWindowScript.cs ⭐ Shop UI manager
│           ├── SubCategoryItemScript.cs ⭐ Individual shop item
│           └── CategoryItemScript.cs
├── _libs/
│   ├── Items.cs ⭐ Static item loader
│   ├── ItemsCollection.cs ⭐ Item template data structure
│   ├── SpriteCollection.cs
│   ├── Common.cs (enums: State, Direction, etc.)
│   └── Utilities.cs
```

---

## 9. Key Design Patterns

### Pattern 1: Singleton Pattern

- `SceneManager.instance` - Global scene state
- `CameraManager.instance` - Global camera control
- All managers use singleton for easy access

### Pattern 2: Template/Prototype Pattern

- `ItemsCollection.ItemData` - Template for each item type
- `BaseItemScript.SetItemData()` - Instantiates from template
- Separates configuration from runtime instances

### Pattern 3: Composition Pattern

- `BaseItemScript` contains Walker, Attacker, Defender, Production, etc.
- Each subsystem handles its domain
- DecoratorPattern for adding behaviors

### Pattern 4: State Pattern

- `BaseItemScript.state` - IDLE, WALK, ATTACK, DESTROYED
- Different behaviors per state

### Pattern 5: Resource Location Pattern

- `Items.LoadItems()` - Resources.Load("ItemsCollection")
- Resources folder contains ScriptableObject definitions

### Pattern 6: Event-Driven Architecture

- CameraManager broadcasts events (OnItemTap, OnItemDrag, etc.)
- SceneManager subscribes to input events
- Decouples input from game logic

---

## 10. Key Data Flows

### Resource Purchase Flow

```
User clicks SubCategoryItem
  → Gets item price (itemData.configuration.price)
  → Calls ConsumeResource(resourceType, price)
    → Checks if numberOfXxxInStorage >= price
    → If yes: Deducts amount, saves to PlayerPrefs, updates UI
    → Returns true/false
  → If successful: Creates item via AddItem()
```

### Item Creation Flow

```
AddItem(itemId, immediate=false, ownedItem=true)
  → Gets itemData from Items.GetItem(itemId)
  → Determines position (from layout, default, or random)
  → Creates BaseItemScript instance
  → Calls instance.SetItemData()
    → Sets health, level, size
    → Initializes subsystems (Walker, Attacker, UI, etc.)
    → Configures collider based on grid dimensions
  → Adds to _itemInstances Dictionary
```

---

## 11. Storage and Persistence

- **Resources**: Stored in PlayerPrefs via `SceneManager.SaveResources()`
- **Item Layout**: Stored in ShopLayoutData (via DataBaseManager)
- **Item Data**: Loaded from ItemsCollection ScriptableObject in Resources folder
- **Game State**: Managed by DataBaseManager (outside scope of this summary)

---

## Summary

**BaseItemScript** is a MonoBehaviour representing any game object (building/unit).
**ItemData** is a serializable template defining item properties.
**Items.cs** loads all ItemData templates from a ScriptableObject.
**ShopWindowScript** manages the shop UI (categories and subcategories).
**SubCategoryItemScript** displays individual shop items and handles purchases.
**SceneManager** manages the scene state, resources, and item lifecycle.

The architecture uses **composition, singletons, templates, and events** to create a maintainable, modular system for a city-building game.
