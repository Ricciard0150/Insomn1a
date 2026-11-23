using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterPickupPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image blackFade;
    [SerializeField] private Image[] letterSlots = new Image[3];
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text counterText;

    [Header("Audio")]
    [SerializeField] private AudioSource crackAudioSource;
    [SerializeField] private AudioClip crackSound;

    [Header("Timing")]
    [SerializeField] private float fadeIn = 0.5f;
    [SerializeField] private float fadeOut = 0.5f;

    [Header("Efeito Final (Crack)")]
    [SerializeField] private float tempoParaCrack = 1.5f;
    [SerializeField] private float tempoSeparadas = 5f;
    [SerializeField] private float velocidadeMovimento = 2f;

    [Header("Ajuste do Crack (Posições e Rotações)")]
    [SerializeField] private Vector2 posFinalCarta1 = new Vector2(-80f, 100f);
    [SerializeField] private float rotFinalCarta1 = 15f;

    [SerializeField] private Vector2 posFinalCarta2 = new Vector2(120f, 0f);
    [SerializeField] private float rotFinalCarta2 = -5f;

    [SerializeField] private Vector2 posFinalCarta3 = new Vector2(-50f, -120f);
    [SerializeField] private float rotFinalCarta3 = 20f;

    [Header("Input")]
    [SerializeField] private string interactButton = "Interact";

    private Coroutine currentRoutine;
    private Vector2[] posicoesOriginais;
    private Quaternion[] rotacoesOriginais;
    private bool podeFechar = false;
    private bool isFinalCard = false;

    // ✅ FIX B: callback direto passado no Show
    private Action _onClosedCallback;

    // Mantido por compatibilidade com quem ainda usa o evento
    public event Action OnPanelClosed;

    void Awake()
    {
        SetAlpha(blackFade, 0f);

        posicoesOriginais = new Vector2[letterSlots.Length];
        rotacoesOriginais = new Quaternion[letterSlots.Length];

        for (int i = 0; i < letterSlots.Length; i++)
        {
            if (letterSlots[i] != null)
            {
                posicoesOriginais[i] = letterSlots[i].rectTransform.anchoredPosition;
                rotacoesOriginais[i] = letterSlots[i].rectTransform.localRotation;

                letterSlots[i].enabled = false;
                SetAlpha(letterSlots[i], 0f);
            }
        }

        SetTextAlpha(itemNameText, 0f);
        SetTextAlpha(counterText, 0f);
    }

    void Update()
    {
        // ✅ Se não for a última carta e o jogador apertar o botão, fecha na hora
        if (podeFechar && !isFinalCard && Input.GetButtonDown(interactButton))
        {
            podeFechar = false;
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            StartCoroutine(FecharAntesDoTempo());
        }
    }

    /// <summary>
    /// ✅ FIX B: agora recebe um callback opcional que roda quando o painel fecha
    /// (seja por fechamento manual em cartas normais, seja no fim da animação final).
    /// Isso funciona mesmo que quem chamou o Show seja desativado/destruído depois.
    /// </summary>
    public void Show(string itemName, int pieceNumber, int collected, int total, Action onClosed = null)
    {
        gameObject.SetActive(true);

        _onClosedCallback = onClosed;

        if (itemNameText != null) itemNameText.text = itemName;
        if (counterText != null) counterText.text = $"{collected}/{total}";

        for (int i = 0; i < letterSlots.Length; i++)
        {
            if (letterSlots[i] != null)
            {
                letterSlots[i].enabled = (i < collected);

                letterSlots[i].rectTransform.anchoredPosition = posicoesOriginais[i];
                letterSlots[i].rectTransform.localRotation = rotacoesOriginais[i];
            }
        }

        isFinalCard = collected >= total;

        if (currentRoutine != null) StopCoroutine(currentRoutine);

        if (isFinalCard)
            currentRoutine = StartCoroutine(AnimateFinal());
        else
            currentRoutine = StartCoroutine(AnimateNormal());
    }

    private IEnumerator AnimateNormal()
    {
        yield return StartCoroutine(DoFadeIn());
        podeFechar = true;
        currentRoutine = null; // ✅ limpa referência (AnimateNormal terminou)
    }

    private IEnumerator FecharAntesDoTempo()
    {
        yield return StartCoroutine(DoFadeOut());
        FinalizarPainel();
    }

    private IEnumerator AnimateFinal()
    {
        yield return StartCoroutine(DoFadeIn());

        // 1. Mostra as cartas juntas por um tempo
        yield return new WaitForSecondsRealtime(tempoParaCrack);

        // 2. Toca o som do crack
        if (crackAudioSource != null && crackSound != null)
            crackAudioSource.PlayOneShot(crackSound);

        // 3. Efeito de quebrar (move e gira)
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.unscaledDeltaTime * velocidadeMovimento;
            float p = Mathf.SmoothStep(0f, 1f, elapsed);

            for (int i = 0; i < letterSlots.Length; i++)
            {
                if (letterSlots[i] != null && letterSlots[i].enabled)
                {
                    Vector2 targetPos = Vector2.zero;
                    float targetRot = 0f;

                    if (i == 0) { targetPos = posFinalCarta1; targetRot = rotFinalCarta1; }
                    else if (i == 1) { targetPos = posFinalCarta2; targetRot = rotFinalCarta2; }
                    else if (i == 2) { targetPos = posFinalCarta3; targetRot = rotFinalCarta3; }

                    letterSlots[i].rectTransform.anchoredPosition =
                        Vector2.Lerp(posicoesOriginais[i], targetPos, p);

                    float currentRotZ = Mathf.Lerp(0f, targetRot, p);
                    letterSlots[i].rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentRotZ);
                }
            }
            yield return null;
        }

        // 4. Segura os pedaços quebrados por um tempo
        yield return new WaitForSecondsRealtime(tempoSeparadas);

        // 5. Fade out final e fecha
        yield return StartCoroutine(DoFadeOut());
        FinalizarPainel();
    }

    /// <summary>
    /// ✅ FIX B: ponto único de fechamento.
    /// Invoca o evento (compatibilidade) e o callback direto, limpa tudo.
    /// </summary>
    private void FinalizarPainel()
    {
        Debug.Log($"[Panel] FinalizarPainel chamado. callback null? {_onClosedCallback == null}");

        gameObject.SetActive(false);
        currentRoutine = null;
        podeFechar = false;

        Action cb = _onClosedCallback;
        _onClosedCallback = null;

        OnPanelClosed?.Invoke();
        cb?.Invoke();
    }

    private IEnumerator DoFadeIn()
    {
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeIn);
            SetAlpha(blackFade, a);
            SetTextAlpha(itemNameText, a);
            SetTextAlpha(counterText, a);
            foreach (var slot in letterSlots)
            {
                if (slot != null) SetAlpha(slot, a);
            }
            yield return null;
        }
    }

    private IEnumerator DoFadeOut()
    {
        float t = 0f;
        while (t < fadeOut)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeOut);
            SetAlpha(blackFade, a);
            SetTextAlpha(itemNameText, a);
            SetTextAlpha(counterText, a);
            foreach (var slot in letterSlots)
            {
                if (slot != null) SetAlpha(slot, a);
            }
            yield return null;
        }
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    private void SetTextAlpha(TMP_Text text, float alpha)
    {
        if (text == null) return;
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }

    void OnDestroy()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        OnPanelClosed = null;
        _onClosedCallback = null;
    }
}