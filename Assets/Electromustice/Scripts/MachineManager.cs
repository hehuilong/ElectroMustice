using UnityEngine;
using System.Collections;

public class MachineManager : MonoBehaviour {

	public GameObject go_machineBall;
	public GameObject go_machinePart;
	public GameObject go_chordControlled;
	public float f_recoverySpeed;

	private float f_health;
	private bool b_broken = false;
	private bool b_reachToFloor = false;
	private int i_indexPlayerRepair = -1;
	private Vector3 v3_posInitMachineBall = Vector3.zero;

	public int getIndexPlayerRepair()
	{
		return i_indexPlayerRepair;
	}

	public void setIndexPlayerRepair(int _i_index)
	{
		i_indexPlayerRepair = _i_index;

		networkView.RPC ("setIndexPlayerRepairRPC", RPCMode.Others, _i_index);
	}

	[RPC]
	public void setIndexPlayerRepairRPC(int _i_index)
	{
		i_indexPlayerRepair = _i_index;
	}

	public bool beBroken()
	{
		return b_broken;
	}

	public bool reachToFloor()
	{
		return b_reachToFloor;
	}

//	public void breakThisMachine(bool _b_bo)
//	{
//		b_broken = _b_bo;
//	}

	public float getHealthMachine()
	{
		return f_health;
	}

	public void destructMachine()
	{
        // breaking sound
        GetComponent<AudioSource>().Play();
        // string disappears
        go_chordControlled.GetComponents<AudioSource>()[0].Play();
		networkView.RPC ("destructMachineRPC", RPCMode.All);
	}

	[RPC]
	public void destructMachineRPC()
	{
		destructMachineLocal ();
	}

	public void destructMachineLocal()
	{
		f_health = -1;
		b_broken = true;

		go_chordControlled.transform.Find ("Particle System").gameObject.SetActive (false);
		go_chordControlled.GetComponent<EnergyBallString>().active = false;
		//			go_chordControlled.SetActive(false);

		go_machineBall.GetComponent<Rigidbody>().isKinematic = false;
		go_machineBall.GetComponent<Rigidbody>().useGravity = true;
	}

	public void damageMachine(float _f_damage)
	{
		setHealthMachine (f_health - _f_damage);
	}

	public void setHealthMachine(float _f_health)
	{
		f_health = _f_health;
		if (f_health <= 0) {
			b_broken = true;
		}
	}

	public float distanceBetweenBallAndMachine()
	{
		return Vector3.Distance (go_machinePart.transform.position, go_machineBall.transform.position);
	}

	public void reset()
	{
		go_machineBall.transform.position = v3_posInitMachineBall;
		i_indexPlayerRepair = -1;
		b_broken = false;
		b_reachToFloor = false;
		f_health = GlobalVariables.F_HEALTH_MACHINE;
		go_chordControlled.transform.Find ("Particle System").gameObject.SetActive (true);

		networkView.RPC ("resetRPC", RPCMode.Others);
	}

	[RPC]
	public void resetRPC()
	{
		go_machineBall.transform.position = v3_posInitMachineBall;
		i_indexPlayerRepair = -1;
		b_broken = false;
		b_reachToFloor = false;
		f_health = GlobalVariables.F_HEALTH_MACHINE;
		go_chordControlled.transform.Find ("Particle System").gameObject.SetActive (true);
		go_chordControlled.GetComponent<EnergyBallString>().active = true;
	}

	public void updateBallPos(Vector3 _v3_newPos)
	{
		go_machineBall.transform.position = _v3_newPos;

		networkView.RPC ("updateBallPosRPC", RPCMode.Others, _v3_newPos);
	}

	[RPC]
	public void updateBallPosRPC(Vector3 _v3_newPos)
	{
		go_machineBall.transform.position = _v3_newPos;
	}

	// Use this for initialization
	void Start () {
		f_health = GlobalVariables.F_HEALTH_MACHINE;
		v3_posInitMachineBall = go_machineBall.transform.position;
	}

	void Update()
	{
		if(!b_broken)
		{
			if(b_reachToFloor)  //the machine is recovering to its origin position
			{
				go_machineBall.transform.position += new Vector3(0, f_recoverySpeed * Time.deltaTime, 0);

				if(distanceBetweenBallAndMachine() < 0.25f)
				{
					if(Network.isServer)
					{
						reset();
					}
				}
			}
		}
		else 
		{
			if(!b_reachToFloor)
			{
				if(go_machineBall.transform.localPosition.y != 0)
				{
					if(go_machineBall.GetComponent<Rigidbody>().velocity.y == 0)
					{
						go_machineBall.GetComponent<Rigidbody>().isKinematic = true;
						go_machineBall.GetComponent<Rigidbody>().useGravity = false;
						b_reachToFloor = true;
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(Network.isServer)
		{
			if(other.gameObject.name.Contains("bullet"))
			{
				if(beBroken())
				{
					BulletManager bulletManager = other.gameObject.GetComponent<BulletManager>();

					NetworkManager.PlayerStatistics[bulletManager.getIndexPlayer()].I_NUM_MACHINE_REPAIR++;

					networkView.RPC("repairMachineRPC", RPCMode.All);
				}
			}
		}
	}

	[RPC]
	public void repairMachineRPC()
	{
		b_broken = false;
	}
}
