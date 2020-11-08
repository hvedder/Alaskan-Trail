using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour, HeldItem {

	public int itemId;
	public int hungerPoints;

	public void LeftClick () {

		if (Game.instance.player.inventory.GetItemCount(itemId) < 1) {
			return;
		}

		Game.instance.player.AddHunger(hungerPoints);

		Game.instance.player.inventory.AddItems(itemId, -1);

		if (Game.instance.player.inventory.GetItemCount(itemId) < 1) {
			Destroy(gameObject);
		}
	}

	public void RightClick () {

	}
}
