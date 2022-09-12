using System.Collections.Generic;
using UnityEngine;

public class NodeValueContainer : MonoBehaviour
{
    private int totalNodeValue = 0;

    private static Dictionary<Transform, int> nodeValues = new Dictionary<Transform, int>();
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Initializing...");
        // InitializeNodeValues();
    }

    void InitializeNodeValues()
    {
        nodeValues.Clear();
        foreach (Transform node in transform)
        {
            if (node.GetComponent<NodeGenerator>().nodeValue != 0)
            {
                nodeValues.Add(node, node.GetComponent<NodeGenerator>().nodeValue);
                // Debug.Log(node + " : " + node.GetComponent<NodeGenerator>().nodeValue);
            }
        }
    }

    public void RemoveFromNodeValues(Transform obj)
    {
        nodeValues.Remove(obj);
    }

    public void CalculateAllNodesSum(Dictionary<Transform, Transform> nodes)
    {
        InitializeNodeValues();
        foreach (KeyValuePair<Transform, Transform> node in nodes)
        {
            Debug.Log("Node Key: " + node.Key.parent + " | Node Value: " + node.Value.parent);
            if (nodeValues.ContainsKey(node.Value.parent))
            {
                nodeValues[node.Value.parent] += nodeValues[node.Key.parent];
            }
            else
            {
                nodeValues.Add(node.Value.parent, nodeValues[node.Key.parent]);
            }
        }

        foreach (KeyValuePair<Transform, int> val in nodeValues)
        {
            Debug.Log(val.Key + " : " + val.Value);
        }
    }
}
