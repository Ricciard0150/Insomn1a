using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowJumpscare : MonoBehaviour
{
    public GameObject panel;
    public GameObject dialogueBox;
    public GameObject pressE;

    public TMP_Text texto;
    public Image fadePreto;

    public KeyCode tecla = KeyCode.E;
    public KeyCode lanterna = KeyCode.Space;

    public float velocidade = 0.03f;

    public string[] lines;

    bool isColliding = false;
    bool dialogoAtivo = false;
    bool escrevendo = false;
   

    int index = 0;

    public KeyItem ki;
    public TopDownMovement tdm;
    public BlurController bc;
    public Door door;

    private void Start()
    {
        pressE.SetActive(false);
        dialogueBox.SetActive(false);
        texto.text = "";
        fadePreto.color = new Color(0, 0, 0, 0);
      
    }

    private void Update()
    {
        
            if (Input.GetKeyDown(tecla) && isColliding && !dialogoAtivo && door.jaUsouComChave && ki.playerHasKey)
            {
                panel.SetActive(true);
                bc.AtivarBlur();
                pressE.SetActive(false);
                tdm.canMove = false;

            }
        

        if (Input.GetKeyDown(lanterna) && !dialogoAtivo)
        {
            bc.DesativarBlur();
            StartCoroutine(Sequencia());
        }

        if (dialogoAtivo && Input.GetKeyDown(KeyCode.E))
        {
            if (escrevendo)
            {
                StopAllCoroutines();
                texto.text = lines[index];
                escrevendo = false;
            }
            else
            {
                ProximaLinha();
            }
        }
    }

    IEnumerator Sequencia()
    {
        dialogoAtivo = true;

        yield return new WaitForSeconds(3f);

        dialogueBox.SetActive(true);

        index = 0;
        StartCoroutine(EscreverLinha());
    }

    void ProximaLinha()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(EscreverLinha());
        }
        else
        {
            StartCoroutine(FinalJumpscare());
        }
    }

    IEnumerator EscreverLinha()
    {
        texto.text = "";
        escrevendo = true;

        foreach (char letra in lines[index])
        {
            texto.text += letra;
            yield return new WaitForSeconds(velocidade);
        }

        escrevendo = false;
    }

    // 💀 PISCAR TELA
    IEnumerator PiscarTela()
    {
        for (int i = 0; i < 6; i++)
        {
            fadePreto.color = new Color(0, 0, 0, 1);
            yield return new WaitForSeconds(0.1f);

            fadePreto.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator FadeFinal()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            fadePreto.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }

    IEnumerator FinalJumpscare()
    {
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(PiscarTela());
        yield return StartCoroutine(FadeFinal());

        dialogueBox.SetActive(false);
        panel.SetActive(false);

        dialogoAtivo = false;
        tdm.canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            isColliding = true;
            pressE.SetActive(true);
        }
    }
}