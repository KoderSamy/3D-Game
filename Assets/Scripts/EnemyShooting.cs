using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f; // Скорость пули (метры в секунду)
    public float fireRate = 1f;
    public Transform firePoint;
    public float minLifetime = 3f;
    public float maxLifetime = 10f;


    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = -transform.forward * bulletSpeed; // Стреляем назад
            rb.useGravity = false; 

            float lifetime = Random.Range(minLifetime, maxLifetime);
            Destroy(bullet, lifetime); 
        }
        else
        {
            Debug.LogError("Rigidbody not found on bullet!");
        }
    }
}