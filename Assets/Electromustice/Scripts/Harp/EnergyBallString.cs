using UnityEngine;
using System.Collections;

public class EnergyBallString : MonoBehaviour {
	// HE Huilong
	public bool active = true;

	private int i_indexPlayerPlaying = -1;

	public int getIndexPlayerPlaying()
	{
		return i_indexPlayerPlaying;
	}

	public void setIndexPlayerPlaying(int _i_index)
	{
		i_indexPlayerPlaying = _i_index;
	}

    private bool _isPlaying;
    public bool IsPlaying
    {
        get
        {
            return _isPlaying;
        }
    }

	private AudioSource _note;

	void Start () {
        _isPlaying = false;
		_note = GetComponents<AudioSource>()[1];
	}

    void OnTriggerEnter(Collider coll)
    {
		if(coll.gameObject.tag == "Player" || coll.gameObject.name == "TestCube" && active)
		{
       		_isPlaying = true;
			#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
			if(coll.gameObject.transform.parent != null)
			{
				PlayerIdentifier playerIdentifier = coll.gameObject.transform.parent.GetComponent<PlayerIdentifier>();
				i_indexPlayerPlaying = playerIdentifier.getIndexPlayer();
			}
			_note.Play();
			#endif
		}
    }

    void OnTriggerExit(Collider coll)
    {
		if(coll.gameObject.tag == "Player" || coll.gameObject.name == "TestCube")
		{
        	_isPlaying = false;
			_note.Stop();
		}
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
