using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

	public int type;
    public int amountRemaining;

    public int Harvest (int amount) {
    	int harvested = 0;

    	if (amountRemaining >= amount) {
    		harvested = amount;
    		amountRemaining -= amount;
    	}
    	else {
    		harvested = amountRemaining;
    		amountRemaining = 0;
    	}

    	if (amountRemaining <= 0) {
    		Destroy(gameObject);
    	}

    	return harvested;
    }
}
