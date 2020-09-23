using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;

    public bool grounded;
    public bool jumped;

    public Vector2Int mapPos;

    public Camera mainCam;

    public Inventory inventory;

	private Rigidbody rb;
    private Vector3 moveVector;
	
    // Start is called before the first frame update
    void Start () {
    	rb = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();

        // inventory.AddItems(0, 1);
        // inventory.AddItems(1, 34);
        // inventory.AddItems(2, 5);

        Cursor.visible = false;
        Screen.lockCursor = true;

        mapPos = GetMapPos();
        Game.UpdateChunks(mapPos);
    }

    // Update is called once per frame
    void Update () {
        
        // Inventory Button
        if (Input.GetKeyDown("e")) {
        	if (Game.instance.inventoryMenu.activeSelf) {
        		inventory.Close();
        	}
        	else {
        		inventory.Open();
        	}
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.25f);
        bool checkGround = false;

        foreach (Collider col in colliders) {
            if (col.tag == "Ground") {
                checkGround = true;
            }
        }

        grounded = checkGround;
    }

    void FixedUpdate () {

        bool moving = false;

        moveVector = Vector3.zero;

        // Move Buttons
        if (Input.GetKey("w")) {
            Move(new Vector3(0, 0, 1));
            moving = true;
        }

        if (Input.GetKey("s")) {
            Move(new Vector3(0, 0, -1));
            moving = true;
        }

        if (Input.GetKey("a")) {
            Move(new Vector3(-1, 0, 0));
            moving = true;
        }

        if (Input.GetKey("d")) {
            Move(new Vector3(1, 0, 0));
            moving = true;
        }

        rb.velocity = new Vector3(moveVector.x * speed, rb.velocity.y, moveVector.z * speed);

        if (moving) {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }

        // Jump Button
        if (Input.GetKey("space")) {
            Jump();
        }
        else {
            jumped = false;
        }

        if (grounded && moving) {
            rb.AddForce(new Vector3(0, -10, 0));
        }
        else {
            rb.AddForce(new Vector3(0, -30, 0));
        }
    }

    void LateUpdate () {
        float xRotation = mainCam.transform.localEulerAngles.x + (Input.GetAxis("Mouse Y") * -3);

        if (xRotation > 90 && xRotation < 180) {
            xRotation = 90;
        }
        if (xRotation < 270 && xRotation > 180) {
            xRotation = 270;
        }

        mainCam.transform.localEulerAngles = new Vector3(xRotation, 0, 0);

        transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * 3, 0);

        Vector2Int currentMapPos = GetMapPos();

        if (currentMapPos != mapPos) {
            Game.UpdateChunks(currentMapPos);
        }

        mapPos = currentMapPos;
    }

    // Moves the player in the given direction
    private void Move (Vector3 direction) {
        moveVector += Vector3.zero + (transform.right * direction.x) + (transform.up * direction.y) + (transform.forward * direction.z);

        // rb.MovePosition(transform.position + (moveVector * speed));

        // rb.velocity = new Vector3(moveVector.x * speed, rb.velocity.y, moveVector.z * speed);
    }

    private void Jump () {
        if (!grounded) {
            return;
        }
        if (jumped) {
            return;
        }

        rb.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);

        jumped = true;
    }

    private Vector2Int GetMapPos () {
        float planeSize = Game.instance.mapGenerator.planeScale * 10;

        int xPos = (int)Mathf.Floor( (transform.position.x + (planeSize / 2.0f)) / planeSize );
        int zPos = (int)Mathf.Floor( (transform.position.z + (planeSize / 2.0f)) / planeSize );

        Vector2Int currentMapPos = new Vector2Int(xPos, zPos);
    
        return currentMapPos;
    }
}






















