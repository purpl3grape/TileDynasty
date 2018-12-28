using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour {

	public float time;
	public TimeSpan currentTime;
	public Transform sunTransform;
	public Light sunLight;
	public Text timeText;
	public int days;

	public float intensity;
	public Color fogDay;
	public Color fogNight;

	public int speed;

	void FixedUpdate () {
		
	}

	public void ChangeTime(){
		time += Time.fixedDeltaTime * speed;
		if (time > 86400) {
			days += 1;
			time = 0;
		}
		currentTime = TimeSpan.FromSeconds (time);
		string[] tempTime = currentTime.ToString ().Split (":" [0]);
		timeText.text = tempTime [0] + ":" + tempTime [1];

		sunTransform.rotation = Quaternion.Euler (new Vector3 ((time - 21600) / 86400 * 360, 0, 0));

		if (time < 43200) {
			intensity = 1 - (43200 - time) / 43200;
		} else {
			intensity = 1 - ((43200 - time) / 43200 * -1);
		}

		RenderSettings.fogColor = Color.Lerp (fogNight, fogDay, intensity);
		sunLight.intensity = intensity;

	}
}
