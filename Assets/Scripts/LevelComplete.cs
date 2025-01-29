using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour
{
    public GameObject levelCompleteUI;
    public TextMeshProUGUI coinsText;
    public Button buyPassButton;
    public Button restartButton;
    public TextMeshProUGUI tooPoorText;


    void Start()
    {
        levelCompleteUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelCompleted();//-------------------------------------------------------------------------------------
        }
    }

    void LevelCompleted()
    {
        levelCompleteUI.SetActive(true);
        Time.timeScale = 0f; // Останавливаем игру

        if (GameManager.Instance != null)
        {
            // Отображаем счет текущего уровня
            coinsText.text = $"Собрано монет: {GameManager.Instance.score}";

            int scoreAfterPurchase = GameManager.Instance.score - 8;

            // Показываем или скрываем кнопку покупки пропуска в зависимости от счета
            buyPassButton.gameObject.SetActive(scoreAfterPurchase >= 0);
            tooPoorText.gameObject.SetActive(scoreAfterPurchase < 0);

            restartButton.gameObject.SetActive(true);// -----------------------------------------------------------------
        }
        else
        {
            Debug.LogError("GameManager не найден!");
            levelCompleteUI.SetActive(false);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Возобновляем игру
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetLevelScore(); // Сбрасываем счет уровня
        }
        PlayerShooting.bulletDamage = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BuyPass()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.score -= 8; // Вычитаем стоимость пропуска
            GameManager.Instance.UpdateScoreText(); // Обновляем текст

            GameManager.Instance.LoadNextLevel(); // Переходим на следующий уровень
        }
    }
}