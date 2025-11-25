using UnityEngine;

public class SquadColorizer : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Material soldiersMaterial;

    private void Awake()
    {
        ColorWheel.OnColorSelected += ColorizeSoldiers;
    }

    private void OnDestroy()
    {
        ColorWheel.OnColorSelected -= ColorizeSoldiers;
    }

    private void ColorizeSoldiers(Color color)
    {
        soldiersMaterial.SetColor("_BaseColor", color);
    }
}
