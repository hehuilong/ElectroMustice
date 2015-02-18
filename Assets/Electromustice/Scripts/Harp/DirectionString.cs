using UnityEngine;
using System.Collections;

public class DirectionString : MonoBehaviour
{
    private float g;
    private AudioSource _movementSound;
    public Vector3 Direction;
    private GameObject Exterieur;
    public float Speed = 8f;
    // Use this for initialization
    void Start()
    {
        g = this.renderer.material.color.g;

		// he Huilong 
		Exterieur = GlobalVariables.GO_EXTERIEUR;

        if (Direction.x > 0)
            _movementSound = GameObject.Find("wall_right").GetComponent<AudioSource>();
        else if (Direction.x < 0)
            _movementSound = GameObject.Find("wall_left").GetComponent<AudioSource>();
    }
	#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
    void OnTriggerEnter(Collider coll)
    {
		if(coll.gameObject.tag == "Player" || coll.gameObject.name == "TestCube" && !_movementSound.isPlaying)
		{
            _movementSound.Play();
		}
	}

    void OnTriggerStay(Collider coll)
    {
		if(coll.gameObject.tag == "Player" || coll.gameObject.name == "TestCube")
		{
			bool b_canMove = false;
			if(gameObject.name == "directionLeft")
			{
				if(Exterieur.GetComponent<ExterieurMove> ().transform.position.x >= -4f)
				{
					b_canMove = true;
				}
			}
			else if(gameObject.name == "directionRight")
			{
				if(Exterieur.GetComponent<ExterieurMove> ().transform.position.x <= 4f)
				{
					b_canMove = true;
				}
			}
			if(b_canMove)
			{
				Exterieur.GetComponent<ExterieurMove> ().CallTranslateRPC (Direction * Speed * Time.deltaTime);
			}
			g += 0.5f * Time.deltaTime;
		}
    }
	#elif UNITY_ANDROID
	#elif UNITY_IPHONE
	#endif
}
