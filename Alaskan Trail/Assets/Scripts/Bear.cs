using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour, Damagable {

	public float detectionRadius;
	public float walkSpeed;
	public int attackDamage;

	public float health;
	public float maxHealth;

	public GameObject bearModel;
	public Material defMaterial;

	public AnimatedLimb[] limbs;
	public AnimatedLimb body;

	private Rigidbody rb;
	
	public MeshRenderer[] renderers;

	private float redTime;
	private float knockTime;
	private bool following;

	public float attackTime;
	public float soundTime;

	void Start () {
		rb = GetComponent<Rigidbody>();
		// mr = GetComponent<MeshRenderer>();

		soundTime = Random.Range(2.0f, 6.0f);
	}

	void Update () {

		if (Vector3.Distance(Game.instance.player.transform.position, transform.position) < detectionRadius) {

			FollowPlayer();

			if (!following) {
				attackTime = 3.0f;

				StartAnimation();
			}

			attackTime -= Time.deltaTime;

			if (attackTime <= 0) {
				Attack();
			}

			soundTime -= Time.deltaTime;

			if (soundTime <= 0) {
				Game.Sound(2, true);

				soundTime = Random.Range(2.0f, 6.0f);
			}

			following = true;
		}
		else {
			if (following) {
				StopAnimation();
			}

			following = false;
		}

		rb.AddForce(new Vector3(0, -30, 0));

		if (redTime > 0) {
			redTime -= Time.deltaTime;

			if (redTime < 0) {

				foreach (MeshRenderer renderer in renderers) {
					renderer.material = defMaterial;
				}

				redTime = 0;
			}
		}

		if (knockTime > 0) {
			knockTime -= Time.deltaTime;
		}

		if (transform.position.y < 0) {
			Game.instance.bearCount--;
			Destroy(gameObject);
		}
	}

	private void FollowPlayer () {
		if (knockTime > 0) {
			return;
		}

		Vector3 lookVector = Game.instance.player.transform.position - transform.position;
		transform.forward = new Vector3(lookVector.x, 0, lookVector.z);

		rb.velocity = new Vector3(bearModel.transform.forward.x * walkSpeed, rb.velocity.y, bearModel.transform.forward.z * walkSpeed);
	}

	void OnCollisionStay (Collision collision) {
    	if (collision.collider.tag != "Ground") {
    		return;
    	}

    	Vector3 upVector = collision.contacts[0].normal;

        bearModel.transform.up = upVector;

        bearModel.transform.RotateAround(bearModel.transform.position, upVector, transform.eulerAngles.y);
    }

    private void StartAnimation () {

    	List<AnimationStep> steps = new List<AnimationStep>() {
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, -35), Vector3.zero, 0.2f),
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 360), Vector3.zero, 0.2f),
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 30), Vector3.zero, 0.2f),
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 0), Vector3.zero, 0.2f),
        };

        limbs[0].Animate(steps);
        limbs[3].Animate(steps);

        steps = new List<AnimationStep>() {
        	new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 30), Vector3.zero, 0.2f),
        	new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 0), Vector3.zero, 0.2f),
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, -35), Vector3.zero, 0.2f),
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 360), Vector3.zero, 0.2f),
        };

        limbs[1].Animate(steps);
        limbs[2].Animate(steps);

        steps = new List<AnimationStep>() {
        	new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, -7), Vector3.zero, 0.2f),
        	new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 360), Vector3.zero, 0.2f),
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 7), Vector3.zero, 0.2f),
            new AnimationStep(new Vector3(0, 0, 0), new Vector3(0, 0, 0), Vector3.zero, 0.2f),
        };

        body.Animate(steps);
    }

    private void StopAnimation () {

    	foreach (AnimatedLimb limb in limbs) {
    		limb.Stop();
    	}

    	body.Stop();
    }

    private void Attack () {

    	Collider[] colliders = Physics.OverlapSphere(transform.position + (transform.forward * 2.5f), 1.5f);

    	foreach (Collider col in colliders) {

    		Damagable damagable = col.GetComponent<Damagable>();

    		if (col.tag == "Player") {
    			damagable = Game.instance.player.GetComponent<Damagable>();
    		}

    		if (damagable == null || col.tag == "Bear") {
    			continue;
    		}

    		damagable.TakeDamage(attackDamage, transform.forward);
    	}

    	attackTime = 3.0f;
    }

    public void TakeDamage (int damage, Vector3 direction) {

    	Game.Sound(1, false);

    	health -= damage;

    	if (health <= 0) {
    		Destroy(gameObject);
    	}

		// mr.material = Game.instance.damageMaterial;

		foreach (MeshRenderer renderer in renderers) {
			renderer.material = Game.instance.damageMaterial;
		}

    	redTime = 0.25f;
    	knockTime = 0.5f;

    	rb.AddForce(new Vector3(direction.x * 50, 35, direction.z * 50), ForceMode.Impulse);
    }
}














