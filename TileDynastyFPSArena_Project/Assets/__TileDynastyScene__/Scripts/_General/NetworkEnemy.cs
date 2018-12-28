using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NetworkEnemy : Photon.MonoBehaviour, IPunObservable {

	NetworkManager nManager;
	ScoringManager sManager;
	NavMeshAgent agent;
	EnemyMovement enemyMovement;
	Health health;
	TeamMember teamMember;
	Transform tr;
	Vector3 realPosition = Vector3.zero;
	Vector3 enemyVelocity = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	Vector3 targetPosition = Vector3.zero;
	Vector3 startPosition = Vector3.zero;
	NavMeshPath realPath;
	AudioSource audioSource;
	public Animator animator;
	float fullDistanceToRealPosition = 0f;
	float currentDistanceToRealPosition = 0f;
	float lastPackageReceivedTimeStamp = 0f;
	float syncTime = 0f;
	float syncDelay = 0f;
	float syncThreshold = 0f;
	bool gotFirstUpdate = false;
	float currentDistance = 0f;
	float fullDistance = 0f;
	float progress = 0f;
	float velocityPredictionValue=0f;
	float enemyPredictionValue = 0f;
	float syncDistanceValue=0f;

	float pingInSeconds = 0f;
	float timeSinceLastUpdate = 0f;
	float totalTimePassedSinceLastUpdate = 0f;

	int currRocketBulletFired = 0;
	int currRocketBulletHit = 0;
	int currDamageReceivedCount = 0;
	int currDamageDealtCount = 0;
	int currDeathCount = 0;
	int currKillCount = 0;
	int currKillStreakCount = 0;
	int currKillStreakHighestCount = 0;
	string currLastPlayerKilledName = string.Empty;

	int RocketBulletFired = 0;
	int RocketBulletHit = 0;

	int DamageReceivedCount = 0;
	int DamageDealtCount = 0;

	int DeathCount = 0;
	int KillCount = 0;
	int KillStreakCount = 0;
	int KillStreakHighestCount = 0;
	string LastPlayerKilledName = string.Empty;


	public AudioClip[] audioList;

	void Awake () {
		nManager = GameObject.FindGameObjectWithTag ("SceneScripts").GetComponent<NetworkManager> ();
		sManager = nManager.GetComponent<ScoringManager> ();
		agent = GetComponent<NavMeshAgent>();
		enemyMovement = GetComponent<EnemyMovement> ();
		tr = GetComponent<Transform> ();
		health = GetComponent<Health> ();
		teamMember = GetComponent<TeamMember> ();
		audioSource = GetComponent<AudioSource> ();
		//		PhotonNetwork.sendRate = 33;
		//		PhotonNetwork.sendRateOnSerialize = 20;
		//		PhotonNetwork.sendRate = 10;
		//		PhotonNetwork.sendRateOnSerialize = 10;
		syncThreshold = (1 / 20);
	}

	void Update () {
		enemyPredictionValue = nManager.enemyPredVal;
		velocityPredictionValue = nManager.velocityPredVal;
		syncDistanceValue = nManager.syncDisnceVal / 10;

		animator.SetFloat (animSpeed, enemyVelocity.magnitude);


		adjustVerticalAimAngle ();

		if (PhotonNetwork.isMasterClient) {
			animator.SetFloat (animSpeed, agent.velocity.magnitude);
			if (agent.velocity.magnitude > 1f) {
				if (audioSource) {
					if (!audioSource.isPlaying)
						audioSource.PlayOneShot (audioList [0]);
				}	
			} else {
				if (audioSource) {
					audioSource.Stop ();
				}
			}
		} else {

			if (agent.enabled) {
				agent.enabled = false;
			}

			animator.SetFloat (animSpeed, enemyVelocity.magnitude);
			if (enemyVelocity.magnitude > 1f) {
				if (audioSource) {
					if (!audioSource.isPlaying)
						audioSource.PlayOneShot (audioList [0]);
				}	
			} else {
				if (audioSource) {
					audioSource.Stop ();
				}
			}


			pingInSeconds = (float)PhotonNetwork.GetPing () * 0.001f;
			timeSinceLastUpdate = (float)(PhotonNetwork.time - lastPackageReceivedTimeStamp);
			totalTimePassedSinceLastUpdate = pingInSeconds + timeSinceLastUpdate;

			currentDistance = Vector3.Distance (tr.position, realPosition);
			fullDistance = Vector3.Distance (startPosition, realPosition);
			if (fullDistance != 0) {
				progress = currentDistance / fullDistance;
			}

			if (enemyPredictionValue != 0) {
				progress /= enemyPredictionValue;
			}
			syncTime += Time.deltaTime;


			if ( 1< 0 && currentDistance > (syncDistanceValue / enemyPredictionValue) || enemyVelocity.magnitude == 0f) {
				tr.rotation = realRotation;
				tr.position = realPosition;
			} else {
				tr.rotation = Quaternion.Lerp (tr.rotation, realRotation, syncTime / syncDelay);
				tr.position = Vector3.Lerp (tr.position, realPosition, progress);
			}

		}
	}


	string animAimAngle = "AimAngle";
	string animSpeed = "Speed";
	void adjustVerticalAimAngle(){
		float myAimAngle = 0.0f;
		myAimAngle = tr.rotation.eulerAngles.x <= 90 ? -1 * tr.rotation.eulerAngles.x : 360 - tr.rotation.eulerAngles.x;

		//Set parameter on animator component during runtime
		if (myAimAngle >= 90f)
			myAimAngle = 89f;
		else if (myAimAngle <= -90f)
			myAimAngle = -89f;
		animator.SetFloat(animAimAngle, myAimAngle);
	}


	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting) {
			stream.SendNext (tr.position);
			stream.SendNext (tr.rotation);
			stream.SendNext (agent.velocity);
			stream.SendNext (agent.steeringTarget);
			stream.SendNext (enemyMovement.RocketFiredCount);
			stream.SendNext (enemyMovement.RocketHitCount);
			stream.SendNext (health.DamageReceivedCount);
			stream.SendNext (enemyMovement.DamageDealtCount);
//
//			stream.SendNext (health.DeathCount);
//			stream.SendNext (health.KillCount);
//			stream.SendNext (health.KillStreakCount);
//			stream.SendNext (health.KillStreakHighestCount);
//			stream.SendNext (health.LastPlayerKilledName);

		} else {

			startPosition = realPosition;

			realPosition = (Vector3)stream.ReceiveNext ();
			realRotation = (Quaternion)stream.ReceiveNext ();
			enemyVelocity = (Vector3)stream.ReceiveNext ();
			targetPosition = (Vector3)stream.ReceiveNext ();
			RocketBulletFired = (int)stream.ReceiveNext ();
			RocketBulletHit = (int)stream.ReceiveNext ();
			DamageReceivedCount = (int)stream.ReceiveNext ();
			DamageDealtCount = (int)stream.ReceiveNext ();
//
//			DeathCount = (int)stream.ReceiveNext ();
//			KillCount = (int)stream.ReceiveNext ();
//			KillStreakCount = (int)stream.ReceiveNext ();
//			KillStreakHighestCount = (int)stream.ReceiveNext ();
//			LastPlayerKilledName = (string)stream.ReceiveNext ();

//			if (currRocketBulletFired != RocketBulletFired)
				sManager.SetScore (name, sManager.shotfired_rocketStat, RocketBulletFired);
//			if (currRocketBulletHit != RocketBulletHit)
				sManager.SetScore (name, sManager.shothit_rocketStat, RocketBulletHit);

//			if (currDamageDealtCount != DamageDealtCount)
				sManager.SetScore (name, sManager.damagedealtStat, DamageDealtCount);
//			if (currDamageReceivedCount != DamageReceivedCount)
				sManager.SetScore (name, sManager.damagereceivedStat, DamageReceivedCount);

//			currRocketBulletFired = RocketBulletFired;
//			currRocketBulletHit = RocketBulletHit;
//			currDamageReceivedCount = DamageReceivedCount;
//			currDamageDealtCount = DamageDealtCount;
//			currDeathCount = DeathCount;
//			currKillCount = KillCount;
//			currKillStreakCount = KillStreakCount;
//			currKillStreakHighestCount = KillStreakHighestCount;
//			currLastPlayerKilledName = LastPlayerKilledName;


			syncTime = 0f;
			syncDelay = Time.time - lastPackageReceivedTimeStamp;
			lastPackageReceivedTimeStamp = Time.time;

			//Velocity Prediction
			realPosition = realPosition + (realPosition * syncDelay) * velocityPredictionValue;

			if (gotFirstUpdate == false) {
				tr.position = realPosition;
				tr.rotation = realRotation;
				gotFirstUpdate = true;
			}

		}
	}
}