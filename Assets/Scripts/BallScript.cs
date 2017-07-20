using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

    public Transform paddle1;
    public Transform paddle2;
    public BallState ballState = BallState.FREE;
    public GameObject crack;

    private Rigidbody rb;
    private Vector3 curveDirection = new Vector3(0, 0, 0);
    private float velocity = 3;
    private int counter = 0;
    private GameStateManager gsm;

    private System.Random rand = new System.Random();

    public enum BallState {FREE, CATCHED_BY_P1, CATCHED_BY_P2}

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(-transform.forward, ForceMode.Impulse);
        rb.maxAngularVelocity = 70;
        gsm = GameObject.Find("GameManager").GetComponent<GameStateManager>();

        float rx = (float)(rand.NextDouble() * 2 - 1);
        float ry = (float)(rand.NextDouble() * 1 - 0.5);
        rb.AddForce(new Vector3(rx, ry), ForceMode.Impulse);
    }

    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }
	
	// Update is called once per frame
	void Update () {

        if (gsm.gameState == GameStateManager.GameState.BALL_ATTACK)
            return;
           
        if (ballState == BallState.CATCHED_BY_P1 || ballState == BallState.CATCHED_BY_P2)
        {
            rb.angularVelocity = new Vector3(0, 0, 0);
            Transform paddle;
            if (ballState == BallState.CATCHED_BY_P1)
                paddle = paddle1;
            else
                paddle = paddle2;
            this.transform.position = paddle.position - paddle.up*0.2f;

            if (ballState == BallState.CATCHED_BY_P1)
            {
                if (Input.GetMouseButtonDown(0))
                    ThrowBall(paddle);
                float velocity = (paddle.GetComponent<Move>().GetVelocity() * 1000).sqrMagnitude;
                if (velocity > 1.5)
                    ThrowBall(paddle);
            }
               
        }

    }

    public void ThrowBall(Transform paddle)
    {
        Vector3 velocity = paddle.gameObject.GetComponent<Move>().GetVelocity();
        velocity *= 10000;
        velocity.z = 10;
        this.rb.AddForce(velocity, ForceMode.Impulse);
        ballState = BallState.FREE;
    }

    void LateUpdate()
    {
        if (ballState != BallState.FREE)
            return;

        Vector3 av = rb.angularVelocity;
        Vector3 curveForce = new Vector3(0, 0, 0);
        curveForce = Vector3.Cross(av, rb.velocity);
        curveForce *= 0.015f;
        rb.AddForce(curveForce, ForceMode.Force);

        if (Mathf.Abs(rb.velocity.z) <= 1f)
            rb.velocity = Vector3.Scale(rb.velocity, new Vector3(1, 1, 3f));
        rb.velocity = velocity * (rb.velocity.normalized);
        
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Paddle")
        {
            Move move = collisionInfo.collider.gameObject.GetComponent<Move>();
            Vector3 paddleVelocity = move.GetVelocity();

            Vector3 angularVelocity = new Vector3(paddleVelocity.y, paddleVelocity.x, 0) * 20000 * move.curveFactor;
            if (Mathf.Abs(angularVelocity.x) > 30)
                angularVelocity.x = Mathf.Sign(angularVelocity.x) * 30;
            if (Mathf.Abs(angularVelocity.y) > 30)
                angularVelocity.y = Mathf.Sign(angularVelocity.y) * 30;
            rb.angularVelocity += angularVelocity;

            rb.AddForce(paddleVelocity*10000);
            Blinks[] blinks = collisionInfo.collider.gameObject.GetComponentsInChildren<Blinks>();
            foreach (Blinks b in blinks)
                b.ChangeTexture(transform.position);
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (gsm.gameState == GameStateManager.GameState.BALL_ATTACK)
        {
            BallAttackCollision(collisionInfo);
            return;
        }

        if (collisionInfo.gameObject.name == "P1Catch")
        {
            ScoreManager sm = GameObject.Find("GameManager").GetComponent<ScoreManager>();
            sm.P2Scored();
            ballState = BallState.CATCHED_BY_P1;
        }

        if (collisionInfo.gameObject.name == "P2Catch")
        {
            ScoreManager sm = GameObject.Find("GameManager").GetComponent<ScoreManager>();
            sm.P1Scored();
            ballState = BallState.CATCHED_BY_P2;
        }

        if (collisionInfo.gameObject.tag == "Wall" || collisionInfo.gameObject.tag == "BackWall")
        {
            GameObject crackInstance = Instantiate(crack);
            crackInstance.transform.position = this.transform.position;
            Vector3 normal = collisionInfo.contacts[0].normal;
            crackInstance.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, normal);
            
        }
    }

    void BallAttackCollision(Collision collisionInfo)
    {
        ScoreManager sm = GameObject.Find("GameManager").GetComponent<ScoreManager>();
        if (collisionInfo.gameObject.name == "P1Catch")
        {
            sm.P2Scored();
            Destroy(this.gameObject);
        }

        if (collisionInfo.gameObject.name == "P2Catch")
        {
            sm.P1Scored();
            Destroy(this.gameObject);
        }
    }
}
