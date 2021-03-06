﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour, HeldItem {

	private bool chopped;

    // Start is called before the first frame update
    void Start () {
        chopped = true;
    }

    // Update is called once per frame
    void Update () {
        if (!chopped && Game.instance.player.hand.currentStep == 1) {
        	Chop();
        }
    }

    private void Chop () {
    	chopped = true;

    	PlayerController player = Game.instance.player;
    	Collider[] colliders = Physics.OverlapSphere(player.mainCam.transform.position + (player.mainCam.transform.forward * 1.5f), 1.25f);

    	foreach (Collider col in colliders) {
            if (col.GetComponent<Damagable>() != null) {
                col.GetComponent<Damagable>().TakeDamage(10, player.mainCam.transform.forward);
            }

    		if (col.tag != "Tree") {
    			continue;
    		}

            Game.Sound(3 + Random.Range(0, 2), true);

    		if (col.GetComponent<Rigidbody>() == null) {
    			col.transform.SetParent(null);
    			col.gameObject.AddComponent<Rigidbody>();
    			Rigidbody tempRB = col.GetComponent<Rigidbody>();
    			tempRB.mass = 8;
    			tempRB.centerOfMass = new Vector3(0, 4, 0);
    			tempRB.velocity = new Vector3(tempRB.velocity.x, 0, tempRB.velocity.z);
    			tempRB.angularVelocity = new Vector3(tempRB.angularVelocity.x, 0, tempRB.angularVelocity.z);
                tempRB.angularDrag = 5;

    			col.gameObject.AddComponent<Resource>();
    			Resource tempRC = col.GetComponent<Resource>();
    			tempRC.type = 3;
    			tempRC.amountRemaining = Random.Range(5, 10);
    		}
    		else {
    			int amount = col.GetComponent<Resource>().Harvest(1);

    			player.inventory.AddItems(3, amount);
    		}
    	}
    }

    public void LeftClick () {
    	chopped = false;

    	List<AnimationStep> steps = new List<AnimationStep>() {
            new AnimationStep(new Vector3(-0.9f, 0.1f, 0), new Vector3(0, 0, 90), Vector3.zero, 0.3f),
            new AnimationStep(new Vector3(0.6f, 0.1f, 0.1f), new Vector3(0, 110, 90), Vector3.zero, 0.1f),
            new AnimationStep(0.3f)
        };

        Game.instance.player.hand.Animate(steps);
    }

    public void RightClick () {

    }
}
