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
    [SerializeField] private string punchButton = "Punch";
    [SerializeField] private float punchDuration = 0.2f;

    [Header("References")]
    [SerializeField] private JumpscareManager jumpscareManager; // ← ADICIONAR!

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
    }

    void Update()
    {
        if (!isActive || !canPunch || isPunching) return;

        if (Input.GetButtonDown(punchButton))
        {
            Debug.Log($"✊ SOCO DETECTADO! Estágio: {stage}");
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
            Debug.Log($"✊ Mão sprite: {handSprites[stage].name}");
        }

        yield return new WaitForSeconds(punchDuration);

        stage++;
        Debug.Log($"✊ Estágio agora: {stage}");

        if (stage < glassStages.Length && glassRenderer != null)
        {
            glassRenderer.sprite = glassStages[stage];
            Debug.Log($"✊ Vidro sprite: {glassStages[stage].name}");
        }

        handObject.SetActive(false);
        isPunching = false;

        // ⚠️ QUANDO QUEBRA → CHAMA DIRETAMENTE O JUMPScare MANAGER
        if (stage >= glassStages.Length - 1)
        {
            Debug.Log("💥 VIDRO QUEBROU! Chamando JumpscareManager...");
            canPunch = false;

            // ✅ CHAMA DIRETAMENTE
            if (jumpscareManager != null)
            {
                jumpscareManager.OnGlassBroken();
                Debug.Log("✅ JumpscareManager.OnGlassBroken() CHAMADO!");
            }
            else
            {
                Debug.LogError("❌ jumpscareManager é NULL! Arraste no Inspector.");
            }
        }
        else
        {
            Debug.Log($"✊ Aguardando próximo soco (estágio {stage}/{glassStages.Length - 1})");
        }
    }

    public void ActivateWindow()
    {
        isActive = true;
        gameObject.SetActive(true);
        Debug.Log("✅ GlassPunch ATIVADO!");
    }

    public void EnablePunch()
    {
        canPunch = true;
        stage = 0;

        if (glassRenderer != null && glassStages.Length > 0)
            glassRenderer.sprite = glassStages[0];

        Debug.Log("✅ GlassPunch.EnablePunch() CHAMADO! canPunch=true, stage=0");
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
}