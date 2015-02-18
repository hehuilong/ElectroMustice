using UnityEngine;
using System.Collections;

public class EnergyBallFactory : MonoBehaviour
{
    private GameObject Exterieur;
    
    private static EnergyBallFactory _instance;
    public static EnergyBallFactory Instance
    {
        get
        {
            _instance = FindObjectOfType(typeof(EnergyBallFactory)) as EnergyBallFactory;
            if (_instance == null)
            {
                _instance = GameObject.Instantiate(GlobalVariables.GO_ENERGYBALL_FACTORY) as EnergyBallFactory;
            }

            return _instance;
        }
    }
 
	void Start(){
		// he huilong
		Exterieur = GlobalVariables.GO_EXTERIEUR;
	}

    public void spawnEnergyBalls(int _i_num, float speed)
    {
        for (int i = 0; i < _i_num; ++i)
        {
            Vector3 v3_posSpawn;
            int typeEnergyBall;
            GameObject go_energyBall;

            v3_posSpawn.x = Random.Range(-4f,4f); // spawn position is relative to exterieur
            v3_posSpawn.z = -60;
            v3_posSpawn.y = 1.25f;

            // randomly choose the type of EB to instantiate
			// HE Huilong modified
            typeEnergyBall = Random.Range(1, 5);

            // instantiate it as a child of the Exterieur gameobject
            if (typeEnergyBall == 1)
            {
				go_energyBall = (GameObject) Network.Instantiate(GlobalVariables.GO_ENERGYBALL_1, Exterieur.transform.position + v3_posSpawn, Quaternion.identity, 0);
                go_energyBall.transform.parent = Exterieur.transform;
				go_energyBall.GetComponent<EnergyBallMovement>().setSpeed(speed);
            }
            else if (typeEnergyBall == 2)
            {
				go_energyBall = (GameObject)Network.Instantiate(GlobalVariables.GO_ENERGYBALL_2,  Exterieur.transform.position + v3_posSpawn, Quaternion.identity, 0);
                go_energyBall.transform.parent = Exterieur.transform;
				go_energyBall.GetComponent<EnergyBallMovement>().setSpeed(speed);
            }
			// HE Huilong added
			else if (typeEnergyBall == 3)
			{
				go_energyBall = (GameObject)Network.Instantiate(GlobalVariables.GO_ENERGYBALL_3,  Exterieur.transform.position + v3_posSpawn, Quaternion.identity, 0);
				go_energyBall.transform.parent = Exterieur.transform;
				go_energyBall.GetComponent<EnergyBallMovement>().setSpeed(speed);
			}
			else if (typeEnergyBall == 4)
			{
				go_energyBall = (GameObject)Network.Instantiate(GlobalVariables.GO_ENERGYBALL_4,  Exterieur.transform.position + v3_posSpawn, Quaternion.identity, 0);
				go_energyBall.transform.parent = Exterieur.transform;
				go_energyBall.GetComponent<EnergyBallMovement>().setSpeed(speed);
			}
        }
    }
}
