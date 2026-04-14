using System;
using System.Collections;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class WindowReflection : MonoBehaviour
{
    public GameObject window;
    public GameObject dialogueBox;
    public TMP_Text texto;

    WindowJumpscare wj;

    public KeyCode tecla = KeyCode.E;

    public string[] lines;
    int index = 0;
    public float velocidade = 0.03f;


    private bool isColliding = false;
    private bool dialogoAtivo = false;
    private bool escrevendo = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        window.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(tecla) && isColliding){
            window.SetActive(true);
            if (!dialogoAtivo)
            {
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
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            isColliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            isColliding = false;
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

}
