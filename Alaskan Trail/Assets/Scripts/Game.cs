using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	public static Game instance;

    public int chunkRadius;

	public InventoryItem[] items;

    public PlayerController player;
    public MapGenerator mapGenerator;

    public bool mapGenerated;

    public Sled sled;
    public Dog[] dogs;

    public AudioSource[] sounds;
    private float[] pitches;

    public GameObject water;
    public GameObject waterEffect;
    public GameObject pointer;

    private float growthTime;
    private float spawnTime;

    public int bearCount;

	[Header("Menus")]
	public GameObject inventoryMenu;
	public ListItem listItem;
    public RectTransform itemSet;
    public Text actionText1;
    public Text actionText2;

    public PlayerStatus playerStatus;

    public GameObject damageMenu;
    public GameObject healthMeter;
    public Text healthText;
    private float damageTime;

    public GameObject deathScreen;

    [Header("Prefabs")]
    public GameObject rope;
    public GameObject sledPrefab;
    public GameObject dogPrefab;
    public GameObject bearPrefab;

    [Header("Object Groups")]
    public GameObject trees;

    public List<GameObject> pooledTrees = new List<GameObject>();
    public List<GameObject> pooledBushes = new List<GameObject>();

    public List<GameObject> activeBushes = new List<GameObject>();

    [Header("Materials")]
    public Material damageMaterial;

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

        growthTime = 60.0f;
        spawnTime = 5.0f;

        pitches = new float[sounds.Length];

        for (int i = 0; i < sounds.Length; i++) {
            pitches[i] = sounds[i].pitch;
        }
    }

    void Update () {

        if (deathScreen.activeSelf) {

            if (Input.GetKeyDown("enter") || Input.GetKeyDown("return")) {
                SceneManager.LoadScene("MainMenu");
            }

            return;
        }

        growthTime -= Time.deltaTime;
        spawnTime -= Time.deltaTime;

        if (damageTime > 0) {
            damageTime -= Time.deltaTime;

            if (damageTime <= 0) {
                damageTime = 0;
                damageMenu.SetActive(false);
            }
        }

        // if (growthTime <= 0 && activeBushes.Count > 0) {

        //     bool[] addedBush = new bool[activeBushes.Count];

        //     for (int i = 0; i < Mathf.Ceil(activeBushes.Count / 8.0f); i++) {

        //         int bushIndex = Random.Range(0, activeBushes.Count);

        //         if (activeBushes[bushIndex].transform.Find("Berries") == null || addedBush[bushIndex]) {
        //             continue;
        //         }

        //         addedBush[bushIndex] = true;

        //         GameObject berries = (GameObject)Instantiate(mapGenerator.bushBerries, activeBushes[bushIndex].transform.position, new Quaternion(0, 0, 0, 0));
        //         berries.transform.SetParent(activeBushes[bushIndex].transform);
        //         berries.transform.localPosition = Vector3.zero;
        //         berries.name = "Berries";

        //         Debug.Log("Added Berries");
        //     }

        //     growthTime = 5.0f;
        // }

        if (spawnTime <= 0 && bearCount < 5) {

            Vector3 spawnPos = new Vector3(Random.Range(150, 300), 100, Random.Range(150, 300));

            if (Random.Range(0, 2) == 1) {
                spawnPos = new Vector3(spawnPos.x * -1, spawnPos.y, spawnPos.z);
            }
            if (Random.Range(0, 2) == 1) {
                spawnPos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z * -1);
            }

            spawnPos += player.transform.position;

            GameObject newBear = (GameObject)Instantiate(bearPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

            SpawnObject(newBear, spawnPos);

            bearCount++;
            spawnTime = 5.0f;
        }
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
                    // instance.mapGenerator.mapPlanes[x, z].SetActive(false);
                    instance.DeactivateChunk(x, z);
                }
                else {
                    // instance.mapGenerator.mapPlanes[x, z].SetActive(true);
                    instance.ActivateChunk(x, z);
                }
            }
        }
    }

    public void ActivateChunk (int x, int z) {
        if (mapGenerator.mapPlanes[x, z].activeSelf) {
            return;
        }

        mapGenerator.mapPlanes[x, z].SetActive(true);

        // Debug.Log(pooledTrees.Count);

        foreach (Vector4 treePos in mapGenerator.treeSpots[x, z]) {
            switch ((int)treePos.w) {
                case 0:
                    PlaceTree(treePos, x, z);
                break;

                case 1:
                    PlaceBush(treePos, x, z);
                break;
            }
        }
    }

    private void PlaceTree (Vector3 pos, int x, int z) {
        if (pooledTrees.Count < 1) {
            return;
        }

        pooledTrees[0].SetActive(true);
        pooledTrees[0].transform.position = new Vector3(pos.x, pos.y, pos.z);
        pooledTrees[0].transform.SetParent(mapGenerator.mapPlanes[x, z].transform);

        pooledTrees.Remove(pooledTrees[0]);
    }

    private void PlaceBush (Vector3 pos, int x, int z) {
        if (pooledBushes.Count < 1) {
            return;
        }

        pooledBushes[0].SetActive(true);
        pooledBushes[0].transform.position = new Vector3(pos.x, pos.y, pos.z);
        pooledBushes[0].transform.SetParent(mapGenerator.mapPlanes[x, z].transform);

        GameObject berries = (GameObject)Instantiate(mapGenerator.bushBerries, new Vector3(pos.x, pos.y, pos.z), new Quaternion(0, 0, 0, 0));
        berries.transform.SetParent(pooledBushes[0].transform);
        berries.transform.localPosition = Vector3.zero;
        berries.name = "Berries";

        activeBushes.Add(pooledBushes[0]);
        pooledBushes.Remove(pooledBushes[0]);
    }

    public void DeactivateChunk (int x, int z) {
        if (!mapGenerator.mapPlanes[x, z].activeSelf) {
            return;
        }

        List<Transform> tempList = new List<Transform>();

        foreach (Transform child in mapGenerator.mapPlanes[x, z].transform) {
            tempList.Add(child);
        }

        foreach (Transform child in tempList) {
            switch (child.tag) {
                case "Tree":
                    pooledTrees.Add(child.gameObject);
                    child.SetParent(trees.transform);
                    child.gameObject.SetActive(false);
                break;

                case "Bush":
                    Transform berries = child.Find("Berries");

                    if (berries != null) {
                        Destroy(berries.gameObject);
                    }

                    activeBushes.Remove(child.gameObject);
                    pooledBushes.Add(child.gameObject);
                    child.SetParent(trees.transform);
                    child.gameObject.SetActive(false);
                break;
            }
        }

        mapGenerator.mapPlanes[x, z].SetActive(false);
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

    public static void Sound (int number, bool randomize) {

        instance.sounds[number].pitch = instance.pitches[number];

        if (randomize) {
            instance.sounds[number].pitch += Random.Range(-0.1f, 0.1f);
        }

        instance.sounds[number].Play();
    }

    public static Vector3 Point (GameObject source, GameObject destination) {
        instance.pointer.transform.position = source.transform.position;
        instance.pointer.transform.LookAt(destination.transform);

        return instance.pointer.transform.eulerAngles;
    }

    public void DamageScreen () {
        healthMeter.transform.localScale = new Vector3(player.health / (player.maxHealth * 1.0f), 1, 1);
        healthText.text = Mathf.Floor(player.health + 0.5f) + " / " + player.maxHealth;

        damageMenu.SetActive(true);

        damageTime = 0.75f;
    }

    public void DeathScreen () {
        deathScreen.SetActive(true);
    }

    public static void ToggleWater () {
        instance.waterEffect.SetActive(!instance.waterEffect.activeSelf);
        instance.water.SetActive(!instance.water.activeSelf);
    }
}


















