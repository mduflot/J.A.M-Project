using System;
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
        else GetComponent<SpriteRenderer>().color = Color.white;
        boundW = transform.GetComponent<SpriteRenderer>().bounds.size.x * 5f;
        boundH = transform.GetComponent<SpriteRenderer>().bounds.size.y * 5f;
    }
    
    private void Update()
    {
        RotatePointer();
        MovePointer();
        Flicker();
    }

    private void MovePointer()
    {
        Vector2 boundsBL = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector2 boundsTR = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));
        var pos = target.transform.position;
        pos.x = Mathf.Clamp(pos.x, boundsBL.x + boundW, boundsTR.x - boundW);
        pos.y = Mathf.Clamp(pos.y, boundsBL.y + boundH, boundsTR.y - boundH);
        transform.position = pos;
    }

    private void RotatePointer()
    {
        var dir = target.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);
        float scale = Mathf.Clamp(dir.magnitude / 100f, 6f, 10f);
        transform.localScale = new Vector3(scale, scale * 1.5f, 1);
    }

    private void Flicker()
    {
        var color = GetComponent<SpriteRenderer>().color;
        color.a = Mathf.Cos(Time.time * 2.5f * (flicker ? 2f : 1f)) + 1.25f;
        GetComponent<SpriteRenderer>().color = color;
    }
}