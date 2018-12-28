using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Preloader : Photon.MonoBehaviour {
    void Update()
    {
        //Wait for both MainMenu & game scene to be loaded (since mainmenu doesnt check on game scene)
//		if (Application.GetStreamProgressForLevel (1) >= 1 && Application.GetStreamProgressForLevel (2) >= 1 && Application.GetStreamProgressForLevel (3) >= 1 && Application.GetStreamProgressForLevel (4) >= 1 && Application.GetStreamProgressForLevel (5) >= 1)
		if (Application.GetStreamProgressForLevel (1) >= 1 && 
			Application.GetStreamProgressForLevel (2) >= 1)
			SceneManager.LoadScene (1);
    }
}