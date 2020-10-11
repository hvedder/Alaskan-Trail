using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour {

	public GameObject dogModel;
    public GameObject frontHitch;
    public GameObject backHitch;

    public GameObject frontDog;
    public bool tied;

	public float speed;
    public float maxSpeed;

	public Rigidbody rb;

    // Start is called before the first frame update
    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void Update () {
    	if (Input.GetKey("j")) {
        	transform.Rotate(new Vector3(0, -90 * Time.deltaTime, 0));
        }

        if (Input.GetKey("l")) {
        	transform.Rotate(new Vector3(0, 90 * Time.deltaTime, 0));
        }

        if (frontDog != null) {
            Vector3 pointVector = Game.Point(gameObject, frontDog);

            // transform.forward = new Vector3(frontDog.transform.position.x - transform.position.x, 0, frontDog.transform.position.z - transform.position.z);

            // Debug.Log(pointVector.y);

            Quaternion newRotation = Quaternion.Euler(0, pointVector.y, 0);
            rb.MoveRotation(newRotation);
        }
    }

    void FixedUpdate () {
        Vector3 localVelocity = dogModel.transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0;
        rb.velocity = dogModel.transform.TransformDirection(localVelocity);

        if (tied) {
            rb.AddForce(new Vector3(0, -30, 0));
        }
    }

    public void Move (float moveSpeed) {
        rb.AddForce(transform.forward * moveSpeed);
    }

    void OnCollisionStay (Collision collision) {
    	if (collision.collider.tag != "Ground") {
    		return;
    	}

    	Vector3 upVector = collision.contacts[0].normal;

        dogModel.transform.up = upVector;

        dogModel.transform.RotateAround(dogModel.transform.position, upVector, transform.eulerAngles.y);
    }

    void OnTriggerEnter (Collider other) {
        if (other.tag != "Player") {
            return;
        }

        if (!tied) {
            Game.instance.player.SetActionButton(ActionButtons.TieDog, gameObject);
        }
        else {
            Game.instance.player.SetActionButton(ActionButtons.UntieDogs, gameObject);
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.tag != "Player") {
            return;
        }

        if (Game.instance.player.actionObject == gameObject) {
            Game.instance.player.SetActionButton(ActionButtons.None, null);
        }
    }
}















