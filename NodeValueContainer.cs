using System.Collections.Generic;
using UnityEngine;

public class NodeValueContainer : MonoBehaviour
{
    [SerializeField] private List<Transform> inputList = new List<Transform>();
    [SerializeField] private Transform outputImage;
    [SerializeField] private string actualOutput;

    private string totalNodeValue = "";
    private static Dictionary<Transform, string> nodeValues = new Dictionary<Transform, string>();

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
            if (node.GetComponent<NodeGenerator>().nodeValue != "")
            {
                nodeValues.Add(node, node.GetComponent<NodeGenerator>().nodeValue);
            }
            else
            {
                nodeValues.Add(node, "");
            }
        }
    }

    public void SetNodeValueToNull(Transform obj)
    {
        if (nodeValues.ContainsKey(obj.parent))
        {
            nodeValues[obj.parent] = "";
        }
    }

    //Calculates sum of all nodes by adding values of all output connected nodes
    public void CalculateAllNodesSum(Dictionary<Transform, HashSet<Transform>> i2o)
    {
        totalNodeValue = "";
        foreach (Transform node in transform)
        {
            if (i2o.ContainsKey(node))
            {
                foreach (Transform t in i2o[node])
                {
                    totalNodeValue += nodeValues[t];
                }
                nodeValues[node] = totalNodeValue;
            }
        }

        // foreach (KeyValuePair<Transform, string> val in nodeValues)
        // {
        //     Debug.Log(val.Key + " : " + val.Value);
        // }
    }

    public void CalculateNodeValues(Dictionary<Transform, Transform> o2i)
    {
        InitializeNodeValues();
        foreach (Transform t in inputList)
        {
            Transform tempKey = null;
            int count = 0;
            totalNodeValue = "";
            if (o2i.ContainsKey(t))
            {
                tempKey = t;
                while (o2i.ContainsKey(tempKey))
                {
                    count++;
                    // if (o2i.ContainsKey(tempKey))
                    // {
                    //     if (o2i[tempKey] == outputImage)
                    //         break;
                    // }

                    totalNodeValue += nodeValues[o2i[tempKey]];
                    tempKey = o2i[tempKey];

                    if (count > 12)
                        break;
                }
                nodeValues[t] = totalNodeValue;
            }
        }

        // Debug.Log("Input image 1: " + nodeValues[inputList[0]]);
        // Debug.Log("Input image 2: " + nodeValues[inputList[1]]);

        string calculatedOutput = nodeValues[inputList[0]] + nodeValues[inputList[1]];

        Debug.Log("Calculated: " + calculatedOutput);
        Debug.Log("Actual: " + actualOutput);

        if (calculatedOutput == actualOutput)
        {
            Debug.Log("TEST PASSED");
        }
    }
}
