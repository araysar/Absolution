using UnityEngine;
using UnityEngine.UI;

public class UnscaledTimeUI : MonoBehaviour
{
    private Material myMaterial;

    private void Start()
    {
        myMaterial = GetComponent<Image>().material;
    }
    void Update()
    {
        myMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}
