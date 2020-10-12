using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sled : MonoBehaviour {

    public GameObject hitch;
    public GameObject hitch2;

	public GameObject[] mountPoints;
	public GameObject[] mountObjects;
    public RigidbodyConstraints[] mountConstraints;

    // Dog Variables
    public List<Dog> dogs = new List<Dog>();
    public Dog leadDog;
    public float dogSpeed;
    public float dogMaxSpeed;

    public List<GameObject> ropes = new List<GameObject>();

	public Vector3 com;
	private Rigidbody rb;

    public bool moving;

    // Start is called before the first frame update
    void Start () {
        mountObjects = new GameObject[mountPoints.Length];
        mountConstraints = new RigidbodyConstraints[mountPoints.Length];

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = com;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space") && mountObjects[0] != null) {
            UnMount(0);
        }
    }

    void FixedUpdate () {

    	Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
    	localVelocity.x = 0;
    	rb.velocity = transform.TransformDirection(localVelocity);

        rb.AddForce(new Vector3(0, -30, 0));
        rb.AddForce(transform.up * -30);
    }

    public void UpdateControls () {
        if (leadDog == null) {
            return;
        }

        if (Input.GetKey("w")) {
            Move();
        }
        else if (moving) {
            StopMoving();
        }

        if (Input.GetKey("a")) {
            leadDog.transform.Rotate(new Vector3(0, -90 * Time.deltaTime, 0));
        }

        if (Input.GetKey("d")) {
            leadDog.transform.Rotate(new Vector3(0, 90 * Time.deltaTime, 0));
        }
    }

    public void Move () {
        if (!moving) {
            foreach (Dog dog in dogs) {
                dog.rb.constraints = RigidbodyConstraints.FreezeRotation;
            }

            rb.constraints = RigidbodyConstraints.None;
        }

        leadDog.Move(dogSpeed);

        if (leadDog.rb.velocity.magnitude > dogMaxSpeed) {
            leadDog.rb.velocity = Vector3.Normalize(leadDog.rb.velocity) * dogMaxSpeed;
        }

        moving = true;
    }

    public void StopMoving () {
        foreach (Dog dog in dogs) {
            // dog.rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            dog.rb.velocity = new Vector3(0, dog.rb.velocity.y, 0);
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        moving = false;
    }

    public void Mount (GameObject mountObject, int point) {
        Rigidbody mountRB = mountObject.GetComponent<Rigidbody>();

        mountObject.transform.SetParent(mountPoints[point].transform);
        mountObject.transform.localPosition = new Vector3(0, 0, 0);
        mountObject.transform.localEulerAngles = new Vector3(0, 0, 0);

        mountRB.isKinematic = true;
        mountConstraints[point] = mountRB.constraints;
        mountRB.constraints = RigidbodyConstraints.None;

        PlayerController player = mountObject.GetComponent<PlayerController>();

        if (player != null) {
            player.mounted = true;
            Game.instance.player.SetActionButton(ActionButtons.None, null);
        }

        mountObjects[point] = mountObject;
    }

    public void UnMount (int point) {
        Rigidbody mountRB = mountObjects[point].GetComponent<Rigidbody>();

        mountObjects[point].transform.SetParent(null);
        mountRB.isKinematic = false;
        mountRB.constraints = mountConstraints[point];

        if (point == 0) {
            mountObjects[0].GetComponent<PlayerController>().mounted = false;
            Game.instance.player.SetActionButton(ActionButtons.MountSled, null);
            Game.instance.player.transform.eulerAngles = new Vector3(0, Game.instance.player.transform.eulerAngles.y, 0);
        }

        mountObjects[point] = null;
    }

    public void AddDog (Dog dog) {
        if (dogs.Contains(dog)) {
            return;
        }

        if (leadDog == null) {
            dog.transform.position = transform.position + (transform.forward * 5);
            dog.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            LinkObjects(gameObject, dog.gameObject, hitch, dog.backHitch);
            LinkObjects(gameObject, dog.gameObject, hitch2, dog.backHitch);

            dogSpeed += dog.speed;
            dogMaxSpeed += dog.maxSpeed;
        }
        else {
            dog.transform.position = leadDog.transform.position + (leadDog.transform.forward * 3);
            dog.transform.eulerAngles = new Vector3(0, leadDog.transform.eulerAngles.y, 0);

            LinkObjects(leadDog.gameObject, dog.gameObject, leadDog.frontHitch, dog.backHitch);

            leadDog.frontDog = dog.gameObject;

            dogSpeed += (dog.speed * 0.5f);
            dogMaxSpeed += (dog.maxSpeed * 0.25f);
        }

        dogs.Add(dog);
        leadDog = dog;
        dog.tied = true;
    }

    public void FreeDogs () {
        foreach (Dog dog in dogs) {
            dog.frontDog = null;

            dog.tied = false;
        }

        foreach (GameObject rope in ropes) {
            Destroy(rope);
        }

        dogs.Clear();
        ropes.Clear();
        leadDog = null;

        dogSpeed = 0;
        dogMaxSpeed = 0;
    }

    private void LinkObjects (GameObject object1, GameObject object2, GameObject hitch1, GameObject hitch2) {

        GameObject rope = (GameObject)Instantiate(Game.instance.rope, Vector3.zero, new Quaternion(0, 0, 0, 0));
        Rope ropeScript = rope.GetComponent<Rope>();

        rope.transform.position = (hitch2.transform.position + hitch1.transform.position) / 2.0f;
        rope.transform.forward = Vector3.Normalize(hitch2.transform.position - hitch1.transform.position);
        rope.transform.localScale = new Vector3(1, 1, Vector3.Distance(hitch2.transform.position, hitch1.transform.position));

        ropeScript.leftJoint.connectedBody = object1.GetComponent<Rigidbody>();
        ropeScript.rightJoint.connectedBody = object2.GetComponent<Rigidbody>();

        ropes.Add(rope);
    }

    void OnTriggerEnter (Collider other) {
        if (other.tag != "Player") {
            return;
        }

        if (!Game.instance.player.mounted) {
            Game.instance.player.SetActionButton(ActionButtons.MountSled, gameObject);
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

















