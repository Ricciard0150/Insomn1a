using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public KeyItem key;
    public KeyCode tecla = KeyCode.E;

    private bool playerNear = false;
    public bool jaUsouComChave = false;
    public bool test = false;

    public GameObject wind;

    public CameraShake camShake;
    public TypeDialogue dialogue;
    public TopDownMovement playerMovement;
    public Camera2Dfollowing cameraFollow;

    [Header("Som Batida")]
    public AudioSource audioSource;
    public AudioClip somBatida;

    [TextArea] public string lineSemChave;
    [TextArea] public string lineComChave;

    private void Start()
    {
        wind.SetActive(true);
    }
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
                Debug.Log("true");
                test = true;    
            }
        }
    }

    IEnumerator PrimeiraVez()
    {
        jaUsouComChave = true;

        playerMovement.canMove = false;
        cameraFollow.canFollow = false;

        // 🔊 TOCA O SOM AQUI
        audioSource.PlayOneShot(somBatida);

        // 💥 SHAKE
        yield return StartCoroutine(camShake.Shake(0.2f, 0.3f));

        cameraFollow.canFollow = true;

        dialogue.IniciarDialogo(lineComChave);

        yield return new WaitUntil(() => !dialogue.IsAtivo());

        wind.SetActive(false);
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