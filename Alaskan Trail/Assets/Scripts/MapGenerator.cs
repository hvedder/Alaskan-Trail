using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public float heightScale;
    public float planeScale;

	public GameObject plane;
    public GameObject terrain;
    public GameObject tree;
    public GameObject bush;
    public GameObject bushBerries;

	public GameObject[,] mapPlanes;
    public List<Vector4>[,] treeSpots;

    private Color32[] whites;
    private Color32[] browns;
    private Color32[] greens;
    private Color32[] blues;

    // Start is called before the first frame update
    void Start () {
        InitializeColors();
        GenerateMap(30, 30, 11, 11, 25);

        Game.instance.mapGenerated = true;
    }

    public void SetVertexHeights (GameObject vPlane, float[,] heightMap) {
    	int sizeX = heightMap.GetLength(0);
    	int sizeZ = heightMap.GetLength(1);
    	int vertex = 0;

    	MeshFilter meshFilter = vPlane.GetComponent<MeshFilter>();
    	Vector3[] vertices = meshFilter.mesh.vertices;

    	for (int x = 0; x < sizeX; x++) {
    		for (int z = 0; z < sizeZ; z++) {
    			float height = heightMap[x, z];
                float randomX = 0;
                float randomY = 0;
                float randomZ = 0;

                if (x > 0 && x < sizeX - 1 && z > 0 && z < sizeZ - 1 && height > 0.7f) {
                    randomX = Random.Range(-0.25f, 0.25f);
                    // randomY = Random.Range(-0.15f, 0.15f);
                    randomZ = Random.Range(-0.25f, 0.25f);
                }

    			vertices[vertex] = new Vector3(vertices[vertex].x + randomX, (height * heightScale) + randomY, vertices[vertex].z + randomZ);
    			vertex++;
    		}
    	}

    	meshFilter.mesh.vertices = vertices;
    	// meshFilter.mesh.RecalculateBounds();
    	// meshFilter.mesh.RecalculateNormals();
    	vPlane.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
    }

    public void GenerateMap (int amountX, int amountZ, int sizeX, int sizeZ, float scale) {

    	mapPlanes = new GameObject[amountX, amountZ];
        treeSpots = new List<Vector4>[amountX, amountZ];

    	Wave[] waves = GenerateWaves(10);

    	for (int x = 0; x < amountX; x++) {
    		for (int z = 0; z < amountZ; z++) {
                treeSpots[x, z] = new List<Vector4>();

    			mapPlanes[x, z] = (GameObject)Instantiate(plane, Vector3.zero, new Quaternion(0, 0, 0, 0));
                mapPlanes[x, z].transform.localScale = new Vector3(planeScale, planeScale / 2.0f, planeScale);

    			float[,] heightMap = GenerateHeightMap(sizeX, sizeZ, scale, 10 * x, 10 * z, waves);
    			// Texture2D planeTexture = GenerateTexture(heightMap);

    			mapPlanes[x, z].transform.position += new Vector3(10 * x * planeScale, 0, 10 * z * planeScale);
    			// mapPlanes[x, z].GetComponent<MeshRenderer>().material.mainTexture = planeTexture;
    			SetVertexHeights(mapPlanes[x, z], heightMap);

                mapPlanes[x, z].transform.SetParent(terrain.transform);
                // mapPlanes[x, z].SetActive(false);

                Mesh mesh = mapPlanes[x, z].GetComponent<MeshFilter>().mesh;

                SplitMesh(mesh);
                ColorTriangles(mesh);

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                // mapPlanes[x, z].GetComponent<MeshCollider>().sharedMesh = mesh;
    		}
    	}

        for (int i = 0; i < 20; i++) {
            int xPos = Random.Range(0, amountX);
            int zPos = Random.Range(0, amountZ);

            GameObject tempPlane = mapPlanes[xPos, zPos];

            GenerateTrees(new Vector3(tempPlane.transform.position.x, 60, tempPlane.transform.position.z), 350, 10);   
        }

        InstantiateTrees();

        for (int x = 0; x < amountX; x++) {
            for (int z = 0; z < amountZ; z++) {
                mapPlanes[x, z].SetActive(false);
            }
        }
    }

    public float[,] GenerateHeightMap (int sizeX, int sizeZ, float scale, float offsetX, float offsetZ, Wave[] waves) {

    	float[,] heightMap = new float[sizeX, sizeZ];

    	for (int x = 0; x < sizeX; x++) {
    		for (int z = 0; z < sizeZ; z++) {
    			float sampleX = (x - offsetZ) / scale;
    			float sampleZ = (z - offsetX) / scale;

    			// float noise = Mathf.PerlinNoise(sampleX, sampleZ);

    			float noise = 0.0f;
    			float normalization = 0.0f;

    			foreach (Wave wave in waves) {
    				noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
    				normalization += wave.amplitude;
    			}

    			noise /= normalization;
    			noise *= 0.95f;

                noise *= 2;

    			if (noise > 1.1f) {
    				noise *= 1.5f;
    			}
                else if (noise > 1.0f) {
                    noise *= 1.35f;
                }
                else if (noise > 0.92f) {
                    noise *= 1.2f;
                }
    			else if (noise > 0.9f) {
    				noise *= 1.1f;
    			}

                if (noise < 0.7f) {
                    noise = 0.675f;
                }

    			heightMap[x, z] = noise;
    		}
    	}

    	return heightMap;
    }

    private Texture2D GenerateTexture (float[,] heightMap) {
    	int textureX = heightMap.GetLength(0);
    	int textureZ = heightMap.GetLength(1);

    	Texture2D newTexture = new Texture2D(textureX, textureZ);

    	for (int x = 0; x < textureX; x++) {
    		for (int z = 0; z < textureZ; z++) {
                float randomDarkness = Random.Range(0, 0.1f);

    			Color pixelColor = new Color(1 - randomDarkness, 1 - randomDarkness, 1 - randomDarkness, 1);
    			
                if (heightMap[x, z] < 0.7f) {
                    pixelColor = new Color(0.7f, 0.9f, 0.9f, 1);
                }
    			else if (heightMap[x, z] < 0.85f) {
    				pixelColor = new Color(0, 0.6f + Random.Range(-0.1f, 0.1f), 0.1f, 1);
    			}
				else if (heightMap[x, z] < 0.95f) {
					pixelColor = new Color(0.35f, 0.25f, 0.125f, 1);
    			}

    			newTexture.SetPixel(z, x, pixelColor);
    		}
    	}

    	newTexture.wrapMode = TextureWrapMode.Clamp;
    	newTexture.Apply();

    	return newTexture;
    }

    private void SplitMesh (Mesh mesh) {
        Vector3[] newVerts = new Vector3[mesh.triangles.Length];
        int[] newTriangles = new int[mesh.triangles.Length];

        for (int i = 0; i < mesh.triangles.Length; i++) {
            newVerts[i] = mesh.vertices[mesh.triangles[i]];
            // newNorms[i] = mesh.normals[mesh.triangles[i]];
            newTriangles[i] = i;
        }

        // mesh.Clear();
        mesh.vertices = newVerts;
        mesh.triangles = newTriangles;
        // mesh.normals = newNorms;

        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        // meshCollider.sharedMesh = mesh;
    }

    private void ColorTriangles (Mesh mesh) {
        Color32[] colors = new Color32[mesh.vertices.Length];
        // Color lightBlue = new Color(0.7f, 0.9f, 0.9f, 1);
        // Color brown = new Color(0.35f, 0.25f, 0.125f, 1);
        // Color green = new Color(0, 0.6f, 0.1f, 1);
        // Color white = new Color(1, 1, 1, 1);

        for (int i = 0; i < mesh.triangles.Length / 3; i++) {
            float averageHeight = (mesh.vertices[i * 3].y + mesh.vertices[i * 3 + 1].y + mesh.vertices[i * 3 + 2].y) / 3.0f;
            averageHeight = averageHeight / heightScale;
            // float randomDarkness = Random.Range(0, 0.15f);

            // Color color = white;
                
            // if (averageHeight < 0.7f) {
            //     color = lightBlue;
            // }
            // else if (averageHeight < 0.85f) {
            //     color = green;
            // }
            // else if (averageHeight < 0.95f) {
            //    color = brown;
            // }

            // color = new Color(color.r - randomDarkness, color.g - randomDarkness, color.b - randomDarkness, 1);

            int randomIndex = Random.Range(0, whites.Length);

            Color32 color = whites[randomIndex];

            if (averageHeight < 0.7f) {
                color = blues[randomIndex];
            }
            else if (averageHeight < 0.85f) {
                color = greens[randomIndex];
            }
            else if (averageHeight < 0.95f) {
               color = browns[randomIndex];
            }

            colors[i * 3] = color;
            colors[i * 3 + 1] = color;
            colors[i * 3 + 2] = color;
        }

        mesh.colors32 = colors;
    }

    private void GenerateTrees (Vector3 pos, float radius, int density) {
        Collider[] colliders = Physics.OverlapSphere(pos, radius);

        foreach (Collider col in colliders) {
            if (col.tag != "Ground") {
                continue;
            }

            Vector3[] vertices = col.GetComponent<MeshFilter>().mesh.vertices;
            List<Vector3> usedVerts = new List<Vector3>();

            for (int i = 0; i < density; i++) {
                int index = Random.Range(0, vertices.Length);

                if (vertices[index].y / heightScale < 0.7f) {
                    continue;
                }
                if (usedVerts.Contains(vertices[index])) {
                    continue;
                }

                usedVerts.Add(vertices[index]);

                Vector3 treePos = col.transform.position + new Vector3(vertices[index].x * planeScale, vertices[index].y * planeScale / 2, vertices[index].z * planeScale);

                int treeType = 0;

                if (Random.Range(0, 10) == 7) {
                    treeType = 1;
                }

                treeSpots[(int)(col.transform.position.x / planeScale / 10.0f), (int)(col.transform.position.z / planeScale / 10.0f)].Add(new Vector4(treePos.x, treePos.y, treePos.z, treeType));

                // GameObject newTree = (GameObject)Instantiate(tree, treePos, new Quaternion(0, 0, 0, 0));
                // newTree.transform.localScale = new Vector3(3, Random.Range(3.0f, 5.0f), 3);
                // newTree.transform.SetParent(col.transform);
            }
        }
    }

    private void InstantiateTrees () {
        // Instantiate Pooled Trees
        for (int i = 0; i < 1500; i++) {
            GameObject newTree = (GameObject)Instantiate(tree, Vector3.zero, new Quaternion(0, 0, 0, 0));
            newTree.transform.localScale = new Vector3(3, Random.Range(3.0f, 5.0f), 3);
            newTree.transform.SetParent(Game.instance.trees.transform);
            newTree.SetActive(false);

            Game.instance.pooledTrees.Add(newTree);
        }

        // Instantiate Pooled Bushes
        for (int i = 0; i < 200; i++) {
            GameObject newBush = (GameObject)Instantiate(bush, Vector3.zero, new Quaternion(0, 0, 0, 0));
            // newTree.transform.localScale = new Vector3(3, Random.Range(3.0f, 5.0f), 3);
            newBush.transform.SetParent(Game.instance.trees.transform);
            newBush.SetActive(false);

            Game.instance.pooledBushes.Add(newBush);
        }
    }

    private Wave[] GenerateWaves (int waveCount) {
    	Wave[] waveSet = new Wave[waveCount];

    	for (int i = 0; i < 10; i++) {
    		int seed = Random.Range(1000, 5000);
    		float frequency = Random.Range(0.25f, 1.0f);
    		int amplitude = Random.Range(1, 5);

    		waveSet[i] = new Wave(seed, frequency, amplitude);
    	}

    	return waveSet;
    }

    private void InitializeColors () {
        whites = new Color32[8];
        browns = new Color32[8];
        greens = new Color32[8];
        blues = new Color32[8];

        Color lightBlue = new Color(0.7f, 0.9f, 0.9f, 1);
        Color brown = new Color(0.35f, 0.25f, 0.125f, 1);
        Color green = new Color(0, 0.6f, 0.1f, 1);
        Color white = new Color(1, 1, 1, 1);

        for (int i = 0; i < 8; i++) {
            float darkness = 0.02f * i;

            whites[i] = new Color(white.r - darkness, white.g - darkness, white.b - darkness, 1);
            browns[i] = new Color(brown.r - darkness, brown.g - darkness, brown.b - darkness, 1);
            greens[i] = new Color(green.r - darkness, green.g - darkness, green.b - darkness, 1);
            blues[i] = new Color(lightBlue.r - darkness, lightBlue.g - darkness, lightBlue.b - darkness, 1);
        }
    }
}

public class Wave {

	public float seed;
	public float frequency;
	public float amplitude;

	public Wave (float seed, float frequency, float amplitude) {
		this.seed = seed;
		this.frequency = frequency;
		this.amplitude = amplitude;
	}
}













