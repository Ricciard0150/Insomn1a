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

    [HideInInspector] public bool estaAtivo = false;

    [Header("Perguntas")]
    public PerguntaRPG[] perguntas;

    private int rodada = 0;
    private int pontos = 0;
    private bool podeEscolher = false;

    private bool estaEscrevendo = false;

    void Update()
    {
        if (!estaAtivo) return;

        // Skip do texto
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (estaEscrevendo)
                estaEscrevendo = false;
        }

        if (podeEscolher)
        {
            for (int i = 0; i < opcoesText.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    StartCoroutine(ProcessarEscolha(i));
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

        panel.SetActive(true);
        StartCoroutine(MostrarRodada());
    }

    IEnumerator MostrarRodada()
    {
        podeEscolher = false;

        perguntaText.text = "";
        feedbackText.text = "";

        // Limpa todas opções
        foreach (var txt in opcoesText)
            txt.text = "";

        var perguntaAtual = perguntas[rodada];

        if (perguntaAtual.opcoes.Length == 0)
        {
            Debug.LogError("A pergunta precisa ter opções!");
            yield break;
        }

        yield return StartCoroutine(Escrever(perguntaText, perguntaAtual.pergunta));

        // Preenche dinamicamente
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
        podeEscolher = false;

        var perguntaAtual = perguntas[rodada];

        if (escolha == perguntaAtual.respostaCorreta)
        {
            pontos++;
            yield return StartCoroutine(Escrever(feedbackText, perguntaAtual.feedbackCorreto));
        }
        else
        {
            pontos--;
            yield return StartCoroutine(Escrever(feedbackText, perguntaAtual.feedbackErrado));
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
        perguntaText.text = "";
        foreach (var txt in opcoesText)
            txt.text = "";

        string resultado = pontos > 1
            ? "Muito obrigado por comparecer. Seu resultado será comunicado com a atendente na saída"
            : "Obrigado, mas há outros candidatos à frente.";

        yield return StartCoroutine(Escrever(feedbackText, resultado));

        yield return new WaitForSeconds(2f);

        panel.SetActive(false);
        estaAtivo = false;
    }

    IEnumerator Escrever(TMP_Text textoUI, string frase)
    {
        estaEscrevendo = true;
        textoUI.text = "";

        foreach (char letra in frase)
        {
            if (!estaEscrevendo)
            {
                textoUI.text = frase;
                yield break;
            }

            textoUI.text += letra;
            yield return new WaitForSeconds(velocidadeTexto);
        }

        estaEscrevendo = false;
    }
}