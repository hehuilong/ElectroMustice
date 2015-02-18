using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ShootManager : MonoBehaviour {

	public AvatarHP avatarHP;
	private float f_timerShootLastTime = -1;

	private GameObject go_menuClient;
	//private GameObject go_menuGame;
	
	private System.Int32 dwFlag = new int();
	private const int INTERNET_CONNECTION_MODEM = 1; 
	private const int INTERNET_CONNECTION_LAN = 2;
	[DllImport("wininet.dll")]
	private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
	
	private const string s_typeName = "MyUniqueElectromusiticeGame";
	private HostData[] hostList;

	public delegate void MenuEventHandler();
	public event MenuEventHandler MenuEvent;

	Color colorOrigin = Color.blue;
	string s_nameMenu = null;
	Collider col = null;

	private bool onMenu = false;
	
	private AudioSource audioHit;

//	private MachineManager machineManagerControlled = null;

	void Start()
	{
		go_menuClient = GameObject.Find ("MenuClient");

		EventManager.AddEventFunction(EnumEvent.OnMagnetDown, ShootFunction);

		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		//go_menuClient.SetActive (false);
		if (!InternetGetConnectedState(ref dwFlag, 0))
		{
			Debug.Log("no network!");
		}
		else if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
		{
			Debug.Log("network by modem!");
		}
		else if((dwFlag & INTERNET_CONNECTION_LAN)!=0)   
		{
			Debug.Log("network by card!");  
		}
		
		#elif UNITY_ANDROID
		//go_menuClient.SetActive (true);
		RefreshHostList();
		audioHit = GetComponent<AudioSource>();
		#elif UNITY_IPHONE
		#endif
	}

//	void Update()
//	{
//		if(Network.isClient)  //if the player has been connected to the server
//		{
//			if(machineManagerControlled != null && 
//			   machineManagerControlled.beBroken() &&
//			   machineManagerControlled.reachToFloor() &&
//			   machineManagerControlled.getIndexPlayerRepair() == NetworkManager.I_INDEX_MY_PLAYER)
//			{
//				if(machineManagerControlled.distanceBetweenBallAndMachine() <= 0.05)
//				{
//					machineManagerControlled.reset();
//				}
//				else
//				{
//					Vector3 v3_newPos = this.gameObject.transform.parent.transform.position
//										+ this.gameObject.transform.forward * 0.1f; 				//parent is the eyes the position of the eyes
//
//					machineManagerControlled.updateBallPos(v3_newPos);
//				}
//			}
//		}
//	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.name.Contains("Monster"))
		{
			avatarHP.damage(GlobalVariables.F_DAMAGE_MONSTER_TO_PLAYER);
			#if UNITY_ANDROID
			Handheld.Vibrate();
			audioHit.Play();
			#endif
		}
//		if(other.gameObject.layer == 5)  //5 is the index of the layer "UI"
//		{
//			s_nameMenu = other.gameObject.name;
//			colorOrigin = other.gameObject.GetComponent<MeshRenderer> ().material.color;
//			col = other;
//			EventManager.AddEventFunction(EnumEvent.OnMagnetDown, MenuFunction);
//			onMenu = true;
//		}

//		if(other.gameObject.name.Contains("machineBall"))
//		{
//			machineManagerControlled = other.gameObject.GetComponent<MachineManager>();
//		}
	}

	void OnTriggerExit(Collider other)
	{
//		if(other.gameObject.layer == 5)  //5 is the index of the layer "UI"
//		{
//			EventManager.RemoveEventFunction(EnumEvent.OnMagnetDown, MenuFunction);
//			other.gameObject.GetComponent<MeshRenderer> ().material.color = colorOrigin;
//			s_nameMenu = null;
//			onMenu = false;
//
//			EventManager.AddEventFunction(EnumEvent.OnMagnetDown, ShootFunction);
//		}
	}

	public void ShootFunction()
	{
		bool b_canShoot = false;

		if(f_timerShootLastTime == -1)
		{
			f_timerShootLastTime = Time.time;
			b_canShoot = true;
		}
		else
		{
			float f_timer = Time.time;

			float f_hp = avatarHP.getHp();
			float f_maxHp = avatarHP.getMaxHp();

			if(f_timer - f_timerShootLastTime > GlobalVariables.F_INTERVAL_SHOOT && f_hp > GlobalVariables.F_ENERGY_SHOOT)
			{
				b_canShoot = true;
				f_timerShootLastTime = f_timer;
			}
		}

		if(b_canShoot)
		{
			Vector3 v3_posShot = Vector3.zero;
			Quaternion quat_rotShot;

			avatarHP.damage(GlobalVariables.F_ENERGY_SHOOT);

			v3_posShot = this.gameObject.transform.position - this.gameObject.transform.up * 0.25f; // get the position of the eyes
			quat_rotShot = Quaternion.Euler(this.gameObject.transform.forward); // get the forward direction of the eyes

			NetworkManager.PlayerStatistics[NetworkManager.I_INDEX_MY_PLAYER].I_NUM_BULLET_SHOOT ++;

			if(NetworkManager.I_INDEX_MY_PLAYER == 0)
			{
				Network.Instantiate (GlobalVariables.GO_BULLET1, 
				                     v3_posShot, this.gameObject.transform.rotation, 0);
			}
			else if(NetworkManager.I_INDEX_MY_PLAYER == 1)
			{
				Network.Instantiate (GlobalVariables.GO_BULLET2, 
				                     v3_posShot, this.gameObject.transform.rotation, 0);
			}
		}

		
//		if(machineManagerControlled != null &&
//		   machineManagerControlled.beBroken() && 
//		   machineManagerControlled.reachToFloor() && 
//		   ( machineManagerControlled.getIndexPlayerRepair() == -1 || machineManagerControlled.getIndexPlayerRepair() == NetworkManager.I_INDEX_MY_PLAYER))
//		{
//			machineManagerControlled.setIndexPlayerRepair(NetworkManager.I_INDEX_MY_PLAYER);
//		}
//		else
//		{
//			machineManagerControlled = null;
//		}
	}
	
//	public void MenuFunction()
//	{
//		
//		switch (s_nameMenu) {
//		case "menuClient":
//			col.gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
//			RefreshHostList();
//			break;
//		default:
//			break;
//		}
//	}
//
//	public void menuEventFunction()
//	{
//		if(MenuEvent != null)
//		{
//			MenuEvent();
//		}
//	}

	private void RefreshHostList()
	{
		// HE huilong
		MasterServer.port = CommonVars.PORT_MASTER_SERVER;
		MasterServer.ipAddress = CommonVars.IP_SERVER;
		MasterServer.RequestHostList (s_typeName);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived)
		{
			hostList = MasterServer.PollHostList();
			JoinServer();
		}
	}

	private void JoinServer()
	{
		Network.Connect (hostList[0]);
		go_menuClient.SetActive (false);
		// remove MenuFunction from EventManager
		if (onMenu) {
			onMenu = false;
//			EventManager.RemoveEventFunction(EnumEvent.OnMagnetDown, MenuFunction);
//
//			EventManager.AddEventFunction(EnumEvent.OnMagnetDown, ShootFunction);
		}
	}
}
