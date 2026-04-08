using System.Collections;
using UnityEngine;
using TMPro;

public class BattleRPG : MonoBehaviour
{
    [Header("UI")]
    public GameObject door;
    public GameObject panel;
    public TMP_Text perguntaText;
    public TMP_Text opcao1Text;
    public TMP_Text opcao2Text;
    public TMP_Text feedbackText;

    [Header("Config")]
    public float velocidadeTexto = 0.03f;

    [HideInInspector] public bool estaAtivo = false;
    


    private int rodada = 0;
    private int pontos = 0;
    private bool podeEscolher = false;

    private bool estaEscrevendo = false;
    private string fraseAtual;
    private TMP_Text textoAtual;

    private string[] perguntas = {
        "Quais são seus defeitos no ambiente de trabalho?",
        "Qual as suas experiências no mercado?",
        "Qual o salário que você pretende ganhar?"
    };

    private string[,] opcoes = {
        { "Ah, sou perfeccionista demais", "Dificuldade de adaptação inicial" },
        { "Já trabalhei como escritor e roteirista", "Não muitas..." },
        { "O máximo que a empresa oferecer", "Um salário que caiba no orçamento da empresa para alguém que está começando" }
    };

    private int[] respostasCorretas = { 1, 0, 1 };

    void Update()
    {
        if (!estaAtivo) return;

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

        yield return StartCoroutine(Escrever(perguntaText, perguntas[rodada]));

        opcao1Text.text = "1 - " + opcoes[rodada, 0];
        opcao2Text.text = "2 - " + opcoes[rodada, 1];

        podeEscolher = true;
    }

    IEnumerator ProcessarEscolha(int escolha)
    {
        podeEscolher = false;

        if (escolha == respostasCorretas[rodada])
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

    // 🔥 sistema de texto com skip
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