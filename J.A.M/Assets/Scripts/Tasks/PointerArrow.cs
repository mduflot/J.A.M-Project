using Unity.VisualScripting;
using UnityEngine;

public class PointerArrow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private bool flicker;

    public void Init(GameObject t, bool f)
    {
        target = t;
        flicker = f;
        if (f) GetComponent<SpriteRenderer>().color = Color.red;
    }
    
    private void FixedUpdate()
    {
        RotatePointer();
        MovePointer();
        Flicker();
    }

    private void MovePointer()
    {
        Camera mainCam = Camera.main;
        Vector3 newPos = new Vector3();

        if (target.transform.position.x < mainCam.transform.position.x)
        {
            newPos.x = target.transform.position.x < (mainCam.transform.position.x - mainCam.pixelWidth/2f) 
                ? mainCam.transform.position.x - mainCam.pixelWidth/2f + (30.0f) 
                : target.transform.position.x;            
        }
        else
        {
            newPos.x = target.transform.position.x > (mainCam.transform.position.x + mainCam.pixelWidth/2f) 
                ? mainCam.transform.position.x + mainCam.pixelWidth/2f - (30.0f)
                : target.transform.position.x;
        }

        if (target.transform.position.y < mainCam.transform.position.y)
        {
            newPos.y = target.transform.position.y < (mainCam.transform.position.y - mainCam.pixelHeight/2f) 
                ? mainCam.transform.position.y - mainCam.pixelHeight/2f + (15f)
                : target.transform.position.y;
        }
        else
        {
            newPos.y = target.transform.position.y > (mainCam.transform.position.y + mainCam.pixelHeight/2f) 
                ? mainCam.transform.position.y + mainCam.pixelHeight/2f - (15f)
                : target.transform.position.y;
        }
        
        newPos.z = 0f;
        
        transform.position = newPos;
    }

    private void RotatePointer()
    {
        transform.LookAt(target.transform);
        transform.rotation = new Quaternion(0f, 0f, transform.rotation.z, 1);
    }

    private void Flicker()
    {
        if (!flicker) return;
        
        var color = GetComponent<SpriteRenderer>().color;
        color.a = 2 * Mathf.Cos(Time.time) + .25f;
        GetComponent<SpriteRenderer>().color = color;
    }
}