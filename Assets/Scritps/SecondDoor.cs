using UnityEngine;

public class SecondDoor : MonoBehaviour
{
    CollectableItem ci;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent(out IStatusPlayer status) && ci.playerHasKey)
            return;
        
        Destroy(gameObject);
    }
}
