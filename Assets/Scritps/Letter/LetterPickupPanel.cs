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
    [SerializeField] private float hold = 2.5f;
    [SerializeField] private float fadeOut = 0.5f;

    [Header("Efeito Final (Crack)")]
    [SerializeField] private float tempoParaCrack = 1f;
    [SerializeField] private float tempoSeparadas = 5f;
    [SerializeField] private float velocidadeMovimento = 2f;

    [Header("Spots Específicos (Posição e Rotação Alvo)")]
    [Tooltip("Arraste os objetos vazios (Spots) aqui. O script vai copiar a posição E a rotação deles.")]
    [SerializeField] private RectTransform spotCarta1;
    [SerializeField] private RectTransform spotCarta2;
    [SerializeField] private RectTransform spotCarta3;

    private Coroutine currentRoutine;
    private Vector2[] posicoesOriginais;
    private Quaternion[] rotacoesOriginais;

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

    public void Show(string itemName, int pieceNumber, int collected, int total)
    {
        gameObject.SetActive(true);

        if (itemNameText != null) itemNameText.text = itemName;
        if (counterText != null) counterText.text = $"{collected}/{total}";

        for (int i = 0; i < letterSlots.Length; i++)
        {
            if (letterSlots[i] != null)
            {
                letterSlots[i].enabled = (i < pieceNumber);
                // Reseta para a posição e rotação original
                letterSlots[i].rectTransform.anchoredPosition = posicoesOriginais[i];
                letterSlots[i].rectTransform.localRotation = rotacoesOriginais[i];
            }
        }

        if (currentRoutine != null) StopCoroutine(currentRoutine);

        if (collected >= total)
            currentRoutine = StartCoroutine(AnimateFinal());
        else
            currentRoutine = StartCoroutine(AnimateNormal());
    }

    private IEnumerator AnimateNormal()
    {
        yield return StartCoroutine(DoFadeIn());
        yield return new WaitForSecondsRealtime(hold);
        yield return StartCoroutine(DoFadeOut());

        gameObject.SetActive(false);
        currentRoutine = null;
        OnPanelClosed?.Invoke();
    }

    private IEnumerator AnimateFinal()
    {
        yield return StartCoroutine(DoFadeIn());

        yield return new WaitForSecondsRealtime(tempoParaCrack);

        if (crackAudioSource != null && crackSound != null)
            crackAudioSource.PlayOneShot(crackSound);

        // Pega as posições e rotações EXATAS configuradas nos Spots
        Vector2 targetPos1 = spotCarta1 != null ? spotCarta1.anchoredPosition : posicoesOriginais[0];
        Quaternion targetRot1 = spotCarta1 != null ? spotCarta1.localRotation : rotacoesOriginais[0];

        Vector2 targetPos2 = spotCarta2 != null ? spotCarta2.anchoredPosition : posicoesOriginais[1];
        Quaternion targetRot2 = spotCarta2 != null ? spotCarta2.localRotation : rotacoesOriginais[1];

        Vector2 targetPos3 = spotCarta3 != null ? spotCarta3.anchoredPosition : posicoesOriginais[2];
        Quaternion targetRot3 = spotCarta3 != null ? spotCarta3.localRotation : rotacoesOriginais[2];

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
                    Quaternion targetRot = Quaternion.identity;

                    if (i == 0) { targetPos = targetPos1; targetRot = targetRot1; }
                    else if (i == 1) { targetPos = targetPos2; targetRot = targetRot2; }
                    else if (i == 2) { targetPos = targetPos3; targetRot = targetRot3; }

                    // Move
                    letterSlots[i].rectTransform.anchoredPosition = Vector2.Lerp(posicoesOriginais[i], targetPos, p);

                    // Rotaciona (usando Slerp para ir da rotação original até a rotação do Spot)
                    letterSlots[i].rectTransform.localRotation = Quaternion.Slerp(rotacoesOriginais[i], targetRot, p);
                }
            }
            yield return null;
        }

        yield return new WaitForSecondsRealtime(tempoSeparadas);

        yield return StartCoroutine(DoFadeOut());

        gameObject.SetActive(false);
        currentRoutine = null;
        OnPanelClosed?.Invoke();
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
                if (slot != null && slot.enabled) SetAlpha(slot, a);
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
                if (slot != null && slot.enabled) SetAlpha(slot, a);
            yield return null;
        }
    }

    private void SetAlpha(Image img, float a) { if (img == null) return; Color c = img.color; c.a = a; img.color = c; }
    private void SetTextAlpha(TMP_Text txt, float a) { if (txt == null) return; Color c = txt.color; c.a = a; txt.color = c; }
}