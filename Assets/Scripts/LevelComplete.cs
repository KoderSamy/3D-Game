using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour
{
    public GameObject levelCompleteUI;
    public TextMeshProUGUI coinsText;
    public Button buyPassButton;
    public TextMeshProUGUI tooPoorText;

    void Start()
    {
        levelCompleteUI.SetActive(false); // Скрываем UI в начале
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelCompleted();
        }
    }

    void LevelCompleted()
    {
        levelCompleteUI.SetActive(true);
        Time.timeScale = 0f; // Пауза игры

        GameManager gameManager = GameObject.FindObjectOfType<GameManager>(true);
        if (gameManager != null)
        {
            coinsText.text = "Собрано " + gameManager.Score + " монет."; // Обновляем текст

            // Логика отображения/скрытия элементов
            if (gameManager.score >= 8)
            {
                buyPassButton.gameObject.SetActive(true); // Показываем кнопку "Купить проход"
                tooPoorText.gameObject.SetActive(false);   // Скрываем текст "Вы слишком бедны"
            }
            else
            {
                buyPassButton.gameObject.SetActive(false);  // Скрываем кнопку
                tooPoorText.gameObject.SetActive(true);    // Показываем текст
            }
        }
        else
        {
            Debug.LogError("GameManager не найден!");
            // Возможно, здесь нужно деактивировать UI результатов, если GameManager не найден
            levelCompleteUI.SetActive(false); 
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>(true);
        if (gameManager != null)
        {
            gameManager.ResetScore();
        }
        PlayerShooting.bulletDamage = 1f; // Сброс урона здесь
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BuyPass()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>(true);
        if (gameManager != null)
        {
            gameManager.score -= 8;
            gameManager.UpdateScoreText();

             int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

           if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                 SceneManager.LoadScene(nextSceneIndex); // Загружаем следующую сцену
            }
           else
           {
            // Здесь можно добавить логику для последнего уровня (например, загрузить сцену с боссом)
            Debug.Log("Поздравляем! Вы прошли все уровни!");
            // Загрузите сцену с боссом или покажите экран победы
           }

        }
        Time.timeScale = 1f;
    }
}