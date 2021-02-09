using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class AI : MonoBehaviour {

    public GameObject ball;
    public bool savingVectors = false;
    public bool revertedInput = false;
    public AILevel aiLevel = AILevel.EASY1;

    private NeuralNetworkBackProp nn; //General purpose neural network
    private NeuralNetworkBackProp nn2; //Spinnig neural network
    private List<string> learningVectors = new List<string>();
    private int inputCount = 6;
    private double[] input;
    private Move move;

    private int speed1Lvl = 20;
    private int speed2Lvl = 20;
    private float distanceThreshold = 0;
    private bool throwingBall = false;

    private System.Random rand = new System.Random();

    public enum AILevel { EASY1, EASY2, MEDIUM1, MEDIUM2, HARD1, HARD2 }

    // Use this for initialization
    void Start () {
        input = new double[inputCount];
        nn = new NeuralNetworkBackProp(inputCount, 10, 2);
        nn2 = new NeuralNetworkBackProp(inputCount, 10, 2);
        move = GetComponent<Move>();

        LoadWeightFromString(nn, NeuralWeights.weightNormal1);
        LoadWeightFromString(nn2, NeuralWeights.weightCurve1);

        SetAILevel(aiLevel);
    }
	
	// Update is called once per frame
	void Update () {
        GatherInput();
        if (move.controlMode == Move.ControlMode.NEURAL)
        {
            if (ball.GetComponent<BallScript>().ballState == BallScript.BallState.CATCHED_BY_P2)
                ThrowBall();
            else
                BounceBack();
        }
        else if(move.controlMode == Move.ControlMode.PAD)
        {
            float vertical = Input.GetAxis("Joy X");
            float horizontal = Input.GetAxis("Joy Y");
            Vector3 end = transform.position + new Vector3(vertical, horizontal, 0) * 20 * Time.deltaTime;
            move.MoveFromTo(transform.position, end);

            if (savingVectors)
                SaveVectors();
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (savingVectors)
            {
                savingVectors = false;
                Debug.Log("Stopped saving vectors");
                SaveLearningVectorsToFile("vectors.txt");
            }
            else
            {
                savingVectors = true;
                Debug.Log("Started saving vectors");
            }
        }

        

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadWeightsFromFile(nn, "weights_vectorsNormal01.txt");
            LoadWeightsFromFile(nn2, "weights_vectorsCurve01.txt");

            move.controlMode = Move.ControlMode.NEURAL;
        }
    }

    void SaveLearningVectorsToFile(string fileName)
    {
        using (StreamWriter outputFile = new StreamWriter(fileName))
        {
            foreach (string line in learningVectors)
                outputFile.WriteLine(line);
        }
    }

    private void BounceBack()
    {
        float d = Vector3.Distance(ball.transform.position, this.transform.position);
        double[] output;
        float speed = speed1Lvl;
        if (d > distanceThreshold)
        {
            output = nn.GetOutput(input);
            speed = speed1Lvl;
        }
        else
        {
            output = nn2.GetOutput(input);
            speed = speed2Lvl;
        }
        float vertical = (float)output[0];
        float horizontal = (float)output[1];

        Vector3 end = transform.position + new Vector3(vertical, horizontal, 0) * speed * Time.deltaTime;
        move.MoveFromTo(transform.position, end);
    }

    private void ThrowBall()
    {
        if(!throwingBall)
            StartCoroutine("ThrowBallCoroutine");
    }

    IEnumerator ThrowBallCoroutine()
    {
        throwingBall = true;
        float x = (float)(rand.NextDouble() * 1 - 0.5f);
        float y = (float)(rand.NextDouble() * 0.5f - 0.25f);
        Vector3 newPos = new Vector3(x, y, transform.position.z);
        float d = 10;
        while ((d = Vector3.Distance(transform.position, newPos)) >= 0.02f)
        {
            Vector3 direction = newPos - transform.position;
            direction *= 0.5f;
            Vector3 to = transform.position + direction;
            move.MoveFromTo(transform.position, to);
            yield return null;
        }
        ball.GetComponent<BallScript>().ThrowBall(this.transform);
        throwingBall = false;
        
    }

    public void NextAI()
    {
        aiLevel++;
        SetAILevel(aiLevel);
    }

    void SetAILevel(AILevel aiLevel)
    {
        this.aiLevel = aiLevel;
        switch (aiLevel)
        {
            case AILevel.EASY1:
                speed1Lvl = 10;
                speed2Lvl = 20;
                distanceThreshold = 0;
                break;

            case AILevel.EASY2:
                speed1Lvl = 12;
                speed2Lvl = 20;
                distanceThreshold = 0;
                break;

            case AILevel.MEDIUM1:
                speed1Lvl = 12;
                speed2Lvl = 20;
                distanceThreshold = 0.3f;
                break;

            case AILevel.MEDIUM2:
                speed1Lvl = 13;
                speed2Lvl = 20;
                distanceThreshold = 0.3f;
                break;

            case AILevel.HARD1:
                speed1Lvl = 15;
                speed2Lvl = 40;
                distanceThreshold = 0.3f;
                break;

            case AILevel.HARD2:
                speed1Lvl = 16;
                speed2Lvl = 60;
                distanceThreshold = 0.3f;
                break;
        }

        Debug.Log("Set: " + aiLevel.ToString() + " AI Level");
    }

    void SaveVectors()
    {
        string learningVector = "";
        for (int i = 0; i < inputCount; i++)
            learningVector += input[i] + " ";
        float vertical = Input.GetAxis("Joy X");
        float horizontal = Input.GetAxis("Joy Y");
        learningVector += vertical + " " + horizontal;
        learningVectors.Add(learningVector);
    }

    void GatherInput()
    {
        Vector3 ballPosition = ball.transform.position;
        ballPosition -= new Vector3(0, 0, 1);
        ballPosition -= new Vector3(0, 0, 1.5f);
        ballPosition = Vector3.Scale(ballPosition, new Vector3(1, 1 / 0.5f, 1 / 1.5f));
        Vector3 ballVelocity = ball.GetComponent<Rigidbody>().velocity.normalized;

        Vector3 paddlePosition = this.transform.position;
        paddlePosition = Vector3.Scale(paddlePosition, new Vector3(1, 1 / 0.5f, 1));

        Vector3 positionLocal = paddlePosition - ballPosition;
        positionLocal = Vector3.Scale(positionLocal, new Vector3(0.5f, 0.5f, 0.5f));

        if (revertedInput)
        {
            ballPosition = -ballPosition;
            ballVelocity.z = -ballVelocity.z;
        }

        input[0] = positionLocal.x;
        input[1] = positionLocal.y;
        input[2] = ballPosition.z;

        input[3] = ballVelocity.x;
        input[4] = ballVelocity.y;
        input[5] = ballVelocity.z;
    }

    void LoadWeightFromString(NeuralNetworkBackProp nn, string line)
    {
        string[] chunks = line.Split(' ');
        double[] weights = new double[chunks.Length];

        for (int i = 0; i < chunks.Length; i++)
        {
            double d = Convert.ToDouble (chunks [i], CultureInfo.InvariantCulture);
            weights [i] = d;
        }
           
        nn.SetWeights(weights);
    }

    void LoadWeightsFromFile(NeuralNetworkBackProp nn, string fileName)
    {
        string line = "";
        try
        {
            using (StreamReader sr = new StreamReader(fileName))
                line = sr.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        string[] chunks = line.Split(' ');
        double[] weights = new double[chunks.Length];
        for (int i = 0; i < chunks.Length; i++)
            weights[i] = Double.Parse(chunks[i]);

        nn.SetWeights(weights);
    }

    void LearnNeuralNetwork(NeuralNetworkBackProp nn, string fileName)
    {
        Debug.Log("Staring learning...");
        savingVectors = false;
        double[][] trainData;
        List<string> learningVectors = new List<string>();

        try
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    learningVectors.Add(line);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        trainData = new double[learningVectors.Count][];

        for (int i = 0; i < learningVectors.Count; i++)
        {
            string line = learningVectors[i];
            trainData[i] = new double[inputCount + 2];
            string[] chunks = line.Split(' ');
            for (int j = 0; j < inputCount + 2; j++)
                trainData[i][j] = Double.Parse(chunks[j]);
        }

        nn.BackProp(trainData, 200, 0.05, 0.1, false);
        Debug.Log("OK");

        using (StreamWriter outputFile = new StreamWriter("weights_" + fileName))
        {
            double[] weights = nn.GetWeights();
            string line = "";
            for (int i = 0; i < weights.Length; i++)
                line += weights[i] + " ";
            line = line.Remove(line.Length - 1);
            outputFile.WriteLine(line);
        }
    }
}
