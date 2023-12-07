using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDefilement : MonoBehaviour
{
    [SerializeField] private Transform background1;
    [SerializeField] private Transform background2;
    [SerializeField] private float defilementSpeed;

    private Transform leftBackground;
    private float initialPos;
    
    // Start is called before the first frame update
    void Awake()
    {
        leftBackground = background1;
        initialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        background1.Translate(Vector3.left*defilementSpeed);
        background2.Translate(Vector3.left*defilementSpeed);
        if (leftBackground.position.x < -4000)
        {
            Vector3 resetPos = new Vector3(4000, 0, initialPos);
            leftBackground.position = resetPos;
            //leftBackground = (leftBackground == background1) ? background2 : background1;
            if (leftBackground == background1)
            {
                leftBackground = background2;
            }
            else
            {
                leftBackground = background1;
            }
        }
    }
}
