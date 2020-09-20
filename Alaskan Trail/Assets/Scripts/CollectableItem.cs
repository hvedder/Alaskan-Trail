using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour {

	public int itemID;
   	
   	void OnTriggerEnter (Collider other) {

   		// Debug.Log("Stick Trigger");

   		if (other.tag != "Player") {
   			return;
   		}

   		other.attachedRigidbody.GetComponent<PlayerController>().inventory.AddItems(itemID, 1);

   		Destroy(gameObject);
   	}
}
