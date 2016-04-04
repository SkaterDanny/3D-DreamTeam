﻿using UnityEngine;
using System.Collections;

public class SpiritCollect : MonoBehaviour {

    private float yValue = 0;
    private GameObject player;
    private float speed = 3f;
    private bool reachedTop = false;
    private ParticleSystem[] particleSystems;
    private bool isDestroyed = false;

    void Start()
    {
        player = GameObject.Find("N_ThirdPersonPlayer");
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update() {
<<<<<<< HEAD
        if (speed < 25f)
=======
        if (speed < 20f)
>>>>>>> 10e0bc7510ea7efa7ccb542df25cd86d44fb2818
        {
            speed += 0.1f;
        }
        if (transform.position.y < 8f && !reachedTop)
        {
            yValue += 4 * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, yValue, transform.position.z);
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position,new Vector3(player.transform.position.x, player.transform.position.y + 6.5f, player.transform.position.z));
            reachedTop = true;
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x,player.transform.position.y+6.5f,player.transform.position.z), step);
            if (distanceToTarget < 0.5f && !isDestroyed)
            {
                particleSystems[0].Stop();
                particleSystems[1].Stop();
                Destroy(transform.Find("ENEMY Spirit Ball Shine").gameObject);
<<<<<<< HEAD
                Destroy(transform.Find("Point light").gameObject);
=======
>>>>>>> 10e0bc7510ea7efa7ccb542df25cd86d44fb2818
                Invoke("DestroyObject",3f);
                isDestroyed = true;
            }
        }
    }

    void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
