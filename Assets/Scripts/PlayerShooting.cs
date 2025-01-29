using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public float fireRate = 0.3f;
    private float nextFireTime = 0f;
    private PlayerInput playerInput;
    private InputAction fireAction;
    public Transform firePoint;
    public float bulletLifetime = 3f;

    public int bullets = 15;
    public TextMeshProUGUI bulletText;

    public static float bulletDamage = 1f;
    public TextMeshProUGUI damageText;

    private bool damageMultiplierAppliedThisFrame = false;
    private bool fireRatePickupCollectedThisFrame = false;

    void Update()
    {
        damageMultiplierAppliedThisFrame = false;
        fireRatePickupCollectedThisFrame = false;
    }

    void Awake()
    {
        playerInput = new PlayerInput();
        fireAction = playerInput.Player.Fire;
        fireAction.Enable();
    }

    void Start()
    {
        UpdateBulletText();
        UpdateDamageText();
    }

    void FixedUpdate()
    {
        if (fireAction.IsPressed() && Time.time > nextFireTime && bullets > 0)
        {
            Shoot(bulletLifetime);
            nextFireTime = Time.time + fireRate;
            bullets--;
            UpdateBulletText();
        }
    }

    void Shoot(float lifetime)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }
        else
        {
            Debug.LogError("Rigidbody не найден на пуле!");
        }

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.lifetime = lifetime;
        }
        else
        {
            Debug.LogError("Скрипт Bullet не найден на пуле!");
        }
    }

    public void AddBullets(int amount)
    {
        bullets += amount;
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        if (bulletText != null)
        {
            bulletText.text = "Пули: " + bullets.ToString();
        }
    }

    void UpdateDamageText()
    {
        if (damageText != null)
        {
            damageText.text = "Урон: " + bulletDamage.ToString();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireRatePickup") && !fireRatePickupCollectedThisFrame)
        {
            fireRate -= 0.05f;
            fireRate = Mathf.Max(0.1f, fireRate);
            fireRatePickupCollectedThisFrame = true;
            Destroy(other.gameObject); // Уничтожаем объект после подбора
        }
        else if (other.CompareTag("BulletPickup"))
        {
            AddBullets(10);
            Destroy(other.gameObject); // Уничтожаем объект после подбора
        }

        if (other.CompareTag("DamageMultiplier") && !damageMultiplierAppliedThisFrame) 
        {
            bulletDamage *= 2;
            damageMultiplierAppliedThisFrame = true; // Устанавливаем флаг
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("DamageDivider"))
        {
            bulletDamage /= 2;
            bulletDamage = Mathf.Max(1, bulletDamage);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("DamageAdder"))
        {
            bulletDamage += 2;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("DamageSubtractor"))
        {
            bulletDamage -= 2;
            bulletDamage = Mathf.Max(1, bulletDamage);
            Destroy(other.gameObject);
        }
        
        UpdateDamageText(); // Обновляем текст урона после любых изменений
    }
}