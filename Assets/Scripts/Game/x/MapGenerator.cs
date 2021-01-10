using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{

    public const int cacheSize = 16;

    public Spawnable star;
    public Spawnable triplleBlock;
    public Spawnable spinner;


    public Spawnable[] cache = new Spawnable[cacheSize];

    Camera camera;
    Vector3 camPos;

    int y;
    int lastY = 0;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

        Generate();
    }

    public void Generate() {

        for (int i = 0; i < cacheSize; i++) {
            cache[i] = star;

            if ((lastY + i) % 8 == 0) {
                cache[i] = triplleBlock;
            }
        }
    }

    

    public int placed = 0; //рамер занимаемый предедущим блоком

    public int pos = 0;

    // Update is called once per frame
    void Update()
    {
        if (!Player.alive)
            return;

        camPos = camera.ViewportToWorldPoint(new Vector3(0, 1, 0)); // верхний левый угол камеры

        y = (int)camPos.y;

        if(lastY != y)
        {
            //Debug.Log(y);
            
            lastY = y;

            if (placed > 0)
            {
                placed--;
                return;
            }

            pos++;

            if (pos >= cacheSize)
            {
                pos = 0;
                Generate();
            }

            Spawnable sp = cache[pos % cacheSize];
            cache[pos % cacheSize] = null;

            GameObject prefab = sp.gameObject;

            GameObject newObject = Instantiate(prefab);
            newObject.transform.position = Vector3.up * (y) + Vector3.up * sp.size.y/2 + Vector3.forward * sp.posZ;

            placed = (int)sp.size.y; // - 1

        }

    }
}

[Serializable]
public class P {
    public Spawnable spawnable;
    public float p;
}