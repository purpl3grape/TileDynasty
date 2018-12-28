using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrossHair : MonoBehaviour {

	GUITexture texture;
	public Color hitColor = new Color(100f, 185f, 125f, 255f);
	public Color normalColor = new Color(100f, 185f, 125f, 255f);
	public Rect normalCrosshairSize = new Rect(-16,-16,32,32);
	public Rect hitCrosshairSize = new Rect(-24,-24,48,48);


	// Use this for initialization
	void Start () {
		texture = GetComponent<GUITexture> ();
	}
}
