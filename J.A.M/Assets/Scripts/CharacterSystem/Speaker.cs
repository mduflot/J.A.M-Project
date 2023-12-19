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

        private Queue<Tuple<Task, SSDialogueNodeSO, string>> sentences = new();
        private bool isSpeaking;

        public void Update()
        {
            if (isSpeaking || sentences.Count <= 0) return;
			isSpeaking = true;
            TimeTickSystem.OnTick += DisplayDialogue;
        }

        public void AddDialogue(Task task, SSDialogueNodeSO node, string characterName)
        {
            node.Duration *= TimeTickSystem.ticksPerHour;
            sentences.Enqueue(new Tuple<Task, SSDialogueNodeSO, string>(task, node, characterName));
        }

        private void DisplayDialogue(object sender, TimeTickSystem.OnTickEventArgs e)
        {
            var tuple = sentences.Dequeue();
            if (tuple.Item2.IsDialogueTask)
                if (100 - Mathf.Clamp(tuple.Item1.Duration / tuple.Item1.BaseDuration, 0, 1) * 100 <
                    tuple.Item2.PercentageTask)
                    return;
            dialogueContainer.SetActive(true);
            characterNameText.text = tuple.Item3;
            dialogue.text = tuple.Item2.Text;
            if (tuple.Item2.Duration > 0)
            {
                tuple.Item2.Duration -= TimeTickSystem.timePerTick;
                return;
            }
            TimeTickSystem.OnTick -= DisplayDialogue;
            EndDialogue(tuple);
        }

        private void EndDialogue(Tuple<Task, SSDialogueNodeSO, string> tuple)
        {
            tuple.Item2.IsCompleted = true;
            dialogueContainer.SetActive(false);
            isSpeaking = false;
        }
    }
}