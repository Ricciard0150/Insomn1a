using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{

    [SerializeField] Slider volumeSlider;
    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
                Load();
        }
        else
        {
             Load();
        }
    }


    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        Save();
    }

    private void Save ()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}
