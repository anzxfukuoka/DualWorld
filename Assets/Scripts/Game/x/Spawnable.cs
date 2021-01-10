using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawnable : MonoBehaviour
{
    public Vector3 size = Vector3.one;

    public float lifeTime = 30f; //sec

    public float posZ = 0;

    //public bool ignorePlaced = false;

    public float _t = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _t += Time.deltaTime;

        if (lifeTime != -1 && _t > lifeTime) {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.6f, 0, 1f, 0.6f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
