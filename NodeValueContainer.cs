using System.Collections.Generic;
using UnityEngine;

public class NodeValueContainer : MonoBehaviour
{
    private int totalNodeValue = 0;
    private static Dictionary<Transform, int> nodeValues = new Dictionary<Transform, int>();

    // Start is called before the first frame update
    void Start()
    {
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

    //Calculates sum of all nodes by adding values of all output connected nodes
    public void CalculateAllNodesSum(Dictionary<Transform, HashSet<Transform>> i2o)
    {
        foreach (Transform node in transform)
        {
            totalNodeValue = 0;

            if (i2o.ContainsKey(node))
            {
                foreach (Transform t in i2o[node])
                {
                    totalNodeValue += nodeValues[t];
                }
                nodeValues[node] = totalNodeValue;
            }

        }

        foreach (KeyValuePair<Transform, int> val in nodeValues)
        {
            Debug.Log(val.Key + " : " + val.Value);
        }
    }
}
