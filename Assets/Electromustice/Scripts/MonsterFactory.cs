using UnityEngine;
using System.Collections;

public class MonsterFactory : MonoBehaviour {

	private static MonsterFactory _instance;

	public static MonsterFactory Instance
	{
		get
		{
			_instance = FindObjectOfType(typeof(MonsterFactory)) as MonsterFactory;
			if(_instance == null)
			{
				_instance = GameObject.Instantiate(GlobalVariables.GO_MONSTER_FACTORY) as MonsterFactory;
			}
			
			return _instance;
		}
	}
    public void spawnMonster1(int _i_num)
	{
		for(int i = 0; i < _i_num; ++i)
		{
			Vector3 v3_posSpawn;

			v3_posSpawn.x = GlobalVariables.F_POS_X_SPOWN_MONSTER;
			v3_posSpawn.y = Random.Range(GlobalVariables.V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER.x, GlobalVariables.V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER.y);
			v3_posSpawn.z = Random.Range(GlobalVariables.V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER.x, GlobalVariables.V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER.y);

			Network.Instantiate(GlobalVariables.GO_MONSTER_1, v3_posSpawn, Quaternion.identity, 0);

		}	
	}

	public void spawnMonster2(int _i_num)
	{
		for(int i = 0; i < _i_num; ++i)
		{
			Vector3 v3_posSpawn;
			
			v3_posSpawn.x = GlobalVariables.F_POS_X_SPOWN_MONSTER;
			v3_posSpawn.y = Random.Range(GlobalVariables.V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER.x, GlobalVariables.V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER.y);
			v3_posSpawn.z = Random.Range(GlobalVariables.V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER.x, GlobalVariables.V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER.y);
			
			Network.Instantiate(GlobalVariables.GO_MONSTER_2, v3_posSpawn, Quaternion.identity, 0);
			
		}
	}

	public void spawnMonsters(int _i_num)
	{
		for(int i = 0; i < _i_num; ++i)
		{
			Vector3 v3_posSpawn;
			int i_typeMonster;
			GameObject go_monster;

			v3_posSpawn.x = GlobalVariables.F_POS_X_SPOWN_MONSTER;
			v3_posSpawn.y = Random.Range(GlobalVariables.V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER.x, GlobalVariables.V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER.y);
			v3_posSpawn.z = Random.Range(GlobalVariables.V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER.x, GlobalVariables.V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER.y);

			i_typeMonster = Random.Range(1, 3);

			if(i_typeMonster == 1)
			{
				Network.Instantiate(GlobalVariables.GO_MONSTER_1, v3_posSpawn, Quaternion.identity, 0);
			}
			else if(i_typeMonster == 2)
			{
				Network.Instantiate(GlobalVariables.GO_MONSTER_2, v3_posSpawn, Quaternion.identity, 0);
			}
		}
	}
}
