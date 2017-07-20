using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinks : MonoBehaviour {

    public Texture mid;
    public Texture nw;
    public Texture ne;
    public Texture sw;
    public Texture se;

	// Update is called once per frame
	void Update () {
        Color color = GetComponent<Renderer>().material.color;
        if (color.a > 0)
            color.a -= 0.02f;
        GetComponent<Renderer>().material.color = color;
    }

    public void ChangeTexture(Vector3 ballPosition)
    {
        float d = Vector2.Distance(ballPosition, transform.position);
        if (d < 0.2f)
            GetComponent<Renderer>().material.mainTexture = mid;
        else
        {
            Vector3 localCo = ballPosition - transform.position;
            if (localCo.x > 0 && localCo.y > 0)
                GetComponent<Renderer>().material.mainTexture = sw;
            else if (localCo.x > 0 && localCo.y < 0)
                GetComponent<Renderer>().material.mainTexture = nw;
            else if (localCo.x < 0 && localCo.y > 0)
                GetComponent<Renderer>().material.mainTexture = se;
            else if (localCo.x < 0 && localCo.y < 0)
                GetComponent<Renderer>().material.mainTexture = ne;
        }
        StartFade();
    }

    void StartFade()
    {
        Color color = GetComponent<Renderer>().material.color;
        color.a = 1;
        GetComponent<Renderer>().material.color = color;
    }
}
