using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerTest : MonoBehaviour
{
    public float Speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float x;
    void Update()
    {
        x += Time.deltaTime * Speed;
        transform.rotation = Quaternion.Euler(x, 0, 0);
        
    }
}
