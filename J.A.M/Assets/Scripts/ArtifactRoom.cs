using UnityEngine;
using Random = UnityEngine.Random;

public class ArtifactRoom : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer renderer;
    private int lastIndex;

    private void OnBecameInvisible()
    {
        var index = Random.Range(0, sprites.Length);
        while (index == lastIndex)
        {
            index = Random.Range(0, sprites.Length);
        }
        renderer.sprite = sprites[index];
        lastIndex = index;
    }
}