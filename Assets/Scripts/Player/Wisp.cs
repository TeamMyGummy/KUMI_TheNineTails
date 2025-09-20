using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour
{
    private IMovement _targetMovement;
    private Transform _target;
    public float followSpeed = 2f;
    public Vector3 offset = new Vector3(0, 1.5f, -1f);
    
    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        _targetMovement = player.GetComponent<IMovement>();
        _target = player.transform;
    }
    
    void Update()
    {
        if (_target == null) return;

        Vector3 desiredPos = _target.position + new Vector3(offset.x * _targetMovement.Direction.x, offset.y, 0f);

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
    }
}
