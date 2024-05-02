using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArtifactRoom : MonoBehaviour
{
    [Serializable]
    private struct RoomGraphs
    {
        public Sprite roomSprite;
        public Sprite doorFrame;
        public Sprite doorLeft;
        public Sprite doorRight;
    }
    
    [SerializeField] private RoomGraphs[] sprites;
    [SerializeField] private SpriteRenderer renderer;
    private int lastIndex;
    [SerializeField] private SpriteRenderer doorFrame;
    [SerializeField] private SpriteRenderer doorLeft;
    [SerializeField] private SpriteRenderer doorRight;

    private void OnBecameInvisible()
    {
        var index = Random.Range(0, sprites.Length);
        while (index == lastIndex)
        {
            index = Random.Range(0, sprites.Length);
        }
        renderer.sprite = sprites[index].roomSprite;
        doorFrame.sprite = sprites[index].doorFrame;
        doorLeft.sprite = sprites[index].doorLeft;
        doorRight.sprite = sprites[index].doorRight;
        lastIndex = index;
    }
}