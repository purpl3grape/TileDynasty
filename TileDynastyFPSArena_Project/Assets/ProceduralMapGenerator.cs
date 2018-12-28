using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using UnityEditor;

[ExecuteInEditMode]
public class ProceduralMapGenerator : MonoBehaviour {

	NetworkManager nManager;
	MatchTimer mTimer;
	PhotonView pv;


	public NavMeshSurface[] surfaces;

	public int Seed = 0;
	[SerializeField] public int[] CellSeed;

	public GameObject InitTilePrefab;
	private GameObject initTile;
	public GameObject[] MapTilePrefab;
	public GameObject[] AuxiliaryTilePrefab;

	[Range(1,99)] public int[] MinTileLimit;
	[Range(1,99)] public int[] MaxTileLimit;

	public int[] TileLimitIndex;
	[Range(0,100)] public int[] DistributionIndex;

	public GameObject WallPrefab;

	[Range(1,999)] public int CellSize = 40;
	[Range(3,7)] public int GridSize = 3;
	[Range(5,999)] public int CenterTileIndex = 5;

	[HideInInspector] public GameObject[] InstantiatedMapObjects;
	[HideInInspector] public GameObject[] InstantiatedWallObjectsSouth;
	[HideInInspector] public GameObject[] InstantiatedWallObjectsNorth;
	[HideInInspector] public GameObject[] InstantiatedWallObjectsEast;
	[HideInInspector] public GameObject[] InstantiatedWallObjectsWest;

	public GameObject MapContainer;
	public GameObject WallContainer;

	public bool isRandomRotation = false;
	public bool isBuildMap = false;
	public bool isProcessComplete = false;

	private Vector3 MapPos = new Vector3 (1000, 0, 0);
	private Vector3 WallPos = new Vector3 (1000, 0, 0);


	private int GridCount = 0;
	private int GridRowCount = 0;
	private int TileIndex = 0;
	private int WallCount = 0;
	private int TileTargetCount = 0;

	public Vector3 initialWallPositionEast = new Vector3 (-71.5f, -0.5f, 60);
	public Vector3 initialWallPositionWest = new Vector3 (311.5f, -0.5f, -60);
	public Vector3 initialWallPositionNorth = new Vector3 (-60, -0.5f, 299.5f);
	public Vector3 initialWallPositionSouth = new Vector3 (60, -0.5f, -59.5f);

	private Vector3 tempPosition = Vector3.zero;
	public Vector3[] targetRotation;
	private int currentXPos = 0;
	private int currentZPos = 0;


	bool hasReceivedSeed = false;
	bool isInit = false;

	void Start () {
		Init ();
	}

	void Init () {
		if (Application.isPlaying) {
			if (!SteamManager.Initialized)
				return;
		}

		Debug.Log ("init1");
        Debug.Log("Surface number: " + surfaces.Length);
        if (TileLimitIndex.Length != MapTilePrefab.Length)
			TileLimitIndex = new int[MapTilePrefab.Length];
		if (MinTileLimit.Length != MapTilePrefab.Length)
			MinTileLimit = new int[MapTilePrefab.Length];
		if (MaxTileLimit.Length != MapTilePrefab.Length)
			MaxTileLimit = new int[MapTilePrefab.Length];
		if (DistributionIndex.Length != MapTilePrefab.Length)
			DistributionIndex = new int[MapTilePrefab.Length];		

		TileTargetCount = GridSize * GridSize;
		targetRotation = new Vector3[TileTargetCount];

		if(Application.isPlaying){
			DestroyMap ();
			DestroyWall ();		
		}
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		mTimer = nManager.GetComponent<MatchTimer> ();
		pv = GetComponent<PhotonView> ();

		#if UNITY_STANDALONE
		if (Application.isPlaying && PhotonNetwork.isMasterClient) {
			pv.RPC ("SyncRandomSeed", PhotonTargets.AllBuffered, Random.Range (0, 999));
		}
		#endif
		#if UNITY_EDITOR
		SyncRandomSeed(Random.Range(0,999));
		#endif

		nManager.SceneLoadingPanel.SetActive (true);
		isBuildMap = true;
		isProcessComplete = false;
	}


	bool resetGOs = false;
	void Update () {

		if (nManager == null)
			return;
		
		if (MapContainer == null)
			return;
		if (MapTilePrefab == null)
			return;
		if (WallPrefab == null)
			return;
		
		if (GridSize == 0)
			return;
		if (CellSize == 0)
			return;
		if (!hasReceivedSeed)
			return;
		if (Application.isPlaying) {
			if (!SteamManager.Initialized)
				return;
			if (mTimer.isReady)
				return;
		}
		else if (!Application.isPlaying) {
			if (!resetGOs) {
				resetGOs = true;
				DestroyWall ();
				DestroyMap ();
			}
		}

		if (Mathf.Repeat (GridSize, 2) == 0) {
			if(GridSize>3)
				GridSize -= 1;
		}
		CenterTileIndex = (GridSize + 1) / 2 - 1;

		if (MapContainer.transform.position != MapPos) {
			MapContainer.transform.position = MapPos;
		}
		if (WallContainer.transform.position != WallPos) {
			WallContainer.transform.position = WallPos;
		}

		if (TileLimitIndex.Length != MapTilePrefab.Length)
			TileLimitIndex = new int[MapTilePrefab.Length];
		if (MinTileLimit.Length != MapTilePrefab.Length)
			MinTileLimit = new int[MapTilePrefab.Length];
		if (MaxTileLimit.Length != MapTilePrefab.Length)
			MaxTileLimit = new int[MapTilePrefab.Length];
		if (DistributionIndex.Length != MapTilePrefab.Length)
			DistributionIndex = new int[MapTilePrefab.Length];
		
		TileTargetCount = GridSize * GridSize;
//		targetRotation = new Vector3[TileTargetCount];
		

		if (isBuildMap) {
			if (!isProcessComplete) {

				DestroyMap ();
				DestroyWall ();

				if (GenerateMapCO == null) {
					GenerateMapCO = GenerateMap_CO ();
					StartCoroutine (GenerateMapCO);

				}
				if (GenerateWallsCO == null) {
					GenerateWallsCO = GenerateWalls_CO ();
					StartCoroutine (GenerateWallsCO);
				}
				isProcessComplete = true;
			}
		} else {
			if (!isProcessComplete) {
				DestroyMap ();
				DestroyWall ();
				isProcessComplete = true;
			}
		}


	}

	public void DestroyWall(){
		while (WallContainer.transform.childCount > 0) {
			GameObject.DestroyImmediate (WallContainer.transform.GetChild (0).gameObject);
		}
		WallCount = 0;
		InstantiatedWallObjectsSouth = new GameObject[0];
		InstantiatedWallObjectsNorth = new GameObject[0];
		InstantiatedWallObjectsEast = new GameObject[0];
		InstantiatedWallObjectsWest = new GameObject[0];
		GenerateWallsCO = null;
	}

	public void DestroyMap(){

		Debug.Log ("Deleting Cell count: " + TileIndex);

		while (MapContainer.transform.childCount > 0) {
			GameObject.DestroyImmediate (MapContainer.transform.GetChild (0).gameObject);
		}

		for (int i = 0; i < surfaces.Length; i++) {
			surfaces [i].RemoveData ();
		}   

		TileIndex = 0;
		InstantiatedMapObjects = new GameObject[0];
		GenerateMapCO = null;
	}		

	[PunRPC] public void GenerateWallsRPC(){
		Vector3 tempPositionSouth = initialWallPositionSouth;
		Vector3 tempPositionNorth = initialWallPositionNorth + new Vector3 (0, 0, CellSize * (GridSize - 3));
		Vector3 tempPositionWest = initialWallPositionWest + new Vector3 (CellSize * (GridSize - 3), 0, 0);
		Vector3 tempPositionEast = initialWallPositionEast;
		InstantiatedWallObjectsSouth = new GameObject[GridSize];
		InstantiatedWallObjectsNorth = new GameObject[GridSize];
		InstantiatedWallObjectsEast = new GameObject[GridSize];
		InstantiatedWallObjectsWest = new GameObject[GridSize];

		while (WallCount < GridSize) {

			if (Application.isPlaying && SteamManager.Initialized) {
				InstantiatedWallObjectsSouth [WallCount] = GameObject.Instantiate (WallPrefab);
			} else {
				#if UNITY_EDITOR
				InstantiatedWallObjectsSouth [WallCount] = GameObject.Instantiate (WallPrefab);
				#endif
			}
			if (InstantiatedWallObjectsSouth [WallCount].transform != null) {
				InstantiatedWallObjectsSouth [WallCount].transform.position = tempPositionSouth;
				InstantiatedWallObjectsSouth [WallCount].transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				InstantiatedWallObjectsSouth [WallCount].transform.SetParent (WallContainer.transform);
			}

			if (Application.isPlaying && SteamManager.Initialized) {
				InstantiatedWallObjectsEast [WallCount] = GameObject.Instantiate (WallPrefab);
			} else {
				#if UNITY_EDITOR
				InstantiatedWallObjectsEast [WallCount] = GameObject.Instantiate (WallPrefab);
				#endif
			}
			if (InstantiatedWallObjectsSouth [WallCount].transform != null) {
				InstantiatedWallObjectsEast [WallCount].transform.position = tempPositionWest;
				InstantiatedWallObjectsEast [WallCount].transform.rotation = Quaternion.Euler (new Vector3 (0, 90, 0));
				InstantiatedWallObjectsEast [WallCount].transform.SetParent (WallContainer.transform);
			}

			if (Application.isPlaying && SteamManager.Initialized) {
				InstantiatedWallObjectsNorth [WallCount] = GameObject.Instantiate (WallPrefab);
			} else {
				#if UNITY_EDITOR
				InstantiatedWallObjectsNorth [WallCount] = GameObject.Instantiate (WallPrefab);
				#endif
			}
			if (InstantiatedWallObjectsSouth [WallCount].transform != null) {
				InstantiatedWallObjectsNorth [WallCount].transform.position = tempPositionNorth;
				InstantiatedWallObjectsNorth [WallCount].transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				InstantiatedWallObjectsNorth [WallCount].transform.SetParent (WallContainer.transform);
			}

			if (Application.isPlaying && SteamManager.Initialized) {
				InstantiatedWallObjectsWest [WallCount] = GameObject.Instantiate (WallPrefab);
			} else {
				#if UNITY_EDITOR
				InstantiatedWallObjectsWest [WallCount] = GameObject.Instantiate (WallPrefab);
				#endif
			}
			if (InstantiatedWallObjectsSouth [WallCount].transform != null) {
				InstantiatedWallObjectsWest [WallCount].transform.position = tempPositionEast;
				InstantiatedWallObjectsWest [WallCount].transform.rotation = Quaternion.Euler (new Vector3 (0, 270, 0));
				InstantiatedWallObjectsWest [WallCount].transform.SetParent (WallContainer.transform);
			}

			tempPositionSouth += new Vector3 (CellSize, 0, 0);
			tempPositionNorth += new Vector3 (CellSize, 0, 0);
			tempPositionWest += new Vector3 (0, 0, CellSize);
			tempPositionEast += new Vector3 (0, 0, CellSize);

			WallCount++;
			//			yield return null;
		}	
	}

	public IEnumerator GenerateWallsCO;
	public IEnumerator GenerateWalls_CO(){

		GenerateWallsRPC ();

		GenerateWallsCO = null;
		yield return null;
	}


	[PunRPC] public void SyncMapSeed(int cellIndx, int val){
		Seed = val;
		CellSeed [cellIndx] = val;
	}

	[PunRPC] public void SyncRandomSeed(int seed){
		Random.InitState (seed);

		for (int i = 0; i < targetRotation.Length; i++) {
			targetRotation [i] = new Vector3 (0, Random.Range (0, 4) * 90, 0);
			Debug.Log ("rot " + targetRotation [i]);
		}
		hasReceivedSeed = true;
	}

	int endTileCount = 0;
	int randStartTile = 0;

	[PunRPC] public void GenerateMapRPC(){

		randStartTile = Random.Range (0, TileTargetCount);
		endTileCount = randStartTile == 0 ? TileTargetCount : randStartTile - 1;
		GridCount = Random.Range (0, GridSize - 1);
		GridRowCount = Random.Range (0, GridSize - 1);
		if (Mathf.Repeat (GridSize, 2) == 0) {
			if(GridSize>3)
				GridSize -= 1;
		}
		CenterTileIndex = (GridSize + 1) / 2 - 1;

		for (int i = 0; i < TileTargetCount; i++) {

			if (Mathf.Repeat (i, GridSize) == 0) {
				currentXPos = 0;
				GridCount++;
			}
			GridRowCount++;
			if (GridCount >= GridSize) {
				Debug.Log ("reset gridcount " + GridCount + ", girdsize: " + GridSize);
				GridCount = 0;
				currentZPos = 0;
			}
			if (GridRowCount >= GridSize) {
				Debug.Log ("reset gridcount " + GridCount + ", girdsize: " + GridSize);
				GridRowCount = 0;
				currentXPos = 0;
			}

			currentXPos = CellSize * GridRowCount;
			currentZPos = CellSize * GridCount;

			if (GridRowCount == CenterTileIndex && GridCount == CenterTileIndex) {
				tempPosition = new Vector3 (currentXPos, 0, currentZPos);
				initTile = GameObject.Instantiate (InitTilePrefab);
				initTile.transform.position = tempPosition;
				initTile.transform.SetParent (MapContainer.transform);
				continue;
			}
			
			Debug.Log ("randstartile: " + randStartTile + ", xPos: " + (currentXPos - 1000) + ", zPos: " + currentZPos);


			if (TileLimitIndex [TileIndex] < MaxTileLimit [TileIndex]) {			
				if (Application.isPlaying && SteamManager.Initialized) {
					InstantiatedMapObjects [TileIndex] = GameObject.Instantiate (MapTilePrefab [TileIndex]);
				} else {
					#if UNITY_EDITOR
					InstantiatedMapObjects [TileIndex] = GameObject.Instantiate (MapTilePrefab [TileIndex]);
					#endif
				}
				TileLimitIndex [TileIndex]++;

			} else {
				if (Application.isPlaying && SteamManager.Initialized) {
					InstantiatedMapObjects [TileIndex] = GameObject.Instantiate (AuxiliaryTilePrefab [0]);
				} else {
					#if UNITY_EDITOR
					InstantiatedMapObjects [TileIndex] = GameObject.Instantiate (AuxiliaryTilePrefab [0]);
					#endif
				}
			}

			tempPosition = new Vector3 (currentXPos, 0, currentZPos);
			InstantiatedMapObjects [TileIndex].name = "Cell_" + TileIndex + "_" + InstantiatedMapObjects [TileIndex].name;
			InstantiatedMapObjects [TileIndex].transform.position = tempPosition;
			if (isRandomRotation) {
//				targetRotation = new Vector3 (0, Random.Range (0, 4) * 90, 0);
				InstantiatedMapObjects [TileIndex].transform.rotation = Quaternion.Euler (targetRotation[i]);
			} else {
//				targetRotation = new Vector3 (0, 0, 0);
				InstantiatedMapObjects [TileIndex].transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			}
			InstantiatedMapObjects [TileIndex].transform.SetParent (MapContainer.transform);

			currentXPos += CellSize;

			randStartTile += 1;
			if (randStartTile > TileTargetCount) {
				randStartTile = 0;
			}
					
			if (TileIndex < (MapTilePrefab.Length - 1)) {
				TileIndex++;
			} else {
				TileIndex = 0;
			}

		}
	}

	public IEnumerator GenerateMapCO;
	public IEnumerator GenerateMap_CO(){

		for (int i = 0; i < MinTileLimit.Length; i++) {
			TileLimitIndex [i] = 0;
			MinTileLimit [i] = MinTileLimit [i] > MaxTileLimit [i] ? MaxTileLimit [i] : MinTileLimit [i];
		}

		TileTargetCount = GridSize * GridSize;
		TileIndex = 0;
		InstantiatedMapObjects = new GameObject[TileTargetCount];
		currentXPos = 0;
		currentZPos = 0;

		CellSeed = new int[TileTargetCount];

		GenerateMapRPC ();

		//Both online and offline gets this called
		if (Application.isPlaying) {
			nManager.Init ();
		}
        
		for (int i = 0; i < surfaces.Length; i++) {
			surfaces [i].buildHeightMesh = true;
			//surfaces [i].BuildNavMesh ();
            Debug.Log("Build: " + i);
		}
        surfaces[0].BuildNavMesh();
        nManager.SceneLoadingPanel.SetActive (false);

		GenerateMapCO = null;
		yield return null;
	}		

}