using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour {
    
	void Start ()
    {
        Destroy(this.gameObject, .1f);
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Building>() != null)
        {
            other.GetComponent<Building>().HitBuilding();
        }
        else if (other.GetComponent<CivilianController>() != null)
        {
            other.GetComponent<CivilianController>().Hit();
        }
    }
}
