using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    // public int TotalScore { get; private set; } = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bulletText;
    public GameObject coinPrefab;
    public GameObject bulletPickupPrefab;
    public GameObject fireRatePickupPrefab;
    public ObjectPool objectPool;
    public TextMeshProUGUI damageText;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            objectPool = FindObjectOfType<ObjectPool>();
            if (objectPool == null)
            {
                GameObject obj = new GameObject("ObjectPooler");
                objectPool = obj.AddComponent<ObjectPool>();
                DontDestroyOnLoad(obj); 
            }

            objectPool.Initialize(coinPrefab, coinPrefab.GetComponent<PoolSize>().poolSize);


            // TotalScore = PlayerPrefs.GetInt("TotalScore", 0);
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

        objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool не найден в Start()!");
        }
        score = 0;
        UpdateScoreText(); 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TextMeshProUGUI>();
        bulletText = GameObject.FindWithTag("BulletText")?.GetComponent<TextMeshProUGUI>();
        damageText = GameObject.FindWithTag("DamageText")?.GetComponent<TextMeshProUGUI>();

        if (scoreText == null) Debug.LogError("Score Text не найден!");
        if (bulletText == null) Debug.LogError("Bullet Text не найден!");
        if (damageText == null) Debug.LogError("Damage Text не найден!");

        objectPool = FindObjectOfType<ObjectPool>(); // <--  Добавляем эту строку
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool не найден после загрузки сцены!");
        }

        if (scene.name.StartsWith("Level"))
        {
            ResetLevelScore();
            PlayerShooting.bulletDamage = 1f;
        }
    }

    void OnDestroy()
    {
        // PlayerPrefs.SetInt("TotalScore", TotalScore);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        // TotalScore += amount;
        UpdateScoreText();
    }

    public void ResetLevelScore()
    {
        score = 0;
        UpdateScoreText();
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

    public void LoadResultsScene()
    {
        SceneManager.LoadScene("ResultsScene");
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Монеты: {score}"; 
        }
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // PlayerPrefs.SetInt("TotalScore", TotalScore);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Поздравляем! Вы прошли все уровни!");
            SceneManager.LoadScene("MainMenu");
        }
    }
}