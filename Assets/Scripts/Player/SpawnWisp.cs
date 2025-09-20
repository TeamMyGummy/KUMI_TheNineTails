using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWisp : MonoBehaviour
{
    public GameObject objectToSpawn;
    void Start()
    {
        Instantiate(objectToSpawn, transform);
    }
}
