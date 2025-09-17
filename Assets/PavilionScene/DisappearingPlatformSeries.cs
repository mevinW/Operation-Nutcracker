using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatformSeries : MonoBehaviour
{
    [Serializable]
    public class PlatformSettings
    {
        [Tooltip("The platform GameObject (with SpriteRenderer + Collider2D).")]
        public GameObject platform;

        [Tooltip("How long this platform stays visible.")]
        public float activeDuration = 1f;

        [Tooltip("How long to wait after hiding this platform before the next one appears.")]
        public float delayBeforeNext = 0.5f;
    }

    [Header("Sequence Settings")]
    [Tooltip("Configure each platform in order, with its timings.")]
    [SerializeField] private List<PlatformSettings> sequence = new List<PlatformSettings>();

    [Tooltip("How long the current and next platform overlap (both visible).")]
    [SerializeField] private float overlapDuration = 0.5f;

    [Tooltip("Start automatically looping the sequence on Awake?")]
    [SerializeField] private bool loop = true;

    [Tooltip("Optional delay before the first platform appears.")]
    [SerializeField] private float initialDelay = 0f;

    private void Awake()
    {
        // Hide all at start
        foreach (var entry in sequence)
            if (entry.platform != null)
                entry.platform.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        do
        {
            for (int i = 0; i < sequence.Count; i++)
            {

                var entry = sequence[i];
                if (entry.platform == null) continue;

                // Show current
                entry.platform.SetActive(true);

                if (i == 0)
                {
                    yield return new WaitForSeconds(overlapDuration);
                }

                // Wait until just before we want the overlap to start
                float beforeOverlap = Mathf.Max(0f, entry.activeDuration - overlapDuration);
                yield return new WaitForSeconds(beforeOverlap);

                // Kick off next (if any) so they overlap
                if (i + 1 < sequence.Count && sequence[i + 1].platform != null)
                    sequence[i + 1].platform.SetActive(true);

                // Finish current’s active time
                yield return new WaitForSeconds(overlapDuration);

                // Hide current
                entry.platform.SetActive(false);

                // Wait remaining "gap" before the next’s own delay
                yield return new WaitForSeconds(entry.delayBeforeNext);
            }
        }
        while (loop);
    }

    /// <summary>
    /// Call to restart the whole sequence from the top.
    /// </summary>
    public void Restart()
    {
        StopAllCoroutines();
        StartCoroutine(RunSequence());
    }
}
