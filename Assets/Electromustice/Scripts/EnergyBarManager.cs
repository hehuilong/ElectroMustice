using UnityEngine;
using System.Collections;

public class EnergyBarManager : MonoBehaviour {

	public GameObject go_energyBar;
	public GameObject go_energyBarContainer;
	private float f_maxNumEnergy = -1;

	private float f_currentNumEnergy = 0;
	private float f_lengthAdded = 0;  // The length added of the energy bar each time obtaining a energy ball
	private Vector3 v3_localPosStart = Vector3.zero;

	float f_timer = 0;

	public float getNumEnergy()
	{
		return f_currentNumEnergy;
	}

	// Use this for initialization
	void Start () {
		f_maxNumEnergy = GlobalVariables.F_MAX_NUM_ENERGY;

		v3_localPosStart = new Vector3 (go_energyBarContainer.transform.localPosition.x - go_energyBarContainer.transform.localScale.x * 0.5f,
		                                go_energyBarContainer.transform.localPosition.y, 
		                                go_energyBarContainer.transform.localPosition.z);
		f_lengthAdded = (float)(go_energyBarContainer.transform.localScale.x / (float)f_maxNumEnergy);

		resetEnergyBar ();
	}

	public void addEnergy(float _f_numAdded)
	{
		networkView.RPC ("addEnergyRPC", RPCMode.All, _f_numAdded);
	}

	[RPC]
	public void addEnergyRPC(float _f_numAdded)
	{
		if(f_currentNumEnergy == 0 && _f_numAdded > 0)
		{
			go_energyBar.SetActive(true);
		}


		if(f_currentNumEnergy + _f_numAdded >= f_maxNumEnergy)
		{
			f_currentNumEnergy = f_maxNumEnergy;
		}
		else
		{
			f_currentNumEnergy += _f_numAdded;
		}

		if(f_currentNumEnergy > 0 && go_energyBar.activeSelf)
		{
			go_energyBar.transform.localScale = new Vector3 (f_currentNumEnergy * f_lengthAdded, 
			                                                 go_energyBar.transform.localScale.y,
			                                                 go_energyBar.transform.localScale.z);

			correctPosEnergyBar ();
		}
	}

	public void reduceEnergy(float _f_numReduced)
	{
		networkView.RPC ("reduceEnergyRPC", RPCMode.All, _f_numReduced);
	}

	[RPC]
	public void reduceEnergyRPC(float _f_numReduced)
	{
		if(_f_numReduced >= 0)
		{
			if(f_currentNumEnergy - _f_numReduced > 0)
			{
				f_currentNumEnergy -= _f_numReduced;
			}
			else
			{
				resetEnergyBar();
			}
		}
		
		if(f_currentNumEnergy > 0 && go_energyBar.activeSelf)
		{
			go_energyBar.transform.localScale = new Vector3 (f_currentNumEnergy * f_lengthAdded, 
			                                                 go_energyBar.transform.localScale.y,
			                                                 go_energyBar.transform.localScale.z);
			
			correctPosEnergyBar ();
		}
	}

	public void resetEnergyBar()
	{
		go_energyBar.transform.localScale = new Vector3 (f_lengthAdded * 1.00f, 
		                                                 go_energyBar.transform.localScale.y,
		                                                 go_energyBar.transform.localScale.z);
		f_currentNumEnergy = 0;
		correctPosEnergyBar ();
		go_energyBar.SetActive (false);
	}

	public void correctPosEnergyBar()
	{
		go_energyBar.transform.localPosition = 
			new Vector3 (v3_localPosStart.x + go_energyBar.transform.localScale.x * 0.5f,
			             v3_localPosStart.y, v3_localPosStart.z);
	}

//	// Update is called once per frame
//	void Update () {
//		f_timer += Time.deltaTime;
//
//		if(f_timer > 1f)
//		{
//			addEnergy(1);
//			f_timer = 0;
//		}
//	}
}
