using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Points : MonoBehaviour
{
    [SerializeField] private GameObject pointerPrefab;
    [SerializeField] private Material lineMaterial;

    private Transform nodeContainer;
    private GameObject pointer;
    private NodeValueContainer nodeValueContainer;
    private Vector3 startMousePosition;
    private Vector3 endMousePosition;
    private LineRenderer lineRenderer;

    private static List<Transform> selectedObj = new List<Transform>();
    private static Dictionary<Transform, Transform> connected = new Dictionary<Transform, Transform>();
    private static Dictionary<Transform, List<Transform>> connectedDict = new Dictionary<Transform, List<Transform>>();
    // private static Dictionary<Transform, int> nodeValues = new Dictionary<Transform, int>();

    void Start()
    {
        nodeContainer = GameObject.Find("NodeContainer_1").transform;
        if (nodeContainer)
        {
            nodeValueContainer = nodeContainer.GetComponent<NodeValueContainer>();
        }
        // Debug.Log("Initializing...");
        // foreach (Transform node in nodeContainer)
        // {
        //     if (node.GetComponent<NodeGenerator>().nodeValue != 0)
        //     {
        //         nodeValues.Add(node, node.GetComponent<NodeGenerator>().nodeValue);
        //     }
        // }
    }

    Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        return mousePosition;
    }

    void PrintAllObjects(List<Transform> objects)
    {
        foreach (Transform obj in objects)
        {
            Debug.Log(obj);
        }
    }

    void PrintAllConnectedObjects(Dictionary<Transform, Transform> objects)
    {
        foreach (KeyValuePair<Transform, Transform> obj in objects)
        {
            Debug.Log(obj.Key + "->" + obj.Value);
        }
    }

    bool HasMoreThanOneValue(Dictionary<Transform, Transform> objects, Transform valTransform)
    {
        int count = 0;

        foreach (KeyValuePair<Transform, Transform> obj in objects)
        {
            if (obj.Value == valTransform)
                count++;

            if (count > 1)
                return true;
        }

        return false;
    }

    void OnMouseDown()
    {
        //Prevent connecting an input node to an output node
        if (selectedObj.Count == 0 && tag == "DotsInput")
            return;

        //Prevent connecting an output node to an output node
        if (selectedObj.Count == 1 && tag == "DotsOutput")
        {
            SetSpriteColor(selectedObj[0].gameObject, Color.white);
            selectedObj.Clear();
            return;
        }

        if (selectedObj.Count == 1 && tag == "DotsInput")
        {
            string inputParentTag = transform.parent.tag;
            string outputParentTag = selectedObj[0].parent.tag;
            if (inputParentTag[inputParentTag.Length - 1] > outputParentTag[outputParentTag.Length - 1] + 1)
                return;
        }

        if (transform.GetComponent<LineRenderer>())
        {
            Transform tempObj = null;
            if (transform.GetComponent<LineRenderer>().enabled)
            {
                // Debug.Log(transform + " already contains line renderer. Disabling...");
                transform.GetComponent<LineRenderer>().enabled = false;
                SetSpriteColor(gameObject, Color.white);

                if (connected.ContainsKey(transform))
                {
                    //Check whether an output node is already connected to the current input node
                    if (!HasMoreThanOneValue(connected, connected[transform]))
                        SetSpriteColor(connected[transform].gameObject, Color.white);
                    //Required to sum node values AFTER node is removed in the next step
                    tempObj = connected[transform];
                }
                connected.Remove(transform);
                if (tempObj)
                {
                    //Remove value of node from NodeValues dictionary
                    nodeValueContainer.RemoveFromNodeValues(tempObj);
                    nodeValueContainer.CalculateAllNodesSum(connected);
                }
                selectedObj.Clear();
                return;
            }
            else
            {
                //Set node color to green when an output node is clicked
                SetSpriteColor(gameObject, Color.green);
            }
        }


        if (!selectedObj.Contains(transform))
        {
            selectedObj.Add(transform);
        }

        if (selectedObj.Count == 1)
        {
            //Create line renderer on Output node
            if (!selectedObj[0].GetComponent<LineRenderer>())
            {
                lineRenderer = selectedObj[0].gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.material = lineMaterial;
                SetSpriteColor(selectedObj[0].gameObject, Color.green);
            }
        }

        if (selectedObj.Count == 2)
        {
            //Draw line from Output node to Input node
            lineRenderer = selectedObj[0].GetComponent<LineRenderer>();
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            if (lineRenderer.enabled)
            {
                lineRenderer.SetPosition(0, selectedObj[0].position);
                lineRenderer.SetPosition(1, selectedObj[1].position);
                if (connectedDict.ContainsKey(selectedObj[1]))
                {
                    connectedDict[selectedObj[1]].Add(selectedObj[0]);
                }
                else
                {
                    List<Transform> connectedList = new List<Transform>();
                    connectedList.Add(selectedObj[0]);
                    connectedDict.Add(selectedObj[1], connectedList);
                }
                SetSpriteColor(selectedObj[0].gameObject, Color.blue);
                SetSpriteColor(selectedObj[1].gameObject, Color.blue);
                selectedObj.Clear();
            }
        }
        nodeValueContainer.CalculateAllNodesSum(connected);
    }

    void SetSpriteColor(GameObject obj, Color color)
    {
        obj.GetComponent<Image>().color = color;
    }
}