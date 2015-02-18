using UnityEngine;
using System.Collections;

public class BulletManager : MonoBehaviour {

	public float f_speedScale;

	private Vector3 v3_direction = Vector3.zero;
	private bool b_setSpeed = false;
	private Vector3 v3_speed = Vector3.zero;
	private float f_damage = 0;
	private int i_indexMyPlayer = -1;

	public int getIndexPlayer()
	{
		return i_indexMyPlayer;
	}

	// Use this for initialization
	void Start () {
		if(networkView.isMine)  
		{
			setSpeed (this.transform.forward);

			i_indexMyPlayer = NetworkManager.I_INDEX_MY_PLAYER;
			networkView.RPC("server_setIndexPlayerRPC", RPCMode.Server, i_indexMyPlayer); //just need inform the server, other client don't need know
		}
	}

	[RPC]
	public void server_setIndexPlayerRPC(int _i_index)
	{
		i_indexMyPlayer = _i_index;
	}

	// Update is called once per frame
	void Update () {
		if(networkView.isMine && b_setSpeed == true)
		{
			this.transform.position += v3_speed * Time.deltaTime;

			if(transform.position.x < -5f || transform.position.x > 5f || 
			   transform.position.y < -5f || transform.position.y > 5f ||
			   transform.position.z < -5f || transform.position.z > 5f)
			{
				Network.Destroy(this.gameObject);
			}
			else
			{
				networkView.RPC ("updateBulletPositionRPC", RPCMode.Others, this.transform.position);
			}
		}
	}

	public void setSpeed(Vector3 _v3_dir)
	{
		v3_direction = _v3_dir;

		float f_sqrt = Mathf.Sqrt (v3_direction.x * v3_direction.x 
		                           + v3_direction.y * v3_direction.y
		                           + v3_direction.z * v3_direction.z);

		v3_speed.x = f_speedScale * v3_direction.x / f_sqrt;
		v3_speed.y = f_speedScale * v3_direction.y / f_sqrt;
		v3_speed.z = f_speedScale * v3_direction.z / f_sqrt;

		b_setSpeed = true;
	}

	[RPC]
	public void updateBulletPositionRPC(Vector3 _v3_newPos)
	{
		this.transform.position = _v3_newPos;
	}
}
















