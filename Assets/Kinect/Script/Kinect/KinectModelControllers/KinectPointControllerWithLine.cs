/*
 * KinectModelControllerWithLine.cs - Moves every 'bone' given to match
 * 				the position of the corresponding bone given by
 * 				the kinect. And draw lines between them to show the skeleton
 * 
 * 		Developed by Peter Kinney -- 6/30/2011
 * 		Modified by HE Huilong -- 21/01/2015
 * 
 */

using UnityEngine;
using System;
using System.Collections;

public class KinectPointControllerWithLine : MonoBehaviour {
	
	public SkeletonWrapper sw;
	
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
	
	private GameObject[] _bones; //internal handle for the bones of the model
	//private Vector4[] _bonePos; //internal handle for the bone positions from the kinect
	
	public int player = -1;
	
	public float scale = 1.0f;

	// added by HE Huilong, for drawing line between bones positions
	private LineRenderer lineRenderer;
	private GameObject[] _bonesTranverse;
	private const int bonesTranverseNum = 29;

	// added by HE Huilong, for converting sw positions into array
	private float[,] skeleton = new float[20, 3];

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
	
	// Update is called once per frame
	void Update () {
		if(player == -1)
			return;
		//update all of the bones positions
		if (sw.pollSkeleton())
		{
			for( int ii = 0; ii < (int)Kinect.NuiSkeletonPositionIndex.Count; ii++) {
				//_bonePos[ii] = sw.getBonePos(ii);
					//_bones[ii].transform.localPosition = sw.bonePos[player,ii];
				_bones[ii].transform.localPosition = new Vector3(
					// huilong changed to raw bone pos
					sw.rawBonePos[player,ii].x * scale,
					sw.rawBonePos[player,ii].y * scale,
					sw.rawBonePos[player,ii].z * scale);
				// convert to world space
				_bones[ii].transform.localPosition = CommonVars.M_KINECT_TO_WORLD.MultiplyPoint3x4(_bones[ii].transform.localPosition);
				// added by HE Huilong
				skeleton[ii,0] = _bones[ii].transform.localPosition.x;
				skeleton[ii,1] = _bones[ii].transform.localPosition.y;
				skeleton[ii,2] = _bones[ii].transform.localPosition.z;
			}
		}
		//draw lines between bones positions
		for( int ii = 0; ii < bonesTranverseNum; ii++) {
			lineRenderer.SetPosition(ii, _bonesTranverse[ii].transform.localPosition);
		}
	}

	// get skeleton positions array
	public float[,] getSkeletonArray(){
		return this.skeleton;
	}
}
