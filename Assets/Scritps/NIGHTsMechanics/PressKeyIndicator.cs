using UnityEngine;
using System.Collections;

public class PressEIndicator : MonoBehaviour
{
    [SerializeField] private GameObject indicatorObject;
    [SerializeField] private float blinkSpeed = 0.5f;

    private bool isShowing = false;

    void Start()
    {
        if (indicatorObject != null)
            indicatorObject.SetActive(false);
    }

    public void Show()
    {
        if (indicatorObject != null)
        {
            indicatorObject.SetActive(true);
            isShowing = true;
            StartCoroutine(BlinkRoutine());
            Debug.Log("✅ PressEIndicator MOSTRADO!");
        }
    }

    public void Hide()
    {
        if (indicatorObject != null)
        {
            indicatorObject.SetActive(false);
            isShowing = false;
            StopAllCoroutines();
            Debug.Log("❌ PressEIndicator ESCONDIDO!");
        }
    }

    private IEnumerator BlinkRoutine()
    {
        while (isShowing)
        {
            indicatorObject.SetActive(!indicatorObject.activeSelf);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}