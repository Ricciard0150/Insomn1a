using System.Collections;
using UnityEngine;
using System;

public class Death : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Tempo que a morte fica na tela antes de desaparecer")]
    [SerializeField] private float tempoNaTela = 3f;

    private bool triggerAtivado = false;

    // ✅ Evento para avisar que a morte sumiu
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
        // 1. Espera o tempo na tela
        yield return new WaitForSecondsRealtime(tempoNaTela);

        // 2. Desativa a morte
        gameObject.SetActive(false);

        // 3. Avisa que sumiu
        OnDeathDesapareceu?.Invoke();
    }
}