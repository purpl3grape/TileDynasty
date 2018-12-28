using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class togglemeshrenderer : MonoBehaviour {

	public bool isEnableRenderer = false;
	public MeshRenderer[] childRenderers;

	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () {

		if (Application.isPlaying)
			return;

		if (isEnableRenderer) {
			childRenderers = GetComponentsInChildren<MeshRenderer> ();
			foreach (MeshRenderer m in childRenderers) {
				m.enabled = true;
			}
		} else {
			childRenderers = GetComponentsInChildren<MeshRenderer> ();
			foreach (MeshRenderer m in childRenderers) {
				m.enabled = false;
			}

		}
	}
}
