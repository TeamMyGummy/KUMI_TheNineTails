using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWisp : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Wisp wisp;
    void Start()
    {
        wisp = Instantiate(objectToSpawn, transform.position, Quaternion.identity).GetComponent<Wisp>();
    }
}
