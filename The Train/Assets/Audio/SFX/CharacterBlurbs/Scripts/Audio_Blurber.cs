using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterKatLLC.AudioManagement
{
    public class Audio_Blurber : MonoBehaviour, IAudioPlayer
    {
        [Header("Dependencies")]
        [SerializeField] private GameObject audioGameObjectPrefab;
        [SerializeField] private Transform audioSourceContainer;

        [Space(10)]
        
        [Header("Configuration")]
        [SerializeField] private List<AudioClip> blurbs = new List<AudioClip>();

        //internal
        private Dictionary<AudioClip, GameObject> audioGameObjectReferences = new Dictionary<AudioClip, GameObject>();
        private Dictionary<AudioClip, AudioSource> audioSourceReferences = new Dictionary<AudioClip, AudioSource>();
        private int last_blurb = 0;

        private void Awake()
        {
            foreach (var blurb in blurbs)
            {
                if (audioGameObjectReferences.ContainsKey(blurb))
                    continue;

                GameObject newAudioInstance = Instantiate(audioGameObjectPrefab);
                newAudioInstance.transform.parent = audioSourceContainer;
                audioGameObjectReferences.Add(blurb, newAudioInstance);

                AudioSource newAudioSource = newAudioInstance.GetComponent<AudioSource>();
                newAudioSource.clip = blurb;
                audioSourceReferences.Add(blurb, newAudioSource);
            }
        }

        [ContextMenu("Blurb Once")]
        public void PlayAnyBlurb()
        {
            int blurb_index = Random.Range(0, blurbs.Count);
            if (blurb_index == last_blurb)
                blurb_index = Random.Range(0, blurbs.Count);
            last_blurb = blurb_index;

            AudioClip blurb = blurbs[blurb_index];
            audioSourceReferences[blurb].Play();
        }

        public void PlayBlurbs(int blurb_count, float time_between_blurbs) => StartCoroutine(BlurbCorountine(blurb_count, time_between_blurbs));

        [ContextMenu("Play Many Blurbs")]
        public void PlayBlurbs_Debug() => StartCoroutine(BlurbCorountine());

        IEnumerator BlurbCorountine(int blurb_count = 5, float time_between_blurbs = 0.25f)
        {
            WaitForSeconds wait_time = new WaitForSeconds(time_between_blurbs);

            for (int i = 0; i < blurb_count; i++)
            {
                this.PlayAnyBlurb();
                yield return wait_time;
            }

            yield return null;
        }

        void IAudioPlayer.Play() => PlayAnyBlurb();
    }
}