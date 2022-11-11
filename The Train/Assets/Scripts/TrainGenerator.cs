using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainGenerator : MonoBehaviour
{
    [SerializeField] private float _carSeparationDistance, _carScale;
    // Train spawn point is upper left corner of train
    [SerializeField] private GameObject _trainCar, _trainSpawnPoint;
    [SerializeField] private int _carCount;
    [SerializeField] Transform _trainCarParent;

    private void Awake()
    {
        for (int i = 0; i < _carCount; i++)
        {
            GameObject newCar = Instantiate(_trainCar);
            newCar.name = "Train Car (" + i + ")";
            newCar.transform.SetParent(_trainCarParent.transform);
            newCar.transform.localScale = _carScale * Vector3.one;
            newCar.GetComponent<BoxCollider2D>().size *= _carScale;
            newCar.transform.position = _trainSpawnPoint.transform.position + i * (_trainCar.GetComponent<BoxCollider2D>().size.x * _carScale + _carSeparationDistance) * Vector3.right;
        }
    }
}
