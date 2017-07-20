using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {

    public GameObject ball;
    private float delayTime = 5;

    private float timer = 0;

	// Use this for initialization
	void Start () {
        delayTime = 2;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= delayTime)
        {
            timer = 0;
            GameObject ballInstance = Instantiate(ball, transform.position, transform.rotation);
            ballInstance.GetComponent<BallScript>().ballState = BallScript.BallState.FREE;
            if (delayTime > 1)
            {
                delayTime -= 0.01f;
            }
        }
	}
}
