using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour {

	/*
	 *   just the server (PC) can control the monsters, it will then update the information of the monsters to all of the clients (cellphones)
	 * 
	 */	
	public float f_speedScale;	
	public GameObject go_FXDamage;
	public GameObject go_FXDestroy;

	private float f_health;
	private MachineManager[] machineManagers;
	private List<GameObject> list_players;
	private Vector3 v3_posDestination = Vector3.zero;
//	private int i_indexTargetMachine = -1;
//	private bool b_reachTargetMachine = false;
	private Vector3 v3_speed;
	private Vector3 v3_speedNormal;
//	private float f_distanceToMachine = -1;
	private bool b_stop = false;
	bool b_noTargetFound = true;
	private Vector3 v3_posLastTime = Vector3.zero;
	private float f_timerWait = 0;
	private bool b_inited = false;
	private bool b_destroyed = false;
	private AudioSource[] audioSources;

	void Awake()
	{
		audioSources = GetComponents<AudioSource>();

		foreach(AudioSource audio in audioSources)
		{
			if(audio.clip.name == "monstre")
			{
				if(Network.isClient)
				{
					audio.Stop();
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		if(Network.isServer)
		{
			machineManagers = GameObject.Find("machineContainer").GetComponentsInChildren<MachineManager>();
			list_players = new List<GameObject>();

			foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
			{
				if(player.transform.FindChild("03_Head") != null)
					list_players.Add(player.transform.FindChild("03_Head").gameObject);
			}

			f_health = GlobalVariables.F_HEALTH_MONSTER;
		}
	}

	// Update is called once per frame
	void Update () {

		if(Network.isServer)
		{
			if(b_noTargetFound)
			{
				if(f_timerWait < 1)  //The monster will wait for one second to choose its target
				{
					f_timerWait += Time.deltaTime;
				}
				else
				{
					int i_typeTarget = Random.Range(1, 3); //1: player, 2: machine 

					if(i_typeTarget == 1)
					{
						float f_minDistance = 1e8f;

						if(list_players != null)
						{
							foreach(GameObject player in list_players)
							{
								float f_tmpDistance;
//								if(KinectSensor.B_KINECT_IS_INITIALIZED == false)  //Prevent the collider of the player from coinciding with the collider of the floor
//								{
//									f_tmpDistance = Vector3.Distance(transform.position, player.transform.position + new Vector3(0f, 1.5f, 0f));
//								}
//								else
//								{
									f_tmpDistance = Vector3.Distance(transform.position, player.transform.position);
//								}

								if(f_tmpDistance < f_minDistance)
								{
//									if(KinectSensor.B_KINECT_IS_INITIALIZED == false)  //Prevent the collider of the player from coinciding with the collider of the floor
//									{
//										v3_posDestination = player.transform.position + new Vector3(0f, 1.5f, 0f);
//									}
//									else
//									{
										v3_posDestination = player.transform.position;
//									}

									f_minDistance = f_tmpDistance;
									b_noTargetFound = false;
								}
							}
						}

						if(b_noTargetFound)
						{
							i_typeTarget = 2;
						}
					}

					if(i_typeTarget == 2)
					{
						float f_minDistance = 1e8f;

						for(int i = 0; i < machineManagers.Length; ++i)
						{
							if(!machineManagers[i].beBroken())
							{
								float f_tmpDistance = Vector3.Distance(transform.position, 
								                                 machineManagers[i].go_machineBall.transform.position);
								if(f_tmpDistance < f_minDistance)
								{
									v3_posDestination = machineManagers[i].go_machineBall.transform.position;
									f_minDistance = f_tmpDistance;
									b_noTargetFound = false;
//									i_indexTargetMachine = i;
								}
							}
						}

						if(b_noTargetFound == true)
						{
							i_typeTarget = 1;

							if(list_players != null)
							{
								foreach(GameObject player in list_players)
								{
									float f_tmpDistance;
//									if(KinectSensor.B_KINECT_IS_INITIALIZED == false)  //Prevent the collider of the player from coinciding with the collider of the floor
//									{
//										f_tmpDistance = Vector3.Distance(transform.position, player.transform.position + new Vector3(0f, 1.5f, 0f));
//									}
//									else
//									{
										f_tmpDistance = Vector3.Distance(transform.position, player.transform.position);
//									}
									
									if(f_tmpDistance < f_minDistance)
									{
//										if(KinectSensor.B_KINECT_IS_INITIALIZED == false)  //Prevent the collider of the player from coinciding with the collider of the floor
//										{
//											v3_posDestination = player.transform.position + new Vector3(0f, 1.5f, 0f);
//										}
//										else
//										{
											v3_posDestination = player.transform.position;
//										}
										
										f_minDistance = f_tmpDistance;
										b_noTargetFound = false;
									}
								}
							}
						}
					}
				}
			}

			if(b_noTargetFound == false)
			{
				if(b_inited == false)
				{
					f_timerWait = 0;

					Vector3 v3_direction = v3_posDestination - this.transform.position;
					float f_sqrt = Mathf.Sqrt(v3_direction.x * v3_direction.x
					                          + v3_direction.y * v3_direction.y
					                          + v3_direction.z * v3_direction.z);
					
					v3_speed.x = f_speedScale * v3_direction.x / f_sqrt;
					v3_speed.y = f_speedScale * v3_direction.y / f_sqrt;
					v3_speed.z = f_speedScale * v3_direction.z / f_sqrt;
					b_inited = true;
					transform.forward = v3_direction;

					networkView.RPC("initMonsterRPC", RPCMode.Others, v3_speed, v3_direction);
				}
			}
		}

		if(b_inited)
		{
			transform.position += v3_speed * Time.deltaTime;

			if(Network.isServer && transform.position.x < GlobalVariables.F_POS_X_SPOWN_MONSTER)
			{
				//just used by server
				b_noTargetFound = true;
				b_inited = false;

				//used by server and client
				v3_posDestination = Vector3.zero;
//				i_indexTargetMachine = -1;
				v3_speed = Vector3.zero;

				networkView.RPC ("resetMonsterRPC", RPCMode.Others, transform.position);
			}
		}
	}

	[RPC]
	public void initMonsterRPC(Vector3 _v3_speed, Vector3 _v3_forward)
	{
		v3_speed = _v3_speed;
		transform.forward = _v3_forward;
		b_inited = true;
//		i_indexTargetMachine = -1;
	}

	[RPC]
	public void resetMonsterRPC(Vector3 _v3_pos)
	{
		v3_speed = Vector3.zero;
		transform.position = _v3_pos;
	}

	[RPC]
	public void playDamageFXRPC()
	{
		StartCoroutine (playDamageFXLocal());
	}

	IEnumerator playDamageFXLocal()
	{
		//		go_FXDamage.SetActive (true);
		go_FXDamage.GetComponent<ParticleSystem>().Play();

		yield return new WaitForSeconds(0.5f);

		go_FXDamage.GetComponent<ParticleSystem> ().Stop ();
	}

	[RPC]
	public void stopFXRPC()
	{
//		go_FXDamage.SetActive (false);
		go_FXDamage.GetComponent<ParticleSystem> ().Stop ();
	}

	[RPC]
	public void updateTransformRPC(Vector3 _v3_pos, Vector3 _v3_rot)
	{
		this.transform.position = _v3_pos;
		this.transform.forward = _v3_rot;
	}

	[RPC]
	public void updateMachineHp(float hp, int index)
	{
		machineManagers[index].setHealthMachine(hp);
	}

	[RPC]
	public void playDestroyFXRPC()
	{
		this.GetComponent<MeshRenderer>().renderer.enabled = false;
		go_FXDestroy.GetComponent<ParticleSystem>().Play();
	}

	IEnumerator destroyMonster()
	{
		yield return new WaitForSeconds (0.5f);

		Network.Destroy (this.gameObject);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(Network.isServer)
		{
			if(other.gameObject.name.Contains("bullet"))
			{
				if((other.name.Contains("bullet1") && this.name.Contains("Monster1"))
				   || (other.name.Contains("bullet2") && this.name.Contains("Monster2")))
				{
					BulletManager bulletManager = other.gameObject.GetComponent<BulletManager>();

					NetworkManager.PlayerStatistics[bulletManager.getIndexPlayer()].I_NUM_BULLET_DAMAGE_MONSTER++;

					f_health -= GlobalVariables.F_DAMAGE_BULLET;
					if(f_health <= 0f && b_destroyed == false)
					{
						NetworkManager.PlayerStatistics[bulletManager.getIndexPlayer()].I_NUM_MONSTER_KILLED++;
						b_destroyed = true;
						networkView.RPC("playDestroyFXRPC", RPCMode.All);

						StartCoroutine(destroyMonster());
					}
				}
			}
			else if(other.gameObject.name.Contains("machineBall"))
			{
				MachineManager machineManager = other.gameObject.GetComponent<MachineManager>();

				if(machineManager.beBroken() == false && machineManager.reachToFloor() == false) //if the machine is not broken or it isn't in the process of recovering
				{
					networkView.RPC("playDamageFXRPC", RPCMode.All);
					machineManager.destructMachine();
				}
			}
		}

		if(other.gameObject.name == "wall_left" || other.gameObject.name == "wall_right")
		{
			v3_speed.x *= -1f;

			if(v3_speed != Vector3.zero)
				transform.forward = v3_speed;
		}
		else if(other.gameObject.name == "wall_back" || other.gameObject.name == "WindowCollider")
		{
			v3_speed.z *= -1f;

			if(v3_speed != Vector3.zero)
				transform.forward = v3_speed;
		}
		else if(other.gameObject.name == "Floor" || other.gameObject.name == "Roof")
		{
			v3_speed.y *= -1f;

			if(v3_speed != Vector3.zero)
				transform.forward = v3_speed;
		}

//		if(Network.isClient)
//		{
//			if(other.gameObject.name == "03_Head")
//			{
//				AvatarHP avatarHP = other.gameObject.GetComponent<AvatarHP>();
//
//				if(avatarHP != null)  //the player is yourself
//				{
//					avatarHP.damage(GlobalVariables.F_DAMAGE_MONSTER_TO_PLAYER);
//				}
//			}
//		}
	}
//	
//	void OnTriggerExit(Collider other)
//	{
//		if(Network.isServer)
//		{
//			if(other.name.Contains("Monster"))
//			{
//				b_stop = false;
//			}
//		}
//	}
}
