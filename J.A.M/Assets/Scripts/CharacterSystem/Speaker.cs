using System;
using System.Collections;
using System.Collections.Generic;
using SS.ScriptableObjects;
using Tasks;
using TMPro;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogue;

    private Queue<Tuple<Task, SSDialogueNodeSO, string>> sentences = new();
    private bool isSpeaking;

    public void Update()
    {
        if (isSpeaking) return;
        if (sentences.Count <= 0) return;
        isSpeaking = true;
        StartCoroutine(DisplayDialogue());
    }

    public void AddDialogue(Task task, SSDialogueNodeSO node, string characterName)
    {
        sentences.Enqueue(new Tuple<Task, SSDialogueNodeSO, string>(task, node, characterName));
    }

    private IEnumerator DisplayDialogue()
    {
        var tuple = sentences.Dequeue();
        if (tuple.Item2.IsDialogueTask)
            yield return new WaitUntil(() =>
                100 - Mathf.Clamp(tuple.Item1.Duration / tuple.Item1.BaseDuration, 0, 100) * 100 >
                tuple.Item2.PercentageTask);
        dialogueContainer.SetActive(true);
        characterNameText.text = tuple.Item3;
        dialogue.text = tuple.Item2.Text;
        yield return new WaitForSeconds(tuple.Item2.Duration);
        EndDialogue(tuple);
    }

    private void EndDialogue(Tuple<Task, SSDialogueNodeSO, string> tuple)
    {
        tuple.Item2.isCompleted = true;
        dialogueContainer.SetActive(false);
        isSpeaking = false;
    }
}