using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGen : MonoBehaviour
{
    public float animationSpeed = 1.0f;
    public GameObject wallPrefab;
    public GameObject cellPrefab;
    public Transform wallParent;
    public Transform cellParent;
    public Vector2 mazeSize = new Vector2(4, 4);
    public Material visitedFloorMat;

    Dictionary<Vector2, Transform> cellRef = new Dictionary<Vector2, Transform>();
    Dictionary<float, GameObject> wallList = new Dictionary<float, GameObject>();
    Dictionary<Vector2, List<Vector2>> neighbourList = new Dictionary<Vector2, List<Vector2>>();
    List<Vector2> cellList = new List<Vector2>();
    List<Vector2> visitedCell = new List<Vector2>();
    Stack<Vector2> cellStack = new Stack<Vector2>();

    int wallNum = 0;
    bool mazeGenFinished = false;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        GenerateGrid();
        PopulateNeighbours();
        StartCoroutine(GeneratePath());
    }

    void Init()
    {
        wallNum = 0;
        mazeGenFinished = false;
        cellRef = new Dictionary<Vector2, Transform>();
        wallList = new Dictionary<float, GameObject>();
        neighbourList = new Dictionary<Vector2, List<Vector2>>();
        cellList = new List<Vector2>();
        visitedCell = new List<Vector2>();
        cellStack = new Stack<Vector2>();

        foreach (Transform cell in cellParent)
        {
            Destroy(cell.gameObject);
        }

        foreach (Transform wall in wallParent)
        {
            Destroy(wall.gameObject);
        }
    }

    void GenerateGrid()
    {
        Vector3 cellPosition = Vector3.zero;
        Vector3 wallPosition = Vector3.zero;
        Quaternion wallRotation = Quaternion.identity;

        for (int i = 0; i < mazeSize.x; i++)
        {
            for (int j = 0; j < mazeSize.y; j++)
            {
                Vector3 cellScale = cellPrefab.transform.localScale;
                cellPosition = new Vector3(i * cellScale.x, 0, j * cellScale.z);
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                cell.name = "Cell (" + i + ", " + j + ")";
                cell.transform.SetParent(cellParent);

                cellRef.Add(new Vector2(i, j), cell.transform);
                cellList.Add(new Vector2(i, j));

                wallRotation = Quaternion.Euler(0, 90, 0);
                wallPosition = new Vector3(cellPosition.x, 4, cellPosition.z - 3.5f);

                InstantiateWall(wallPosition, wallRotation);

                wallRotation = Quaternion.Euler(0, 0, 0);
                wallPosition = new Vector3(cellPosition.x - 3.5f, 4, cellPosition.z);

                InstantiateWall(wallPosition, wallRotation);

                if (i == mazeSize.x - 1)
                {
                    wallRotation = Quaternion.Euler(0, 0, 0);
                    wallPosition = new Vector3(cellPosition.x + 3.5f, 4, cellPosition.z);
                    InstantiateWall(wallPosition, wallRotation);
                }

                if (j == mazeSize.y - 1)
                {
                    wallRotation = Quaternion.Euler(0, 90, 0);
                    wallPosition = new Vector3(cellPosition.x, 4, cellPosition.z + 3.5f);
                    InstantiateWall(wallPosition, wallRotation);
                }
            }
        }
    }

    void InstantiateWall(Vector3 wallPosition, Quaternion wallRotation)
    {
        GameObject wall = Instantiate(wallPrefab, wallPosition, wallRotation);
        wallNum++;
        wall.name = "Wall (" + wallNum + ")";
        wall.transform.SetParent(wallParent);
        wallList.Add(wallNum, wall);
    }

    void PopulateNeighbours()
    {
        List<Vector2> neighbourOffset = new List<Vector2>();
        neighbourOffset.Add(new Vector2(0, 1));
        neighbourOffset.Add(new Vector2(1, 0));
        neighbourOffset.Add(new Vector2(0, -1));
        neighbourOffset.Add(new Vector2(-1, 0));

        foreach (Vector2 cell in cellList)
        {
            neighbourList.Add(cell, new List<Vector2>());
            foreach (Vector2 offset in neighbourOffset)
            {
                Vector2 neighbour = cell + offset;

                if (cellList.Contains(neighbour))
                {
                    neighbourList[cell].Add(neighbour);
                }
            }
        }
    }

    Vector2 GetNeighbour(Vector2 cell)
    {
        bool containsN = true;

        foreach (Vector2 nCell in neighbourList[cell])
        {
            if (!visitedCell.Contains(nCell))
            {
                containsN = false;
            }
        }

        if (containsN)
        {
            if (cellStack.Count > 0)
                cellStack.Pop();

            return cellStack.Peek();
        }

        int randN = Random.Range(0, neighbourList[cell].Count);
        Vector2 neighbour = neighbourList[cell][randN];

        while (visitedCell.Contains(neighbour))
        {
            neighbourList[cell].Remove(neighbour);
            randN = Random.Range(0, neighbourList[cell].Count);
            neighbour = neighbourList[cell][randN];
        }

        neighbourList[cell].Remove(neighbour);
        return neighbour;
    }

    IEnumerator GeneratePath()
    {
        Vector2 nextCell = cellList[0];
        visitedCell.Add(nextCell);
        cellStack.Push(nextCell);
        cellRef[nextCell].GetComponent<MeshRenderer>().material = visitedFloorMat;

        while (visitedCell.Count < (mazeSize.x * mazeSize.y))
        {
            Vector2 prevCell = nextCell;
            nextCell = GetNeighbour(nextCell);
            CalculateEdge(prevCell, nextCell);

            if (!visitedCell.Contains(nextCell))
            {
                visitedCell.Add(nextCell);
                cellRef[nextCell].GetComponent<MeshRenderer>().material = visitedFloorMat;
            }


            if (cellStack.Count > 0)
            {
                if (cellStack.Peek() != nextCell)
                {
                    cellStack.Push(nextCell);
                }
            }

            yield return new WaitForSeconds(animationSpeed);
        }

        Debug.Log("Total visited cells: " + visitedCell.Count);
        Debug.Log("Maze Generation Finished");
        mazeGenFinished = true;
    }

    void CalculateEdge(Vector2 prevCell, Vector2 nextCell)
    {
        float edgeV = 0f;
        float edgeH = 0f;
        //Calculate Horizontal Edge in Right Direction
        if (prevCell.y + 1 == nextCell.y)
        {
            edgeV = ((mazeSize.x * 2) + 1) * (prevCell.x + 1) + (2 * (prevCell.y + 1));
            edgeH = edgeV - (mazeSize.x * 2);

            if (nextCell.x == mazeSize.x - 1)
                edgeH += nextCell.y;

            DestroyWall(edgeH);
            Debug.Log("(+HOR) Edge of " + prevCell + " and " + nextCell + " is " + edgeH);
        }
        //Calculate Horizontal Edge in Left Direction
        else if (prevCell.y - 1 == nextCell.y)
        {
            edgeV = ((mazeSize.x * 2) + 1) * (nextCell.x + 1) + (2 * (nextCell.y + 1));
            edgeH = edgeV - (mazeSize.x * 2);

            if (prevCell.x == mazeSize.x - 1)
                edgeH += prevCell.y;

            DestroyWall(edgeH);
            Debug.Log("(-HOR) Edge of " + prevCell + " and " + nextCell + " is " + edgeH);
        }
        //Calculate Vertical Edge in Down Direction
        else if (prevCell.x + 1 == nextCell.x)
        {
            edgeV = ((mazeSize.x * 2) + 1) * (prevCell.x + 1) + (2 * (prevCell.y + 1));

            if (nextCell.x == mazeSize.x - 1)
                edgeV += nextCell.y;

            DestroyWall(edgeV);
            Debug.Log("(+VRT) Edge of " + prevCell + " and " + nextCell + " is " + edgeV);
        }
        //Calculate Vertical Edge in Up Direction
        else if (prevCell.x - 1 == nextCell.x)
        {
            edgeV = ((mazeSize.x * 2) + 1) * (nextCell.x + 1) + (2 * (nextCell.y + 1));

            if (prevCell.x == mazeSize.x - 1)
                edgeV += prevCell.y;

            DestroyWall(edgeV);
            Debug.Log("(-VRT) Edge of " + prevCell + " and " + nextCell + " is " + edgeV);
        }
    }

    void DestroyWall(float edgeNum)
    {
        if (wallList.ContainsKey(edgeNum))
        {
            Destroy(wallList[edgeNum]);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && mazeGenFinished)
        {
            Start();
        }
    }
}
