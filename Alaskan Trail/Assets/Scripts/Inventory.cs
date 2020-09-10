using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	public Dictionary<int,int> items = new Dictionary<int,int>();

	private List<GameObject> menuItems = new List<GameObject>();

    // Start is called before the first frame update
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        if (!Game.instance.inventoryMenu.activeSelf) {
            return;
        }

        if (menuItems.Count < 6) {
            return;
        }
        
        Game.instance.itemSet.anchoredPosition += new Vector2 (0, Input.mouseScrollDelta.y * -3.0f);

        if (Game.instance.itemSet.anchoredPosition.y < 0) {
            Game.instance.itemSet.anchoredPosition = new Vector2(0, 0);
        }

        float maxY = 50 + (130 * (menuItems.Count - 6));

        if (Game.instance.itemSet.anchoredPosition.y > maxY) {
            Game.instance.itemSet.anchoredPosition = new Vector2(0, maxY);
        }
    }

    // Adds items to the player's inventory based on the given item ID and amount
    public void AddItems (int itemID, int amount) {
    	if (items.ContainsKey(itemID)) {
    		int currentAmount = items[itemID];
    		currentAmount += amount;

    		if (currentAmount > 999) {
    			currentAmount = 999;
    		}

    		items.Remove(itemID);
    		items.Add(itemID, currentAmount);
    	}
    	else {
    		items.Add(itemID, amount);
    	}
    }

    // Opens the player's inventory
    public void Open () {
    	Game.instance.inventoryMenu.SetActive(true);

    	float yPos = 300;

    	foreach (KeyValuePair<int, int> item in items) {
    		GameObject newItem = (GameObject)Instantiate(Game.instance.listItem.gameObject, Vector3.zero, new Quaternion(0, 0, 0, 0));
    		newItem.SetActive(true);
    		newItem.transform.SetParent(Game.instance.listItem.transform.parent);

    		ListItem newListItem = newItem.GetComponent<ListItem>();
    		InventoryItem currentItem = Game.instance.items[item.Key];

    		newListItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yPos);
    		newListItem.icon.texture = currentItem.inventoryImage.texture;
    		newListItem.itemName.text = currentItem.itemName;
    		newListItem.amount.text = $"{item.Value}";
    		newListItem.description.text = currentItem.description;

    		menuItems.Add(newItem);

    		yPos -= 130;
    	}
    }

    // Closes the player's inventory
    public void Close () {
    	Game.instance.inventoryMenu.SetActive(false);

    	foreach (GameObject g in menuItems) {
    		Destroy(g);
    	}

    	menuItems.Clear();
    }
}

[System.Serializable]
public class InventoryItem {

	public string itemName;
	public int itemID;
	public string description;

	public Sprite inventoryImage;
	public GameObject itemModel;
}