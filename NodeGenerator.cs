using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Points point;
    public string nodeValue = "";

    private enum Points { Left, Right, Both };
    private Points oldPoint;
    private static int childNum = 0;

    void Start()
    {
        oldPoint = point;
        GenerateNodePoints();
    }

    Vector3 GetNodeDimensions(Points dots)
    {
        float width = GetComponent<RectTransform>().rect.width;
        float height = GetComponent<RectTransform>().rect.height;

        if (dots == Points.Left)
            return new Vector3(-width / 2, 0, 0);

        return new Vector3(width / 2, 0, 0);
    }

    void SetName(GameObject obj)
    {
        obj.name = "dot" + point.ToString() + childNum;
        childNum++;
    }

    void GenerateNodePoints()
    {
        if (point != Points.Both)
        {
            GameObject dot = Instantiate(dotPrefab, GetNodeDimensions(point), Quaternion.identity);
            dot.transform.SetParent(transform, worldPositionStays: false);

            if (point == Points.Left)
                dot.tag = "DotsInput";

            if (point == Points.Right)
                dot.tag = "DotsOutput";

            SetName(dot);
        }
        else
        {
            GameObject dotLeft = Instantiate(dotPrefab, GetNodeDimensions(Points.Left), Quaternion.identity);
            dotLeft.transform.SetParent(transform, worldPositionStays: false);
            dotLeft.tag = "DotsInput";
            SetName(dotLeft);

            GameObject dotRight = Instantiate(dotPrefab, GetNodeDimensions(Points.Right), Quaternion.identity);
            dotRight.transform.SetParent(transform, worldPositionStays: false);
            dotRight.tag = "DotsOutput";
            SetName(dotRight);
        }
    }
}
