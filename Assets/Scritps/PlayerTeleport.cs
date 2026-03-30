using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [Header("booleans")]

    bool isColliding = false;

    [Header("UI")]

    public Transform teleportUI;

    void Update()
    {
        if (isColliding)
        {
            transform.SetParent(teleportUI);
            transform.localPosition = Vector3.zero;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IStatusPlayer status))
        {
            isColliding = true;
        }
    }
}
