using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    private const int PUZZLE_AMOUNT = 3;
    [Tooltip("Parent objects, each containing its own puzzle mode functionality")] [SerializeField] private GameObject[] _allPuzzleModes = new GameObject[PUZZLE_AMOUNT];
    [SerializeField] private PuzzleMode _currentPuzzleMode;

    private void Awake()
    {
        if (_allPuzzleModes.Length != PUZZLE_AMOUNT)
            throw new System.Exception("Puzzle list is an incorrect size");
        else if (!_allPuzzleModes[(int)_currentPuzzleMode])
            throw new System.Exception("Indexed puzzle item doesn't exist");
        _allPuzzleModes[(int)_currentPuzzleMode].SetActive(true);
    }
    private enum PuzzleMode : int
    {
        // Slide: Click a tile to slide it to its adjacent empty slot; All tiles are 1x1 in size
        // Arrange: Click and drag tiles to empty slots, not necessarily to an adjacent space; Tiles can be different sizes
        // Flow: Click and drag path from one Source tile to its Target; All tiles are 1x1 in size
        Slide, Arrange, Flow
    }
}
