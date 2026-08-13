using UnityEngine;

public class PlayerReturn : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("ReturnPosX"))
        {
            float x = PlayerPrefs.GetFloat("ReturnPosX");
            float y = PlayerPrefs.GetFloat("ReturnPosY");
            float z = PlayerPrefs.GetFloat("ReturnPosZ");

            transform.position = new Vector3(x, y, z);

            PlayerPrefs.DeleteKey("ReturnPosX");
            PlayerPrefs.DeleteKey("ReturnPosY");
            PlayerPrefs.DeleteKey("ReturnPosZ");
            PlayerPrefs.DeleteKey("ReturnScene");
            PlayerPrefs.Save();

            Debug.Log($"Player voltou para: {transform.position}");
        }
    }
}
