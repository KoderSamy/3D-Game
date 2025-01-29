using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Персонаж
    public float smoothSpeed = 0.125f; // Скорость следования
    public Vector3 offset; // Смещение камеры относительно персонажа

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target); // Если нужно, чтобы камера смотрела на персонажа
    }
}