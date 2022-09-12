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
        InitializeNodeValues();
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
            else
            {
                nodeValues.Add(node, 0);
            }
        }
    }

    public void SetNodeValueToZero(Transform obj)
    {
        if (nodeValues.ContainsKey(obj.parent))
        {
            nodeValues[obj.parent] = 0;
        }
    }

    public void CalculateAllNodesSum(Dictionary<Transform, HashSet<Transform>> i2o)
    {
        // InitializeNodeValues();
        foreach (KeyValuePair<Transform, HashSet<Transform>> io in i2o)
        {
            totalNodeValue = 0;
            Debug.Log("--------------- Key: " + io.Key.parent);
            foreach (Transform t in io.Value)
            {
                totalNodeValue += nodeValues[t.parent];
            }

            // if (nodeValues.ContainsKey(io.Key.parent))
            // {
            nodeValues[io.Key.parent] = totalNodeValue;
            // }
            // else
            // {
            //     nodeValues.Add(io.Key.parent, totalNodeValue);
            // }
        }

        foreach (KeyValuePair<Transform, int> val in nodeValues)
        {
            Debug.Log(val.Key + " : " + val.Value);
        }
    }
}
