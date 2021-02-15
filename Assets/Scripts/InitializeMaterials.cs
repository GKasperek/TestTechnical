using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script to initialize the logo of the Taquin according to the platform
 */
public class InitializeMaterials : MonoBehaviour
{
    // Mode is "Apple" or "Android" to choose logo to print on cube
    public string mode;
    // List of Palets and their futures Materials
    public List<GameObject> Palets = new List<GameObject>(9);
    // Apple Logo
    public List<Material> AppleMaterials = new List<Material>(9);
    // Android Logo
    public List<Material> AndroidMaterials = new List<Material>(9);

    // Renderer to aplly Material
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        // For each cube
        for(int i = 0; i < 8; i++)
        {
                // Get the renderer
                rend = Palets[i].GetComponent<Renderer>();
                rend.enabled = true;

                // Apply Apple or Android slice according to the mode chose
                if (this.mode == "Apple")
                {
                    rend.sharedMaterial = AppleMaterials[i];
                }
                if (this.mode == "Android")
                {
                    rend.sharedMaterial = AndroidMaterials[i];
                }
         }
    }
}
