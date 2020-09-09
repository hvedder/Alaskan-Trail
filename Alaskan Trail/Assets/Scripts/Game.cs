using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game instance;

	public InventoryItem[] items;

	// Menus
	public GameObject inventoryMenu;
	public ListItem listItem;

    // Start is called before the first frame update
    void Start () {
        if (instance != null && instance != this) {
        	Destroy(this.gameObject);
        }
        else {
        	instance = this;
        }
    }

    // Update is called once per frame
    void Update () {
        
    }
}
