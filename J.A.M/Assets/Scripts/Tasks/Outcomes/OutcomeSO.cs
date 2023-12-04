using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Outcome")]
public class OutcomeSO : ScriptableObject
{
    [SerializeField] public Outcome[] Outcomes;
}
