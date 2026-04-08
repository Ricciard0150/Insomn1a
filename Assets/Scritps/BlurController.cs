using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurController : MonoBehaviour
{
    public Volume volume;
    private DepthOfField dof;

    void Start()
    {
        if (volume.profile.TryGet(out dof))
        {
            dof.active = false; // começa sem blur
        }
    }
    public void AtivarBlur()
    {
        dof.active = true;
    }

    public void DesativarBlur()
    {
        dof.active = false;
    }
}