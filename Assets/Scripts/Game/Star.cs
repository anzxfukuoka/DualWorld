using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public int cost = 2;

    public float lifeTime = 4f;
    float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        //color
        //SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        //renderer.color = Game.instance.currentColorTheme.accentColor;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if(t > lifeTime)
            Destroy(gameObject);

        if (Player.magnet) {

            Vector3 dir = (Player.instance.gameObject.transform.position - transform.position);
            float d = dir.magnitude;

            if (d < 4f) {
                transform.Translate(dir * (1 / d) * 0.2f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("~☆★☆★");
            collision.gameObject.SendMessage("AddScore", cost);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Spinner")
        {
            Destroy(gameObject);
        }
    }
}
