using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreativeButton : MonoBehaviour
{
    private void Start()
    {
        // Prevents clicking transparent space of image
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
