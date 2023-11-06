using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDefilement : MonoBehaviour
{
    [SerializeField] private GameObject background1;
    [SerializeField] private GameObject background2;
    [SerializeField] private float defilementSpeed;

    private GameObject leftBackground;
    
    // Start is called before the first frame update
    void Start()
    {
        leftBackground = background1;
    }

    // Update is called once per frame
    void Update()
    {
        background1.transform.Translate(Vector3.left*defilementSpeed);
        background2.transform.Translate(Vector3.left*defilementSpeed);
        if (leftBackground.transform.position.x < -4000)
        {
            Vector3 resetPos = new Vector3(4000, 0, 500);
            leftBackground.transform.position = resetPos;
        }
    }
}
