using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [Header("booleans")]

    bool isColliding = false;

    [Header("UI")]

    public Transform target;
    public Transform teleportUI;

    void Update()
    {
        if (isColliding)
        {
            target.position = teleportUI.position;
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
