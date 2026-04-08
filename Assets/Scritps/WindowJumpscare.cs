using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowJumpscare : MonoBehaviour
{
    public GameObject panel;
    public GameObject pressE;
    public KeyCode tecla = KeyCode.E;
    public KeyCode lanterna = KeyCode.Space;

    bool isOpen = false; 

    bool isColliding = false;

    public TopDownMovement tdm; 
    public BlurController bc;

    private void Start()
    {
        pressE.SetActive(false);
    }

    private void Update()
    {
       if(Input.GetKeyDown(tecla) && isColliding)
        {
            panel.SetActive(true);
            isOpen = true;
            bc.AtivarBlur();
            pressE.SetActive(false);
            tdm.canMove = false;
        }
       if(Input.GetKeyDown(lanterna))
        {
            bc.DesativarBlur();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IStatusPlayer player))
        {
            isColliding = true;
            pressE.SetActive(true);
        }
    }
}
