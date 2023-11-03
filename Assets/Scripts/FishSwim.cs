using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwim : MonoBehaviour
{
    public GameObject OppSpawn;
    public GameObject hook;
    public bool left;
    public float depth;
    public float speed;
    public int fishType;
    public bool hooked = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!left) {
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    public void Hooked() {
        hooked = true;
        transform.Rotate(0,0,270);
        transform.position = hook.transform.position;
    
    }

    // Update is called once per frame
    void Update()
    {
        if (!hooked) 
        {
            if (left) {
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y);
                if (transform.position.x <= OppSpawn.transform.position.x) {
                    Destroy(gameObject);
                }
            }

            if (!left)
            {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y);
                if (transform.position.x >= OppSpawn.transform.position.x)
                {
                    Destroy(gameObject);
                }
            }
        }

        if (hooked) {
            transform.position = hook.transform.position;
        }
    }
}
