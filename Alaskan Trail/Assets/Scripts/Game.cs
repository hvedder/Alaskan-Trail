using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public static Game instance;

    public int chunkRadius;

	public InventoryItem[] items;

    public PlayerController player;
    public MapGenerator mapGenerator;

    public bool mapGenerated;

    public Sled sled;
    public Dog[] dogs;

    public GameObject water;
    public GameObject waterEffect;
    public GameObject pointer;

	[Header("Menus")]
	public GameObject inventoryMenu;
	public ListItem listItem;
    public RectTransform itemSet;
    public Text actionText1;
    public Text actionText2;

    public PlayerStatus playerStatus;

    [Header("Prefabs")]
    public GameObject rope;
    public GameObject sledPrefab;
    public GameObject dogPrefab;

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

    public static void SpawnObject (GameObject spawnObject, Vector3 pos) {
        RaycastHit hit;
        bool foundGround = Physics.Raycast(new Vector3(pos.x, 500, pos.z), new Vector3(0, -1, 0), out hit, 500);

        if (foundGround) {
            spawnObject.transform.position = new Vector3(pos.x, hit.point.y + 2, pos.z);
        }
        else {
            spawnObject.transform.position = new Vector3(pos.x, 500, pos.z);
        }
    }

    public static Vector3 Point (GameObject source, GameObject destination) {
        instance.pointer.transform.position = source.transform.position;
        instance.pointer.transform.LookAt(destination.transform);

        return instance.pointer.transform.eulerAngles;
    }

    public static void ToggleWater () {
        instance.waterEffect.SetActive(!instance.waterEffect.activeSelf);
        instance.water.SetActive(!instance.water.activeSelf);
    }
}


















