using UnityEngine;

public class NightmareFlagSetter : MonoBehaviour
{
    void Start()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SetPassedNightmare(true);
            Debug.Log("[NightmareFlagSetter] Flag 'passedNightmare' definida como TRUE");
        }
    }
}