using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool empty = true;
    public int iv_fishtype = 0;
    public Sprite Tuna;
    public Sprite Salmon;
    public Sprite Yellowtail;
    public Sprite Eel;
    public Sprite Crab;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = null;
        GetComponent<Image>().color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        switch (iv_fishtype) 
        {
            case 0:
                GetComponent<Image>().sprite = null;
                break;

            case 1:
                GetComponent<Image>().sprite = Tuna;
                GetComponent<Image>().color = Color.white;
                break;

            case 2:
                GetComponent<Image>().sprite = Salmon;
                GetComponent<Image>().color = Color.white;
                break;

            case 3:
                GetComponent<Image>().sprite = Yellowtail;
                GetComponent<Image>().color = Color.white;
                break;

            case 4:
                GetComponent<Image>().sprite = Eel;
                GetComponent<Image>().color = Color.white;
                break;

            case 5:
                GetComponent<Image>().sprite = Crab;
                GetComponent<Image>().color = Color.white;
                break;
        }

        if (empty) {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = Color.clear;
        }

    }
}
