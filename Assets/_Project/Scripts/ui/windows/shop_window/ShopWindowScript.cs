using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindowScript : WindowScript
{
	/* prefabs */
	public GameObject CategoryItem;
	public GameObject SubCategoryItem;

	/* references */
	public ScrollRect ScrollView;
	public GameObject ItemsList;
	public GameObject CategoryList;
	public GameObject BackButton;

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
		BARRACK,
		BOAT,
		D4,
		CAMP,
		CANNON,
		// ELIXIR_COLLECTOR,
		// ELIXIR_STORAGE,
		B7,
		B8,
		C4,
		GOLD_STORAGE,
		TOWER,
		C1,
		TREE1,
		TREE2,
		TREE3,
		LIBRARY,
		WALL

	}


	void Awake()
	{
		this.Init();
	}

	public void Init()
	{
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
		this.BackButton.SetActive(true);

		this.ClearItemsList();

		SubCategory[] subItems = new SubCategory[0];

		switch (category)
		{
			// case Category.ARMY:
			// 	subItems = new SubCategory[]{ SubCategory.BARRACK, SubCategory.CAMP, SubCategory.BOAT};
			// 	break;
			case Category.RESOURCES:
				subItems = new SubCategory[] { SubCategory.C4, SubCategory.GOLD_STORAGE, SubCategory.LIBRARY };
				break;
			case Category.DECORATIONS:
				subItems = new SubCategory[] { SubCategory.TREE1, SubCategory.TREE2, SubCategory.TREE3 };
				break;
			// case Category.DEFENCE:
			// 	subItems = new SubCategory[]{ SubCategory.CANNON, SubCategory.TOWER};
			// 	break;
			case Category.SERVICE:
				subItems = new SubCategory[] { SubCategory.C1, SubCategory.D4, SubCategory.WALL };
				break;

			case Category.STUDENT:
				subItems = new SubCategory[] { SubCategory.B7, SubCategory.B8 };
				break;
		}

		List<SubCategory> validSubItems = new List<SubCategory>();
		for (int index = 0; index < subItems.Length; index++)
		{
			SubCategory subCat = subItems[index];

			// Allow walls and trees to be bought multiple times
			bool canBuyMultiple = (subCat == SubCategory.WALL || subCat == SubCategory.TREE1 || subCat == SubCategory.TREE2 || subCat == SubCategory.TREE3);
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
			case SubCategory.GOLD_STORAGE: return 9074;
			case SubCategory.C1: return 2496;
			case SubCategory.LIBRARY: return 6677;
			case SubCategory.WALL: return 7666;
			case SubCategory.TREE1: return 2949;
			case SubCategory.TREE2: return 1251;
			case SubCategory.TREE3: return 5341;
			case SubCategory.B7: return 3336;
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

}
