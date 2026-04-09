using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionBed : MonoBehaviour
{
    [Header("Config")]
    public string nomeDaCena;
    public KeyCode teclaInteragir = KeyCode.E;
    public GameObject pressE; 

    [Header("Fade")]
    public Image fadeImage; // imagem preta na tela
    public float duracaoFade = 2f;

    private bool playerPerto = false;
    private bool transicaoAtiva = false;

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(teclaInteragir) && !transicaoAtiva)
        {
            StartCoroutine(Transicao());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerPerto = true;
            pressE.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerPerto = false;
            pressE.SetActive(false);
        }
    }

    IEnumerator Transicao()
    {
        transicaoAtiva = true;

        // Fade para preto
        float tempo = 0f;
        Color cor = fadeImage.color;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            float alpha = tempo / duracaoFade;

            fadeImage.color = new Color(cor.r, cor.g, cor.b, alpha);
            yield return null;
        }

        // Garante que ficou totalmente preto
        fadeImage.color = new Color(cor.r, cor.g, cor.b, 1f);

        // Carrega cena
        SceneManager.LoadScene(nomeDaCena);
    }
}
