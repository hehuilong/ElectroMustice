using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour {

	private GameObject serverCamera;

	private float f_timer = 0;
	private float energy_timer = 0;
	private bool b_startGame = false;
	private int i_numMonsters = 0;
	private bool b_gameFinished = false;
	private bool b_gameWin = false;
	private int i_numLevels = 0;
	private int i_currentLevel = 0;

	#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
	private int i_numMonsterSpown = 0;
	private List<LevelDescription> list_levels = new List<LevelDescription>();
	#endif

	MonsterFactory go_monsterFactory;
	EnergyBallFactory go_energyBallFactory;
	EnergyBarManager go_energyBarManager;

	#region UI
	private bool b_informNewLevel = false;
	private string s_nameNewLevel = null;
	private float f_durationNewLevel = -1;
	private float f_timerInform = 0;
	private Color co_colorInform = new Color (0f, 0.4f, 1f, 0.05f);
	private GUIStyle informGUIStyle = null;
	private string s_infoLevel;
	private Texture2D backGround;
	private Texture2D fillIn;
	private Color colorGUI = new Color( 0f, 0.4f, 1f, 0.3f );
	private GUIStyle backgroundStyle = null;
	private GUIStyle filledinStyle = null;
	private bool b_canDisplayInfoGame = true;

	public TextMesh[] textPlayers;  //Those text meshes are attached to the winndow gameobject
	#endregion UI

	AudioSource backgroundMusic;

	private static GameManager _instance;
	
	public static GameManager Instance
	{
		get
		{
			_instance = FindObjectOfType(typeof(GameManager)) as GameManager;
			if(_instance == null)
			{
				_instance = GameObject.Instantiate(GlobalVariables.GO_MONSTER_FACTORY) as GameManager;
			}
			
			return _instance;
		}
	}

//	private void SetStyles(Color color)
//	{
//		informGUIStyle = new GUIStyle( GUI.skin.box );
//		informGUIStyle.fontSize = 25;
////		informGUIStyle.font.material.color = new Color (255, 255, 255, 0.2f);
//		GUI.contentColor = new Color (255, 255, 255, 0.5f);
//
//		informGUIStyle.contentOffset = Vector2.zero;
//	}

	private void SetStyles(Texture2D tex, ref GUIStyle sty)
	{
		sty = new GUIStyle( GUI.skin.box );
		sty.normal.background = tex;
		sty.fontSize = 20;
	}

	public void initLevels()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		list_levels = XmlHelpers.LoadFromTextAsset<LevelDescription> (GlobalVariables.TEXT_LEVELS, null);
		i_numLevels = list_levels.Count - 1;
		
		i_currentLevel = 0;
//		networkView.RPC("initLevelRPC", RPCMode.Others, i_numLevels);
		#endif
	}

	[RPC]
	public void client_initLevelRPC(int _i_numLevels, int _i_currentLevel, string _s_nameLevel, float _f_durationLevel)
	{
		i_numLevels = _i_numLevels;
		i_currentLevel = 0;

		InformNewLevelLocal(i_currentLevel, _s_nameLevel, _f_durationLevel);
	}

	public void startGame()
	{
		b_startGame = true;
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
		// huilong, 31/01
		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		go_monsterFactory = MonsterFactory.Instance;
		// loic energy
		go_energyBallFactory = EnergyBallFactory.Instance;

		go_energyBarManager = GameObject.Find("EnergyBarManager").GetComponent<EnergyBarManager>();

		serverCamera = GameObject.Find("ServerCamera");

//		i_numLevels = GlobalVariables.LIST_DURATION_LEVELS.Count - 1;  //from 0 to GlobalVariables.LIST_DURATION_LEVELS.Count -1
//		i_currentLevel = 0;
		initLevels();

		backgroundMusic = GameObject.Find("ServerCamera").GetComponent<AudioSource>();

		#elif UNITY_ANDROID
		#elif UNITY_IPHONE
		#endif

		// prepare texture
		backGround = MakeTex (1, 1, new Color (0f, 0f, 0f, 0.2f));
		fillIn = MakeTex (1, 1, colorGUI);
	}


	// Update is called once per frame
	void Update () {

		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		if(Network.isServer && b_startGame && i_currentLevel <= i_numLevels && !b_gameFinished && !b_gameWin)
		{
			if(f_timer == 0 && i_currentLevel == 0)  //inform the level 0
			{
				InformNewLevelLocal(i_currentLevel, list_levels[i_currentLevel].Name, list_levels[i_currentLevel].Duration);
				networkView.RPC("client_initLevelRPC", RPCMode.Others, i_numLevels, i_currentLevel, list_levels[i_currentLevel].Name, list_levels[i_currentLevel].Duration);
			}

			// huilong , 31/01
			f_timer += Time.deltaTime;

			if(f_timer > list_levels[i_currentLevel].Duration)
			{
				i_currentLevel ++;
				if(i_currentLevel <= i_numLevels)
				{
					networkView.RPC("InformNewLevelRPC", RPCMode.All, i_currentLevel, list_levels[i_currentLevel].Name, list_levels[i_currentLevel].Duration);
				}

				i_numMonsters = 0;
				energy_timer = 0;
				f_timer = 0;

				if(i_currentLevel > i_numLevels)
				{
					b_gameFinished = true;
					string s_strFinish = "Finish";
//					displayGameStatistics();
					networkView.RPC("InformNewLevelRPC", RPCMode.All, i_currentLevel, s_strFinish, 0f);
				}
			}

			switch (i_currentLevel)
			{
			case 0:
				break;
			case 1:
				#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
                // play music
				if (!backgroundMusic.isPlaying)
					backgroundMusic.Play();
				#endif

				if(f_timer - i_numMonsters*8 > 0)
				{
					i_numMonsters ++;

					if(f_timer < 15)
					{
						go_monsterFactory.spawnMonster1(1);
					}
					else if(f_timer < 30)
					{
						go_monsterFactory.spawnMonster2(1);
					}
					else
					{
						go_monsterFactory.spawnMonsters(1);
					}
				}

				break;
			case 2:
				break;
			case 3:

				if(f_timer - i_numMonsters*7f > 0)
				{
					i_numMonsters ++;
					
					if(f_timer < 15)
					{
						go_monsterFactory.spawnMonster1(1);
					}
					else if(f_timer < 30)
					{
						go_monsterFactory.spawnMonster2(1);
					}
					else
					{
						go_monsterFactory.spawnMonsters(1);
					}
				}

				break;
			case 4:
				break;
			}

//			if(f_timer > 3 && i_numMonsters < 0)
//			{
//				f_timer = 0;
//				go_monsterFactory.spawnMonsters(1);
//				i_numMonsters ++;
//			}

			if(i_currentLevel != 0 && i_currentLevel != 2)
			{
				energy_timer += Time.deltaTime;

				if(energy_timer > 3.5f){
					energy_timer = 0;
					go_energyBallFactory.spawnEnergyBalls(1, 5);
				}
			}

//			if(go_energyBarManager.getNumEnergy() >= GlobalVariables.F_MAX_NUM_ENERGY)
//			{
//				b_gameFinished = true;
//				b_gameWin = true;
//				networkView.RPC("gameWinRPC", RPCMode.Others);
//			}
		}

		if(Network.isServer && b_canDisplayInfoGame && b_gameFinished)
		{
			for(int i = 0; i < NetworkManager.I_NUM_CLIENTS; ++i)
			{
				if(NetworkManager.PlayerStatistics[i].B_UPDATE_FINISH == false)
				{
					b_canDisplayInfoGame = false;
				}
			}
			if(NetworkManager.I_NUM_CLIENTS == 0)
			{
				b_canDisplayInfoGame = true;
			}

			if(b_canDisplayInfoGame)
			{
				sendStatistics();
				displayGameInfo();
				b_canDisplayInfoGame = false;
				// huilong, set camera to the final position to show the info well
				serverCamera.GetComponent<CameraMove>().setToFinal();
			}
			else
			{
				b_canDisplayInfoGame = true;
			}
		}
		#elif UNITY_ANDROID
		#elif UNITY_IPHONE
		#endif
	}

	[RPC]
	public void server_updateShootInfoRPC(int _i_numBulletShoot, int _i_indexPlayer)
	{
		NetworkManager.PlayerStatistics [_i_indexPlayer].I_NUM_BULLET_SHOOT = _i_numBulletShoot;
		NetworkManager.PlayerStatistics [_i_indexPlayer].B_UPDATE_FINISH = true;
	}
	
	public void displayGameInfo()
	{
		for(int i = 0; i < textPlayers.Length; ++i)
		{
			if(i == 0)   //0 is just the name of the information
			{
				textPlayers[i].text += ("\n");
				textPlayers[i].text += ("Bullets shooted:\n");
				textPlayers[i].text += ("Bullets that hit the monsters:\n");
				textPlayers[i].text += ("Monsters killed:\n");
				textPlayers[i].text += ("Machines repaired:\n");
				textPlayers[i].text += ("Energy balls absorbed:\n");
				textPlayers[i].text += ("Energy balls hit:\n");
				textPlayers[i].text += ("\n");
				textPlayers[i].text += ("Final Score:\n");
			}
			else
			{
//				if(i == 1)
//				{
//					textPlayers[i].color = new Color(227f, 25f, 214f);
//				}
//				else if(i == 2)
//				{
//					textPlayers[i].color = new Color(238f, 151f, 19f);
//				}
				textPlayers[i].text += ("Player " + i + "\n");
				textPlayers[i].text += ("   " + NetworkManager.PlayerStatistics[i-1].I_NUM_BULLET_SHOOT + '\n');
				textPlayers[i].text += ("   " + NetworkManager.PlayerStatistics[i-1].I_NUM_BULLET_DAMAGE_MONSTER + '\n');
				textPlayers[i].text += ("   " + NetworkManager.PlayerStatistics[i-1].I_NUM_MONSTER_KILLED + '\n');
				textPlayers[i].text += ("   " + NetworkManager.PlayerStatistics[i-1].I_NUM_MACHINE_REPAIR + '\n');
				textPlayers[i].text += ("   " + NetworkManager.PlayerStatistics[i-1].I_NUM_ENERGYBALL_ABSORB + '\n');
				textPlayers[i].text += ("   " + NetworkManager.PlayerStatistics[i-1].I_NUM_ENERGYBALL_HIT + '\n');
				textPlayers[i].text += ("\n");

				int i_finalScore = 0;
				i_finalScore += ( NetworkManager.PlayerStatistics[i-1].I_NUM_BULLET_DAMAGE_MONSTER * 1 +
				                  NetworkManager.PlayerStatistics[i-1].I_NUM_MONSTER_KILLED * 2 +
				                  NetworkManager.PlayerStatistics[i-1].I_NUM_MACHINE_REPAIR * 2 +
				                 NetworkManager.PlayerStatistics[i-1].I_NUM_ENERGYBALL_ABSORB * 5);

				textPlayers[i].text += ("   " + i_finalScore + '\n');
								
			}
		}
	}

	public void resetGameInfo()
	{
		NetworkManager.I_INDEX_MY_PLAYER = -1;
		NetworkManager.I_NUM_CLIENTS = 0;

		for(int i = 0; i < NetworkManager.PlayerStatistics.Length; ++i)
		{
			NetworkManager.PlayerStatistics[i].I_INDEX_PLAYER = i;
			NetworkManager.PlayerStatistics[i].I_NUM_BULLET_DAMAGE_MONSTER = 0;
			NetworkManager.PlayerStatistics[i].I_NUM_BULLET_SHOOT = 0;
			NetworkManager.PlayerStatistics[i].I_NUM_ENERGYBALL_ABSORB = 0;
			NetworkManager.PlayerStatistics[i].I_NUM_ENERGYBALL_HIT = 0;
			NetworkManager.PlayerStatistics[i].I_NUM_MACHINE_REPAIR = 0;
			NetworkManager.PlayerStatistics[i].I_NUM_MONSTER_KILLED = 0;
			NetworkManager.PlayerStatistics[i].B_UPDATE_FINISH = false;
		}
	}

	public void sendStatistics()
	{
		BinaryFormatter binFormatter = new BinaryFormatter ();
		MemoryStream memStream = new MemoryStream (); // Stream whose backing store is memory. Defined in namespace System.IO
		binFormatter.Serialize (memStream,NetworkManager.PlayerStatistics);
		byte[] serializedPl = memStream.ToArray (); // Convert the serialized object to byte array
		memStream.Close ();

		networkView.RPC ("client_updateStatisticsRPC", RPCMode.Others, serializedPl);
	}

	[RPC]
	public void client_updateStatisticsRPC(byte[] serializedPlayerList)
	{
		BinaryFormatter binFormatter = new BinaryFormatter ();
		MemoryStream memStream = new MemoryStream (); 
		// Write the byte data we received into the stream 
		// The second parameter specifies the offset to the beginning of the stream
		// The third parameter specifies the maximum number of bytes to be written
		memStream.Write(serializedPlayerList,0,serializedPlayerList.Length); 
		// Stream internal "reader" is now at the last position
		// Shift it back to the beginning for reading
		memStream.Seek(0, SeekOrigin.Begin); 
		NetworkManager.PlayerStatistics = (GameStatistics[])binFormatter.Deserialize (memStream);
//		displayGameStatistics ();
		displayGameInfo ();
		resetGameInfo ();
	}

	[RPC]
	public void gameWinRPC()
	{
		b_gameFinished = true;
		b_gameWin = true;
	}

	[RPC]
	public void InformNewLevelRPC(int _i_currentLevel, string _s_nameLevel, float _f_durationLevel)
	{
		InformNewLevelLocal (_i_currentLevel, _s_nameLevel, _f_durationLevel);

	}

	public void InformNewLevelLocal(int _i_currentLevel, string _s_nameLevel, float _f_durationLevel)
	{
		i_currentLevel = _i_currentLevel;
		b_informNewLevel = true;
		
		s_nameNewLevel = _s_nameLevel;
		f_durationNewLevel = _f_durationLevel;
		
		if(i_currentLevel > i_numLevels)
		{
			b_gameFinished = true;
			b_informNewLevel = true;
		}
	}

	void OnGUI()
	{
		if(b_informNewLevel)
		{
			f_timerInform += Time.deltaTime;

			if(f_timerInform < 4f)
			{
				if(b_gameFinished)
				{
					if(Network.isClient)
					{
						networkView.RPC("server_updateShootInfoRPC", RPCMode.Server, 
					    	            NetworkManager.PlayerStatistics[NetworkManager.I_INDEX_MY_PLAYER].I_NUM_BULLET_SHOOT,
					        	        NetworkManager.I_INDEX_MY_PLAYER);
					}

					if(b_gameWin)
					{
						s_infoLevel = "You Win !!!";
					}
					else
					{
						s_infoLevel = "You Lose !!!";
					}
				}
				else
				{
					s_infoLevel = s_nameNewLevel;
					s_infoLevel += "\n";
					s_infoLevel +="Duration : " + f_durationNewLevel + "s";
				}
				GUI.BeginGroup (new Rect(0f, 0f, Screen.width/1f, Screen.height/1f));
				//draw the background
				SetStyles( backGround,ref backgroundStyle);
				//				SetStyles(new Color( 0f, 0f, 0f, 0.05f ) );
				GUI.Box(new Rect(Screen.width/2f * (1.5f/10f), Screen.height * (2f/5f), Screen.width/2f * (7f/10f), Screen.height/8f), "", backgroundStyle);
				//draw the filled in part 	
				SetStyles(fillIn, ref filledinStyle);
				GUI.BeginGroup (new Rect(0f, 0f, Screen.width/1f, Screen.height/1f));
				//				SetStyles(co_colorInform);
				GUI.Box(new Rect(Screen.width/2f * (1.5f/10f), Screen.height * (2f/5f), Screen.width/2f * (7f/10f), Screen.height/8f), s_infoLevel, filledinStyle);
				GUI.EndGroup();
				GUI.EndGroup();

				if(Network.isClient)
				{
					GUI.BeginGroup (new Rect(0f, 0f, Screen.width/1f, Screen.height/1f));
					//draw the background
					SetStyles( backGround,ref backgroundStyle);
					//				SetStyles(new Color( 0f, 0f, 0f, 0.05f ) );
					GUI.Box(new Rect(Screen.width/2f * (1f + 1.5f/10f), Screen.height * (2f/5f), Screen.width/2f * (7f/10f), Screen.height/8f), "", backgroundStyle);
					//draw the filled in part 	
					SetStyles(fillIn, ref filledinStyle);
					GUI.BeginGroup (new Rect(0f, 0f, Screen.width/1f, Screen.height/1f));
					//				SetStyles(co_colorInform);
					GUI.Box(new Rect(Screen.width/2f * (1f + 1.5f/10f), Screen.height * (2f/5f), Screen.width/2f * (7f/10f), Screen.height/8f), s_infoLevel, filledinStyle);
					GUI.EndGroup();
					GUI.EndGroup();
				}
			}
			else
			{
				f_timerInform = 0f;
				b_informNewLevel = false;
			}
		}

//		if(b_gameFinished)
//		{
//			if(GUI.Button(new Rect(0, 0, Screen.width/6, Screen.height/8), "Return to Menu"))
//			{
//				resetGameInfo();
//				Application.LoadLevel("MenuStart");
//			}
//		}
	}
}
