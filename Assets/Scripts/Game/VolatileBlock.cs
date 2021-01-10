using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatileBlock : MonoBehaviour
{

    bool interracted = false;

    public State activeState = State.Day;
    Renderer renderer;

    public float lifeTime = 4f;
    float t = 0;

    private void OnEnable()
    {
        renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        t += Time.deltaTime;

        if (t > lifeTime)
            Destroy(gameObject);
    }

    public void SetSize(Vector3 size) {
        transform.localScale = size;
    }

    public void SetPos(Vector3 pos) {
        transform.position = pos + transform.localScale / 2;
    }

    public void SetActiveState(State state) {
        activeState = state;

        Material material = null;

        switch (state) {
            case State.Night:
                material = Game.instance.blockNightMaterial;
                break;
            case State.Day:
                material = Game.instance.blockDayMaterial;
                break;
        }

        renderer.material = material;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("z");

        if (collision.gameObject.tag == "Player" && Game.gameState != activeState)
        {

            if (interracted)
                return;

            if (Player.instance.godMode)
                return;

            Debug.Log("death (x - x)");

            //collision.gameObject.SendMessage("Kill");

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            for (var i = 0; i < players.Length; i++)
            {
                players[i].SendMessage("Kill");
            }

            if (Player.currentBuff != BuffType.DoubleShadow)
            {
                Player.alive = false;

            }
            else {
                //Player.instance.shadows[1].animator.SetTrigger("showOff");
                Player.currentBuff = BuffType.None;
            }

            Player.currentBuff = BuffType.None;
            

            interracted = true;
        }

        if (collision.gameObject.tag == "Star")
        {
            Debug.Log("~");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Spinner")
        {
            Destroy(gameObject);
        }
    }
}
