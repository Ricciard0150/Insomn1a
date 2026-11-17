using System.Collections;
using UnityEngine;

public class Death : MonoBehaviour
{
    [Header("Configurações do Flash")]
    [SerializeField] private float totalDuration = 1.5f; // Tempo que o susto fica piscando
    [SerializeField] private float minBlinkTime = 0.04f; // Tempo mínimo aparecendo
    [SerializeField] private float maxBlinkTime = 0.15f; // Tempo máximo aparecendo
    [SerializeField] private bool distortScale = true; // Efeito de distorcer o tamanho (opcional)

    private Vector3 originalScale;
    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
        originalScale = myTransform.localScale;

        // Começa desativado
        gameObject.SetActive(false);
    }

    // Chame esta função para o susto acontecer
    public void TriggerFlash()
    {
        gameObject.SetActive(true);
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        // Loop do susto (piscando)
        while (timer < totalDuration)
        {
            // 1. APARECE
            gameObject.SetActive(true);

            if (distortScale)
            {
                // Muda o tamanho levemente pra dar agonia
                float randomScale = Random.Range(0.95f, 1.1f);
                myTransform.localScale = originalScale * randomScale;
            }

            // Espera um tempo mínimo (piscada rápida)
            float waitOn = Random.Range(minBlinkTime, maxBlinkTime);
            yield return new WaitForSecondsRealtime(waitOn);
            timer += waitOn;

            // 2. SOME
            gameObject.SetActive(false);

            // Espera um tempo mínimo (sumiço rápido)
            float waitOff = Random.Range(minBlinkTime, maxBlinkTime);
            yield return new WaitForSecondsRealtime(waitOff);
            timer += waitOff;
        }

        // 3. FINAL: Garante que sumiu e reseta
        myTransform.localScale = originalScale;

        // O objeto já está desativado pelo loop, mas garantimos aqui
        gameObject.SetActive(false);
    }
}