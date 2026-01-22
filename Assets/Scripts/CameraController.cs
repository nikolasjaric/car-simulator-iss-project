using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -6);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float forwardLookOffset = 5f;

    private Rigidbody playerRB;

    void Awake()
    {
        if (player != null)
            playerRB = player.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (player == null || playerRB == null)
            return;

        Vector3 playerForward =
            (playerRB.linearVelocity + player.forward).normalized;

        Vector3 targetPosition =
            player.position +
            player.TransformVector(offset) +
            playerForward * -forwardLookOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(player.position);
    }
}
