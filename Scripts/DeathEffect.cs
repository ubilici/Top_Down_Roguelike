using UnityEngine;
using System.Collections;

public class DeathEffect : MonoBehaviour {

    ParticleSystem myPS;

    void Awake()
    {
        myPS = GetComponent<ParticleSystem>();
        Destroy(gameObject, myPS.startLifetime);
    }
}
