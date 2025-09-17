using System.Collections;
using UnityEngine;
using Pathfinding;    // for AIDestinationSetter

public class Beehive : MonoBehaviour
{
    [Header("Bee Settings")]
    [Tooltip("Prefab must have Bee.cs, an AIDestinationSetter, a 2D Trigger Collider, and (optionally) a Rigidbody2D.")]
    [SerializeField] private GameObject beePrefab;

    [Tooltip("Minimum time between spawns (seconds).")]
    [SerializeField] private float minSpawnInterval = 2f;
    [Tooltip("Maximum time between spawns (seconds).")]
    [SerializeField] private float maxSpawnInterval = 5f;
    [Tooltip("How long each bee lives before auto‑destroy (seconds).")]
    [SerializeField] private float beeLifetime = 4f;  // set this to 4 in the Inspector

    [Header("Player References")]
    [Tooltip("Drag your Player One Transform here.")]
    [SerializeField] private Transform playerOne;
    [Tooltip("Drag your Player Two Transform here.")]
    [SerializeField] private Transform playerTwo;

    private void Start()
    {
        StartCoroutine(SpawnBees());
    }

    private IEnumerator SpawnBees()
    {
        while (true)
        {
            // 1) Wait a random time
            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(wait);

            // 2) Spawn the bee
            GameObject go = Instantiate(beePrefab, transform.position, Quaternion.identity);

            // 3) Find the closer player
            Transform target = playerOne;
            if (playerOne != null && playerTwo != null)
            {
                float d1 = Vector3.Distance(transform.position, playerOne.position);
                float d2 = Vector3.Distance(transform.position, playerTwo.position);
                target = (d2 < d1) ? playerTwo : playerOne;
            }
            else if (playerTwo != null)
            {
                target = playerTwo;
            }

            // 4) Assign the target to the AIDestinationSetter
            var setter = go.GetComponent<AIDestinationSetter>();
            if (setter != null)
            {
                setter.target = target;
            }

            // 5) Destroy after lifetime
            Destroy(go, beeLifetime);
        }
    }
}
