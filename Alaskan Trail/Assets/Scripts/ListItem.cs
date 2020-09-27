using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour {

	public RawImage icon;
	public Text itemName;
	public Text description;
	public Text amount;

	public int itemID;

	public void ListButton () {
		GameObject item = Game.instance.items[itemID].itemModel;

		if (item == null) {
			return;
		}

		Destroy(Game.instance.player.heldObject);

		GameObject newItem = (GameObject)Instantiate(item, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

		newItem.transform.SetParent(Game.instance.player.hand.transform);
		newItem.transform.localPosition = new Vector3(0, 0, 0);
		newItem.transform.localEulerAngles = new Vector3(0, 0, 0);
		newItem.transform.localScale = new Vector3(1, 1, 1);

		Game.instance.player.heldObject = newItem;
		Game.instance.player.heldItem = newItem.GetComponent<HeldItem>();

		Game.instance.player.inventory.Close();
	}
}
