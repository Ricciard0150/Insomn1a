using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionBed : MonoBehaviour
{
    [Header("Config")]
    public string sceneName;                 // cena de destino
    public string idDoSpawn;                 // ID do SpawnPoint na cena de destino
    public KeyCode interactKey = KeyCode.E;
    public GameObject pressE;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    private bool playerNear = false;
    private bool transitionActive = false;

    void Start()
    {
        // Garante que o fade começa transparente
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, 0f);
        }

        if (pressE != null)
            pressE.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !transitionActive && Input.GetKeyDown(interactKey))
        {
            StartCoroutine(Transition());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IStatusPlayer player)) return;

        playerNear = true;
        if (pressE != null)
            pressE.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IStatusPlayer player)) return;

        playerNear = false;
        if (pressE != null)
            pressE.SetActive(false);
    }

    IEnumerator Transition()
    {
        transitionActive = true;

        SpawnManager.proximoID = idDoSpawn;

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Clamp01(time / fadeDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        }

        SceneManager.LoadScene(sceneName);
    }
}