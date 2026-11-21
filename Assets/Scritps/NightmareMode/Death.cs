using System.Collections;
using UnityEngine;
using System;

public class Death : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Tempo que a morte fica na tela antes de desaparecer")]
    [SerializeField] private float tempoNaTela = 3f;

    private bool triggerAtivado = false;

    public event Action OnDeathDesapareceu;

    void Awake()
    {
        gameObject.SetActive(true);
    }

    public void IniciarDesaparecimento()
    {
        if (triggerAtivado) return;
        triggerAtivado = true;

        StartCoroutine(DesaparecerRoutine());
    }

    private IEnumerator DesaparecerRoutine()
    {
        yield return new WaitForSecondsRealtime(tempoNaTela);

        gameObject.SetActive(false);

        OnDeathDesapareceu?.Invoke();
    }
}