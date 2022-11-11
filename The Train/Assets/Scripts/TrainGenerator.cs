using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainGenerator : MonoBehaviour
{
    [SerializeField] private float _carSeparationDistance, _carScale;
    // Train spawn point is upper left corner of train
    [SerializeField] private GameObject _trainSpawnPoint;
    [SerializeField] private List<GameObject> _trainCars;
    [SerializeField] Transform _trainCarParent;

    private void Awake()
    {
        // Keeps track of length of cars generated so far
        float subtrainLength = 0;
        for (int i = 0; i < _trainCars.Count; i++)
        {
            _trainCars[i].SetActive(true);
            _trainCars[i].name = "Train Car (" + i + ")";
            _trainCars[i].transform.SetParent(_trainCarParent.transform);
            _trainCars[i].transform.localScale = _carScale * Vector3.one;
            _trainCars[i].GetComponent<BoxCollider2D>().size *= _carScale;
            _trainCars[i].transform.position = _trainSpawnPoint.transform.position + subtrainLength * Vector3.right;
            subtrainLength += _trainCars[i].GetComponent<BoxCollider2D>().size.x * _carScale + _carSeparationDistance;
        }
    }
}
