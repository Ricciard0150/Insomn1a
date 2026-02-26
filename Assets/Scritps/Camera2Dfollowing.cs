using UnityEngine;

public class Camera2Dfollowing : MonoBehaviour
{
    public Transform target;

    public float smoothTime = -2.3f;

    private float velocityX = 0f;
    private float velocityY = 0f;

    void Update()
    {
        float posX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocityX, smoothTime);
        float posY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocityY, smoothTime);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
