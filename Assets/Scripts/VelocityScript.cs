using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class VelocityScript: MonoBehaviour
{
    [SerializeField]
    Vector3 v3Force;
    [SerializeField]
    KeyCode positive;
    [SerializeField]
    KeyCode negative;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (transform.position.y <= 1.05f)
            {
                Rigidbody c = GetComponent<Rigidbody>();
                c.AddForce(Vector3.up * 100);
            }
        }
        if (Input.GetKey(positive)) 
        {
            Rigidbody c = GetComponent<Rigidbody>();
            c.velocity += v3Force;
        }
        if (Input.GetKey(negative))
        {
            Rigidbody c = GetComponent<Rigidbody>();
            c.velocity -= v3Force;  
        }

    }
}
