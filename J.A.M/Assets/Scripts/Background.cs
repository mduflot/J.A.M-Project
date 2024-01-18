using System;
using System.Text;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class Background : MonoBehaviour
{
    [Header("Values")] 
    [SerializeField] private float minX;
    [SerializeField] private float yOffset;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float minScale, maxScale;
    [SerializeField] private float starScale;
    [SerializeField] private float galaxyScale;
    
    [Header("Display")]
    [SerializeField] private GameObject[] backgroundTiles;
    [SerializeField] private SpriteRenderer[] planetBackgrounds;
    [SerializeField] private SpriteRenderer[] planetForegrounds;

    [Header("Assets")] 
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private Sprite[] planetBack;
    [SerializeField] private Sprite[] planetFronts;
    [SerializeField] private Sprite[] stars;
    [SerializeField] private Sprite[] galaxies;

    private void Start()
    {
        for (int i = 0; i < planetBackgrounds.Length; i++)
            ResetPlanet(i);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < backgroundTiles.Length; i++)
        {
            if (backgroundTiles[i].transform.position.x < minX)
                ResetTile(i);
            
            backgroundTiles[i].transform.Translate(scrollSpeed * TimeTickSystem.timeScale * Vector3.left);
        }

        for (int i = 0; i < planetBackgrounds.Length; i++)
        {
            if (planetBackgrounds[i].transform.position.x < minX)
            {
                Vector3 pos = planetBackgrounds[i].transform.position;
                pos.x = -minX;
                pos.y = Random.Range(-yOffset, yOffset);
                pos.z = Random.Range(250f, 500f);
                planetBackgrounds[i].transform.position = pos;
                planetForegrounds[i].transform.position = pos;
                ResetPlanet(i);
            }
            
            planetBackgrounds[i].transform.Translate(scrollSpeed * planetBackgrounds[i].transform.localScale.x * TimeTickSystem.timeScale * Vector3.left);
            planetForegrounds[i].transform.Translate(scrollSpeed * planetBackgrounds[i].transform.localScale.x * TimeTickSystem.timeScale * Vector3.left);
        }
    }

    private void ResetTile(int index)
    {
        Vector3 pos = backgroundTiles[index].transform.position;
        pos.x = -minX;
        backgroundTiles[index].transform.position = pos;

        backgroundTiles[index].GetComponent<SpriteRenderer>().sprite = backgrounds[Random.Range(0, backgrounds.Length)];
    }

    private void ResetPlanet(int index)
    {
        var rand = Random.Range(0, 3);

        Sprite newBg;
        Sprite newFg;
        float newScale;
        
        switch (rand)
        {
            case 0:
                var planetIndex = Random.Range(0, planetFronts.Length);
                
                
                newBg = planetBack[planetIndex < 12 ? 0 : 1];
                newFg = planetFronts[planetIndex];
                
                planetBackgrounds[index].GetComponent<SpriteRenderer>().color =
                    new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                
                planetForegrounds[index].GetComponent<SpriteRenderer>().color =
                    new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                
                newScale = Random.Range(minScale, maxScale);
                
                break;
            
            case 1:
                newBg = stars[Random.Range(0, stars.Length)];
                newFg = null;
                newScale = Random.Range(minScale * starScale, maxScale * starScale);
                break;
            
            case 2:
                newBg = galaxies[Random.Range(0, galaxies.Length)];
                newFg = null;
                newScale = Random.Range(minScale * galaxyScale, maxScale * galaxyScale);
                break;
            
            default:
                newBg = null;
                newFg = null;
                newScale = 1f;
                break;
        }
        
        planetBackgrounds[index].GetComponent<SpriteRenderer>().sprite = newBg;
        planetForegrounds[index].GetComponent<SpriteRenderer>().sprite = newFg;
        
        planetBackgrounds[index].transform.SetScaleX(newScale);
        planetBackgrounds[index].transform.SetScaleY(newScale);
        
        planetForegrounds[index].transform.SetScaleX(newScale);
        planetForegrounds[index].transform.SetScaleY(newScale);
    }
}
