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
	
	public Button FirstRollButton;
	public Button LastRollButton;

	public ProgressPanelScript GoldInfo;
	public ProgressPanelScript DiamondInfo;
	public ProgressPanelScript StudentInfo;
	public Animator anim;

	/* Map Shop references */
	private bool _isMapShopMode = false;
	private string _currentMapShopName = "";
	private Category _currentCategory = Category.SERVICE;

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
		ITIMS,
		SECURITY_ROOM,
		PC_LAB,
		GIAI_PHONG_GATE,
		TDN_GATE,
		TREE3,
		LIBRARY,
		WALL,
		MONEY_LAKE,
		D9,
		TTVD,
		Alumni,
		SAN_C2,
		ICEA,
		KHUON_VIEN_C1
	}

	public static SubCategory[] ServiceSubItems = new SubCategory[] { SubCategory.C1, SubCategory.C2, SubCategory.C3, SubCategory.C3B, SubCategory.C5, SubCategory.C6, SubCategory.C9, SubCategory.C10, SubCategory.D4, SubCategory.D35, SubCategory.D6, SubCategory.D8 };
	public static SubCategory[] ResourcesSubItems = new SubCategory[] { SubCategory.C4, SubCategory.LIBRARY, SubCategory.Canteen, SubCategory.GaraD6, SubCategory.ITIMS, SubCategory.SECURITY_ROOM, SubCategory.PC_LAB, SubCategory.TTVD, SubCategory.Alumni, SubCategory.ICEA };
	public static SubCategory[] StudentSubItems = new SubCategory[] { SubCategory.C7, SubCategory.D9 };
	public static SubCategory[] DecorationsSubItems = new SubCategory[] { SubCategory.MONEY_LAKE, SubCategory.SAN_C2, SubCategory.KHUON_VIEN_C1 };



	void Awake()
	{
		instance = this;
		this.Init();
	}

	void Start()
	{
		if (FirstRollButton != null)
		{
			FirstRollButton.onClick.AddListener(OnClickFirstRoll);
		}
		if (LastRollButton != null)
		{
			LastRollButton.onClick.AddListener(OnClickLastRoll);
		}

		if (this.GoldInfo != null && SceneManager.instance != null)
		{
			this.GoldInfo.hasMaxValue = true;
			this.GoldInfo.maxValue = SceneManager.instance.goldStorageCapacity;
			this.GoldInfo.value = SceneManager.instance.numberOfGoldInStorage;
		}

		if (this.DiamondInfo != null && SceneManager.instance != null)
		{
			this.DiamondInfo.hasMaxValue = true;
			this.DiamondInfo.maxValue = SceneManager.instance.diamondStorageCapacity;
			this.DiamondInfo.value = SceneManager.instance.numberOfDiamondsInStorage;
		}

		if (this.StudentInfo != null && SceneManager.instance != null)
		{
			this.StudentInfo.hasMaxValue = true;
			this.StudentInfo.maxValue = SceneManager.instance.studentStorageCapacity;
			this.StudentInfo.value = SceneManager.instance.numberOfStudentInStorage;
			this.StudentInfo.showAsCurrentMax = true;
		}
	}

	public void Init()
	{
		_isMapShopMode = false;
		_currentMapShopName = "";
		_currentCategory = Category.SERVICE;
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
			CategoryItemScript itemScript = inst.GetComponent<CategoryItemScript>();
			itemScript.SetCategory(categories[index]);
			itemScript.SetActiveState(categories[index] == _currentCategory);
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

			bool unlockedA = SceneManager.instance.currentLevel >= dataA.configuration.unlockItemAtSemester;
			bool unlockedB = SceneManager.instance.currentLevel >= dataB.configuration.unlockItemAtSemester;

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

	public static string GetCategoryStringFromItemId(int itemId)
	{
		foreach (var sub in ServiceSubItems)
		{
			if (GetItemIdFromSubCategory(sub) == itemId) return "Dịch vụ";
		}
		foreach (var sub in ResourcesSubItems)
		{
			if (GetItemIdFromSubCategory(sub) == itemId) return "Thương mại";
		}
		foreach (var sub in StudentSubItems)
		{
			if (GetItemIdFromSubCategory(sub) == itemId) return "Sinh viên";
		}
		foreach (var sub in DecorationsSubItems)
		{
			if (GetItemIdFromSubCategory(sub) == itemId) return "Trang trí";
		}
		return "";
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
			case SubCategory.ITIMS: return 3090;
			case SubCategory.SECURITY_ROOM: return 1628;
			case SubCategory.PC_LAB: return 9138;
			case SubCategory.MONEY_LAKE: return 9242;
			// case SubCategory.B8: return 5342;
			case SubCategory.D9: return 9818;
			case SubCategory.TTVD: return 3702;
			case SubCategory.Alumni: return 8099;
			case SubCategory.SAN_C2: return 6437;
			case SubCategory.ICEA: return 4073;
			case SubCategory.KHUON_VIEN_C1: return 4563;
			default: return 0;
		}
	}

	public void OnClickCategory(Category category)
	{
		_currentCategory = category;

		foreach (Transform child in CategoryList.transform)
		{
			CategoryItemScript item = child.GetComponent<CategoryItemScript>();
			if (item != null)
			{
				item.SetActiveState(item.GetCategory() == _currentCategory);
			}
		}

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

	private Coroutine _scrollCoroutine;

	private float EaseOutBack(float t)
	{
		float c1 = 1.70158f;
		float c3 = c1 + 1f;
		return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
	}

	private IEnumerator TweenScrollTo(float targetPosition, float duration = 0.3f)
	{
		if (this.ScrollView == null) yield break;

		float startPosition = this.ScrollView.horizontalNormalizedPosition;
		float timeElapsed = 0f;

		while (timeElapsed < duration)
		{
			float t = timeElapsed / duration;
			float easedT = EaseOutBack(t);
			this.ScrollView.horizontalNormalizedPosition = Mathf.LerpUnclamped(startPosition, targetPosition, easedT);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		this.ScrollView.horizontalNormalizedPosition = targetPosition;
	}

	public void OnClickFirstRoll()
	{
		if (this.ScrollView != null)
		{
			if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine);
			_scrollCoroutine = StartCoroutine(TweenScrollTo(0.0f));
		}
	}

	public void OnClickLastRoll()
	{
		if (this.ScrollView != null)
		{
			if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine);
			_scrollCoroutine = StartCoroutine(TweenScrollTo(1.0f));
		}
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

			bool unlockedA = SceneManager.instance.currentLevel >= dataA.configuration.unlockItemAtSemester;
			bool unlockedB = SceneManager.instance.currentLevel >= dataB.configuration.unlockItemAtSemester;

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

	public void HideWindow()
	{
		if (anim != null) anim.Play("Hide");
	}

	public void ShowWindow()
	{
		if (anim != null) anim.Play("Show");
	}
}
