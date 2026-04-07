using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followDamping = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followDamping * Time.deltaTime);
    }
}
