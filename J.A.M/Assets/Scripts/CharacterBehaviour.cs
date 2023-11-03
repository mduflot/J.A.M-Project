using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public CharacterDataScriptable data;


    public void MoveTo(Transform destination)
    {
        float timeToTravel = Vector2.Distance(transform.position, destination.position) * moveSpeed;
        //Ajouter une étape avec la porte de la pièce active
        
    }
}
