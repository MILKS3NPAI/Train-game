using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterKatLLC.AudioManagement.Debugging
{
    public class Blurb_Tester : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Audio_Blurber blurber;

        [Header("Configuration")]
        [SerializeField] private int blurb_count = 10;
        [SerializeField] private float time_between_blurbs = 0.5f;
        [SerializeField] private bool blurb_on_awake = true;

        private void Start()
        {
            if (blurb_on_awake)
                StartCoroutine(BlurbCorountine());
        }

        [ContextMenu("StartBlurb()")]
        public void StartBlurb()
        {
            StartCoroutine(BlurbCorountine());
        }

        IEnumerator BlurbCorountine()
        {
            WaitForSeconds wait_time = new WaitForSeconds(time_between_blurbs);

            for (int i = 0; i < blurb_count; i++)
            {
                blurber.PlayAnyBlurb();
                yield return wait_time;   
            }

            yield return null;
        }
    }
}