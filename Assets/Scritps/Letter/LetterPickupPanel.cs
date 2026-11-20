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

    [Header("Timing")]
    [SerializeField] private float fadeIn = 0.5f;
    [SerializeField] private float hold = 2.5f;
    [SerializeField] private float fadeOut = 0.5f;

    private Coroutine currentRoutine;

    // ✅ Evento para avisar quando o painel terminou de sumir
    public event Action OnPanelClosed;

    void Awake()
    {
        SetAlpha(blackFade, 0f);
        foreach (var slot in letterSlots)
        {
            if (slot != null) { slot.enabled = false; SetAlpha(slot, 0f); }
        }
        SetTextAlpha(itemNameText, 0f);
        SetTextAlpha(counterText, 0f);
        // ❌ REMOVA ESTA LINHA: gameObject.SetActive(false);
    }

    public void Show(string itemName, int pieceNumber, int collected, int total)
    {
        gameObject.SetActive(true);

        if (itemNameText != null) itemNameText.text = itemName;
        if (counterText != null) counterText.text = $"{collected}/{total}";

        for (int i = 0; i < letterSlots.Length; i++)
        {
            if (letterSlots[i] != null)
                letterSlots[i].enabled = (i < pieceNumber);
        }

        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
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

        yield return new WaitForSecondsRealtime(hold);

        t = 0f;
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

        gameObject.SetActive(false);
        currentRoutine = null;

        // ✅ Avisa que o painel fechou!
        OnPanelClosed?.Invoke();
    }

    private void SetAlpha(Image img, float a) { if (img == null) return; Color c = img.color; c.a = a; img.color = c; }
    private void SetTextAlpha(TMP_Text txt, float a) { if (txt == null) return; Color c = txt.color; c.a = a; txt.color = c; }
}