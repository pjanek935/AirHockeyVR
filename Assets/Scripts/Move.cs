using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public float curveFactor = 1;
    public float speed = 0.3f;
    public ControlMode controlMode = ControlMode.MOUSE;

    private Vector3 prevPos = new Vector3();
    private Vector3 velocity = new Vector3(0, 0, 0);
    private Vector3 moveToPosition = new Vector3(0, 0, 0);

    public enum ControlMode { MOUSE, PAD, NEURAL, NONE};

    // Use this for initialization
    void Start () {
        prevPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (controlMode == ControlMode.MOUSE)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position,
                Camera.main.transform.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.name == "FrontPanel")
                {
                    moveToPosition = hit.point;
                }
            }
            MoveFromTo(transform.position, moveToPosition);
        }
        
        velocity = (transform.position - prevPos) * Time.deltaTime;
        prevPos = transform.position;
    }

    public void MoveFromTo(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        direction.z = 0;
        direction *= speed;
        Vector3 newPosition = transform.position + direction;

        BoxCollider collider = GetComponent<BoxCollider>();
        float x = 1f;
        float y = 0.5f;
        if (newPosition.x > x)
        {
            transform.position.Set(x, transform.position.y,
                transform.position.z);
            direction.x = 0;
        }
        if (newPosition.x < -x)
        {
            transform.position.Set(-x, transform.position.y,
                transform.position.z);
            direction.x = 0;
        }
        if (newPosition.y > y)
        {
            transform.position.Set(transform.position.x, y,
                transform.position.z);
            direction.y = 0;
        }
        if (newPosition.y < -y)
        {
            transform.position.Set(transform.position.x, y,
                transform.position.z);
            direction.y = 0;
        }

        transform.position += direction;
    }

    public Vector3 GetVelocity() { return velocity; }

    
        
}
