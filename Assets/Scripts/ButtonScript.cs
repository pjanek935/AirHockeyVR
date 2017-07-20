using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    private GameObject loading;
    private bool gaze = false;
    private float timer = 0;
    private float limiter = 2;

	// Use this for initialization
	void Start () {
        loading = transform.Find("Loading").gameObject;
        loading.transform.localScale = new Vector3(0, 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
        if (gaze)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GetComponent<ButtonAction>().Action();
                timer = 0;
                gaze = false;
                return;
            }

            timer += Time.deltaTime;

            Vector3 newScale = new Vector3(timer / limiter, loading.transform.localScale.y,
                loading.transform.localScale.z);
            Vector3 newPosition = new Vector3(-0.5f * (timer / limiter) + 0.5f, loading.transform.localPosition.y,
                loading.transform.localPosition.z);

            loading.transform.localScale = newScale;
            loading.transform.localPosition = newPosition;

            if (timer > limiter)
            {
                GetComponent<ButtonAction>().Action();
                timer = 0;
                gaze = false;
            }
        }
	}

    public void PointerEnter()
    {
        gaze = true;
    }

    public void PointerExit()
    {
        gaze = false;
        timer = 0;

        Vector3 newScale = new Vector3(0, loading.transform.localScale.y,
                loading.transform.localScale.z);
        loading.transform.localScale = newScale;
    }
}
