using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVariables : MonoBehaviour {

	public static GameObject GO_PLAYER_ME; 
	public GameObject go_playerMe;

	public static GameObject GO_PLAYER_OTHER; 
	public GameObject go_playerOther;
	
	public static GameObject GO_EXTERIEUR; 
	public GameObject go_exterieur;

	public static GameObject GO_ROOM; 
	public GameObject go_room;

	public static GameObject GO_PLAYER_COMPLETE;
	public GameObject go_playerComplete;

	public static MonsterFactory GO_MONSTER_FACTORY;
	public MonsterFactory go_monsterFactory;

	public static GameObject GO_MONSTER_1;
	public GameObject go_monster1;

	public static GameObject GO_MONSTER_2;
	public GameObject go_monster2;

	public static GameObject GO_MONSTER_1_EMPTY;
	public GameObject go_monster1Empty;

	public static GameObject GO_MONSTER_2_EMPTY;
	public GameObject go_monster2Empty;

	public static GameObject GO_BULLET1;
	public GameObject go_bullet1;

	public static GameObject GO_BULLET2;
	public GameObject go_bullet2;

	public static GameManager GO_GAME_MANAGER;
	public GameManager go_gameManager;

	public static GameObject GO_AUDIO_MANAGER;
	public GameObject go_audioManager;

	// loic
	public static EnergyBallFactory GO_ENERGYBALL_FACTORY;
	public EnergyBallFactory go_energyBallFactory;
	
	public static GameObject GO_ENERGYBALL_1;
	public GameObject go_energyBall1;
	
	public static GameObject GO_ENERGYBALL_2;
	public GameObject go_energyBall2;
	
	public static GameObject GO_ENERGYBALL_3;
	public GameObject go_energyBall3;
	
	public static GameObject GO_ENERGYBALL_4;
	public GameObject go_energyBall4;
	
	// HE Huilong, kinect related
	public static GameObject GO_KINECT_PREFAB; 
	public GameObject go_kinect_prefab;

	public static float F_SIZE_ROOM;
	public float f_sizeRoom;

//	public static float F_SIZE_INIT_ROOM;
//	public float f_sizeInitRoom;

//	public static float F_HEIGHT_INIT_ROOM;
//	public float f_heightInitRoom;

	public static float F_SIZE_INIT_MONSTER;
	public float f_sizeInitMonster;

	public static float F_HEALTH_MACHINE;
	public float f_healthMachine;

	public static float F_DAMAGE_MONSTER_TO_MACHINE;
	public float f_damageMonsterToMachine;

	public static float F_DAMAGE_MONSTER_TO_PLAYER;
	public float f_damageMonsterToPlayer;
	
	public static float F_DAMAGE_BULLET;
	public float f_damageBullet;

	public static float F_POS_X_SPOWN_MONSTER;
	public float f_posXSpownMonster;

	public static Vector2 V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER;
	public Vector2 v2_rangePosZAxisSpownMonster;

	public static Vector2 V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER;
	public Vector2 v2_rangePosYAxisSpownMonster;

	public static TextAsset TEXT_LEVELS;
	public TextAsset text_levels;

	// huilong
	public static float PLAYER_HP;
	public float playerHp;

	public static float F_INTERVAL_SHOOT;
	public float f_intervalShoot;

	public static float F_HEALTH_MONSTER;
	public float f_healthMonster;

	public static float F_ENERGY_SPEED_RECOVERY;
	public float f_energySpeedRecovery;

	public static float F_ENERGY_SHOOT;
	public float f_energyShoot;

	public static float F_MAX_NUM_ENERGY;
	public float f_maxNumEnergy;

	// Use this for initialization
	void Awake () {
		GO_PLAYER_ME = go_playerMe;
		GO_PLAYER_OTHER = go_playerOther;
		GO_PLAYER_COMPLETE = go_playerComplete;
		GO_MONSTER_FACTORY = go_monsterFactory;
		GO_MONSTER_1 = go_monster1;
		GO_MONSTER_2 = go_monster2;
		GO_MONSTER_1_EMPTY = go_monster1Empty;
		GO_MONSTER_2_EMPTY = go_monster2Empty;
		GO_BULLET1 = go_bullet1;
		GO_BULLET2 = go_bullet2;
		GO_GAME_MANAGER = go_gameManager;
		GO_AUDIO_MANAGER = go_audioManager;

		F_SIZE_ROOM = f_sizeRoom;
//		F_SIZE_INIT_ROOM = f_sizeInitRoom;
//		F_HEIGHT_INIT_ROOM = f_heightInitRoom;
		// corrected by He Huilong
		F_SIZE_INIT_MONSTER = f_sizeInitMonster;
		F_HEALTH_MACHINE = f_healthMachine;
		F_DAMAGE_MONSTER_TO_MACHINE = f_damageMonsterToMachine;
		F_DAMAGE_MONSTER_TO_PLAYER = f_damageMonsterToPlayer;
		F_DAMAGE_BULLET = f_damageBullet;
		
		// HE Huilong, kinect related
		GO_KINECT_PREFAB = go_kinect_prefab;

		// loic energy balls
		GO_ENERGYBALL_FACTORY = go_energyBallFactory;
		GO_ENERGYBALL_1 = go_energyBall1;
        GO_ENERGYBALL_2 = go_energyBall2;
		// HE huilong added
		GO_ENERGYBALL_3 = go_energyBall3;
		GO_ENERGYBALL_4 = go_energyBall4;

		// HE Huilong exterieur container
		GO_EXTERIEUR = go_exterieur;
		// huilong 03/02/2015
		GO_ROOM = go_room;
		
		F_POS_X_SPOWN_MONSTER = f_posXSpownMonster;
		V2_RANGE_POS_Z_AXIS_SPOWN_MONSTER = v2_rangePosZAxisSpownMonster;
		V2_RANGE_POS_Y_AXIS_SPOWN_MONSTER = v2_rangePosYAxisSpownMonster;

		TEXT_LEVELS = text_levels;
		
		// huilong
		PLAYER_HP = playerHp;
		F_INTERVAL_SHOOT = f_intervalShoot;

		F_HEALTH_MONSTER = f_healthMonster;
		F_ENERGY_SPEED_RECOVERY = f_energySpeedRecovery;
		F_ENERGY_SHOOT = f_energyShoot;

		F_MAX_NUM_ENERGY = f_maxNumEnergy;
	}
}
