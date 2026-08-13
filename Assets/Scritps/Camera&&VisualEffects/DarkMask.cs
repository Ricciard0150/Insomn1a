using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    public Light2D light2D;

    public float intensidade = 2f;
    public float raio = 6f;

    void Start()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        float pulsar = Mathf.Sin(Time.time * 3f) * 0.2f;

        light2D.intensity = intensidade + pulsar;
        light2D.pointLightOuterRadius = raio + pulsar;
    }
}