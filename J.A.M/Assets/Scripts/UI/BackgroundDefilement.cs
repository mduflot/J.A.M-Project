using Managers;
using UnityEngine;

public class BackgroundDefilement : MonoBehaviour
{
    [SerializeField] private Transform background1;
    [SerializeField] private Transform background2;
    [SerializeField] private float defilementSpeed;

    private Transform leftBackground;
    private float initialPos;

    void Awake()
    {
        leftBackground = background1;
        initialPos = background1.transform.position.z;
    }

    void Update()
    {
        background1.Translate(defilementSpeed * TimeTickSystem.timeScale * Vector3.left);
        background2.Translate(defilementSpeed * TimeTickSystem.timeScale * Vector3.left);
        if (leftBackground.position.x < -4000)
        {
            Vector3 resetPos = new Vector3(4000, 0, initialPos);
            leftBackground.position = resetPos;
            leftBackground = (leftBackground == background1) ? background2 : background1;
        }
    }
}