using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using B70.Balance;

public class SceneManager : MonoBehaviour
{

	public static SceneManager instance;

	/* prefabs */
	public GameObject BaseItem;
	public GameObject MenuWindow;
	public GameObject MapShopAreaPrefab;

	public GameObject RenderQuad;
	public Material RenderQuadMaterial;

	/* object refs */
	public GameObject Design;
	public GameObject ItemsContainer;
	public GameObject ParticlesContainer;

	public GameObject Grid;

	/* public vars */
	public Common.GameMode gameMode = Common.GameMode.NORMAL;

	/* private vars */
	private Dictionary<int, BaseItemScript> _itemInstances;
	private Dictionary<int, MapShopAreaScript> _activeShopAreas = new Dictionary<int, MapShopAreaScript>();
	private ShopLayoutData _shopLayout;

	//resource values
	public int numberOfGoldInStorage;
	// public int numberOfElixirInStorage;
	public int numberOfDiamondsInStorage;
	public int numberOfHappyInStorage;
	public int numberOfStudentInStorage;
	public int numberOfEducationInStorage;

	public int goldStorageCapacity;
	public int diamondStorageCapacity;
	public int happyStorageCapacity;
	public int studentStorageCapacity;
	public int educationStorageCapacity;
	public int currentSemester;
	public int currentLevel = 0;
	public float levelProgress; 
	private bool _hasShownUnlockThisLevel = false;
	public bool isTutorialActive;
	private bool _isCompletingSemester = false;
	
	public int totalSpawnedNPCs = 0;

	void Awake()
	{
		instance = this;
		this.Design.SetActive(false);

		this._itemInstances = new Dictionary<int, BaseItemScript>();

		/* registering events */
		CameraManager.instance.OnItemTap += this.OnItemTap;
		CameraManager.instance.OnItemDragStart += this.OnItemDragStart;
		CameraManager.instance.OnItemDrag += this.OnItemDrag;
		CameraManager.instance.OnItemDragStop += this.OnItemDragStop;
		CameraManager.instance.OnTapGround += this.OnTapGround;

		GroundManager.instance.UpdateAllNodes();
		this._shopLayout = DataBaseManager.instance.GetShopLayout();
		this.Init();
	}

	// Start is a coroutine to ensure UIManager is initialized before showing MenuWindow
	private System.Collections.IEnumerator Start()
	{
		// wait until UIManager.instance is available
		while (UIManager.instance == null || UIManager.instance.WindowsContainer == null)
		{
			yield return null;
		}

		// instantiate MenuWindow into WindowsContainer if assigned
		if (this.MenuWindow != null)
		{
			UIManager.instance.ShowWindow(this.MenuWindow);
		}
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	public void Init()
	{
		// Do not enter normal mode automatically. Show MenuWindow first and wait for user Play.
		this.goldStorageCapacity = 10000;
		this.diamondStorageCapacity = 20;
		this.studentStorageCapacity = 500;    // Base capacity — tăng thêm khi xây công trình.
		this.happyStorageCapacity = 100;       // Happiness [0, 100]
		this.educationStorageCapacity = 100;   // Education [0, 100]
		// this.elixirStorageCapacity = 500;

		// ── Giá trị mặc định lần đầu chơi ────────────────────────────────
		// Happiness = 50 : ngưỡng trung lập → không bị phạt dropout, không thưởng.
		// Education  = 50 : t1 = 50 → bắt đầu có freshmen nhập học.
		this.numberOfGoldInStorage      = PlayerPrefs.GetInt("numberOfGoldInStorage",      200);
		this.numberOfDiamondsInStorage  = PlayerPrefs.GetInt("numberOfDiamondsInStorage",  20);
		this.numberOfStudentInStorage   = PlayerPrefs.GetInt("numberOfStudentInStorage",   0);
		this.numberOfHappyInStorage     = PlayerPrefs.GetInt("numberOfHappyInStorage",     50);   // neutralH = 50
		this.numberOfEducationInStorage = PlayerPrefs.GetInt("numberOfEducationInStorage", 50);   // t1 = 30
		this.currentSemester            = PlayerPrefs.GetInt("currentSemester",            1);
		this.currentLevel               = PlayerPrefs.GetInt("currentLevel",               1);
		this.levelProgress              = PlayerPrefs.GetFloat("levelProgress",            0);
		this.totalSpawnedNPCs           = PlayerPrefs.GetInt("totalSpawnedNPCs",           0);
	}

	private void OnApplicationQuit()
	{
		if (DataBaseManager.instance != null)
		{
			DataBaseManager.instance.SaveScene();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && DataBaseManager.instance != null)
		{
			DataBaseManager.instance.SaveScene();
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus && DataBaseManager.instance != null)
		{
			DataBaseManager.instance.SaveScene();
		}
	}

	/// <summary>
	/// Saves current resource values to PlayerPrefs.
	/// </summary>
	public void SaveResources()
	{
		PlayerPrefs.SetInt("numberOfGoldInStorage", this.numberOfGoldInStorage);
		PlayerPrefs.SetInt("numberOfDiamondsInStorage", this.numberOfDiamondsInStorage);
		PlayerPrefs.SetInt("numberOfHappyInStorage", this.numberOfHappyInStorage);
		PlayerPrefs.SetInt("numberOfStudentInStorage", this.numberOfStudentInStorage);
		PlayerPrefs.SetInt("numberOfEducationInStorage", this.numberOfEducationInStorage);
		PlayerPrefs.SetInt("currentSemester", this.currentSemester);
		PlayerPrefs.SetInt("currentLevel", this.currentLevel);
		PlayerPrefs.SetFloat("levelProgress", this.levelProgress);
		PlayerPrefs.SetInt("totalSpawnedNPCs", this.totalSpawnedNPCs);
		// PlayerPrefs.SetInt("numberOfElixirInStorage", this.numberOfElixirInStorage);

		PlayerPrefs.Save();
	}


	/// <summary>
	/// Adds the item with itemId. where itemId is the id which we registered with item prefab as unique.
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="itemId">Item identifier.</param>
	public BaseItemScript AddItem(int itemId, int instanceId, int posX, int posZ, bool immediate, bool ownedItem, int level = 1, double lastCollectedTime = 0, bool isPreview = false, List<Vector2Int> extraFootprint = null)
	{
		BaseItemScript builder = null;

		if (!immediate)
		{
			builder = this.GetFreeBuilder();
			// if (builder == null)
			// {
			// 	Debug.Log("All builders are busy!");
			// 	UIManager.instance.ShowBuildersBusyWindow();
			// 	return null;
			// }
		}

		BaseItemScript instance = Utilities.CreateInstance(this.BaseItem, this.ItemsContainer, true).GetComponent<BaseItemScript>();

		if (instanceId == -1)
		{
			instanceId = this._GetUnusedInstanceId();
		}

		instance.instanceId = instanceId;
		this._itemInstances.Add(instanceId, instance);

		instance.SetItemData(itemId, posX, posZ, level, lastCollectedTime);
		if (extraFootprint != null) instance.extraFootprint = new List<Vector2Int>(extraFootprint);
		
		if (isPreview)
		{
			instance.SetState(Common.State.PREVIEW);
		}
		else
		{
			instance.SetState(Common.State.IDLE);
		}
		if (_activeShopAreas.ContainsKey(itemId))
		{
			if (_activeShopAreas[itemId] != null)
			{
				Destroy(_activeShopAreas[itemId].gameObject);
			}
			_activeShopAreas.Remove(itemId);
		}

		//		GroundManager.Cell freeCell = GroundManager.instance.GetRandomFreeCellForItem (instance);
		//		instance.SetPosition (GroundManager.instance.CellToPosition (freeCell));

		if (!immediate && !isPreview)
		{
			instance.StartConstruction(builder);
			instance.OnConstructionComplete = (item) =>
			{
				this.UpdateStudentStorageCapacity();
						this.UpdateLevelProgress();
			};

			// if (!instance.itemData.configuration.isCharacter && instance.itemData.configuration.buildTime > 0)
			// {
			// 	builder.BuilderAction(instance);
			// }
		}

		if (!instance.itemData.configuration.isCharacter)
		{
			GroundManager.instance.UpdateBaseItemNodes(instance, GroundManager.Action.ADD);
		}

		if (instance.itemData.name == "Wall")
		{
			this.UpdateWalls();
		}
		instance.ownedItem = ownedItem;

		if (ownedItem)
		{
			DataBaseManager.instance.SaveScene();
		}

		return instance;
	}

	public BaseItemScript AddItem(int itemId, bool immediate, bool ownedItem, int level = 1, bool isPreview = false)
	{
		int posX = 0;
		int posZ = 0;
		ShopLayoutItem layoutItem = _shopLayout.items.Find(i => i.itemId == itemId);

		if (!immediate)
		{
			ItemsCollection.ItemData itemData = Items.GetItem(itemId);

			// Check Shop Layout first
			if (layoutItem != null)
			{
				posX = layoutItem.posX;
				posZ = layoutItem.posZ;
			}
			else if (!((itemData.defaultPosX == -1 && itemData.defaultPosZ == -1) || (itemData.defaultPosX == -999 && itemData.defaultPosZ == -999)))
			{
				posX = itemData.defaultPosX;
				posZ = itemData.defaultPosZ;
			}
			else
			{
				Vector3 freePosition = GroundManager.instance.GetRandomFreePositionForItem(itemData.gridWidth, itemData.gridHeight);
				posX = (int)freePosition.x;
				posZ = (int)freePosition.z;
			}
		}
		return this.AddItem(itemId, -1, posX, posZ, immediate, ownedItem, level, 0, isPreview, layoutItem?.extraFootprint);
	}

	/// <summary>
	/// Removes the item.
	/// </summary>
	/// <param name="item">Item.</param>
	public void RemoveItem(BaseItemScript item)
	{
		this._itemInstances.Remove(item.instanceId);
		if (item != null)
		{
			// Clear grid nodes
			if (!item.itemData.configuration.isCharacter)
			{
				GroundManager.instance.UpdateBaseItemNodes(item, GroundManager.Action.REMOVE);
			}

			if (this.selectedItem == item) this.selectedItem = null;
			if (this._dragItem == item) this._dragItem = null;

			// Recreate the Map Shop Area if the item is part of the Shop Layout
			if (this._shopLayout != null && this._shopLayout.items != null && this.MapShopAreaPrefab != null)
			{
				int itemId = item.itemData.id;
				ShopLayoutItem layoutItem = this._shopLayout.items.Find(i => i.itemId == itemId);
				if (layoutItem != null)
				{
					if (!this._activeShopAreas.ContainsKey(itemId) || this._activeShopAreas[itemId] == null)
					{
						ItemsCollection.ItemData data = Items.GetItem(layoutItem.itemId);
						Vector3 pos = new Vector3(layoutItem.posX + (data != null ? data.gridWidth / 2f : 0), 0, layoutItem.posZ + (data != null ? data.gridHeight / 2f : 0));
						
						GameObject shopAreaObj = Utilities.CreateInstance(this.MapShopAreaPrefab, this.ItemsContainer, true);
						shopAreaObj.transform.localPosition = pos;
						
						MapShopAreaScript shopAreaScript = shopAreaObj.GetComponent<MapShopAreaScript>();
						if (shopAreaScript != null)
						{
							shopAreaScript.ClearItems();
							shopAreaScript.AddItem(layoutItem.itemId);
							
							if (data != null)
							{
								shopAreaScript.areaName = "Buy " + data.name;
							}
							
							this._activeShopAreas[itemId] = shopAreaScript;
						}
					}
				}
			}

			DataBaseManager.instance.RemoveItem(item);
			Destroy(item.gameObject);
		}
	}

	private int _GetUnusedInstanceId()
	{
		int instanceId = Random.Range(10000, 99999);
		if (this._itemInstances.ContainsKey(instanceId))
		{
			return _GetUnusedInstanceId();
		}
		return instanceId;
	}


	private BaseItemScript _dragItem;
	/// <summary>
	/// Raises the item drag start event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void OnItemDragStart(CameraManager.CameraEvent evt)
	{
		if (this.gameMode == Common.GameMode.ATTACK || evt.baseItem == null)
		{
			return;
		}

		this._dragItem = evt.baseItem;
		this._dragItem.OnItemDragStart(evt);

		if (this._dragItem.itemData.name == "Wall")
		{
			this.UpdateWalls();
		}
	}

	/// <summary>
	/// Raises the item drag event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void OnItemDrag(CameraManager.CameraEvent evt)
	{
		if (this.gameMode == Common.GameMode.ATTACK)
		{
			return;
		}

		if (this._dragItem != null)
		{
			this._dragItem.OnItemDrag(evt);
			// this.ShowGrid();
		}
	}

	/// <summary>
	/// Raises the item drag stop event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void OnItemDragStop(CameraManager.CameraEvent evt)
	{
		if (this.gameMode == Common.GameMode.ATTACK)
		{
			return;
		}


		if (this._dragItem != null)
		{
			this._dragItem.OnItemDragStop(evt);

			if (this._dragItem.itemData.name == "Wall")
			{
				this.UpdateWalls();
			}

			this._dragItem = null;
		}
		// this.HideGrid();
	}

	public BaseItemScript selectedItem;
	/// <summary>
	/// Raises the item tap event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void OnItemTap(CameraManager.CameraEvent evt)
	{
		//		Debug.Log ("OnItemTap");
		if (this.gameMode == Common.GameMode.ATTACK)
		{
			return;
		}

		BaseItemScript tappedItem = evt.baseItem;
		if (tappedItem.Production.readyForCollection)
		{
			tappedItem.Production.Collect();
			return;
		}

		if (this.selectedItem != null)
		{
			// If the currently selected item is in PREVIEW state, cancel it entirely
			if (this.selectedItem.state == Common.State.PREVIEW)
			{
				BaseItemScript previewItem = this.selectedItem;
				this.selectedItem = null;
				UIManager.instance.HideItemOptions();
				this.RemoveItem(previewItem);
				Destroy(previewItem.gameObject);
			}
			else
			{
				this.selectedItem.SetSelected(false);
			}
		}
		this.selectedItem = tappedItem;
		tappedItem.SetSelected(true);
		// this.ShowGrid();
	}


	private BaseItemScript _unit;
	public int selectedUnit = 0;
	private int _swordManCount = 10;
	private int _archerCount = 10;

	private int _swordManExpended = 0;
	private int _archerExpended = 0;

	/// <summary>
	/// Raises the tap ground event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void OnTapGround(CameraManager.CameraEvent evt)
	{
		//		Debug.Log ("OnTapGround");
		// this.HideGrid();
		if (this.gameMode == Common.GameMode.NORMAL)
		{
			if (this.selectedItem != null)
			{
				// If the selected item is in PREVIEW state, cancel it entirely
				if (this.selectedItem.state == Common.State.PREVIEW)
				{
					BaseItemScript previewItem = this.selectedItem;
					this.selectedItem = null;
					UIManager.instance.HideItemOptions();
					this.RemoveItem(previewItem);
					Destroy(previewItem.gameObject);
					return;
				}

				BaseItemScript temp = this.selectedItem;
				this.selectedItem = null;
				temp.SetSelected(false);
			}

			//			if (this._unit == null) {
			//				this._unit = this.AddItem (5492, true);
			//				this._unit.SetState (Common.State.WALK);
			//				this._unit.SetPosition (evt.point);
			//			} else {
			//				this._unit.LookAt (evt.point);
			//			}
		}

		// if (this.gameMode == Common.GameMode.ATTACK)
		// {
		// 	if (selectedUnit == 0)
		// 	{
		// 		if (this._swordManExpended == _swordManCount)
		// 		{
		// 			return;
		// 		}
		// 		this._swordManExpended++;

		// 		AttackOverlayWindowScript.instance.SwordManCounter.text = (this._swordManCount - _swordManExpended).ToString() + "x";

		// 	}
		// 	else if (selectedUnit == 1)
		// 	{
		// 		if (this._archerExpended == _archerCount)
		// 		{
		// 			return;
		// 		}
		// 		this._archerExpended++;

		// 		AttackOverlayWindowScript.instance.ArcherCounter.text = (this._archerCount - _archerExpended).ToString() + "x";
		// 	}

		// 	int[] unitIds = new int[] { _swordMan_ID, _archer_ID };

		// 	evt.point.x = Mathf.Clamp(evt.point.x, 0, GroundManager.nodeWidth - 1);
		// 	evt.point.z = Mathf.Clamp(evt.point.z, 0, GroundManager.nodeHeight - 1);

		// 	this._unit = this.AddItem(unitIds[selectedUnit], true, true);
		// 	this._unit.SetPosition(evt.point);
		// 	this._unit.Attacker.AttackNearestTarget();
		// 	this._unit.OnItemDestroy += this.OnUnitDied;


		// }

		//		if (testCharacter != null) {
		//			testCharacter.LookAt (_builderHutInstance);
		//		}

		//		if (this._builderInstance != null) {
		//			this._builderInstance.gameObject.SetActive (true);
		//			this._builderInstance.Walker.WalkToPosition (evt.point);
		//		}

		//		if (this._swordManInstance != null) {
		//			this._swordManInstance.gameObject.SetActive (true);
		//			this._swordManInstance.Walker.WalkToPosition (evt.point);
		//		}

	}

	public Dictionary<int, BaseItemScript> GetItemInstances()
	{
		return this._itemInstances;
	}

	/// <summary>
	/// Shows the grid.
	/// </summary>

	private IEnumerator _ShowGridEnumerator;
	public void ShowGrid()
	{
		// if (this._ShowGridEnumerator != null)
		// {
		// 	this.StopCoroutine(_ShowGridEnumerator);
		// 	this._ShowGridEnumerator = null;
		// }
		// this._ShowGridEnumerator = this._ShowGrid();
		// this.StartCoroutine(this._ShowGridEnumerator);
		this.Grid.SetActive(true);
	}

	public void HideGrid()
	{
		this.Grid.SetActive(false);
	}

	// private IEnumerator _ShowGrid()
	// {
	// 	this.Grid.SetActive(true);
	// 	yield return new WaitForSeconds(1);
	// 	this.Grid.SetActive(false);
	// 	this._ShowGridEnumerator = null;
	// }


	//private int _builderHut_ID = 3635;
	//private int _townCenter_ID = 2496;
	//private int _builder_ID = 3823;

	private int _swordMan_ID = 6704;
	private int _archer_ID = 5492;

	//private int _barrackID = 8833;
	//private int _elixirCollectorID = 4856;
	//private int _elixirStorageID = 2090;
	//private int _goldMineID = 3265;
	//private int _goldStorageID = 9074;
	//private int _towerID = 4764;
	//private int _townCenterID = 2496;
	//private int _windMillID = 6677;
	//private int _armyCampID = 2728;

	//private BaseItemScript _townCenterInstance;
	//private BaseItemScript _builderHutInstance;
	//private BaseItemScript _builderInstance;
	//private BaseItemScript _towerInstance;
	//private BaseItemScript _armyCampForSwordManInstance;
	//private BaseItemScript _armyCampForArcherInstance;

	//public BaseItemScript testCharacter;
	//private BaseItemScript[] _swordManInstances;
	//private BaseItemScript[] _archerInstances;

	public void LoadUserScene()
	{
		this.ClearScene();
		SceneData sceneData = DataBaseManager.instance.GetScene();

		if (sceneData?.items != null)
		{
			foreach (ItemData itemData in sceneData.items)
			{
				double lastTime = 0;
				double.TryParse(itemData.lastCollectedTime, out lastTime);
				ShopLayoutItem layoutItem = this._shopLayout.items.Find(i => i.itemId == itemData.itemId);
				this.AddItem(itemData.itemId, itemData.instanceId, itemData.posX, itemData.posZ, true, true, itemData.level, lastTime, false, layoutItem?.extraFootprint);
			}
		}

		// Update semester progress after all buildings are loaded
				this.UpdateLevelProgress();

		if (this._shopLayout != null && this._shopLayout.items != null && this.MapShopAreaPrefab != null)
		{
			foreach (var layoutItem in this._shopLayout.items)
			{
				if (!this.IsItemBuiltInScene(layoutItem.itemId))
				{
					ItemsCollection.ItemData data = Items.GetItem(layoutItem.itemId);
					Vector3 pos = new Vector3(layoutItem.posX + (data != null ? data.gridWidth / 2f : 0), 0, layoutItem.posZ + (data != null ? data.gridHeight / 2f : 0));
					
					GameObject shopAreaObj = Utilities.CreateInstance(this.MapShopAreaPrefab, this.ItemsContainer, true);
					shopAreaObj.transform.localPosition = pos;
					
					MapShopAreaScript shopAreaScript = shopAreaObj.GetComponent<MapShopAreaScript>();
					if (shopAreaScript != null)
					{
						shopAreaScript.ClearItems();
						shopAreaScript.AddItem(layoutItem.itemId);
						
						if (data != null)
						{
							shopAreaScript.areaName = "Buy " + data.name;
						}
						
						_activeShopAreas[layoutItem.itemId] = shopAreaScript;

						// Hide immediately if starting fresh tutorial
						if (this.GetBuildingCount() == 0)
						{
							shopAreaObj.SetActive(false);
						}
					}
				}
			}
		}

		if (this.GetBuildingCount() == 0)
		{
			this.SetMapShopAreasVisible(false);
		}

		//LOAD UNITS ON CAMP 
		// BaseItemScript[] armyCamps = GetArmyCamps();
		// if (armyCamps.Length > 0)
		// {
		// 	for (int index = 0; index < _swordManCount; index++)
		// 	{
		// 		var camp = armyCamps[Random.Range(0, armyCamps.Length)];
		// 		BaseItemScript unit = this.AddItem(_swordMan_ID, -1, camp.GetPositionX(), camp.GetPositionZ(), true, true);
		// 		unit.WalkRandom(camp);
		// 	}

		// 	for (int index = 0; index < _archerCount; index++)
		//     {
		//         var camp = armyCamps[Random.Range(0, armyCamps.Length)];
		// 		BaseItemScript unit = this.AddItem(_archer_ID, -1, camp.GetPositionX(), camp.GetPositionZ(), true, true);
		// 		unit.WalkRandom(camp);
		//     }
		// }

		//for (int index = 0; index < 25; index++)
		//{
		//	//tree
		//	BaseItemScript tree = this.AddItem(5341, true, true);
		//	tree.SetPosition(GroundManager.instance.GetRandomFreePosition());
		//}

		GroundManager.instance.UpdateAllNodes();
		this.UpdateWalls();
		this.UpdateStudentStorageCapacity();

		if (NPCSpawner.instance != null && this.totalSpawnedNPCs > 0)
		{
			NPCSpawner.instance.SpawnNPCs(this.totalSpawnedNPCs);
		}

		UIManager.instance.ShowGameOverlayWindow();
	}

	public BaseItemScript[] GetArmyCamps()
	{
		List<BaseItemScript> armyCamps = new List<BaseItemScript>();
		foreach (KeyValuePair<int, BaseItemScript> entry in _itemInstances)
		{
			if (entry.Value.itemData.name == "ArmyCamp")
				armyCamps.Add(entry.Value);
		}

		return armyCamps.ToArray();
	}


	// public void LoadEnemyScene()
	// {
	// 	UIManager.instance.ShowAttackOverlayWindow();
	// 	this.ClearScene();
	// 	SceneData sceneData = DataBaseManager.instance.GetEnemyScene();

	// 	foreach (ItemData itemData in sceneData.items)
	// 	{
	// 		BaseItemScript baseItem = this.AddItem(itemData.itemId, itemData.instanceId, itemData.posX, itemData.posZ, true, false);
	// 		baseItem.OnItemDestroy += this.OnEnemyItemDestroy;
	//          SceneManager.instance.UpdateLevelProgress();
	// 		this.UpdateWalls();
	// 	}


	public void ClearScene()
	{
		foreach (KeyValuePair<int, BaseItemScript> entry in _itemInstances)
		{
			Destroy(entry.Value.gameObject);
		}
		this._itemInstances = new Dictionary<int, BaseItemScript>();

		if (this._activeShopAreas != null)
		{
			foreach (var entry in _activeShopAreas)
			{
				if (entry.Value != null)
				{
					Destroy(entry.Value.gameObject);
				}
			}
			this._activeShopAreas.Clear();
		}
		else
		{
			this._activeShopAreas = new Dictionary<int, MapShopAreaScript>();
		}
	}


	public void EnterNormalMode()
	{
		UIManager.instance.CloseAllWindows();
		UIManager.instance.ShowSceneEnteringWindow(() =>
		{
			this.gameMode = Common.GameMode.NORMAL;
			LoadUserScene();
		});
	}

	public void EnterAttackMode()
	{

		UIManager.instance.CloseAllWindows();
		UIManager.instance.ShowSceneEnteringWindow(() =>
		{
			this.gameMode = Common.GameMode.ATTACK;
			// this.LoadEnemyScene();

			AttackOverlayWindowScript.instance.SwordManCounter.text = this._swordManCount.ToString() + "x";
			AttackOverlayWindowScript.instance.ArcherCounter.text = this._archerCount.ToString() + "x";
		});
	}

	public List<BaseItemScript> GetAllItems()
	{
		List<BaseItemScript> items = new List<BaseItemScript>();
		foreach (KeyValuePair<int, BaseItemScript> entry in this._itemInstances)
		{
			items.Add(entry.Value);
		}

		return items;
	}

	public int GetBuildingCount()
	{
		int count = 0;
		foreach (var entry in this._itemInstances)
		{
			if (entry.Value != null && entry.Value.itemData != null && entry.Value.itemData.configuration != null && !entry.Value.itemData.configuration.isCharacter)
			{
				count++;
			}
		}
		return count;
	}

	public bool IsAnyBuildingUnderConstruction()
	{
		foreach (var entry in this._itemInstances)
		{
			if (entry.Value != null && entry.Value.Production != null && entry.Value.Production.isUnderConstruction)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsItemBuiltInScene(int itemId)
	{
		foreach (KeyValuePair<int, BaseItemScript> entry in _itemInstances)
		{
			if (entry.Value.itemData.id == itemId)
			{
				return true;
			}
		}
		return false;
	}

	public void SetMapShopAreasVisible(bool visible)
	{
		if (this._activeShopAreas == null) return;
		foreach (var entry in this._activeShopAreas)
		{
			if (entry.Value != null)
			{
				entry.Value.gameObject.SetActive(visible);
			}
		}
	}

	public void HideAllShopAreaArrows()
	{
		if (this._activeShopAreas == null) return;
		foreach (var entry in this._activeShopAreas)
		{
			if (entry.Value != null && entry.Value.Arrow != null)
			{
				entry.Value.Arrow.SetActive(false);
			}
		}
	}

	public bool IsItemConstructionFinished(int itemId)
	{
		foreach (KeyValuePair<int, BaseItemScript> entry in _itemInstances)
		{
			if (entry.Value.itemData.id == itemId
				&& entry.Value.state != Common.State.PREVIEW
				&& entry.Value.UI.progressUIInstance == null)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateWalls()
	{
		foreach (KeyValuePair<int, BaseItemScript> entry in _itemInstances)
		{
			BaseItemScript item = entry.Value;
			if (item.itemData.name == "Wall")
			{
				item.UpdateWall();
			}
		}
	}

	public BaseItemScript GetFreeBuilder()
	{
		BaseItemScript builder = null;
		List<BaseItemScript> builderHuts = new List<BaseItemScript>();
		foreach (KeyValuePair<int, BaseItemScript> entry in _itemInstances)
		{
			if (entry.Value.itemData.name == "BuilderHut")
			{
				builderHuts.Add(entry.Value);
			}
		}

		foreach (BaseItemScript hut in builderHuts)
		{
			if (hut.connectedItems[0].buildingItem == null)
			{
				builder = hut.connectedItems[0];
				break;
			}
		}

		return builder;
	}

	public void OnEnemyItemDestroy(BaseItemScript item)
	{
		bool isEverythingDestroyed = true;
		foreach (KeyValuePair<int, BaseItemScript> entry in this._itemInstances)
		{
			BaseItemScript baseItem = entry.Value;
			if (!baseItem.itemData.configuration.isCharacter && baseItem.itemData.name != "Wall")
			{
				//item is not character and not a wall
				//check the item is destroyed or not, if not then everything in the city is not destroyed
				if (!baseItem.isDestroyed)
					isEverythingDestroyed = false;
			}
		}

		if (isEverythingDestroyed)
		{
			//war ends
			AttackOverlayWindowScript.instance.Close();
			UIManager.instance.ShowResultWindow(true, _swordManExpended, _archerExpended);
		}
	}

	private int _diedUnitCount = 0;
	public void OnUnitDied(BaseItemScript unit)
	{
		_diedUnitCount++;
		if (_diedUnitCount == (_swordManCount + _archerCount))
		{
			//war ends
			AttackOverlayWindowScript.instance.Close();
			UIManager.instance.ShowResultWindow(false, _swordManExpended, _archerExpended);
		}
	}

	public bool isLevelProgressUpdatePending = false;

	public void UpdateLevelProgress()
	{
		if (UIManager.instance != null && UIManager.instance.HasEventWindowOpen())
		{
			isLevelProgressUpdatePending = true;
			return;
		}

		isLevelProgressUpdatePending = false;

		List<int> requiredItemIds = ShopWindowScript.GetAllShopItemIds();
		int totalRequired = 0;
		int builtCount = 0;

		foreach (int itemId in requiredItemIds)
		{
			ItemsCollection.ItemData itemData = Items.GetItem(itemId);
			// Kiểm tra yêu cầu theo currentLevel
			if (itemData != null && itemData.configuration.unlockItemAtSemester == this.currentLevel)
			{
				totalRequired++;
				if (this.IsItemConstructionFinished(itemId))
				{
					builtCount++;
				}
			}
		}

		if (totalRequired > 0)
		{
			this.levelProgress = (float)builtCount / totalRequired * 100f;
		}
		else
		{
			this.levelProgress = 0;
		}

		// LOGIC LÊN CẤP (LEVEL UP): Khi hoàn thành 100% nhiệm vụ
		if (this.levelProgress >= 99.9f && totalRequired > 0 && !_hasShownUnlockThisLevel)
		{
			StartCoroutine(HandleLevelUpCoroutine());
		}

		this.SaveResources();
		this.RefreshResourceUIs("level");

		if (GameOverlayWindowScript.instance != null)
		{
			GameOverlayWindowScript.instance.RefreshHint();
		}
	}

	private IEnumerator HandleLevelUpCoroutine()
	{
		_hasShownUnlockThisLevel = true; 
		
		// Đợi chính xác thời gian thanh Progress chạy hết (tweenDuration = 0.75s)
		yield return new WaitForSeconds(0.8f);

		this.currentLevel++; // Tăng level mới
		this.levelProgress = 0; // Reset thanh tiến trình

		// Logic sinh NPC khi lên level 2
		if (this.currentLevel == 2 && NPCSpawner.instance != null)
		{
			int amount = NPCSpawner.instance.npcSpawnedAtLevel2;
			this.totalSpawnedNPCs += amount;
			NPCSpawner.instance.SpawnNPCs(amount);
		}

		this.SaveResources();
		this.RefreshResourceUIs("level");

		if (UIManager.instance != null)
		{
			UIManager.instance.ShowUnlockItemsWindow();
		}

		// Kiểm tra nếu đạt level 2 lần đầu (Tutorial)
		if (this.currentLevel == 2 && GameOverlayWindowScript.instance != null)
		{
			GameOverlayWindowScript.instance.OnReachLevel2();
		}

		_hasShownUnlockThisLevel = false; // Reset cờ để có thể lên level tiếp theo trong cùng session
	}

	// Tham số cân bằng — có thể chuyển sang ScriptableObject để designer tinh chỉnh.
	private readonly BalanceParameters _balanceParams = new BalanceParameters();

	public void CompleteSemester()
	{
		StartCoroutine(CompleteSemesterCoroutine());
	}

	private IEnumerator CompleteSemesterCoroutine()
	{
		this._isCompletingSemester = true;
		yield return null; // Thêm yield để giữ tính chất của Coroutine

		// ── 1. Chạy công thức balance cho kỳ học vừa kết thúc ─────────────────
		BalanceState state = UniversityBalanceFormulas.StateFromSceneManager();
		SemesterBreakdown bd = UniversityBalanceFormulas.ApplySemesterTick(ref state, _balanceParams);

		// ── 3. Ghi kết quả ngược lại vào SceneManager + lưu + refresh UI ─────────
		UniversityBalanceFormulas.ApplyStateToSceneManager(state);

		Debug.Log($"[Semester {this.currentSemester} → {this.currentSemester + 1}] "
			+ $"Education={this.numberOfEducationInStorage} | Happiness={this.numberOfHappyInStorage} "
			+ $"| Freshmen={bd.freshmen:F0} | Dropouts={bd.dropouts:F0} | Graduated={bd.graduated:F0} "
			+ $"| ΔStudents={bd.deltaStudents:F0} | Gold+={bd.semesterGoldIncome:F0} "
			+ $"| GradRate={bd.graduationRate:P1}");

		int finishedSemester = this.currentSemester;

		// ── 4. Tăng kỳ học (Semester độc lập với Level) ─────────────────────────────
		this.currentSemester++;

		// Logic sinh NPC mỗi kỳ sau khi mở khóa level 2
		if (this.currentLevel >= 2 && NPCSpawner.instance != null)
		{
			int amount = NPCSpawner.instance.npcSpawnedPerSemester;
			this.totalSpawnedNPCs += amount;
			NPCSpawner.instance.SpawnNPCs(amount);
		}

		this.SaveResources();
		this.RefreshResourceUIs("semester");

		// Reset TimeManager for the new semester
		if (TimeManager.instance != null)
		{
			TimeManager.instance.ResetTimer();
		}

		if (UIManager.instance != null)
		{
			// Hiện bảng Income Result khi hết thời gian, confirm xong mới qua NewSemesterWindow
			UIManager.instance.ShowIncomeResultWindow(bd, finishedSemester, this.numberOfHappyInStorage, this.numberOfEducationInStorage);
		}

		this._isCompletingSemester = false;
	}

	//RESOURCE  COLLECTION
	public void CollectResource(string resourceType, int amount)
	{
		if (resourceType == "gold")
		{
			this.numberOfGoldInStorage = Mathf.Clamp(this.numberOfGoldInStorage + amount, 0, goldStorageCapacity);
		}
		else if (resourceType == "diamond")
		{
			this.numberOfDiamondsInStorage = Mathf.Clamp(this.numberOfDiamondsInStorage + amount, 0, diamondStorageCapacity);
		}
		else if (resourceType == "happy")
		{
			// Happiness là điểm [0,100] — tòa sản xuất "happy" sẽ tăng chỉ số này khi người chơi thu thập.
			this.numberOfHappyInStorage = Mathf.Clamp(this.numberOfHappyInStorage + amount, 0, happyStorageCapacity);
		}
		else if (resourceType == "education")
		{
			// Education là điểm [0,100] — tòa sản xuất "education" sẽ tăng chỉ số này khi người chơi thu thập.
			this.numberOfEducationInStorage = Mathf.Clamp(this.numberOfEducationInStorage + amount, 0, educationStorageCapacity);
		}

		this.SaveResources();
		this.RefreshResourceUIs(resourceType);
	}

	//RESOURCE COLLECTION
	public bool ConsumeResource(string resourceType, int count)
	{
		if (resourceType == "gold")
		{
			if (this.numberOfGoldInStorage >= count)
			{
				this.numberOfGoldInStorage -= count;
				this.SaveResources();
				this.RefreshResourceUIs(resourceType);
				return true;
			}
		}
		// else if (resourceType == "elixir")
		// {
		//     if (this.numberOfElixirInStorage >= count)
		//     {
		//         this.numberOfElixirInStorage -= count;
		// 		this.SaveResources();
		// 		this.RefreshResourceUIs(resourceType);
		//         return true;
		//     }
		// }
		else if (resourceType == "diamond")
		{
			if (this.numberOfDiamondsInStorage >= count)
			{
				this.numberOfDiamondsInStorage -= count;
				this.SaveResources();
				this.RefreshResourceUIs(resourceType);
				return true;
			}
		}

		return false;
	}

	public bool HasEnoughResource(string resourceType, int count)
	{
		if (resourceType == "gold")
		{
			return this.numberOfGoldInStorage >= count;
		}
		else if (resourceType == "diamond")
		{
			return this.numberOfDiamondsInStorage >= count;
		}

		return false;
	}

	public void RefreshResourceUIs(string resourceType)
	{
		if (GameOverlayWindowScript.instance != null)
		{
			if (resourceType == "gold")
				GameOverlayWindowScript.instance.CollectResource("gold", this.numberOfGoldInStorage);
			// else if (resourceType == "elixir") 
			//     GameOverlayWindowScript.instance.CollectResource("elixir", this.numberOfElixirInStorage);
			else if (resourceType == "diamond")
				GameOverlayWindowScript.instance.CollectResource("diamond", this.numberOfDiamondsInStorage);
			else if (resourceType == "student")
				GameOverlayWindowScript.instance.CollectResource("student", this.numberOfStudentInStorage);
			else if (resourceType == "happy")
				GameOverlayWindowScript.instance.CollectResource("happy", this.numberOfHappyInStorage);
			else if (resourceType == "education")
				GameOverlayWindowScript.instance.CollectResource("education", this.numberOfEducationInStorage);
			else if (resourceType == "semester")
				GameOverlayWindowScript.instance.RefreshSemesterUI();
			else if (resourceType == "level")
				GameOverlayWindowScript.instance.RefreshLevelUI();
		}


		if (TrainTroopsWindowScript.instance != null)
		{
			TrainTroopsWindowScript.instance.UpdateResourcePanel();
		}

		if (MissionWindowScript.instance != null)
		{
			MissionWindowScript.instance.UpdateResourcePanel();
		}
	}

	//PARTICLES
	public GameObject ShowParticle(GameObject prefab, Vector3 position)
	{
		GameObject inst = Utilities.CreateInstance(prefab, this.ParticlesContainer, true);
		inst.transform.position = position;
		return inst;
	}

	public BaseItemScript GetNearestArmyCamp(Vector3 from)
	{
		BaseItemScript[] armyCamps = this.GetArmyCamps();

		if (armyCamps.Length == 0)
			return null;

		if (armyCamps.Length == 1)
			return armyCamps[0];

		float smallDistance = 999999;
		BaseItemScript nearestArmyCamp = null;
		foreach (BaseItemScript armyCamp in armyCamps)
		{
			float dist = Vector3.Distance(armyCamp.GetPosition(), from);
			if (dist < smallDistance)
			{
				smallDistance = dist;
				nearestArmyCamp = armyCamp;
			}
		}

		return nearestArmyCamp;
	}

	public void UpdateStudentStorageCapacity()
	{
		int baseCapacity = 500; // Sức chứa cơ bản (không cần công trình nào).
		int totalIncrease = 0;
		foreach (var item in GetAllItems())
		{
			if (!item.itemData.configuration.isCharacter && item.UI.progressUIInstance == null)
			{
				totalIncrease += item.level * item.itemData.configuration.studentCapacityIncrease;
			}
		}
		this.studentStorageCapacity = baseCapacity + totalIncrease;
		this.SaveResources();

		if (GameOverlayWindowScript.instance != null)
		{
			GameOverlayWindowScript.instance.StudentInfo.maxValue = this.studentStorageCapacity;
			this.RefreshResourceUIs("student");
		}
	}
}
