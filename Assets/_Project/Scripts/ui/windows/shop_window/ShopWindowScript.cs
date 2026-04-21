using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindowScript : WindowScript
{
	public static ShopWindowScript instance;

	/* prefabs */
	public GameObject CategoryItem;
	public GameObject SubCategoryItem;

	/* references */
	public ScrollRect ScrollView;
	public GameObject ItemsList;
	public GameObject CategoryList;
	public GameObject BackButton;

	/* Map Shop references */
	private bool _isMapShopMode = false;
	private string _currentMapShopName = "";

	public enum Category
	{
		// ARMY,
		// DEFENCE,
		RESOURCES,
		SERVICE,
		STUDENT,
		DECORATIONS
	}

	public enum SubCategory
	{
		// BARRACK,
		// BOAT,
		C1,
		C2,
		C3,
		C3B,
		C4,
		C5,
		C6,
		C7,
		C9,
		C10,
		D4,
		D35,
		D6,
		D8,
		// CAMP,
		// CANNON,
		// ELIXIR_COLLECTOR,
		// ELIXIR_STORAGE,
		B8,
		// GOLD_STORAGE,
		// TOWER,
		Canteen,
		GaraD6,
		GIAI_PHONG_GATE,
		TDN_GATE,
		TREE3,
		LIBRARY,
		WALL
	}

	public static SubCategory[] ServiceSubItems = new SubCategory[] { SubCategory.C1, SubCategory.C2, SubCategory.C3, SubCategory.C3B, SubCategory.C5, SubCategory.C6, SubCategory.C9, SubCategory.C10, SubCategory.D4, SubCategory.D35, SubCategory.D6, SubCategory.D8 };
	public static SubCategory[] ResourcesSubItems = new SubCategory[] { SubCategory.C4, SubCategory.LIBRARY, SubCategory.Canteen, SubCategory.GaraD6 };
	public static SubCategory[] StudentSubItems = new SubCategory[] { SubCategory.C7, SubCategory.B8 };
	public static SubCategory[] DecorationsSubItems = new SubCategory[] { SubCategory.GIAI_PHONG_GATE, SubCategory.TDN_GATE, SubCategory.WALL, SubCategory.TREE3 };



	void Awake()
	{
		instance = this;
		this.Init();
	}

	public void Init()
	{
		_isMapShopMode = false;
		_currentMapShopName = "";
		this.RenderCategories();
		this.RenderSubCategories(Category.SERVICE);
	}

	public void RenderCategories()
	{
		this.ClearCategoryList();

		Category[] categories = new Category[] {
			// Category.ARMY,
			Category.SERVICE,
			Category.RESOURCES,
			Category.STUDENT,
			Category.DECORATIONS
			// Category.DEFENCE,
		};

		for (int index = 0; index < categories.Length; index++)
		{
			GameObject inst = Utilities.CreateInstance(this.CategoryItem, this.CategoryList, true);
			inst.GetComponent<CategoryItemScript>().SetCategory(categories[index]);
		}

		RectTransform rt = this.CategoryList.GetComponent<RectTransform>();
		Vector2 sizeDelta = this.CategoryList.GetComponent<RectTransform>().sizeDelta;
		GridLayoutGroup glg = this.CategoryList.GetComponent<GridLayoutGroup>();
		float spacing = glg != null ? glg.spacing.x : 0;
		sizeDelta.x = categories.Length * 250 + categories.Length * spacing;
		rt.sizeDelta = sizeDelta;

		this.ResetScrollPosition();
	}

	public void RenderSubCategories(Category category)
	{
		this.ClearItemsList();

		SubCategory[] subItems = new SubCategory[0];

		switch (category)
		{
			// case Category.ARMY:
			// 	subItems = new SubCategory[]{ SubCategory.BARRACK, SubCategory.CAMP, SubCategory.BOAT};
			// 	break;
			case Category.SERVICE:
				subItems = ServiceSubItems;
				break;
			case Category.RESOURCES:
				subItems = ResourcesSubItems;
				break;
			case Category.STUDENT:
				subItems = StudentSubItems;
				break;
			case Category.DECORATIONS:
				subItems = DecorationsSubItems;
				break;
			// case Category.DEFENCE:
			// 	subItems = new SubCategory[]{ SubCategory.CANNON, SubCategory.TOWER};
			// 	break;
		}

		List<SubCategory> validSubItems = new List<SubCategory>();
		for (int index = 0; index < subItems.Length; index++)
		{
			SubCategory subCat = subItems[index];
			bool canBuyMultiple = (subCat == SubCategory.WALL || subCat == SubCategory.TREE3);
			int itemId = GetItemIdFromSubCategory(subCat);

			if (canBuyMultiple || !SceneManager.instance.IsItemBuiltInScene(itemId))
			{
				validSubItems.Add(subCat);
			}
		}

		// Sắp xếp: item đã unlock lên trước, sau đó sắp xếp theo semester yêu cầu
		validSubItems.Sort((a, b) =>
		{
			int itemIdA = GetItemIdFromSubCategory(a);
			int itemIdB = GetItemIdFromSubCategory(b);
			ItemsCollection.ItemData dataA = Items.GetItem(itemIdA);
			ItemsCollection.ItemData dataB = Items.GetItem(itemIdB);

			bool unlockedA = SceneManager.instance.currentSemester >= dataA.configuration.unlockItemAtSemester;
			bool unlockedB = SceneManager.instance.currentSemester >= dataB.configuration.unlockItemAtSemester;

			// Nếu trạng thái unlock khác nhau, cái nào unlock rồi thì lên trước (-1)
			if (unlockedA != unlockedB)
			{
				return unlockedA ? -1 : 1;
			}

			// Nếu cùng trạng thái (cùng khóa hoặc cùng mở), sắp xếp theo số kỳ yêu cầu
			return dataA.configuration.unlockItemAtSemester.CompareTo(dataB.configuration.unlockItemAtSemester);
		});

		// Hiển thị danh sách đã sắp xếp
		foreach (SubCategory subCat in validSubItems)
		{
			GameObject inst = Utilities.CreateInstance(this.SubCategoryItem, this.ItemsList, true);
			inst.GetComponent<SubCategoryItemScript>().SetSubCategory(subCat);
		}


		RectTransform rt = this.ItemsList.GetComponent<RectTransform>();
		Vector2 sizeDelta = this.ItemsList.GetComponent<RectTransform>().sizeDelta;
		GridLayoutGroup glg = this.ItemsList.GetComponent<GridLayoutGroup>();
		float spacing = glg != null ? glg.spacing.x : 0;
		sizeDelta.x = validSubItems.Count * 250 + validSubItems.Count * spacing;
		rt.sizeDelta = sizeDelta;

		this.ResetScrollPosition();
	}

	public static List<int> GetAllShopItemIds()
	{
		List<int> ids = new List<int>();
		// Chỉ lấy các ID từ những danh mục là tòa nhà (Service, Resources, Student)
		// Bỏ qua Decorations theo yêu cầu
		AddSubItemsToIds(ids, ServiceSubItems);
		AddSubItemsToIds(ids, ResourcesSubItems);
		AddSubItemsToIds(ids, StudentSubItems);
		// AddSubItemsToIds(ids, DecorationsSubItems); // Bỏ qua đồ trang trí
		return ids;
	}

	private static void AddSubItemsToIds(List<int> ids, SubCategory[] subItems)
	{
		foreach (var sub in subItems)
		{
			int id = GetItemIdFromSubCategory(sub);
			if (id != 0 && !ids.Contains(id))
			{
				ids.Add(id);
			}
		}
	}

	public static int GetItemIdFromSubCategory(SubCategory subCategory)
	{
		switch (subCategory)
		{
			case SubCategory.D4: return 3635;
			case SubCategory.C4: return 3265;
			// case SubCategory.GOLD_STORAGE: return 9074;
			case SubCategory.C1: return 2496;
			case SubCategory.C2: return 8216;
			case SubCategory.C3: return 2454;
			case SubCategory.C3B: return 5835;
			case SubCategory.C5: return 3504;
			case SubCategory.C6: return 2617;
			case SubCategory.C9: return 9295;
			case SubCategory.C10: return 8385;
			case SubCategory.D35: return 4407;
			case SubCategory.D6: return 6330;
			case SubCategory.D8: return 5134;
			case SubCategory.Canteen: return 1399;
			case SubCategory.GaraD6: return 4132;
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

	public void OnClickCategory(Category category)
	{
		this.RenderSubCategories(category);
	}

	public void ClearItemsList()
	{
		foreach (Transform child in this.ItemsList.transform)
		{
			Destroy(child.gameObject);
		}
	}

	public void OnClickBackButton()
	{
		this.ClearItemsList();
		this.BackButton.SetActive(false);
	}

	public void ClearCategoryList()
	{
		foreach (Transform child in this.CategoryList.transform)
		{
			Destroy(child.gameObject);
		}
	}

	public void ResetScrollPosition()
	{
		this.ScrollView.horizontalNormalizedPosition = 0.0f;
	}

	/// <summary>
	/// Render items for Map Shop areas (not category-based, just a list of items)
	/// </summary>
	public void RenderMapShop(string areaName, List<int> itemIds)
	{
		_isMapShopMode = true;
		_currentMapShopName = areaName;

		this.ClearItemsList();
		this.ClearCategoryList();
		this.CategoryList.SetActive(false);
		this.BackButton.SetActive(true);

		Debug.Log($"[ShopWindow] RenderMapShop: area='{areaName}', itemCount={itemIds.Count}");

		// Sắp xếp danh sách itemIds trước khi hiển thị
		itemIds.Sort((a, b) =>
		{
			ItemsCollection.ItemData dataA = Items.GetItem(a);
			ItemsCollection.ItemData dataB = Items.GetItem(b);

			bool unlockedA = SceneManager.instance.currentSemester >= dataA.configuration.unlockItemAtSemester;
			bool unlockedB = SceneManager.instance.currentSemester >= dataB.configuration.unlockItemAtSemester;

			if (unlockedA != unlockedB)
			{
				return unlockedA ? -1 : 1;
			}

			return dataA.configuration.unlockItemAtSemester.CompareTo(dataB.configuration.unlockItemAtSemester);
		});

		// Create SubCategoryItem for each itemId (đã sắp xếp)
		for (int i = 0; i < itemIds.Count; i++)
		{
			int itemId = itemIds[i];
			ItemsCollection.ItemData itemData = Items.GetItem(itemId);
			
			if (itemData != null)
			{
				Debug.Log($"[ShopWindow] Creating item UI for itemId={itemId} ({itemData.name})");
				GameObject inst = Utilities.CreateInstance(this.SubCategoryItem, this.ItemsList, true);
				MapShopItemScript shopItem = inst.GetComponent<MapShopItemScript>();
				
				if (shopItem != null)
				{
					shopItem.SetItemData(itemId, itemData);
				}
				else
				{
					Debug.LogWarning($"[ShopWindow] SubCategoryItem prefab doesn't have MapShopItemScript component!");
				}
			}
			else
			{
				Debug.LogWarning($"[ShopWindow] ItemId {itemId} not found in Items database!");
			}
		}


		// Adjust layout for items count
		RectTransform rt = this.ItemsList.GetComponent<RectTransform>();
		Vector2 sizeDelta = this.ItemsList.GetComponent<RectTransform>().sizeDelta;
		GridLayoutGroup glg = this.ItemsList.GetComponent<GridLayoutGroup>();
		float spacing = glg != null ? glg.spacing.x : 0;
		sizeDelta.x = itemIds.Count * 250 + itemIds.Count * spacing;
		rt.sizeDelta = sizeDelta;

		this.ResetScrollPosition();
		Debug.Log($"[ShopWindow] RenderMapShop completed!");
	}

	public void Open()
	{
		this.gameObject.SetActive(true);
	}

	public override void Close()
	{
		_isMapShopMode = false;
		_currentMapShopName = "";
		base.Close();
	}

	public bool IsMapShopMode()
	{
		return _isMapShopMode;
	}

	public string GetCurrentMapShopName()
	{
		return _currentMapShopName;
	}

}
