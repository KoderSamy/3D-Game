using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 6f;
    public float laneDistance = 2f;
    public LayerMask groundLayer;
    public float laneChangeDuration = 0.5f; // Время смены полосы


    private Rigidbody rb;
    private int currentLane = 1;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector3 targetPosition;
    private bool isMovingToLane = false;
    private float laneChangeStartTime;
    private bool isGrounded;

    public float slideDuration = 0.5f;
    public float slideScaleFactor = 0.5f;
    public float scaleChangeSpeed = 5f;
    private bool isSliding = false;
    private Coroutine slideCoroutine;
    private Vector3 originalScale;
    private InputAction slideAction;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();
        moveAction = playerInput.Player.Move;
        jumpAction = playerInput.Player.Jump;

        slideAction = playerInput.Player.Slide;
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        jumpAction.performed += Jump;

        slideAction.Enable();
        slideAction.performed += Slide;
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        jumpAction.performed -= Jump;

        slideAction.Disable();
        slideAction.performed -= Slide;
    }

    void Start()
    {
        targetPosition = transform.position;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        // Движение вперед
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);

        Vector2 input = moveAction.ReadValue<Vector2>();

        // Перемещение по дорожкам
        if (input.x != 0)
        {
            int targetLane = currentLane + (int)Mathf.Sign(input.x);
            targetLane = Mathf.Clamp(targetLane, 0, 2);

            if (targetLane != currentLane && !isMovingToLane)
            {
                currentLane = targetLane;
                targetPosition.x = (currentLane - 1) * laneDistance;
                laneChangeStartTime = Time.time;
                isMovingToLane = true;
            }
        }

        if (isMovingToLane)
        {
            float elapsedTime = Time.time - laneChangeStartTime;
            float t = Mathf.Clamp01(elapsedTime / laneChangeDuration);
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), t);

            if (t >= 1f)
            {
                isMovingToLane = false;
            }
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    private void Slide(InputAction.CallbackContext context)
    {
        if (slideCoroutine == null && isGrounded)
        {
            slideCoroutine = StartCoroutine(SlideCoroutine());
        }
    }

    private System.Collections.IEnumerator SlideCoroutine()
    {
        isSliding = true;

        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * slideScaleFactor;

        while (elapsedTime < slideDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleChangeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;


        yield return new WaitForSeconds(slideDuration);

        elapsedTime = 0f;
        while (elapsedTime < slideDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleChangeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;

        isSliding = false;
        slideCoroutine = null;
    }


    void OnTriggerEnter(Collider other)
    {
        GameManager gameManager = FindObjectOfType<GameManager>(); // Не используйте (true), если GameManager не скрытый
        if (gameManager == null) 
        {
            Debug.LogError("GameManager не найден!");
            return; // Прекращаем выполнение метода, если GameManager не найден
        }


        if (other.CompareTag("Enemy1") || other.CompareTag("Obstacle") || (other.CompareTag("Enemy2") && !isSliding))
        {
            Die();
        }

        if (other.CompareTag("Coin"))
        {
            gameManager.IncreaseScore(1); // Используем найденный gameManager
            gameManager.objectPool.ReturnObject(other.gameObject); // Тоже используем найденный gameManager
        }

        if (other.CompareTag("BulletPickup"))
        {
            PlayerShooting shootingScript = GetComponent<PlayerShooting>();
            if (shootingScript != null)
            {
                shootingScript.AddBullets(10);
            }

            Destroy(other.gameObject); // Уничтожаем BulletPickup
        }
    }

    void Die()
    {
        GameManager gameManager = FindObjectOfType<GameManager>(); //  Не используйте (true), если GameManager не скрытый
        if (gameManager != null)
        {
            gameManager.ResetLevelScore(); // Сбрасываем счет уровня
        }
        PlayerShooting.bulletDamage = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //  Перезагружаем сцену по имени, а не по индексу
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.1f, 0.45f, groundLayer);
    }
}