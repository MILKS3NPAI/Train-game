using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterKatLLC.AudioManagement
{
    public class Clip_Randomizer_Mono : MonoBehaviour, IAudioPlayer
    {
        [Header("Dependencies")]
        [SerializeField] private GameObject audioGameObjectPrefab;
        [SerializeField] private Transform audioSourceContainer;

        [Space(10)]

        [Header("Configuration")]
        [SerializeField] private int default_blurb_count = 5;
        [SerializeField] private float default_time_between_blurbs = 0.25f;

        [Space()]
        [SerializeField] private bool wait_for_last_blurb_to_end = true;

        [Space()]

        [SerializeField] private List<AudioClip> blurbs = new List<AudioClip>();

        //internal
        private Dictionary<AudioClip, GameObject> audioGameObjectReferences = new Dictionary<AudioClip, GameObject>();
        private Dictionary<AudioClip, AudioSource> audioSourceReferences = new Dictionary<AudioClip, AudioSource>();

        private AudioSource last_blurber = null;
        private float time_last_blurbed = 0;

        private int blurb_random_index = 0;
        private int[] blurb_random_index_array;

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

            blurb_random_index_array = new int[blurbs.Count];
            for (int i = 0; i < blurb_random_index_array.Length; i++)
            {
                blurb_random_index_array[i] = i;
            }
            Shuffle_Index_Array();
            Shuffle_Index_Array();
            Shuffle_Index_Array();
        }

        private void Shuffle_Index_Array()
        {
            for (int i = 0; i< blurb_random_index_array.Length; i++)
            {
                int cur_value = blurb_random_index_array[i];
                int next_value_index = Random.Range(0, blurb_random_index_array.Length);
                blurb_random_index_array[i] = blurb_random_index_array [next_value_index];
                blurb_random_index_array[next_value_index] = cur_value;
            }
        }

        [ContextMenu("Blurb Once")]
        public void PlayAnyBlurb()
        {
            //Get random blurb from grab bag
            int blurb_index = blurb_random_index_array[blurb_random_index];
            blurb_random_index += 1;
            if (blurb_random_index >= blurb_random_index_array.Length)
            {
                Shuffle_Index_Array();
                blurb_random_index = 0;
            }
            Debug.Log(blurb_index);
            //Play Blurb
            AudioClip blurb = blurbs[blurb_index];
            last_blurber = audioSourceReferences[blurb];
            audioSourceReferences[blurb].Play();
        }

        public void PlayBlurbs() => PlayBlurbs(default_blurb_count, default_time_between_blurbs);
        public void PlayBlurbs(int blurb_count, float time_between_blurbs) => StartCoroutine(BlurbCorountine(blurb_count, time_between_blurbs));

        IEnumerator BlurbCorountine(int blurb_count, float time_between_blurbs)
        {
            WaitForSeconds wait_time = new WaitForSeconds(time_between_blurbs);

            for (int i = 0; i < blurb_count; i++)
            {
                this.PlayAnyBlurb();
                time_last_blurbed = Time.time;

                if (wait_for_last_blurb_to_end)
                {
                    while (last_blurber.isPlaying)
                    {
                        yield return null;
                    }
                }
                
                while (Time.time - time_last_blurbed < time_between_blurbs)
                {
                    yield return null;
                }

                yield return null;
            }

            yield return null;
        }

        void IAudioPlayer.Play() => PlayAnyBlurb();

        [ContextMenu("Play Many Blurbs")]
        public void PlayBlurbs_Debug() => PlayBlurbs();
    }
}