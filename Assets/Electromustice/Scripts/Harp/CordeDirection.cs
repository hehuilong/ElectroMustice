using UnityEngine;
using System.Collections;

public class CordeDirection : MonoBehaviour
{

    private Color col;
    private float g;
    public Vector3 Direction;
    public GameObject Exterieur;
    private float Speed = 10f;
    // Use this for initialization
    void Start()
    {
        col = this.renderer.material.color;
        g = this.renderer.material.color.g;
    }

    void OnTriggerEnter(Collider coll)
    {
        Debug.Log("touché");
    }

    void OnTriggerStay(Collider coll)
    {
        Exterieur.transform.Translate(Direction * Speed * Time.deltaTime);
        g += 0.5f * Time.deltaTime;
        this.renderer.material.color = new Color(this.renderer.material.color.r, g, this.renderer.material.color.b);
    }

    void OnTriggerExit(Collider coll)
    {
        this.renderer.material.color = col;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
