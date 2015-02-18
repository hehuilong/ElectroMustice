using UnityEngine;
using System.Collections;


/*
 * This script is for the 2D audio clips 
 * 
 */

public enum Enum_AudioClip
{
	AC_BACKGROUND,
	AC_PLAYER_SHOOT,
	AC_PLAY_HARP,
	AC_ENERGYBALL_HIT,
	AC_ENERGYBALL_ABSORB,
	AC_MONSTER_ATTACK,
	AC_MONSTER_DESTROYED,
	AC_MACHINE_DESTRUCTED,
	AC_MACHINE_RECOVERY
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

	public static AudioClip AC_BACKGROUND;
	public AudioClip ac_background;

	public static AudioClip AC_PLAYER_SHOOT;
	public AudioClip ac_playerShoot;

	public static AudioClip AC_PLAY_HARP;
	public AudioClip ac_playHarp;

	public static AudioClip AC_ENERGYBALL_HIT;
	public AudioClip ac_energyBallHit;

	public static AudioClip AC_ENERGYBALL_ABSORB;
	public AudioClip ac_energyBallAbsorb;

	public static AudioClip AC_MONSTER_ATTACK;
	public AudioClip ac_monsterAttack;

	public static AudioClip AC_MONSTER_DESTROYED;
	public AudioClip ac_monsterDestroyed;

	public static AudioClip AC_MACHINE_DESTRUCTED;
	public AudioClip ac_machineDestructed;

	public static AudioClip AC_MACHINE_RECOVERY;
	public AudioClip ac_machineRecovery;

	private static AudioManager _instance;
	public static AudioManager Instance
	{
		get
		{
			_instance = FindObjectOfType(typeof(AudioManager)) as AudioManager;
			if(_instance == null)
			{
				_instance = GameObject.Instantiate(GlobalVariables.GO_AUDIO_MANAGER) as AudioManager;
			}
			
			return _instance;
		}
	}

	void Awake()
	{
		DontDestroyOnLoad (this.gameObject);

		AC_BACKGROUND = ac_background;
		AC_PLAYER_SHOOT = ac_playerShoot;
		AC_PLAY_HARP = ac_playHarp;
		AC_ENERGYBALL_HIT = ac_energyBallHit;
		AC_ENERGYBALL_ABSORB = ac_energyBallAbsorb;
		AC_MONSTER_ATTACK = ac_monsterAttack;
		AC_MONSTER_DESTROYED = ac_monsterDestroyed;
		AC_MACHINE_DESTRUCTED = ac_machineDestructed;
		AC_MACHINE_RECOVERY = ac_machineRecovery;
	}

	public void play2DAudioAtPoint(Enum_AudioClip _enum_audio, Vector3 _v3_pos)
	{
		switch (_enum_audio)
		{
		case Enum_AudioClip.AC_BACKGROUND:
			AudioSource.PlayClipAtPoint(AudioManager.AC_BACKGROUND, _v3_pos);
			break;
		case Enum_AudioClip.AC_ENERGYBALL_ABSORB:
			AudioSource.PlayClipAtPoint(AudioManager.AC_ENERGYBALL_ABSORB, _v3_pos);
			break;
		case Enum_AudioClip.AC_ENERGYBALL_HIT:
			AudioSource.PlayClipAtPoint(AudioManager.AC_ENERGYBALL_HIT, _v3_pos);
			break;
		case Enum_AudioClip.AC_MACHINE_DESTRUCTED:
			AudioSource.PlayClipAtPoint(AudioManager.AC_MACHINE_DESTRUCTED, _v3_pos);
			break;
		case Enum_AudioClip.AC_MACHINE_RECOVERY:
			AudioSource.PlayClipAtPoint(AudioManager.AC_MACHINE_RECOVERY, _v3_pos);
			break;
		case Enum_AudioClip.AC_MONSTER_ATTACK:
			AudioSource.PlayClipAtPoint(AudioManager.AC_MONSTER_ATTACK, _v3_pos);
			break;
		case Enum_AudioClip.AC_MONSTER_DESTROYED:
			AudioSource.PlayClipAtPoint(AudioManager.AC_MONSTER_DESTROYED, _v3_pos);
			break;
		case Enum_AudioClip.AC_PLAY_HARP:
			AudioSource.PlayClipAtPoint(AudioManager.AC_PLAY_HARP, _v3_pos);
			break;
		case Enum_AudioClip.AC_PLAYER_SHOOT:
			AudioSource.PlayClipAtPoint(AudioManager.AC_PLAYER_SHOOT, _v3_pos);
			break;
		default:
			break;
		}
	}
}
