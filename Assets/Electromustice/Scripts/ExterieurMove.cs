using UnityEngine;
using System.Collections;

public class ExterieurMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CallTranslateRPC(Vector3 _v3_pos){
		networkView.RPC ("TranslateRPC", RPCMode.All, _v3_pos);
	}

	[RPC]
	void TranslateRPC(Vector3 _v3_pos){
		transform.Translate(_v3_pos);
	}

}
