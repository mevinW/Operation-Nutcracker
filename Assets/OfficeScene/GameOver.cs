using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private SceneChanger sceneChanger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.winner = 0;
            sceneChanger.loadEnd();
        }
        if (other.CompareTag("PlayerTwo"))
        {
            GameManager.Instance.winner = 1;
            sceneChanger.loadEnd();
        }
    }
}
