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

    // public void CalculateAllNodesSum(Dictionary<Transform, HashSet<Transform>> i2o)
    // {
    //     // InitializeNodeValues();
    //     foreach (KeyValuePair<Transform, HashSet<Transform>> io in i2o)
    //     {
    //         totalNodeValue = 0;
    //         // Debug.Log("-------------------- KEYS START -----------------------");
    //         Debug.Log("Key: " + io.Key.parent);
    //         // Debug.Log("--------------------- KEYS END ------------------------");

    //         // Debug.Log("-------------------- VALUES START ---------------------");
    //         foreach (Transform t in io.Value)
    //         {
    //             // Debug.Log(t.parent);
    //             totalNodeValue += nodeValues[t.parent];
    //         }
    //         // Debug.Log("--------------------- VALUES END ----------------------");
    //         // nodeValues[io.Key.parent] = totalNodeValue;
    //         // Debug.Log("-------------------- TOTAL START ----------------------");
    //         // Debug.Log(totalNodeValue);
    //         // Debug.Log("--------------------- TOTAL END -----------------------");
    //     }

    //     foreach (KeyValuePair<Transform, int> val in nodeValues)
    //     {
    //         Debug.Log(val.Key + " : " + val.Value);
    //     }
    // }

    public void CalculateAllNodesSum(Dictionary<Transform, HashSet<Transform>> i2o)
    {
        // InitializeNodeValues();
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
