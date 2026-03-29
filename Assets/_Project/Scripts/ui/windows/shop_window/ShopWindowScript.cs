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
		C4,
		D4,
		// CAMP,
		// CANNON,
		// ELIXIR_COLLECTOR,
		// ELIXIR_STORAGE,
		B8,
		C7,
		// GOLD_STORAGE,
		// TOWER,
		GIAI_PHONG_GATE,
		TDN_GATE,
		TREE3,
		LIBRARY,
		WALL
	}


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
				subItems = new SubCategory[] { SubCategory.C1, SubCategory.D4 };
				break;
			case Category.RESOURCES:
				subItems = new SubCategory[] { SubCategory.C4, SubCategory.LIBRARY };
				break;
			case Category.STUDENT:
				subItems = new SubCategory[] { SubCategory.C7, SubCategory.B8 };
				break;
			case Category.DECORATIONS:
				subItems = new SubCategory[] { SubCategory.GIAI_PHONG_GATE, SubCategory.TDN_GATE, SubCategory.WALL, SubCategory.TREE3 };
				break;
			// case Category.DEFENCE:
			// 	subItems = new SubCategory[]{ SubCategory.CANNON, SubCategory.TOWER};
			// 	break;
		}

		List<SubCategory> validSubItems = new List<SubCategory>();
		for (int index = 0; index < subItems.Length; index++)
		{
			SubCategory subCat = subItems[index];

			// Allow walls and trees to be bought multiple times
			bool canBuyMultiple = (subCat == SubCategory.WALL || subCat == SubCategory.TREE3);
			int itemId = GetItemIdFromSubCategory(subCat);

			if (canBuyMultiple || !SceneManager.instance.IsItemBuiltInScene(itemId))
			{
				GameObject inst = Utilities.CreateInstance(this.SubCategoryItem, this.ItemsList, true);
				inst.GetComponent<SubCategoryItemScript>().SetSubCategory(subCat);
				validSubItems.Add(subCat);
			}
		}

		RectTransform rt = this.ItemsList.GetComponent<RectTransform>();
		Vector2 sizeDelta = this.ItemsList.GetComponent<RectTransform>().sizeDelta;
		GridLayoutGroup glg = this.ItemsList.GetComponent<GridLayoutGroup>();
		float spacing = glg != null ? glg.spacing.x : 0;
		sizeDelta.x = validSubItems.Count * 250 + validSubItems.Count * spacing;
		rt.sizeDelta = sizeDelta;

		this.ResetScrollPosition();
	}

	public int GetItemIdFromSubCategory(SubCategory subCategory)
	{
		switch (subCategory)
		{
			case SubCategory.D4: return 3635;
			case SubCategory.C4: return 3265;
			// case SubCategory.GOLD_STORAGE: return 9074;
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

		// Create SubCategoryItem for each itemId
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
