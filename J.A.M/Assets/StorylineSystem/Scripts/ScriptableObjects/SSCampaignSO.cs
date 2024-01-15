using System.Collections.Generic;
using UnityEngine;

namespace SS.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Campaign", menuName = "SSSystem/Campaign")]
    public class SSCampaignSO : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public List<SSNodeContainerSO> Storylines { get; set; }

        [ContextMenu("Initialize GUID")]
        private void Initialize()
        {
            ID = System.Guid.NewGuid().ToString();
        }
    }
}