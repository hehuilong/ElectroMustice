using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable] // serializable for transfering on network
public class PlayerInfo{
	// Attention: if not set to empty string, the playerID will not be instantiated
	// even in case of playerList[i] = new PlayerInfo();
	public string playerID = "";
	public bool active = false;
	// head rotation info (by cardboard) : euler angles
	public float rx;
	public float ry;
	public float rz;
	// kinect skeleton position info
	public float[,] skeleton = new float[20, 3];
};

[Serializable] // serializable for transfering on network
public class GameStatistics
{
	public int I_INDEX_PLAYER;
	public int I_NUM_BULLET_DAMAGE_MONSTER;
	public int I_NUM_MONSTER_KILLED;
	public int I_NUM_MACHINE_REPAIR;
	public int I_NUM_ENERGYBALL_ABSORB;
	public int I_NUM_ENERGYBALL_HIT;
	public int I_NUM_BULLET_SHOOT;
	public bool B_UPDATE_FINISH;
}

public class NetworkManager : MonoBehaviour {

	public static GameStatistics[] PlayerStatistics;

	public GameObject ServerCamera;
	
	private GameManager gameManager; // siqun

	private System.Int32 dwFlag = new int();
	private const int INTERNET_CONNECTION_MODEM = 1; 
	private const int INTERNET_CONNECTION_LAN = 2;
	[DllImport("wininet.dll")]
	private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);


	private int i_maxNumOfClients = 2;   // maximum clients number
	private const string s_typeName = "MyUniqueElectromusiticeGame";
	private string s_gameName = "DefaultRoomName";
	private bool b_serverStarted = false;
	private bool b_isServer = false;
	public static int I_INDEX_MY_PLAYER = -1;    //just for the clients : the index of the value of the array iArray_idPlayer of my own player
	public static int I_NUM_CLIENTS = 0;

	private GameObject[] go_playersArray; // player avatars on server or client
	
	private bool b_startGame = false; // siqun
	
	private PlayerInfo[] playersInfo;
	private GameObject clientSidePlayer;

	// HE Huilong
	private CardboardHead head;

	private GameObject kinect;
	private int skeletonId = 0; // server side id for identify kinect skeleton
	public float kinectScale = 1f; // server side parameter for kinect skeleton scale

	public Vector3 v3_speed = new Vector3(1, 1, 1);

	
	// server side control of camera , added by he Huilong
	public float camSpeed = 1.0f;
	public float sensitivityX = 5.0f;
	public float sensitivityY = 5.0f;

	void Awake()
	{
		playersInfo = new PlayerInfo[i_maxNumOfClients];
		go_playersArray = new GameObject[i_maxNumOfClients];

		for (int i=0; i<playersInfo.Length; i++) {
			playersInfo[i] = new PlayerInfo();
		}

		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		Application.runInBackground = true;

		// initialize kinect prefab

		kinect = GameObject.Instantiate (
			GlobalVariables.GO_KINECT_PREFAB, Vector3.zero, Quaternion.identity) as GameObject;

		// to be modified
		//ServerCamera.SetActive(true);

		b_isServer = true;
		b_serverStarted = false;

		gameManager = GameManager.Instance;

		I_NUM_CLIENTS = 0;
		#elif UNITY_ANDROID
		b_isServer = false;

		// to be modified
		ServerCamera.SetActive (false);
	
//		if(KinectSensor.B_KINECT_IS_INITIALIZED == false)   //Prevent the collider of the player from coinciding with the collider of the floor
//		{
//			clientSidePlayer = GameObject.Instantiate (
//				GlobalVariables.GO_PLAYER_ME, new Vector3(0f,1.5f,0f), Quaternion.identity) as GameObject;
//		}
//		else
//		{
			clientSidePlayer = GameObject.Instantiate (
				GlobalVariables.GO_PLAYER_ME, new Vector3(0f,0f,0f), Quaternion.identity) as GameObject;
//		}

		// cardboard
		// HE huilong
		head = Camera.main.GetComponent<StereoController>().Head;
		
		#elif UNITY_IPHONE
		#endif

		PlayerStatistics = new GameStatistics[i_maxNumOfClients];

		for(int i = 0; i < PlayerStatistics.Length; ++i)
		{
			PlayerStatistics[i] = new GameStatistics();
			PlayerStatistics[i].I_INDEX_PLAYER = i;
			PlayerStatistics[i].I_NUM_BULLET_DAMAGE_MONSTER = 0;
			PlayerStatistics[i].I_NUM_BULLET_SHOOT = 0;
			PlayerStatistics[i].I_NUM_ENERGYBALL_ABSORB = 0;
			PlayerStatistics[i].I_NUM_ENERGYBALL_HIT = 0;
			PlayerStatistics[i].I_NUM_MACHINE_REPAIR = 0;
			PlayerStatistics[i].I_NUM_MONSTER_KILLED = 0;
			PlayerStatistics[i].B_UPDATE_FINISH = false;
		}
	}

	// Use this for initialization
	void Start () {
		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		createRoom(GlobalVariables.F_SIZE_ROOM);
		// debug 
		Debug.Log ("server address and port: "+CommonVars.IP_SERVER+" :"+CommonVars.PORT_SERVER);

		#endif
	}

	public void createRoom (float _f_size_room)
	{
		GameObject go_room = GameObject.Find ("room_v2");
		go_room.transform.localScale *= _f_size_room;

		ParticleSystem[] particles_harp = go_room.GetComponentsInChildren<ParticleSystem>();

		foreach(ParticleSystem particle in particles_harp)
		{
			particle.startLifetime *= _f_size_room; 
		}
	}

	private void StartServer()
	{
		/* Launch MasterServer */
		//			string path = Application.dataPath;
		//			Debug.Log(path);
		//			System.Diagnostics.Process.Start(path + @"\MasterServer\MasterServer.exe");
		
		/* Bind to it */
		MasterServer.ipAddress = CommonVars.IP_SERVER;
		MasterServer.port = CommonVars.PORT_MASTER_SERVER;
		Network.InitializeServer (i_maxNumOfClients, CommonVars.PORT_SERVER, false);
		//Network.InitializeServer (i_maxNumOfClients, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost (s_typeName, s_gameName);

		Debug.Log ("server initialised");
	}

	// client side code
	void OnConnectedToServer(){
		// Send client's player info to server
		networkView.RPC("serverInsertIntoPlayerList", RPCMode.Server, Network.player.ToString());
		// set the player avatar's base position to zero
		//clientSidePlayer.transform.position = Vector3.zero;
		//clientSidePlayer.transform.Find ("03_Head").gameObject.transform.position = new Vector3 (0, 1.5f, 0);
	}
	
	// server side code. rpc called by the client
	[RPC]
	void serverInsertIntoPlayerList(string np){

		I_NUM_CLIENTS ++;

		int maxPlayer = i_maxNumOfClients;
		for (int i=0; i<maxPlayer; i++) {
			if(!playersInfo[i].active){
				playersInfo[i].playerID = np;
				playersInfo[i].active = true;
				// init player avatar
				go_playersArray[i] = GameObject.Instantiate(GlobalVariables.GO_PLAYER_COMPLETE, Vector3.zero, Quaternion.identity) as GameObject;
				go_playersArray[i].GetComponent<PlayerIdentifier>().setIndexPlayer(i);

				go_playersArray[i].GetComponent<KinectPointControllerWithLine>().sw = kinect.GetComponent<SkeletonWrapper>();
				go_playersArray[i].GetComponent<KinectPointControllerWithLine>().player = skeletonId;
				go_playersArray[i].GetComponent<KinectPointControllerWithLine>().scale = kinectScale;
				skeletonId++;
				break;
			}
		}
		// inform the clients the new player list
		// Attention: don't do this in OnPlayerConnected.
		// Because when client connected, its info may not arrived at the server yet.
		sendPlayerListToClients ();
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		for(int i = 0; i < playersInfo.Length; ++i)
		{
			if(playersInfo[i].playerID.Equals(player.ToString(),StringComparison.Ordinal))
			{
				playersInfo[i].playerID 	= "";
				playersInfo[i].active = false;
				if(go_playersArray[i] != null)
				{
					Destroy(go_playersArray[i]);
					go_playersArray[i] = null;
				}

				// inform the clients the new player list
				sendPlayerListToClients ();

				break;
			}
		}
	}

	void OnGUI()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		if(b_serverStarted == false && b_isServer)
		{
			if (GUI.Button(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2), "StartServer"))
			{
				StartServer();
				b_serverStarted = true;
			}
		}
		if(Network.isServer && !b_startGame && b_serverStarted)
        {
            if (GUI.Button(new Rect(Screen.width/8, Screen.height/8, Screen.width/4, Screen.height/8), "StartGame"))
            {
                gameManager.startGame();
                b_startGame = true;
				// huilong, begin camera auto movement
				ServerCamera.GetComponent<CameraMove>().beginCamLoop();
            }
        }
		#elif UNITY_ANDROID
		#elif UNITY_IPHONE
		#endif
	}

	void Update()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		// inform clients the change of player avatar skeletons
		if(b_serverStarted){
			for(int i=0; i<playersInfo.Length; i++){
				if(playersInfo[i].active){
					playersInfo[i].skeleton = go_playersArray[i].GetComponent<KinectPointControllerWithLine>().getSkeletonArray();
					//Debug.Log("Server: Head position sent: "+playersInfo[i].skeleton[3,0]+", "+playersInfo[i].skeleton[3,1]+", "+playersInfo[i].skeleton[3,2] );
				}
			}
			sendPlayerListToClients ();
		}
		// server side camera control
//		if(Input.GetKey(KeyCode.UpArrow))
//		{
//			ServerCamera.transform.position += camSpeed*Time.deltaTime*ServerCamera.transform.forward;
//		}
//		else if(Input.GetKey(KeyCode.DownArrow))
//		{
//			ServerCamera.transform.position -= camSpeed*Time.deltaTime*ServerCamera.transform.forward;
//		}
//		else if(Input.GetKey(KeyCode.LeftArrow))
//		{
//			ServerCamera.transform.position -= camSpeed*Time.deltaTime*ServerCamera.transform.right;
//		}
//		else if(Input.GetKey(KeyCode.RightArrow))
//		{
//			ServerCamera.transform.position += camSpeed*Time.deltaTime*ServerCamera.transform.right;
//		}
//
//		Vector3 tmpRot =  new Vector3(0, 0, 0);
//		tmpRot.y = Input.GetAxis("Mouse X") * sensitivityX;
//		tmpRot.x = Input.GetAxis("Mouse Y") * sensitivityY;
//
//		tmpRot.y += ServerCamera.transform.localEulerAngles.y;
//		tmpRot.x =	ServerCamera.transform.localEulerAngles.x - tmpRot.x;
//
//		ServerCamera.transform.localEulerAngles = tmpRot;

		#elif UNITY_ANDROID
		UpdateRotation();
		#elif UNITY_IPHONE
		#endif
	}

	// client
	private void UpdateRotation()
	{
		if(Network.isClient)
		{
			Vector3 v3_rot = head.transform.rotation.eulerAngles;

			// HE Huilong
			if(Mathf.Abs( playersInfo[I_INDEX_MY_PLAYER].ry-v3_rot.y) > 3f
			   ||Mathf.Abs( playersInfo[I_INDEX_MY_PLAYER].rx-v3_rot.x) > 3f
			   ||Mathf.Abs( playersInfo[I_INDEX_MY_PLAYER].rz-v3_rot.z) > 3f){
				networkView.RPC ("server_requestUpdateRotationRPC", RPCMode.Server, v3_rot, I_INDEX_MY_PLAYER);
			}
		}
	}

	// server
	[RPC]
	public void server_requestUpdateRotationRPC(Vector3 _v3_rot, int _i_indexPlayer){
		if(_i_indexPlayer != -1){
			playersInfo [_i_indexPlayer].rx = _v3_rot.x;
			playersInfo [_i_indexPlayer].ry = _v3_rot.y;
			playersInfo [_i_indexPlayer].rz = _v3_rot.z;
			
			if(go_playersArray[_i_indexPlayer]!= null)
			{
				go_playersArray[_i_indexPlayer].GetComponent<KinectPointControllerWithLine>().Head.transform.eulerAngles = _v3_rot;
			}
		}
	}

	// server side code
	void sendPlayerListToClients(){
		BinaryFormatter binFormatter = new BinaryFormatter ();
		MemoryStream memStream = new MemoryStream (); // Stream whose backing store is memory. Defined in namespace System.IO
		binFormatter.Serialize (memStream,playersInfo);
		byte[] serializedPl = memStream.ToArray (); // Convert the serialized object to byte array
		memStream.Close ();
		networkView.RPC ("clientRefreshPlayerList",RPCMode.Others,serializedPl);
	}
	
	// client side code: rpc called by the server
	[RPC]
	void clientRefreshPlayerList(byte[] serializedPlayerList){
		BinaryFormatter binFormatter = new BinaryFormatter ();
		MemoryStream memStream = new MemoryStream (); 
		// Write the byte data we received into the stream 
		// The second parameter specifies the offset to the beginning of the stream
		// The third parameter specifies the maximum number of bytes to be written
		memStream.Write(serializedPlayerList,0,serializedPlayerList.Length); 
		// Stream internal "reader" is now at the last position
		// Shift it back to the beginning for reading
		memStream.Seek(0, SeekOrigin.Begin); 
		playersInfo = (PlayerInfo[])binFormatter.Deserialize (memStream);

		for (int i=0; i<playersInfo.Length; i++) {

			// update active player's skeleton and head rotation
			if(go_playersArray[i] != null && playersInfo[i].active){
				// skeleton
				go_playersArray[i].GetComponent<KinectSkeletonRenderer>().skeleton = playersInfo[i].skeleton;
				go_playersArray[i].GetComponent<KinectSkeletonRenderer>().updateSkeleton();
				// head rotation
				Vector3 vRot = new Vector3(playersInfo [i].rx, playersInfo [i].ry, playersInfo [i].rz);
				go_playersArray[i].GetComponent<KinectSkeletonRenderer>().Head.transform.eulerAngles = vRot;
			}

			if(I_INDEX_MY_PLAYER==-1 && playersInfo[i].playerID.Equals(Network.player.ToString(),StringComparison.Ordinal)){
				// allocation of player id
				I_INDEX_MY_PLAYER = i;
				go_playersArray[i] = clientSidePlayer;
			}
			else if(playersInfo[i].active && go_playersArray[i] == null){
				// initialize avatar of new incoming player
				go_playersArray[i] = 
					GameObject.Instantiate(GlobalVariables.GO_PLAYER_OTHER, 
					                       Vector3.zero,
					                       Quaternion.identity) as GameObject;
			}
			else if(!playersInfo[i].active && go_playersArray[i] != null){
				// other player disconnected
				Destroy(go_playersArray[i]);
				go_playersArray[i] = null;
			}
		}
	}
}













