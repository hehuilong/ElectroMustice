using UnityEngine;
using System.Collections;

public class Tremble : MonoBehaviour {

	private float timer = 0.4f;
	private bool go = false;
	private float speed = 1f;
	private Vector3 pos;
	private Vector3 originPos;

	// Use this for initialization
	void Start () {
		this.originPos = transform.position;
		this.pos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (go) {

			pos = transform.position;
			if(timer >= 0.3f){
				pos.x += speed * Time.deltaTime;
			}
			else if(timer >= 0.2f){
				pos.x -= speed * Time.deltaTime;
			}
			else if(timer >= 0.1f){
				pos.x += speed * Time.deltaTime;
			}
			else if(timer >= 0f){
				pos.x -= speed * Time.deltaTime;
			}
			else{
				go = false;
				timer = 0.4f;
				pos = originPos;
			}
			transform.position = pos;
			timer -= Time.deltaTime;
		}
	}


	public void tremble(){
		go = true;
	}

}
