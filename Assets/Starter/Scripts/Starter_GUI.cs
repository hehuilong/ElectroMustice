using UnityEngine;
using System.Collections;
// HE Huilong
public class Starter_GUI : MonoBehaviour {

	private CardboardHead head;
	Vector3 v3_rot = Vector3.zero;
	string ip = "192.168.1.75";
	int port = 25000;
	Vector3 v3_pos_kinect = Vector3.zero;
	Vector3 v3_rot_kinect = Vector3.zero;
	Vector3 v3_pos_lookat = Vector3.zero;
	string s_kinect_pos_x = "0";
	string s_kinect_pos_y = "0.65";
	string s_kinect_pos_z = "-2";
	/*
	string s_lookat_pos_x = "0";
	string s_lookat_pos_y = "0";
	string s_lookat_pos_z = "0";
	*/
	string s_kinect_rot_y = "180";
	void OnGUI(){

		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		#elif UNITY_ANDROID
		GUI.matrix = Matrix4x4.TRS(new Vector3(50f, 10f, 0), Quaternion.identity,new Vector3(2.5f, 2.5f, 1));

		GUILayout.BeginHorizontal();
		GUILayout.Label("Rotation of Cardboard: ");
		GUILayout.Label ("<"+v3_rot.y+">");
		GUILayout.EndHorizontal();
		#endif
		GUILayout.Label("Server Info:");
		GUILayout.BeginHorizontal();
		GUILayout.Label("IP");
		ip = GUILayout.TextField(ip,15);
		GUILayout.Label("port");
		string s_port = GUILayout.TextField(port.ToString(),10);
		port = int.Parse (s_port);
		GUILayout.EndHorizontal();
		#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
		GUILayout.Label("=================== Kinect Calibration =================");
		GUILayout.Label("Position:");
		GUILayout.BeginHorizontal();
		GUILayout.Label("x");
		s_kinect_pos_x = GUILayout.TextField(s_kinect_pos_x,10);
		GUILayout.Label("y");
		s_kinect_pos_y = GUILayout.TextField(s_kinect_pos_y,10);
		GUILayout.Label("z");
		s_kinect_pos_z = GUILayout.TextField(s_kinect_pos_z,10);
		GUILayout.EndHorizontal();
		/*
		GUILayout.BeginHorizontal("kinectlook");
		GUILayout.Label("Look at:");
		GUILayout.Label("x");
		s_lookat_pos_x = GUILayout.TextField(s_lookat_pos_x,10);
		GUILayout.Label("y");
		s_lookat_pos_y = GUILayout.TextField(s_lookat_pos_y,10);
		GUILayout.Label("z");
		s_lookat_pos_z = GUILayout.TextField(s_lookat_pos_z,10);
		GUILayout.EndHorizontal();
		*/
		GUILayout.Label("Rotation:");
		GUILayout.Label("y");
		s_kinect_rot_y = GUILayout.TextField(s_kinect_rot_y,10);
		#elif UNITY_ANDROID
		#endif
		GUILayout.Label("==================== Start Game =======================");
		if(GUILayout.Button ("\nStart Game\n")){
			SartGame("Electromustice");
		}
	}

	void Update(){
		v3_rot = head.transform.rotation.eulerAngles;
	}

	void SartGame(string sceneName){

		CommonVars.IP_SERVER = ip;
		CommonVars.PORT_SERVER = port;

		CalKinectInfo ();

		Application.LoadLevel(sceneName);
	}

	void Start(){
		head = Camera.main.GetComponent<StereoController>().Head;
	}

	void CalKinectInfo(){
		v3_pos_kinect.x = float.Parse (s_kinect_pos_x);
		v3_pos_kinect.y = float.Parse (s_kinect_pos_y);
		v3_pos_kinect.z = float.Parse (s_kinect_pos_z);
		/*
		v3_pos_lookat.x = float.Parse (s_lookat_pos_x);
		v3_pos_lookat.y = float.Parse (s_lookat_pos_y);
		v3_pos_lookat.z = float.Parse (s_lookat_pos_z);
		*/
		v3_rot_kinect.y = float.Parse (s_kinect_rot_y);

		CommonVars.V3_KINECT_POSITION = v3_pos_kinect;
		CommonVars.V3_KINECT_ROTATION = v3_rot_kinect;
		//CommonVars.V3_KINECT_LOOKAT = v3_pos_lookat;
		Matrix4x4 trans = new Matrix4x4();
		trans.SetTRS( v3_pos_kinect, Quaternion.identity, Vector3.one );
		Matrix4x4 rot = new Matrix4x4();
		rot.SetTRS( Vector3.zero, Quaternion.Euler(v3_rot_kinect), Vector3.one);
		Matrix4x4 flipMatrix = new Matrix4x4();
		flipMatrix.SetTRS( Vector3.zero, Quaternion.identity, new Vector3(1,1,-1));
		CommonVars.M_KINECT_TO_WORLD = trans*rot*flipMatrix;
	}
}
