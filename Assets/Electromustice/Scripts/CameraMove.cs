using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMove : MonoBehaviour {
	
	public List<Vector3> camPosArray = new List<Vector3>();
	public List<Vector3> camRotArray = new List<Vector3>();
	public List<float> intervals = new List<float>();

	public Vector3 finalPos = new Vector3 (2,2f,2f);
	public Vector3 finalRot = new Vector3 (10,204,0);

	private bool loopCam = false;

	public float timer = 0;
	private int curr = 0;

	// Use this for initialization
	void Start () {
		camPosArray.Add (new Vector3(2,2,2));
		camRotArray.Add (new Vector3(10,204,0));
		intervals.Add (15);
		camPosArray.Add (new Vector3(-2,1.4f,2.5f));
		camRotArray.Add (new Vector3(0,142,0));
		intervals.Add (15);
		camPosArray.Add (new Vector3(0,1.3f,2.6f));
		camRotArray.Add (new Vector3(0,180,0));
		intervals.Add (5);
		camPosArray.Add (new Vector3(-2,2.25f,0));
		camRotArray.Add (new Vector3(27,90,0));
		intervals.Add (5);
		camPosArray.Add (new Vector3(0,1.5f,-3.5f));
		camRotArray.Add (new Vector3(5,0,0));
		intervals.Add (5);

		transform.position = camPosArray [0];
		transform.localRotation = Quaternion.Euler (camRotArray[0]);
		curr = 0;
		timer = 0;
		loopCam = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (loopCam) {
			timer += Time.deltaTime;
			if(timer > intervals[curr]){
				// change cam rotation and position
				timer = 0;
				curr = (curr + 1) % camPosArray.Count;
				transform.position = camPosArray [curr];
				transform.localRotation = Quaternion.Euler (camRotArray[curr]);
			}
		}
	}

	// call this function after the game finished and the statistics shown
	public void setToFinal(){
		timer = 0;
		loopCam = false;
		transform.position = finalPos;
		transform.localRotation = Quaternion.Euler(finalRot);
	}

	public void beginCamLoop(){
		loopCam = true;
	}
}
