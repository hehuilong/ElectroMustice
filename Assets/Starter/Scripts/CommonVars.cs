using UnityEngine;
using System.Collections;

public class CommonVars : MonoBehaviour {

	public static Vector3 V3_KINECT_POSITION;
	public Vector3 V3_Kinect_Position;
	public static Vector3 V3_KINECT_ROTATION;
	public Vector3 V3_Kinect_Rotation;
	/*
	public static Vector3 V3_KINECT_LOOKAT;
	public Vector3 V3_Kinect_Lookat;
	*/
	public static Matrix4x4 M_KINECT_TO_WORLD;
	public Matrix4x4 M_Kienct_To_World;
	public static string IP_SERVER = "192.168.1.75"; 
	public string Ip_Server;
	public static int PORT_MASTER_SERVER = 23466;
	public int Port_Master_Server;
	public static int PORT_SERVER = 25000;
	public int Port_Server;

	// survive when loading a new scene.
	void Awake () {
		DontDestroyOnLoad (this.gameObject);
	}

	void Update (){
		V3_Kinect_Position = V3_KINECT_POSITION;
		V3_Kinect_Rotation = V3_KINECT_ROTATION;
		//V3_Kinect_Lookat = V3_KINECT_LOOKAT;
		M_Kienct_To_World = M_KINECT_TO_WORLD;
		Ip_Server = IP_SERVER;
		Port_Master_Server = PORT_MASTER_SERVER;
		Port_Server = PORT_SERVER;
	}

}
