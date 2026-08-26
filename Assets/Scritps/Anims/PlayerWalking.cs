using UnityEngine;
using DG.Tweening;

public class PlayerWalkAnimation : MonoBehaviour
{
    [Header("Walk Sprites")]
    public Sprite[] upSprites;
    public Sprite[] downSprites;
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;

    [Header("Settings")]
    public float frameRate = 0.15f;
    public Sprite idleSprite;
    public float moveDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private int frameIndex = 0;
    private float timer = 0;
    private Sprite[] currentSprites;
    private bool wasWalking = false;
    private TopDownMovement movementScript;
    private Tweener moveTweener;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found in children!");
            return;
        }

        movementScript = GetComponent<TopDownMovement>();
        currentSprites = downSprites;

        if (currentSprites == null || currentSprites.Length == 0)
        {
            currentSprites = new Sprite[] { null };
        }

        if (idleSprite != null)
            spriteRenderer.sprite = idleSprite;
    }

    void Update()
    {
        if (spriteRenderer == null || movementScript == null) return;

        bool isWalking = movementScript.isWalking;

        if (isWalking)
        {
            Vector2 movement = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;

            if (movement != Vector2.zero)
            {
                Vector3 targetPos = transform.position + (Vector3)movement * 0.1f;

                if (moveTweener != null)
                    moveTweener.Kill();

                moveTweener = transform.DOMove(targetPos, moveDuration)
                    .SetEase(Ease.Linear);

                UpdateAnimation(movement);
            }
        }
        else
        {
            if (wasWalking && idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
                frameIndex = 0;
                timer = 0;
            }
        }

        wasWalking = isWalking;
    }

    void UpdateAnimation(Vector2 direction)
    {
        Sprite[] newSprites = downSprites;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            newSprites = direction.x > 0 ? rightSprites : leftSprites;
        }
        else
        {
            newSprites = direction.y > 0 ? upSprites : downSprites;
        }

        if (newSprites == null || newSprites.Length == 0)
        {
            newSprites = new Sprite[] { null };
        }

        if (newSprites != currentSprites)
        {
            currentSprites = newSprites;
            frameIndex = 0;
            timer = 0;
        }

        if (currentSprites == null || currentSprites.Length == 0)
        {
            currentSprites = new Sprite[] { null };
            return;
        }

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0;
            frameIndex = (frameIndex + 1) % currentSprites.Length;

            if (currentSprites[frameIndex] != null)
            {
                spriteRenderer.sprite = currentSprites[frameIndex];
            }
        }
    }

    void OnDestroy()
    {
        if (moveTweener != null)
            moveTweener.Kill();
    }
}