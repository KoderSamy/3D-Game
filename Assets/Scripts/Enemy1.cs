using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public int health;
    public TextMesh healthText;
    private bool isDead = false;

    private void Awake()
    {
        healthText = GetComponentInChildren<TextMesh>();
    }

    private void OnEnable()
    {
        isDead = false;
    }

    void Start()
    {
        UpdateHealthText();
    }


    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Enemy collided with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Bullet"))
        {
            TakeDamage();
            // Debug.Log("Enemy hit by bullet"); 
        }
    }

    public void TakeDamage()
    {
        health -= (int)PlayerShooting.bulletDamage; // Use bulletDamage here!
        UpdateHealthText();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Destroy(gameObject); // Просто уничтожаем врага
    }


    private void UpdateHealthText()
    {
       if (healthText != null)
        {
             healthText.text = health.ToString();
        }
        else
        {
           Debug.LogError("HealthText is null on " + gameObject.name);
       }
    }
}