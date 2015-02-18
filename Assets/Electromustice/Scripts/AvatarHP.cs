using UnityEngine;
using System.Collections;

public class AvatarHP : MonoBehaviour {

	public Vector2 hudPositionPercentage = new Vector2(18f,32f); // screen position , percentage
	public Vector2 hudSizePercentage = new Vector2(16f,1.5f); // size of the hud bar, percentage
	
	private Vector2 hudPosition;
	private Vector2 hudSize;
	
	private float maxHp;
	private float hp;
	public float f_speedRecovery;

	private GUIStyle backgroundStyle = null;
	private GUIStyle filledinStyle = null;

	private Color colEnergy = new Color( 0f, 0.4f, 1f, 0.3f );

	Texture2D backGround;
	Texture2D fillIn;

	private void SetStyles(Texture2D tex, ref GUIStyle sty)
	{
		sty = new GUIStyle( GUI.skin.box );
		sty.normal.background = tex;
	}
	
	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	// Use this for initialization
	void Start () {
		this.maxHp = GlobalVariables.PLAYER_HP;
		hp = maxHp;
		Vector2 screenVector= new Vector2 (Screen.width,Screen.height);
		hudPosition.x = screenVector.x*hudPositionPercentage.x/100;
		hudPosition.y = screenVector.y*hudPositionPercentage.y/100;
		hudSize.x = screenVector.x* hudSizePercentage.x/100;
		hudSize.y = screenVector.y* hudSizePercentage.y/100;
		// prepare texture
		backGround = MakeTex (1, 1, new Color (0f, 0f, 0f, 0.2f));
		fillIn = MakeTex (1, 1, colEnergy);
	}

	public void setHp(float _hp){
		hp = _hp;
	}

	public float getHp()
	{
		return hp;
	}

	public float getMaxHp()
	{
		return maxHp;
	}

	public void damage(float d){
		hp = hp - d;
	}
	
	// Update is called once per frame
	void Update () {
		if(Network.isClient)
		{
			if(hp < maxHp)
			{
				hp += GlobalVariables.F_ENERGY_SPEED_RECOVERY * Time.deltaTime;
				if(hp > maxHp)
				{
					hp = maxHp;
				}
			}
		}
	}

//	void OnTriggerEnter(Collider other)
//	{
//		if(other.gameObject.name.Contains("Monster"))
//		{
//			damage(GlobalVariables.F_DAMAGE_MONSTER_TO_PLAYER);
//		}
//	}

	
	// display the hud bars of hp and energy
	void OnGUI() {

		if (NetworkManager.I_INDEX_MY_PLAYER == 0) {
			colEnergy = new Color (1f, 0f, 0.89f, 0.6f);
		} else {
			colEnergy = new Color (1f, 0.68f, 0f, 0.6f);
		}

		GUI.BeginGroup (new Rect (hudPosition.x, hudPosition.y, hudSize.x, hudSize.y));
		//draw the background
		SetStyles( backGround,ref backgroundStyle);
		GUI.Box (new Rect(0,0,hudSize.x,hudSize.y),"",backgroundStyle);
		//draw the filled in part
		SetStyles(fillIn, ref filledinStyle);
		GUI.BeginGroup (new Rect (0, 0, hudSize.x * hp / maxHp, hudSize.y));
		GUI.Box (new Rect(0,0,hudSize.x,hudSize.y),"",filledinStyle);
		GUI.EndGroup ();
		GUI.EndGroup ();

		GUI.BeginGroup (new Rect (hudPosition.x, hudPosition.y, hudSize.x+Screen.width/2f , hudSize.y));
		//draw the background
		SetStyles( backGround,ref backgroundStyle);
		GUI.Box (new Rect(Screen.width/2f,0,hudSize.x,hudSize.y),"",backgroundStyle);
		//draw the filled in part
		SetStyles(fillIn, ref filledinStyle);
		GUI.BeginGroup (new Rect (0, 0, hudSize.x * hp / maxHp + Screen.width/2f, hudSize.y));
		GUI.Box (new Rect(Screen.width/2f,0,hudSize.x,hudSize.y),"",filledinStyle);
		GUI.EndGroup ();
		GUI.EndGroup ();
	}
}









