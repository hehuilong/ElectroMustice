using UnityEngine;
using System.Collections;

public class EnergyBallMovement : MonoBehaviour {

    private float _speed = 0;
    private Vector3 _direction;
    private EnergyBallString[] _strings;
    private GameObject _window;

	// huilong
	public bool absorded = false;

	// huilong
	private EnergyBarManager go_energyBarManager;

//	private bool b_isPlayed = false;
	private bool b_canBeAbsorbed = false;

    public int EnergyBallType;

	void Start () {
		// huilong
		this.gameObject.transform.parent = GlobalVariables.GO_EXTERIEUR.transform;
		//_speed = 6;//Random.Range(5, 10);
        _direction = new Vector3(0, 0, 1f);
        _strings = new EnergyBallString[4];
        _strings[0] = GameObject.Find("string1").GetComponent<EnergyBallString>();
        _strings[1] = GameObject.Find("string2").GetComponent<EnergyBallString>();
        _strings[2] = GameObject.Find("string3").GetComponent<EnergyBallString>();
        _strings[3] = GameObject.Find("string4").GetComponent<EnergyBallString>();
		// get energy bar manager  and room, huilong
		go_energyBarManager = GameObject.Find("EnergyBarManager").GetComponent<EnergyBarManager>();
		// set particale system , huilong
		this.gameObject.particleSystem.Play();
		this.gameObject.particleSystem.startSpeed = 2;

        _window = GameObject.Find("WindowCollider");
	}

//	public bool isPlayed()
//	{
//		return b_isPlayed; 
//	}

	public bool canBeAbsorbed()
	{
		return b_canBeAbsorbed;
	}

	public void destroyBall()
	{
		if(Network.isServer)
		{
    		Network.Destroy(this.gameObject);
		}
	}

    // We can't destroy the ball here because some other scripts need the information of the ball
    void OnTriggerEnter(Collider other)
    {
		if(other.name.Contains("EnergyBallDetector"))
		{
			b_canBeAbsorbed = true;
			this.gameObject.particleSystem.startSpeed = 8;
		}

        // Energy Balls are destroyed if they hit the window, unless the proper
        // harp string is being played         
		if (other.name.Contains("WindowCollider"))
		{
			//other.gameObject.renderer.material.SetColor("_Color", Color.red);
			if(!absorded){
				this.gameObject.renderer.enabled = false;
				b_canBeAbsorbed = false;
				GlobalVariables.GO_ROOM.GetComponent<Tremble>().tremble();
				if(Network.isServer)
				{
					//play explosion sound
					_window.GetComponents<AudioSource>()[1].Play();

					for(int i = 0; i < NetworkManager.PlayerStatistics.Length; ++i)
					{
						NetworkManager.PlayerStatistics[i].I_NUM_ENERGYBALL_HIT ++;
					}

					go_energyBarManager.reduceEnergy(0.5f);

					StartCoroutine(SelfDestroy(0.3f));
				}
			}
        }
    }

	IEnumerator SelfDestroy(float waitTime){
		yield return new WaitForSeconds (waitTime);
		if(Network.isServer){
			Network.Destroy(this.gameObject);
		}
	}
	
	void Absorption()
    {
        // play absorption sound
        _window.GetComponents<AudioSource>()[0].Play();
		// add energy , huilong
		go_energyBarManager.addEnergy (1);
		networkView.RPC("effectFeforeAbsorbRPC", RPCMode.All);
		StartCoroutine(SelfDestroy(0.5f));
	}

	[RPC]
	public void effectFeforeAbsorbRPC()
	{
		absorded = true;
		effectBeforeAbsorb ();
	}

	public void effectBeforeAbsorb(){
		this.gameObject.renderer.enabled = false;
		this.gameObject.particleSystem.startSpeed = 100;
	}

	public void setSpeed(float speed){
		networkView.RPC("setSpeedRPC", RPCMode.All, speed);
	}

	[RPC]
	private void setSpeedRPC(float speed){
		this._speed = speed;
	}

	void Update () {
        // Energy Balls go in a straight line from their spawn point
        transform.Translate(_direction * _speed * Time.deltaTime);
		if(Network.isServer){
			// Checking if the corresponding string is playing            
			if (!absorded && b_canBeAbsorbed && _strings[EnergyBallType - 1].active && _strings[EnergyBallType - 1].IsPlaying)
			{
				int i_indexPlayerPlaying = _strings[EnergyBallType - 1].getIndexPlayerPlaying();
				if(i_indexPlayerPlaying != -1){
					NetworkManager.PlayerStatistics[i_indexPlayerPlaying].I_NUM_ENERGYBALL_ABSORB ++;
				}
				Absorption();
				//b_isPlayed = true;
			}
			// if they get past the position of the window, they can't be collected
	        // anymore and are destroyed
			if (transform.position.z > 1f) {
				Network.Destroy(this.gameObject);
			}
		}
	}
}
