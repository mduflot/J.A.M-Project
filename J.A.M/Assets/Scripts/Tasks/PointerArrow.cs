using UnityEngine;

public class PointerArrow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private bool flicker;
    private float boundW;
    float boundH;
    
    public void Init(GameObject t, bool f)
    {
        target = t;
        flicker = f;
        if (f) GetComponent<SpriteRenderer>().color = Color.red;
        boundW = transform.GetComponent<SpriteRenderer>().bounds.size.x * 1.5f;
        boundH = transform.GetComponent<SpriteRenderer>().bounds.size.y * 1.5f;
    }
    
    private void Update()
    {
        RotatePointer();
        MovePointer();
        Flicker();
    }

    private void MovePointer()
    {
        Vector2 bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        var pos = target.transform.position;
        pos.x = Mathf.Clamp(pos.x, bounds.x + boundW, bounds.x + Screen.width - boundW);
        pos.y = Mathf.Clamp(pos.y, bounds.y + boundH, bounds.y + Screen.height - boundH);
        Debug.Log($"{bounds.x + Screen.width - boundW} // {pos.x}");
        transform.position = pos;
    }

    private void RotatePointer()
    {
        var dir = target.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);
    }

    private void Flicker()
    {
        if (!flicker) return;
        
        var color = GetComponent<SpriteRenderer>().color;
        color.a = 2 * Mathf.Cos(Time.time) + .25f;
        GetComponent<SpriteRenderer>().color = color;
    }
}