using UnityEngine;
using System.Collections;

public class GlassPunch : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer glassRenderer;
    [SerializeField] private Sprite[] glassStages;

    [Header("Hand")]
    [SerializeField] private GameObject handObject;
    [SerializeField] private Sprite[] handSprites;

    [Header("Settings")]
    [SerializeField] private float punchDuration = 0.2f;

    [Header("References")]
    [SerializeField] private JumpscareManager jumpscareManager;

    private int stage = 0;
    private bool canPunch = false;
    private bool isPunching = false;
    private bool isActive = false;

    void Start()
    {
        if (glassRenderer != null && glassStages.Length > 0)
            glassRenderer.sprite = glassStages[0];

        if (handObject != null)
            handObject.SetActive(false);

        gameObject.SetActive(false);
        Debug.Log($"🔍 GlassPunch Start - isActive:{isActive}");
    }

    void Update()
    {
        if (Input.GetButtonDown("Blink"))
        {
            Debug.Log($"⚠️ Blink pressionado! isActive:{isActive}, canPunch:{canPunch}, isPunching:{isPunching}");
        }

        if (!isActive || !canPunch || isPunching) return;

        if (Input.GetButtonDown("Blink"))
        {
            Debug.Log($"✊ SOCO! Estágio: {stage}");
            StartCoroutine(DoPunch());
        }
    }

    private IEnumerator DoPunch()
    {
        Debug.Log($"✊ DoPunch() INICIADO! Estágio: {stage}");

        isPunching = true;

        handObject.SetActive(true);
        SpriteRenderer handRenderer = handObject.GetComponent<SpriteRenderer>();
        if (handRenderer != null && handSprites.Length > stage)
        {
            handRenderer.sprite = handSprites[stage];
        }

        yield return new WaitForSeconds(punchDuration);

        stage++;

        if (stage < glassStages.Length && glassRenderer != null)
        {
            glassRenderer.sprite = glassStages[stage];
            Debug.Log($"✊ Vidro estágio: {stage}");
        }

        handObject.SetActive(false);
        isPunching = false;

        if (stage >= glassStages.Length - 1)
        {
            Debug.Log("💥 VIDRO QUEBROU!");
            canPunch = false;

            if (jumpscareManager != null)
            {
                jumpscareManager.OnGlassBroken();
            }
            else
            {
                Debug.LogError("❌ jumpscareManager é NULL!");
            }
        }
        else
        {
            Debug.Log($"✊ Aguardando próximo soco ({stage}/{glassStages.Length - 1})");
        }
    }

    public void ActivateWindow()
    {
        Debug.Log($"🔍 ActivateWindow() CHAMADO!");
        isActive = true;
        gameObject.SetActive(true);
        Debug.Log($"✅ Window ATIVADA! isActive={isActive}");

        ProtectedObject protectedObj = GetComponent<ProtectedObject>();
        if (protectedObj != null)
        {
            protectedObj.DestruirProtecao();
            Debug.Log("🔥 Proteção DESTRUÍDA!");
        }
    }

    public void EnablePunch()
    {
        Debug.Log($"🔍 EnablePunch() CHAMADO! canPunch antes: {canPunch}");
        canPunch = true;
        stage = 0;

        if (glassRenderer != null && glassStages.Length > 0)
            glassRenderer.sprite = glassStages[0];

        Debug.Log($"✅ Punch ENABLED! canPunch={canPunch}");
    }

    public void ResetPunch()
    {
        stage = 0;
        canPunch = false;
        isPunching = false;
        isActive = false;
        handObject.SetActive(false);
        gameObject.SetActive(false);

        if (glassRenderer != null && glassStages.Length > 0)
            glassRenderer.sprite = glassStages[0];
    }

    public bool IsActive() => isActive;
    public bool CanPunch() => canPunch;
}