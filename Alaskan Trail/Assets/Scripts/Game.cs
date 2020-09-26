using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game instance;

    public int chunkRadius;

	public InventoryItem[] items;

    public PlayerController player;
    public MapGenerator mapGenerator;

	// Menus
	public GameObject inventoryMenu;
	public ListItem listItem;
    public RectTransform itemSet;

    // Start is called before the first frame update
    void Start () {
        if (instance != null && instance != this) {
        	Destroy(this.gameObject);
        }
        else {
        	instance = this;
        }

        Time.fixedDeltaTime = 0.02f;
        Time.maximumDeltaTime = 0.02f;
    }

    public static void UpdateChunks (Vector2Int mapPos) {
        for (int x = mapPos.x - instance.chunkRadius - 1; x <= mapPos.x + instance.chunkRadius + 1; x++) {
            for (int z = mapPos.y - instance.chunkRadius - 1; z <= mapPos.y + instance.chunkRadius + 1; z++) {
                if (x < 0 || x >= instance.mapGenerator.mapPlanes.GetLength(0)) {
                    continue;
                }
                else if (z < 0 || z >= instance.mapGenerator.mapPlanes.GetLength(1)) {
                    continue;
                }

                if (x < mapPos.x - instance.chunkRadius || x > mapPos.x + instance.chunkRadius || z < mapPos.y - instance.chunkRadius || z > mapPos.y + instance.chunkRadius) {
                    instance.mapGenerator.mapPlanes[x, z].SetActive(false);
                }
                else {
                    instance.mapGenerator.mapPlanes[x, z].SetActive(true);
                }
            }
        }
    }
}


















