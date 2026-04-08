using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public KeyItem key;
    public KeyCode tecla = KeyCode.E;

    private bool playerNear = false;
    private bool jaUsouComChave = false;

    public CameraShake camShake;
    public TypeDialogue dialogue;
    public TopDownMovement playerMovement;
    public Camera2Dfollowing cameraFollow;

    [TextArea] public string lineSemChave;
    [TextArea] public string lineComChave;

    void Update()
    {
        if (!playerNear) return;

        if (Input.GetKeyDown(tecla))
        {
            if (dialogue.IsAtivo()) return;

            if (!key.playerHasKey)
            {
                dialogue.IniciarDialogo(lineSemChave);
            }
            else
            {
                if (!jaUsouComChave)
                    StartCoroutine(PrimeiraVez());
                else
                    dialogue.IniciarDialogo(lineComChave);
            }
        }
    }

    IEnumerator PrimeiraVez()
    {
        jaUsouComChave = true;

        playerMovement.canMove = false;
        cameraFollow.canFollow = false;

        yield return StartCoroutine(camShake.Shake(0.3f, 0.2f));

        cameraFollow.canFollow = true;

        dialogue.IniciarDialogo(lineComChave);

        yield return new WaitUntil(() => !dialogue.IsAtivo());

        playerMovement.canMove = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
            playerNear = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            playerNear = false;

            if (dialogue.IsAtivo())
            {
                dialogue.Fechar();
                playerMovement.canMove = true;
            }
        }
    }
}