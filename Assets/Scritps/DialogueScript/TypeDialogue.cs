using UnityEngine;
using TMPro;

public class TypeDialogue : MonoBehaviour
{
    public GameObject window;
    public GameObject dialogueBox;
    public TextMeshProUGUI texto;
    public float velocidade = 0.05f;

    private string linhaAtual;
    private int charIndex;

    private float tempo;
    private bool escrevendo = false;
    private bool ativo = false;

    private float inputDelay = 0.2f;

    private void Start()
    {
        window.SetActive(false);
    }
    public void IniciarDialogo(string linha)
    {
        if (string.IsNullOrEmpty(linha)) return;

        linhaAtual = linha;

        texto.text = "";
        charIndex = 0;

        dialogueBox.SetActive(true);

        ativo = true;
        escrevendo = true;
        tempo = 0f;

        inputDelay = 0.2f;
    }

    void Update()
    {
        if (!ativo) return;

        // evita bug do primeiro clique
        if (inputDelay > 0)
        {
            inputDelay -= Time.deltaTime;
            return;
        }

        // DIGITAÇÃO
        if (escrevendo)
        {
            tempo += Time.deltaTime;

            if (tempo >= velocidade)
            {
                tempo = 0f;

                if (charIndex < linhaAtual.Length)
                {
                    texto.text += linhaAtual[charIndex];
                    charIndex++;
                }
                else
                {
                    escrevendo = false;
                }
            }
        }

       // 👉 TECLA E (avançar / completar)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (escrevendo)
            {
                texto.text = linhaAtual;
                escrevendo = false;
            }
        }

        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Fechar();
            window.SetActive(true);
           
        }
    }

    public void Fechar()
    {
        dialogueBox.SetActive(false);
        ativo = false;
    }

    public bool IsAtivo()
    {
        return ativo;
    }
}