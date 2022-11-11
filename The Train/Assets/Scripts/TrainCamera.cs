using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCamera : MonoBehaviour
{
    [SerializeField] private float _cameraLerpSpeed, _cameraHeightOffset;
    [SerializeField] private Transform _target;
    // Prevents camera's z from being set to target transform's z
    private float _cameraZ;
    private void Awake()
    {
        _cameraZ = transform.position.z;
    }
    private void FixedUpdate()
    {
        Vector3 targetPos = _target.position;
        transform.position = Vector3.Lerp(transform.position, targetPos, _cameraLerpSpeed) + _cameraHeightOffset * Vector3.up + _cameraZ * Vector3.forward;
    }
}
