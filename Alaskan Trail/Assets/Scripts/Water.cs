using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

	public float waterSpeed;

	private float waterHeight;
	private int waterDirection;

    // Start is called before the first frame update
    void Start () {
        waterHeight = 52f;
        waterDirection = 1;
    }

    // Update is called once per frame
    void Update () {
    	waterHeight += (waterDirection * Time.deltaTime * waterSpeed);

    	if (waterHeight < 52) {
    		waterDirection = 1;
    		waterHeight = 52;
    	}
    	if (waterHeight > 52.2f) {
    		waterDirection = -1;
    		waterHeight = 52.2f;
    	}

        transform.position = new Vector3(Game.instance.player.transform.position.x, waterHeight, Game.instance.player.transform.position.z);
    }
}
