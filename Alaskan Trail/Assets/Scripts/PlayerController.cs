using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionButtons {
    None = 0,
    PickUp = 1,
    MountSled = 2,
    TieDog = 3,
    UntieDogs = 4
}

public class PlayerController : MonoBehaviour {

    public float speed;

    public bool grounded;
    public bool jumped;
    public bool mounted;

    public Vector2Int mapPos;

    public GameObject heldObject;
    public HeldItem heldItem;

    public Camera mainCam;

    public Inventory inventory;

    public ActionButtons actionButton;
    public GameObject actionObject;

    public AnimatedLimb hand;
	private Rigidbody rb;
    private Vector3 moveVector;
    private bool spawned;
	
    // Start is called before the first frame update
    void Start () {
    	rb = GetComponent<Rigidbody>();
        inventory = GetComponent<Inventory>();

        inventory.AddItems(0, 1);
        // inventory.AddItems(1, 34);
        // inventory.AddItems(2, 5);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        mapPos = GetMapPos();
        Game.UpdateChunks(mapPos);
    }

    // Update is called once per frame
    void Update () {
        if (Game.instance.mapGenerated && !spawned) {
            Game.SpawnObject(gameObject, transform.position + new Vector3(0, 0, 1));
            Game.SpawnObject(Game.instance.sled.gameObject, transform.position + new Vector3(0, 0, 6));

            for (int i = 0; i < 5; i++) {
                GameObject newDog = (GameObject)Instantiate(Game.instance.dogPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0));
                Game.SpawnObject(newDog, transform.position + new Vector3(Random.Range(-6.0f, 6.0f), 0, Random.Range(-6.0f, 6.0f)));
            }

            spawned = true;
        }
        
        // Inventory Button
        if (Input.GetKeyDown("e")) {
        	if (Game.instance.inventoryMenu.activeSelf) {
        		inventory.Close();
        	}
        	else {
        		inventory.Open();
        	}
        }

        if (Input.GetKeyDown("enter") || Input.GetKeyDown("return")) {
            ActionButton();
        }

        if (Input.GetMouseButtonDown(0)) {
            if (heldItem != null && !Game.instance.inventoryMenu.activeSelf) {
                heldItem.LeftClick();
            }
        }
        else if (Input.GetMouseButtonDown(1)) {
            if (heldItem != null && !Game.instance.inventoryMenu.activeSelf) {
                heldItem.RightClick();
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

        if (mounted) {
            Game.instance.sled.UpdateControls();

            return;
        }

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

        // Debug.Log(moveVector);

        if (moving) {
            if (rb.constraints != RigidbodyConstraints.FreezeRotation) {
               rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
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

        if (!Game.instance.inventoryMenu.activeSelf) {
            mainCam.transform.localEulerAngles = new Vector3(xRotation, 0, 0);
        }

        float mouseX = Input.GetAxis("Mouse X") * 3;

        if (mouseX != 0 && !Game.instance.inventoryMenu.activeSelf && !mounted) {
            Quaternion newRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + mouseX, transform.localEulerAngles.z);
            rb.MoveRotation(newRotation);
        }

        if (Time.frameCount % 10 == 0) {
            Vector2Int currentMapPos = GetMapPos();

            if (currentMapPos != mapPos) {
                Game.UpdateChunks(currentMapPos);
            }

            mapPos = currentMapPos;
        }
    }

    // Moves the player in the given direction
    private void Move (Vector3 direction) {
        moveVector += Vector3.zero + (transform.right * direction.x) + (transform.up * direction.y) + (transform.forward * direction.z);

        // rb.MovePosition(transform.position + (moveVector * speed));

        // rb.velocity = new Vector3(moveVector.x * speed, rb.velocity.y, moveVector.z * speed);
    }

    public void SetActionButton (ActionButtons newButton, GameObject newObject) {
        actionButton = newButton;
        actionObject = newObject;

        string actionString = "";

        switch (actionButton) {
            case ActionButtons.PickUp:
                actionString = "[ENTER] - pick up";
            break;

            case ActionButtons.MountSled:
                actionString = "[ENTER] - hop on sled";
            break;

            case ActionButtons.TieDog:
                actionString = "[ENTER] - tie dog to sled";
            break;

            case ActionButtons.UntieDogs:
                actionString = "[ENTER] - untie the dogs";
            break;
        }

        Game.instance.actionText1.text = actionString;
        Game.instance.actionText2.text = actionString;
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

    private void ActionButton () {
        switch (actionButton) {
            case ActionButtons.PickUp:
                actionObject.GetComponent<CollectableItem>().PickUp();
            break;

            case ActionButtons.MountSled:
                actionObject.GetComponent<Sled>().Mount(gameObject, 0);
            break;

            case ActionButtons.TieDog:
                Game.instance.sled.AddDog(actionObject.GetComponent<Dog>());
                SetActionButton(ActionButtons.None, null);
            break;

            case ActionButtons.UntieDogs:
                Game.instance.sled.FreeDogs();
                SetActionButton(ActionButtons.None, null);
            break;
        }
    }
}






















