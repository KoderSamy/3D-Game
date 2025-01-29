using UnityEngine;

public class SpawnCoinsOnStart : MonoBehaviour
{
    public Vector3 spawnPosition;

    void Start()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>(true);
        if (gameManager != null)
        {
            gameManager.SpawnCoin(spawnPosition);
        }
        else
        {
            Debug.LogError("GameManager не найден!");
        }
    }
}