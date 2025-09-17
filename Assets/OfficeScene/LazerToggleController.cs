using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserToggleController : MonoBehaviour
{
    [System.Serializable]
    public class LaserPhase
    {
        [Tooltip("Name this phase (for your reference)")]
        public string phaseName;

        [Tooltip("Which lasers to turn OFF during this phase.")]
        public List<GameObject> lasersToDisable = new List<GameObject>();
    }

    [Header("All Lasers (for lookup)")]
    [Tooltip("Drag every laser here so we can turn them back on each phase.")]
    [SerializeField] private List<GameObject> allLasers = new List<GameObject>();

    [Header("Phase Setup")]
    [Tooltip("Define your sequence of phases. Each phase lists which lasers to disable.")]
    [SerializeField] private List<LaserPhase> phases = new List<LaserPhase>();

    [Header("Timing")]
    [Tooltip("Seconds between each phase transition.")]
    [SerializeField] private float phaseInterval = 1f;

    private int currentPhase = 0;

    private void Start()
    {
        if (allLasers.Count == 0 || phases.Count == 0)
        {
            Debug.LogWarning("LaserToggleController: Need at least one laser and one phase defined!");
            enabled = false;
            return;
        }

        StartCoroutine(PhaseRoutine());
    }

    private IEnumerator PhaseRoutine()
    {
        while (true)
        {
            // Turn all lasers ON
            foreach (var laser in allLasers)
                laser.SetActive(true);

            // Disable only the lasers in the current phase
            foreach (var offLaser in phases[currentPhase].lasersToDisable)
                offLaser.SetActive(false);

            // Advance to next phase (wrap if needed)
            currentPhase = (currentPhase + 1) % phases.Count;

            yield return new WaitForSeconds(phaseInterval);
        }
    }
}
