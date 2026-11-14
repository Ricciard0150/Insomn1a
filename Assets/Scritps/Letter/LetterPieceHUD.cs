using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterPickupPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image blackFade;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text counterText;

    [Header("Timing")]
    [SerializeField] private float fadeIn = 0.5f;
    [SerializeField] private float hold = 1.5f;
    [SerializeField] private float fadeOut = 0.5f;

    private Coroutine currentRoutine;

    void Awake()
    {
        SetAlpha(blackFade, 0f);
        SetAlpha(itemIcon, 0f);
        SetTextAlpha(itemNameText, 0f);
        SetTextAlpha(counterText, 0f);

        if (itemIcon != null) itemIcon.enabled = false;

        gameObject.SetActive(false);
    }

    public void Show(Sprite icon, string itemName, int collected, int total)
    {
        if (icon == null)
        {
            Debug.LogError("[LetterPickupPanel] Sprite NULL");
            return;
        }

        gameObject.SetActive(true);

        // Ícone
        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.type = Image.Type.Simple;
            itemIcon.preserveAspect = true;
            itemIcon.enabled = true;
            itemIcon.color = Color.white;

            RectTransform rt = itemIcon.rectTransform;
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;

            if (rt.sizeDelta.x <= 0f) rt.sizeDelta = new Vector2(300f, 300f);
            if (rt.sizeDelta.y <= 0f) rt.sizeDelta = new Vector2(300f, 300f);

            itemIcon.transform.SetAsLastSibling();
        }

        // Nome
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
            itemNameText.transform.SetAsLastSibling();
        }

        // Contador
        if (counterText != null)
        {
            counterText.text = $"{collected}/{total}";
            counterText.transform.SetAsLastSibling();
        }

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

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
            SetAlpha(itemIcon, a);
            SetTextAlpha(itemNameText, a);
            SetTextAlpha(counterText, a);
            yield return null;
        }
        SetAlpha(blackFade, 1f);
        SetAlpha(itemIcon, 1f);
        SetTextAlpha(itemNameText, 1f);
        SetTextAlpha(counterText, 1f);

        yield return new WaitForSecondsRealtime(hold);

        t = 0f;
        while (t < fadeOut)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeOut);
            SetAlpha(blackFade, a);
            SetAlpha(itemIcon, a);
            SetTextAlpha(itemNameText, a);
            SetTextAlpha(counterText, a);
            yield return null;
        }
        SetAlpha(blackFade, 0f);
        SetAlpha(itemIcon, 0f);
        SetTextAlpha(itemNameText, 0f);
        SetTextAlpha(counterText, 0f);

        gameObject.SetActive(false);
        currentRoutine = null;
    }

    private void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private void SetTextAlpha(TMP_Text txt, float a)
    {
        if (txt == null) return;
        Color c = txt.color;
        c.a = a;
        txt.color = c;
    }
}