using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInGamePanel : MonoBehaviour {

	public Text Label_FPS;
	public Text Label_Speed;
	public Text Value_Health;
	public Text Value_Armor;
	public Text Value_JetResource;
	public Text Label_CurrentWeapon;
	public Text Label_CurrentAccuracy;
	public Text Value_FPS;
	public Text Value_Speed;

	public Image Image_HealthRadial;
	public Image Image_ArmorRadial;
	public Image Image_RocketAmmo;
	public Image Image_PlasmaAmmo;
	public Image Image_RailAmmo;
	public Image Image_MGAmmo;
	public Image Image_HMGAmmo;
	public Image Image_AmmoRadial;
	public Image Image_JetRadial;

	public Slider Slider_CurrentAmmo;
	public Slider Slider_Health;
	public Slider Slider_Armor;
	public Slider Slider_JetResource;
	public Text Value_CurrentAmmo;

	public Image Image_Assist;
	public Text Label_Assist;
	public Text Label_KillConfirm;
	public Text Label_KillConfirm_Name;
	public Image Image_KillConfirm;
	public Text Label_MultiKillConfirm;
	public Image Image_MultiKillConfirm;
	public Text Label_KillStreak;
	public Image Image_KillStreak;

	public Image Image_CrossHair;
	public Image Image_Hud;
	public Sprite Damage_Hud;
	public Sprite Stun_Hud;
	public Sprite Normal_Hud;

	public Color hitColor;
	public Color normalColor;
	public Color nullColor;

	public bool isChatEnabled = false;

	public void doHitCrossHair(){
		if (isChatEnabled)
			return;
		
		Image_CrossHair.GetComponent<RectTransform> ().sizeDelta = new Vector2 (55f, 55f);
		Image_CrossHair.color = hitColor;


	}

	public void doNormalCrossHair(){
		if (isChatEnabled)
			return;
		
		Image_CrossHair.color = normalColor;
		Image_CrossHair.GetComponent<RectTransform> ().sizeDelta = new Vector2 (45f, 45f);

	}

	public void doNullCrossHair(){
		Image_CrossHair.color = nullColor;
	}


}
