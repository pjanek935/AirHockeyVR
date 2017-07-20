using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackScript : MonoBehaviour {

    private Renderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        color.a = 1f;
        renderer.material.color = color;
    }
	
	// Update is called once per frame
	void Update () {
        Color color = renderer.material.color;
        if (color.a > 0)
        {
            color.a -= 0.01f;
            renderer.material.color = color;
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
}
