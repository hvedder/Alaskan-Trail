using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour {

	public int itemID;

	public void PickUp () {
		Game.instance.player.inventory.AddItems(itemID, 1);
      Game.instance.player.SetActionButton(ActionButtons.None, null);

		Destroy(gameObject);
	}
   	
   	void OnTriggerEnter (Collider other) {
   		if (other.tag != "Player") {
   			return;
   		}

   		Game.instance.player.SetActionButton(ActionButtons.PickUp, gameObject);
   	}

   	void OnTriggerExit (Collider other) {
   		if (other.tag != "Player") {
   			return;
   		}

   		if (Game.instance.player.actionObject == gameObject) {
   			Game.instance.player.SetActionButton(ActionButtons.None, null);
   		}
   	}
}
