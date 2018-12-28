using UnityEngine;
using System.Collections;

public enum DestroyFXType{
	Explosion,
	EmpExplosion,
	TeleportExplosion,
	MG,
	MGSound,
	HMG,
	HMGSound,
	MissSound,
	MissAxeSound,
	HitSound,
	HitAxeSound,
	HurtSound,
	BulletHitSound,
	ProjectileHitSound,
	RailReloadSound,
	Blood,
}

public class ExplosionDestroy : MonoBehaviour {

	WaitForSeconds waitforSelfDestructTime;
	FXManager fxManager;
	public float selfDestructTime = 1.0f;
	public GameObject sceneScripts;
	public DestroyFXType destroyFXType;

	void Start(){
		fxManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<FXManager> ();
		waitforSelfDestructTime = new WaitForSeconds (selfDestructTime);
	}

	void FixedUpdate(){
		if (fxManager) {
			if (DestroyFxCO == null) {
				DestroyFxCO = DestroyFX_CO ();
				StartCoroutine (DestroyFxCO);
			}
		}
	}

	IEnumerator DestroyFxCO;
	IEnumerator DestroyFX_CO(){
		yield return waitforSelfDestructTime;
		//		Debug.Log ("fx removed after: " + selfDestructTime + " --> " + gameObject.name);
		if (destroyFXType == DestroyFXType.Explosion)
			fxManager.explosionList = fxManager.DestroyFXPrefab (gameObject, fxManager.explosionList);
		else if (destroyFXType == DestroyFXType.EmpExplosion)
			fxManager.empExplosionList = fxManager.DestroyFXPrefab (gameObject, fxManager.empExplosionList);
		else if (destroyFXType == DestroyFXType.TeleportExplosion)
			fxManager.teleportExplosionList = fxManager.DestroyFXPrefab (gameObject, fxManager.teleportExplosionList);
		else if (destroyFXType == DestroyFXType.MG)
			fxManager.mgList = fxManager.DestroyFXPrefab (gameObject, fxManager.mgList);
		else if (destroyFXType == DestroyFXType.MGSound)
			fxManager.mgSoundList = fxManager.DestroyFXPrefab (gameObject, fxManager.mgSoundList);
		else if (destroyFXType == DestroyFXType.HMG)
			fxManager.hmgList = fxManager.DestroyFXPrefab (gameObject, fxManager.hmgList);
		else if (destroyFXType == DestroyFXType.HMGSound)
			fxManager.hmgSoundList = fxManager.DestroyFXPrefab (gameObject, fxManager.hmgSoundList);
		else if (destroyFXType == DestroyFXType.MissSound)
			fxManager.missShotList = fxManager.DestroyFXPrefab (gameObject, fxManager.missShotList);
		else if (destroyFXType == DestroyFXType.MissAxeSound)
			fxManager.axeMissSoundList = fxManager.DestroyFXPrefab (gameObject, fxManager.axeMissSoundList);
		else if (destroyFXType == DestroyFXType.HitSound)
			fxManager.hitConfirmList = fxManager.DestroyFXPrefab (gameObject, fxManager.hitConfirmList);
		else if (destroyFXType == DestroyFXType.HitAxeSound)
			fxManager.axeHitSoundList = fxManager.DestroyFXPrefab (gameObject, fxManager.axeHitSoundList);
		else if (destroyFXType == DestroyFXType.HurtSound)
			fxManager.hurtList = fxManager.DestroyFXPrefab (gameObject, fxManager.hurtList);
		else if (destroyFXType == DestroyFXType.BulletHitSound)
			fxManager.bulletHitSoundList = fxManager.DestroyFXPrefab (gameObject, fxManager.bulletHitSoundList);
		else if (destroyFXType == DestroyFXType.ProjectileHitSound)
			fxManager.projectileHitSoundList = fxManager.DestroyFXPrefab (gameObject, fxManager.projectileHitSoundList);
		else if (destroyFXType == DestroyFXType.RailReloadSound)
			fxManager.railList = fxManager.DestroyFXPrefab (gameObject, fxManager.railList);
		else if (destroyFXType == DestroyFXType.Blood)
			fxManager.bloodFXList = fxManager.DestroyFXPrefab (gameObject, fxManager.bloodFXList);
		DestroyFxCO = null;
	}


}