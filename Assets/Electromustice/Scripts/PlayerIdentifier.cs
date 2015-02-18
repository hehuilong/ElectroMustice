using UnityEngine;
using System.Collections;

/*
 * This script is just used by the server
 * 
 */


public class PlayerIdentifier : MonoBehaviour {

	private int i_indexPlayer = -1;

	public void setIndexPlayer(int _i_index)
	{
		i_indexPlayer = _i_index;
	}

	public int getIndexPlayer()
	{
		return i_indexPlayer;
	}
}
