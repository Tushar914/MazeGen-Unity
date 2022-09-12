using UnityEngine;

public class RenderCustomLine : MonoBehaviour
{
    [Header("Dots")]
    [SerializeField] private GameObject dotsPrefab;
    [SerializeField] private Transform dotsParent;

    [Header("Line")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform lineParent;

    void Start()
    {

    }

    Vector3 GetMousePosition()
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePosition.z = 0;
        return worldMousePosition;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            GameObject dot = Instantiate(dotsPrefab, GetMousePosition(), Quaternion.identity, dotsParent);
        }
    }
}
