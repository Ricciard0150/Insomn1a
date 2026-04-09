using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PerguntaRPG
{
    public string pergunta;
    public string[] opcoes;
    public int respostaCorreta;

    [Header("Feedback")]
    public string feedbackCorreto;
    public string feedbackErrado;
}

public class SystemRPGnormal : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text perguntaText;
    public TMP_Text[] opcoesText;
    public TMP_Text feedbackText;

    [Header("Config")]
    public float velocidadeTexto = 0.03f;

    [Header("Sistema")]
    public int vidaMaxima = 3;
    public int acertosMinimos = 2;

    private int vidaAtual;
    private int pontos;
    private int rodada;

    private bool podeEscolher = false;
    private bool estaEscrevendo = false;
    public bool estaAtivo = false;

    [Header("Dialogo Final")]
    public DialogueRPG dialogueSystem;
    public string[] falasVitoria;
    public string[] falasDerrota;

    [Header("Respawn")]
    public GameObject player;
    public Transform pontoRespawn;

    [Header("Perguntas")]
    public PerguntaRPG[] perguntas;

    [Header("Objetos Pós-RPG")]
    public GameObject objetoAtivar;   
    public GameObject objetoDesativar; 
    void Update()
    {
        if (!estaAtivo) return;

        if (podeEscolher)
        {
            for (int i = 0; i < opcoesText.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    podeEscolher = false;
                    StartCoroutine(ProcessarEscolha(i));
                    break;
                }
            }
        }
    }

    public void IniciarSistema()
    {
        if (estaAtivo) return;

        estaAtivo = true;
        rodada = 0;
        pontos = 0;
        vidaAtual = vidaMaxima;

        panel.SetActive(true);
        StartCoroutine(MostrarRodada());
    }

    IEnumerator MostrarRodada()
    {
        podeEscolher = false;

        perguntaText.text = "";
        feedbackText.text = "";

        foreach (var txt in opcoesText)
            txt.text = "";

        var perguntaAtual = perguntas[rodada];

        yield return StartCoroutine(Escrever(perguntaText, perguntaAtual.pergunta));

        for (int i = 0; i < opcoesText.Length; i++)
        {
            if (i < perguntaAtual.opcoes.Length)
            {
                opcoesText[i].text = (i + 1) + " - " + perguntaAtual.opcoes[i];
                opcoesText[i].gameObject.SetActive(true);
            }
            else
            {
                opcoesText[i].gameObject.SetActive(false);
            }
        }

        podeEscolher = true;
    }

    IEnumerator ProcessarEscolha(int escolha)
    {
        var perguntaAtual = perguntas[rodada];

        if (escolha == perguntaAtual.respostaCorreta)
        {
            pontos++;

            yield return StartCoroutine(
                Escrever(feedbackText, perguntaAtual.feedbackCorreto)
            );
        }
        else
        {
            vidaAtual--;

            yield return StartCoroutine(
                Escrever(feedbackText,
                perguntaAtual.feedbackErrado + "\nVida: " + vidaAtual)
            );

            if (vidaAtual <= 0)
            {
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(Finalizar());
                yield break;
            }
        }

        yield return new WaitForSeconds(1f);

        rodada++;

        if (rodada < perguntas.Length)
            StartCoroutine(MostrarRodada());
        else
            yield return StartCoroutine(Finalizar());
    }

    IEnumerator Finalizar()
    {
        panel.SetActive(false);
        estaAtivo = false;

        // 👇 AQUI ACONTECE A TROCA
        if (objetoAtivar != null)
            objetoAtivar.SetActive(true);

        if (objetoDesativar != null)
            objetoDesativar.SetActive(false);

        if (pontos >= acertosMinimos && vidaAtual > 0)
        {
            yield return StartCoroutine(Vitoria());
        }
        else
        {
            yield return StartCoroutine(Derrota());
        }
    }

    IEnumerator Vitoria()
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.IniciarDialogo(falasVitoria);
            yield return new WaitWhile(() => dialogueSystem.EstaRodando());
        }
    }

    IEnumerator Derrota()
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.IniciarDialogo(falasDerrota);
            yield return new WaitWhile(() => dialogueSystem.EstaRodando());
        }

        // Respawn depois do diálogo
        if (player != null && pontoRespawn != null)
        {
            player.transform.position = pontoRespawn.position;
        }
    }

    IEnumerator Escrever(TMP_Text textoUI, string frase)
    {
        estaEscrevendo = true;
        textoUI.text = "";

        foreach (char letra in frase)
        {
            textoUI.text += letra;
            yield return new WaitForSeconds(velocidadeTexto);
        }

        estaEscrevendo = false;
    }
}