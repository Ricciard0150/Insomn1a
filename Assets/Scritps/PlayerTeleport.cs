using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class PlayerTeleport : MonoBehaviour
{
    public Transform target;
    public Transform teleportUI;
    public PlayableDirector director;

    public float delay = 0.5f; 

    bool isTeleporting = false;

    private void Start()
    {
        director.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            if (!isTeleporting)
            {
                StartCoroutine(TeleportWithDelay());
            }
        }
    }

    IEnumerator TeleportWithDelay()
    {
        isTeleporting = true;

        yield return new WaitForSeconds(delay);

        target.position = teleportUI.position;
        director.Play();

        isTeleporting = false;
    }
}