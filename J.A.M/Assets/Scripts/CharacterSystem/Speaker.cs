using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using SS.ScriptableObjects;
using Tasks;
using TMPro;
using UnityEngine;

namespace CharacterSystem
{
    public class Speaker : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueContainer;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI dialogue;

        private Queue<Tuple<Task, SSDialogueNodeSO, string, float>> sentences = new();
        private bool isSpeaking;
        private Tuple<Task, SSDialogueNodeSO, string, float> tuple;
        private float durationToDecrease;

        private void OnDisable()
        {
            sentences.Clear();
            TimeTickSystem.OnTick -= DisplayDialogue;
        }

        public void CallDialogue()
        {
            if (isSpeaking || sentences.Count <= 0) return;
			isSpeaking = true;
            tuple = sentences.Dequeue();
            durationToDecrease = tuple.Item4;
            TimeTickSystem.OnTick += DisplayDialogue;
        }

        public void AddDialogue(Task task, SSDialogueNodeSO node, string characterName)
        {
            var duration = node.Duration * TimeTickSystem.ticksPerHour;
            sentences.Enqueue(new Tuple<Task, SSDialogueNodeSO, string, float>(task, node, characterName, duration));
            CallDialogue();
        }

        private void DisplayDialogue(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            if (tuple.Item2.IsDialogueTask)
                if (100 - Mathf.Clamp(tuple.Item1.Duration / tuple.Item1.BaseDuration, 0, 1) * 100 <
                    tuple.Item2.PercentageTask)
                    return;
            dialogueContainer.SetActive(true);
            characterNameText.text = tuple.Item3;
            dialogue.text = tuple.Item2.Text;
            if (durationToDecrease > 0)
            {
                durationToDecrease -= TimeTickSystem.timePerTick;
                return;
            }
            TimeTickSystem.OnTick -= DisplayDialogue;
            EndDialogue(tuple);
        }

        private void EndDialogue(Tuple<Task, SSDialogueNodeSO, string, float> tuple)
        {
            tuple.Item2.IsCompleted = true;
            dialogueContainer.SetActive(false);
            isSpeaking = false;
            CallDialogue();
        }
    }
}