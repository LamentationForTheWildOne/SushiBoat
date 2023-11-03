using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeReq : MonoBehaviour
{
    public int fishType1;
    public bool hasFish1 = false;
    public GameObject Fish1IV;

    public int fishType2;
    public bool hasFish2 = false;
    public GameObject Fish2IV;

    public GameObject fishCheck1;
    public GameObject fishCheck2;
    public GameObject Prepare;
    public bool ready;
    public Sprite pGray;
    public Sprite pGreen;
    public float timeLeft;

    public int queue;

    // Start is called before the first frame update
    void Start()
    {
        Prepare.GetComponent<SpriteRenderer>().sprite = pGray;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasFish1)
        {
            fishCheck1.SetActive(true);
        }
        else 
        {
            fishCheck1.SetActive(false);
        }
        if (hasFish2)
        {
            fishCheck2.SetActive(true);
        }
        else 
        {
            fishCheck2.SetActive(false);
        }
        if (hasFish1 && hasFish2)
        {
            ready = true;
            Prepare.GetComponent<SpriteRenderer>().sprite = pGreen;
        }
        else 
        {
            ready = false;
            Prepare.GetComponent<SpriteRenderer>().sprite = pGray;
        }
    }
}
