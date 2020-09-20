using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public float heightScale;

	public GameObject plane;

	public GameObject[,] mapPlanes;

    // Start is called before the first frame update
    void Start () {
        GenerateMap(20, 20, 11, 11, 25);
    }

    // Update is called once per frame
    void Update () {
        
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

    			vertices[vertex] = new Vector3(vertices[vertex].x, height * heightScale, vertices[vertex].z);
    			vertex++;
    		}
    	}

    	meshFilter.mesh.vertices = vertices;
    	meshFilter.mesh.RecalculateBounds();
    	meshFilter.mesh.RecalculateNormals();
    	vPlane.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
    }

    public void GenerateMap (int amountX, int amountZ, int sizeX, int sizeZ, float scale) {

    	mapPlanes = new GameObject[amountX, amountZ];
    	Wave[] waves = GenerateWaves(10);

    	for (int x = 0; x < amountX; x++) {
    		for (int z = 0; z < amountZ; z++) {
    			mapPlanes[x, z] = (GameObject)Instantiate(plane, Vector3.zero, new Quaternion(0, 0, 0, 0));
    			float[,] heightMap = GenerateHeightMap(sizeX, sizeZ, scale, 10 * x, 10 * z, waves);
    			Texture2D planeTexture = GenerateTexture(heightMap);

    			mapPlanes[x, z].transform.position += new Vector3(10 * x, 0, 10 * z);
    			mapPlanes[x, z].GetComponent<MeshRenderer>().material.mainTexture = planeTexture;
    			SetVertexHeights(mapPlanes[x, z], heightMap);
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

    			if (noise > 0.7f) {
    				noise *= 2f;
    			}
    			else if (noise > 0.65f) {
    				noise *= 1.25f;
    			}

    			heightMap[x, z] = noise * 2;
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
    			Color pixelColor = new Color(1, 1, 1, 1);
    			
    			if (heightMap[x, z] < 0.9f) {
    				pixelColor = new Color(0, 0.6f, 0.1f, 1);
    			}
				else if (heightMap[x, z] < 0.95f) {
					pixelColor = new Color(0.5f, 0.25f, 0.125f, 1);
    			}

    			newTexture.SetPixel(z, x, pixelColor);
    		}
    	}

    	newTexture.wrapMode = TextureWrapMode.Clamp;
    	newTexture.Apply();

    	return newTexture;
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













