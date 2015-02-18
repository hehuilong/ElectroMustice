/*
 * 
 * 		Developed by Peter Kinney -- 6/30/2011
 * 		Modified by HE Huilong -- 21/01/2015
 * 		This is client side player skeleton controller
 * 
 */

using UnityEngine;
using System;
using System.Collections;

public class KinectSkeletonRenderer : MonoBehaviour {
	
	public GameObject Hip_Center;
	public GameObject Spine;
	public GameObject Shoulder_Center;
	public GameObject Head;
	public GameObject Shoulder_Left;
	public GameObject Elbow_Left;
	public GameObject Wrist_Left;
	public GameObject Hand_Left;
	public GameObject Shoulder_Right;
	public GameObject Elbow_Right;
	public GameObject Wrist_Right;
	public GameObject Hand_Right;
	public GameObject Hip_Left;
	public GameObject Knee_Left;
	public GameObject Ankle_Left;
	public GameObject Foot_Left;
	public GameObject Hip_Right;
	public GameObject Knee_Right;
	public GameObject Ankle_Right;
	public GameObject Foot_Right;

	private int bonesNumber = 20;
	public float[,] skeleton = new float[20, 3];

	private GameObject[] _bones; //internal handle for the bones of the model
	//private Vector4[] _bonePos; //internal handle for the bone positions from the kinect

	// added by HE Huilong, for drawing line between bones positions
	private LineRenderer lineRenderer;
	private GameObject[] _bonesTranverse;
	private const int bonesTranverseNum = 29;

	// Use this for initialization
	void Start () {
		//store bones in a list for easier access
		_bones = new GameObject[(int)Kinect.NuiSkeletonPositionIndex.Count] {Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right};
		//_bonePos = new Vector4[(int)BoneIndex.Num_Bones];

		// added by HE Huilong, initiate Line Renderer
		lineRenderer = this.gameObject.GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount (bonesTranverseNum);
		// added by HE Huilong, this is the traversal vector for drawing the player skeleton
		_bonesTranverse = new GameObject[bonesTranverseNum] {Hand_Left, Wrist_Left, Elbow_Left, 
			Shoulder_Left, Shoulder_Center, Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Wrist_Right, Elbow_Right, Shoulder_Right, Shoulder_Center, Head,
			Shoulder_Center, Spine, Hip_Center,Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Ankle_Left, Knee_Left, Hip_Left, Hip_Center,Hip_Right, Knee_Right, Ankle_Right, Foot_Right
		};
	}
	
	// interface for changing skeleton bones postions
	public void updateSkeleton () {
		//update all of the bones positions
		for( int ii = 0; ii < bonesNumber; ii++) {
			//_bonePos[ii] = sw.getBonePos(ii);
				//_bones[ii].transform.localPosition = sw.bonePos[player,ii];
			_bones[ii].transform.localPosition = new Vector3(
				skeleton[ii,0],
				skeleton[ii,1],
				skeleton[ii,2]);
		}
		//draw lines between bones positions
		for( int ii = 0; ii < bonesTranverseNum; ii++) {
			lineRenderer.SetPosition(ii, _bonesTranverse[ii].transform.localPosition);
		}
	}

}
