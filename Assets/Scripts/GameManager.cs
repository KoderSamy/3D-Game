using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bulletText;
    public GameObject coinPrefab;
    public GameObject bulletPickupPrefab;
    public GameObject fireRatePickupPrefab;
    public ObjectPool objectPool;
    public TextMeshProUGUI damageText;
    public int Score => score;


    void Awake()
    {
        Debug.Log("GameManager.Awake()");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Инициализация ObjectPool ЗДЕСЬ, один раз!
            objectPool = FindFirstObjectByType<ObjectPool>();
            if (objectPool == null)
            {
                Debug.LogError("ObjectPool не найден на сцене!");
            }
            else
            {
                objectPool.Initialize(coinPrefab, coinPrefab.GetComponent<PoolSize>().poolSize);
                // objectPool.Initialize(bulletPickupPrefab, bulletPickupPrefab.GetComponent<PoolSize>().poolSize);
                // objectPool.Initialize(fireRatePickupPrefab, fireRatePickupPrefab.GetComponent<PoolSize>().poolSize);
            }


        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void Start()
    {
        Debug.Log("GameManager.Start()");
        if (Instance == null)
        {
            score = 0;
            UpdateScoreText();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("GameManager.OnSceneLoaded(): " + scene.name);
        scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TextMeshProUGUI>();
        bulletText = GameObject.FindWithTag("BulletText")?.GetComponent<TextMeshProUGUI>();
        damageText = GameObject.FindWithTag("DamageText")?.GetComponent<TextMeshProUGUI>();

        if (scoreText == null) Debug.LogError("Score Text не найден!");
        if (bulletText == null) Debug.LogError("Bullet Text не найден!");
        if (damageText == null) Debug.LogError("Damage Text не найден!");

        if (scene.buildIndex != 0)
        { // Проверяем, что это не стартовая сцена.
            PlayerShooting.bulletDamage = 1f; // Обнуляем только на следующих уровнях.
        }

    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Монеты: {score}";
        }
    }

    public void SpawnCoin(Vector3 position)
    {
        GameObject coin = objectPool.GetPooledObject(coinPrefab);
        if (coin != null)
        {
            coin.transform.position = position;
            coin.SetActive(true);
        }
    }

    public void SpawnBulletPickup(Vector3 position)
    {
        GameObject pickup = objectPool.GetPooledObject(bulletPickupPrefab);
        if (pickup != null)
        {
            pickup.transform.position = position;
            pickup.SetActive(true);
        }
    }

    public void SpawnFireRatePickup(Vector3 position)
    {
        GameObject pickup = objectPool.GetPooledObject(fireRatePickupPrefab);
        if (pickup != null)
        {
            pickup.transform.position = position;
            pickup.SetActive(true);
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
}