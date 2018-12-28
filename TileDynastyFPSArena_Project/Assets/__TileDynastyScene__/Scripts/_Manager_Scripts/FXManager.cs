using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FXManager : MonoBehaviour {

	public GameObject MGLightFXPrefab;
	public GameObject MGHeavyFXPrefab;
	public GameObject RocketFXPrefab;
	public GameObject EnemyRocketFXPrefab2;
	public GameObject GrenadeFXPrefab;
	public GameObject TeleportFXPrefab;
	public GameObject RailFXPrefab;
	public GameObject explosionFXPrefab;
	public GameObject empExplosionFXPrefab;
	public GameObject teleportExplosionFXPrefab;
	public GameObject hurtingSoundFXPrefab;
	public GameObject hitConfirmSoundFXPrefab;
	public GameObject bloodFXPrefab;
	public GameObject missShotFXPrefab;
	public GameObject axeMissFXPrefab;
	public GameObject axeHitFXPrefab;
	public GameObject bulletHitSoundPrefab;
	public GameObject projectileHitSoundPrefab;
	public GameObject MGSoundPrefab;
	public GameObject HMGSoundPrefab;
	public AudioClip[] dyingSounds;

	public bool isProjectileApplyDamage = false;
	public bool isProjectileDirectHit = false;

	//SOUND LIST
	[HideInInspector] public List<GameObject> mgList;
	[HideInInspector] public List<GameObject> mgSoundList;
	[HideInInspector] public List<GameObject> hmgList;
	[HideInInspector] public List<GameObject> hmgSoundList;
	[HideInInspector] public List<GameObject> bulletHitSoundList;
	[HideInInspector] public List<GameObject> axeHitSoundList;
	[HideInInspector] public List<GameObject> projectileHitSoundList;
	//HIT EFFECT LIST
	[HideInInspector] public List<GameObject> bloodFXList;
	[HideInInspector] public List<GameObject> missShotList;
	[HideInInspector] public List<GameObject> axeMissSoundList;
	[HideInInspector] public List<GameObject> hitConfirmList;
	[HideInInspector] public List<GameObject> hurtList;
	//BULLET AND EXPLOSION LIST
	[HideInInspector] public List<GameObject> rocketList;
	[HideInInspector] public List<GameObject> grenadeList;
	[HideInInspector] public List<GameObject> teleportGrenadeList;
	[HideInInspector] public List<GameObject> enemyRocketList;
	[HideInInspector] public List<GameObject> railList;
	[HideInInspector] public List<GameObject> waveList;
	[HideInInspector] public List<GameObject> explosionList;
	[HideInInspector] public List<GameObject> empExplosionList;
	[HideInInspector] public List<GameObject> teleportExplosionList;

	PhotonView scenePV;

	public void PoolPrefab(int size, GameObject prefab, out List<GameObject> list){
		list = new List<GameObject> ();
		for (int i = 0; i < size; i++) {
			GameObject obj = (GameObject)Instantiate (prefab, Vector3.zero, Quaternion.identity);
			obj.SetActive (false);
			list.Add (obj);
		}
	}

	IEnumerator MeshRendererDelayEnable(GameObject obj, float time){
		obj.GetComponent<MeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (time);
		obj.GetComponent<MeshRenderer> ().enabled = true;
	}

	List<GameObject> tempList;
	public List<GameObject> GetFXPrefab(List<GameObject> list, out GameObject obj){
		tempList = list;
		obj = null;
		if (tempList.Count > 0) {
			obj = tempList [0];
			tempList.RemoveAt (0);
			obj.SetActive (true);
			return tempList;
		}
		return tempList;
	}

	public List<GameObject> DestroyFXPrefab(GameObject obj, List<GameObject> list){
		list.Add (obj);
		obj.SetActive (false);
		return list;
	}

	[HideInInspector] public PhotonView localClient;
	public void SetLocalClient(PhotonView go){
		localClient = go;
	}

	void Start (){

		scenePV = GetComponent<PhotonView> ();

		hmgList = null;
		hmgSoundList = null;
		rocketList = null;
		grenadeList = null;
		enemyRocketList = null;
		explosionList = null;
		empExplosionList = null;

		PoolPrefab (150, MGHeavyFXPrefab, out hmgList);
		PoolPrefab (150, HMGSoundPrefab, out hmgSoundList);
		PoolPrefab (300, bulletHitSoundPrefab, out bulletHitSoundList);
		PoolPrefab (300, projectileHitSoundPrefab, out projectileHitSoundList);

		PoolPrefab (300, axeMissFXPrefab, out axeMissSoundList);
		PoolPrefab (300, axeHitFXPrefab, out axeHitSoundList);

		PoolPrefab (100, bloodFXPrefab, out bloodFXList);
		PoolPrefab (300, missShotFXPrefab, out missShotList);
		PoolPrefab (300, hitConfirmSoundFXPrefab, out hitConfirmList);
		PoolPrefab (300, hurtingSoundFXPrefab, out hurtList);
		PoolPrefab (100, RailFXPrefab, out railList);

		PoolPrefab (150, RocketFXPrefab, out rocketList);
		PoolPrefab (150, GrenadeFXPrefab, out grenadeList);
		PoolPrefab (100, TeleportFXPrefab, out teleportGrenadeList);
		PoolPrefab (150, EnemyRocketFXPrefab2, out enemyRocketList);
		PoolPrefab (300, explosionFXPrefab, out explosionList);	//Handles Enemy Rocket and regular rocket explosion so about 30 + 50 = 80 at least
		PoolPrefab (150, empExplosionFXPrefab, out empExplosionList); //Must be at least the same number of how many Grenade projectiles there are (60)
		PoolPrefab (100, teleportExplosionFXPrefab, out teleportExplosionList);	//Handles teleport explosion fx must have at least 20
	}

	[PunRPC]
	public void AxeMissSoundFx(Vector3 hitPosition){
		if(axeMissFXPrefab != null){
			GameObject axeMissSoundFX;
			GetFXPrefab (axeMissSoundList, out axeMissSoundFX);
			if (axeMissSoundFX)
				axeMissSoundFX.transform.position = hitPosition;
		}
	}

	[PunRPC]
	public void AxeHitSoundFx(Vector3 hitPosition){
		if(axeHitFXPrefab != null){
			GameObject axeHitSoundFX;
			GetFXPrefab (axeHitSoundList, out axeHitSoundFX);
			if (axeHitSoundFX)
				axeHitSoundFX.transform.position = hitPosition;
		}
	}

	[PunRPC]
	public void BulletHitSoundFx(Vector3 hitPosition){
		if(bulletHitSoundPrefab != null){
			GameObject bulletHitSoundFX;
			GetFXPrefab (bulletHitSoundList, out bulletHitSoundFX);
			if (bulletHitSoundFX)
				bulletHitSoundFX.transform.position = hitPosition;
		}
	}

	[PunRPC]
	public void ProjectileHitSoundFx(Vector3 hitPosition){
		if(projectileHitSoundPrefab != null){
			GameObject projectileHitSoundFX;
			GetFXPrefab (projectileHitSoundList, out projectileHitSoundFX);
			if (projectileHitSoundFX)
				projectileHitSoundFX.transform.position = hitPosition;
		}
	}

	[PunRPC]
	public void BloodFX(Vector3 hitPosition){
		if(bloodFXPrefab != null){
			GameObject bloodFX;
			GetFXPrefab (bloodFXList, out bloodFX);
			if (bloodFX)
				bloodFX.transform.position = hitPosition;
		}
	}

	[PunRPC]
	public void MGLightFx(Vector3 startPos, Vector3 endPos){
		if(MGLightFXPrefab != null){
			GameObject mgFX;
			GetFXPrefab (mgList, out mgFX);
			if (mgFX == null)
				return;
			mgFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
			mgFX.transform.position = startPos;
		}
		if (MGSoundPrefab != null) {
			GameObject mgSoundFX;
			GetFXPrefab (mgSoundList, out mgSoundFX);
			if (mgSoundFX == null)
				return;
			mgSoundFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
			mgSoundFX.transform.position = startPos;
		}
	}

	[PunRPC]
	public void MGHeavyFx(Vector3 startPos, Vector3 endPos){
		if(MGHeavyFXPrefab != null){
			if (Vector3.Distance (startPos, endPos) >= 5f) {
				GameObject hmgFX;
				GetFXPrefab (hmgList, out hmgFX);
				if (hmgFX == null)
					return;
				hmgFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
				hmgFX.transform.rotation *= Quaternion.Euler (90, 0, 0);
				hmgFX.transform.position = startPos;
				hmgFX.GetComponent<ShotBehavior> ().SetBulletDirection ((endPos - startPos).normalized);
			}
		}
		if (HMGSoundPrefab != null) {
			GameObject hmgSoundFX;
			GetFXPrefab (hmgSoundList, out hmgSoundFX);
			if (hmgSoundFX == null)
				return;
			hmgSoundFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
			hmgSoundFX.transform.position = startPos;
		}
	}

	//need to pass in the players Look Direction in place of quaternion.identity
	[PunRPC]
	public void RocketFx(Vector3 startPos, Vector3 endPos, string ShooterName, int teamID, int photonID, string tagName){
		if (RocketFXPrefab != null) {
			GameObject rocketFX;
			GetFXPrefab (rocketList, out rocketFX);
			if (rocketFX == null)
				return;
			rocketFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
			rocketFX.transform.position = startPos;
			Projectile projectile = rocketFX.GetComponent<Projectile> ();
			projectile.setDistanceToTarget (Vector3.Distance (startPos, endPos));
			projectile.setShooterName (ShooterName);
			projectile.setShooterPhotonID (photonID);
			projectile.setShooterTag (tagName);
			projectile.setTeamID (teamID);
			projectile.setProjectileDirection ((endPos - startPos).normalized);
		}
	}

	//need to pass in the players Look Direction in place of quaternion.identity
	[PunRPC]
	public void EnemyRocketFx(Vector3 startPos, Vector3 endPos, string ShooterName, int teamID, int photonID, string tagName){
		if (EnemyRocketFXPrefab2 != null) {
			GameObject enemyRocketFX;
			GetFXPrefab (enemyRocketList, out enemyRocketFX);
			if (enemyRocketFX == null)
				return;
			enemyRocketFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
			enemyRocketFX.transform.position = startPos;
			Projectile projectile = enemyRocketFX.GetComponent<Projectile> ();
			projectile.setShooterName (ShooterName);
			projectile.setShooterPhotonID (photonID);
			projectile.setShooterTag (tagName);
			projectile.setTeamID (teamID);
			projectile.setProjectileDirection ((endPos - startPos).normalized);
		}
	}

	//Grenade
	[PunRPC]
	public void GrenadeFx(Vector3 startPos, Vector3 endPos, string ShooterName, int teamID, int photonID, string tagName){
		if (GrenadeFXPrefab != null) {
			GameObject grenadeFX;
			GetFXPrefab (grenadeList, out grenadeFX);
			if (grenadeFX == null)
				return;
			grenadeFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
			grenadeFX.transform.position = startPos;
			Projectile projectile = grenadeFX.GetComponent<Projectile> ();
			projectile.setShooterName (ShooterName);
			projectile.setShooterPhotonID (photonID);
			projectile.setShooterTag (tagName);
			projectile.setTeamID (teamID);
			projectile.setProjectileDirection ((endPos - startPos).normalized);
		}
	}

	//Grenade
	[PunRPC]
	public void TeleportFx(Vector3 startPos, Vector3 endPos, string ShooterName, int teamID, int photonID, string tagName){
		if (TeleportFXPrefab != null) {
			GameObject teleportFX;
			GetFXPrefab (teleportGrenadeList, out teleportFX);
			if (teleportFX == null)
				return;
			teleportFX.transform.rotation = Quaternion.LookRotation (endPos - startPos);
			teleportFX.transform.position = startPos;
			Projectile projectile = teleportFX.GetComponent<Projectile> ();
			projectile.setShooterName (ShooterName);
			projectile.setShooterPhotonID (photonID);
			projectile.setShooterTag (tagName);
			projectile.setTeamID (teamID);
			projectile.setProjectileDirection ((endPos - startPos).normalized);
		}
	}

	[PunRPC]
	public void ExplosionFx(Vector3 ExplosionPos){
		if(explosionFXPrefab != null){
			GameObject explosionFX;
			GetFXPrefab (explosionList, out explosionFX);
			if(explosionFX)
				explosionFX.transform.position = ExplosionPos;
		}
	}
	[PunRPC]
	public void EMPExplosionFx(Vector3 ExplosionPos){
		if(empExplosionFXPrefab != null){
			GameObject empExplosionFX;
			GetFXPrefab (empExplosionList, out empExplosionFX);
			if(empExplosionFX)
				empExplosionFX.transform.position = ExplosionPos;
		}
	}
	[PunRPC]
	public void TeleportExplosionFx(Vector3 ExplosionPos){
		if(teleportExplosionList != null){
			GameObject teleportExplosionFX;
			GetFXPrefab (teleportExplosionList, out teleportExplosionFX);
			if(teleportExplosionFX)
				teleportExplosionFX.transform.position = ExplosionPos;
		}
	}

	[PunRPC]
	public void RailReloadSoundFx(Vector3 myPosition){
		if(RailFXPrefab != null){
			GameObject railReloadSoundFX;
			GetFXPrefab (railList, out railReloadSoundFX);
			if (railReloadSoundFX == null)
				return;
			railReloadSoundFX.transform.position = myPosition;
		}
	}

	[PunRPC]
	public void HurtingFx(Vector3 HurtSoundSourcePosition){
		if(hurtingSoundFXPrefab != null){
			GameObject hurtFX;
			GetFXPrefab (hurtList, out hurtFX);
			if (hurtFX == null)
				return;
			hurtFX.transform.position = HurtSoundSourcePosition;
		}
	}

	[PunRPC]
	public void DyingFx(Vector3 DyingSoundSourcePosition){
		AudioSource.PlayClipAtPoint(dyingSounds[0], DyingSoundSourcePosition);
	}

	[PunRPC]
	public void HitConfirmFx(Vector3 HitConfirmSoundSourcePosition){
		if(hitConfirmSoundFXPrefab != null){
			GameObject hitConfirmFX;
			GetFXPrefab (hitConfirmList, out hitConfirmFX);
			if (hitConfirmFX == null)
				return;
			hitConfirmFX.transform.position = HitConfirmSoundSourcePosition;
		}
	}

	[PunRPC]
	public void MissShotFx(Vector3 hitPoint){
		GameObject missShotFX;
		GetFXPrefab (missShotList, out missShotFX);
		if (missShotFX == null)
			return;
		missShotFX.transform.position = hitPoint;
	}

	[PunRPC]
	public void MissAxeFx(Vector3 hitPoint){
		GameObject missAxeFX;
		GetFXPrefab (axeMissSoundList, out missAxeFX);
		if (missAxeFX == null)
			return;
		missAxeFX.transform.position = hitPoint;
	}

	[PunRPC]
	public void HitAxeFx(Vector3 hitPoint){
		GameObject hitAxeFX;
		GetFXPrefab (axeHitSoundList, out hitAxeFX);
		if (hitAxeFX == null)
			return;
		hitAxeFX.transform.position = hitPoint;
	}

}