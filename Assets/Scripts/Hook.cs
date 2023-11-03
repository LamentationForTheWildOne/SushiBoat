using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public GameObject gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit");
        if (gameManager.GetComponent<GameManager>().state == Phase.FISHING)
        {
            if (collision.gameObject.tag == "Fish")
            {
                gameManager.GetComponent<GameManager>().FishCaught(collision.gameObject);
            }

        }
    }
}
