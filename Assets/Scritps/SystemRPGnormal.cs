using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PerguntaRPG
{
    public string pergunta;        // Pergunta que aparece na tela
    public string[] opcoes;        // Array de opções (mínimo 2)
    public int respostaCorreta;    // Índice da opção correta (0 ou 1)
}

public class SystemRPGnormal : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text perguntaText;
    public TMP_Text opcao1Text;
    public TMP_Text opcao2Text;
    public TMP_Text feedbackText;
    public GameObject door;

    [Header("Config")]
    public float velocidadeTexto = 0.03f;

    [HideInInspector] public bool estaAtivo = false;

    [Header("Perguntas")]
    public PerguntaRPG[] perguntas;

    private int rodada = 0;
    private int pontos = 0;
    private bool podeEscolher = false;

    private bool estaEscrevendo = false;
    private string fraseAtual;
    private TMP_Text textoAtual;

    void Update()
    {
        if (!estaAtivo) return;

        // Skip do texto
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (estaEscrevendo)
            {
                estaEscrevendo = false;
            }
        }

        if (podeEscolher)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(ProcessarEscolha(0));
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ProcessarEscolha(1));
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
        opcao1Text.text = "";
        opcao2Text.text = "";
        feedbackText.text = "";

        // Valida se tem pelo menos 2 opções
        if (perguntas[rodada].opcoes.Length < 2)
        {
            Debug.LogError("Cada pergunta precisa ter pelo menos 2 opções!");
            yield break;
        }

        yield return StartCoroutine(Escrever(perguntaText, perguntas[rodada].pergunta));

        opcao1Text.text = "1 - " + perguntas[rodada].opcoes[0];
        opcao2Text.text = "2 - " + perguntas[rodada].opcoes[1];

        podeEscolher = true;
    }

    IEnumerator ProcessarEscolha(int escolha)
    {
        podeEscolher = false;

        if (escolha == perguntas[rodada].respostaCorreta)
        {
            pontos++;
            yield return StartCoroutine(Escrever(feedbackText, "Parece convincente!"));
        }
        else
        {
            pontos--;
            yield return StartCoroutine(Escrever(feedbackText, "Eh, interessante..."));
        }

        yield return new WaitForSeconds(1f);

        rodada++;

        if (rodada < perguntas.Length)
        {
            StartCoroutine(MostrarRodada());
        }
        else
        {
            yield return StartCoroutine(Finalizar());
        }
    }

    IEnumerator Finalizar()
    {
        perguntaText.text = "";
        opcao1Text.text = "";
        opcao2Text.text = "";

        string resultado = pontos > 1
            ? "Muito obrigado por comparecer. Seu resultado será comunicado com a atendente na saída"
            : "Obrigado, mas há outros candidatos à frente.";

        yield return StartCoroutine(Escrever(feedbackText, resultado));

        yield return new WaitForSeconds(2f);

        panel.SetActive(false);
        estaAtivo = false;
        Destroy(door);
    }

    // 🔥 Sistema de texto com skip
    IEnumerator Escrever(TMP_Text textoUI, string frase)
    {
        estaEscrevendo = true;
        fraseAtual = frase;
        textoAtual = textoUI;

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