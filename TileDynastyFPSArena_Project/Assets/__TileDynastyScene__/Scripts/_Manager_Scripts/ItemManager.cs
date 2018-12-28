using UnityEngine;
using System.Collections;

public class ItemManager : MonoBehaviour {

	NetworkManager nManager;
	GuiManager guiManager;
	private BotSpawnSpot [] levelUpSpawns;
	public GameObject lightBeam;
	private SpawnSpot[] garbageSpawns;
	private int randomNumber;
	private string _itemName = string.Empty;
	public bool isRandomSpawnSpotAvailable = false;
	public AudioClip[] itemSounds;
	public float respawnTimer = 15f;

	void Awake () {

		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		guiManager = nManager.GetComponent<GuiManager> ();

		levelUpSpawns = GameObject.FindObjectsOfType<BotSpawnSpot> ();
		garbageSpawns = GameObject.FindObjectsOfType<SpawnSpot> ();
	}

	[PunRPC]
	void ItemRespawn () {
		if (gameObject.name.Equals (_itemName)) {
			if (isRandomSpawnSpotAvailable) {
				if (!name.StartsWith ("Garbage_Item")) {
					randomNumber = Random.Range (0, levelUpSpawns.Length);
					BotSpawnSpot currentLevelUpSpawn = levelUpSpawns [randomNumber];
					gameObject.transform.position = currentLevelUpSpawn.transform.position;
					if (lightBeam) {
						lightBeam.transform.localPosition = new Vector3 (0, -0.15f, 0);
					}
				} else {
					randomNumber = Random.Range (0, garbageSpawns.Length);
					SpawnSpot currentGarbageSpawn = garbageSpawns [randomNumber];
					gameObject.transform.position = currentGarbageSpawn.transform.position;
				}
			}
			GetComponent<MeshCollider> ().enabled = true;
			GetComponent<MeshRenderer> ().enabled = true;
			if (name.StartsWith ("LevelUp")) {
				if (lightBeam) {
					lightBeam.GetComponent<MeshRenderer> ().enabled = true;
				}
			}
			GetComponent<AudioSource> ().PlayOneShot (itemSounds [1]);
		}
	}

	[PunRPC]
	public void PickupItem(string itemName1){
		DeactivateItem(itemName1);
	}

	void DeactivateItem(string itemName){
		if (name.Equals (itemName)) {
			_itemName = itemName;
			GetComponent<MeshCollider> ().enabled = false;
			GetComponent<MeshRenderer> ().enabled = false;
			if (name.StartsWith ("LevelUp")) {
				if (lightBeam) {
					lightBeam.GetComponent<MeshRenderer> ().enabled = false;
				}
			}
			GetComponent<AudioSource> ().PlayOneShot (itemSounds [0]);
			StartCoroutine (DelayedItemRespawn ());

//			if (itemName.StartsWith ("Ammo_HMG")) {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED UP H.M.G. AMMO");
//			} else if (itemName.StartsWith ("Ammo_Rocket")) {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED UP ROCKET AMMO");
//			} else if (itemName.StartsWith ("Ammo_Rail")) {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED UP RIFLE AMMO");
//			} else if (itemName.StartsWith ("Ammo_Grenade")) {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED UP GRENADE AMMO");
//			} else if (itemName.StartsWith ("Ammo_MG")) {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED UP PORTER AMMO");
//			} else if (itemName.StartsWith ("Health")) {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED HEALTH");
//			} else if (itemName.StartsWith ("Armor")) {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED ARMOR");
//			} else {
//				guiManager.ItemPickupMessageCoroutine = guiManager.ItemPickupMessage_Coroutine ("PICKED UP ITEM");
//			}
//			StartCoroutine (guiManager.ItemPickupMessageCoroutine);			
		}
	}

	IEnumerator DelayedItemRespawn(){
		yield return new WaitForSeconds (respawnTimer);
		if (PhotonNetwork.isMasterClient)
			this.GetComponent<PhotonView> ().RPC ("ItemRespawn", PhotonTargets.AllBuffered);
		yield return null;
	}
}