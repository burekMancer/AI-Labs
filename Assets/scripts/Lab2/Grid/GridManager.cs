using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")] [SerializeField]
    private int width = 10;

    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    [Header("Prefabs and Mats")] [SerializeField]
    private GameObject tilePrefab;

    [SerializeField] public Material walkableMaterial;
    [SerializeField] public Material wallMaterial;
    [SerializeField] public Material startMaterial;
    [SerializeField] public Material pathMaterial;
    [SerializeField] public Material goalMaterial;

    private Node[,] nodes;
    private Dictionary<GameObject, Node> tileToNode = new();

    private InputAction clickAction;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    private void Awake()
    {
        GenerateGrid();
    }

    private void OnEnable()
    {
        clickAction = new InputAction
        (
            name: "Click",
            type: InputActionType.Button,
            binding: "<Mouse>/leftButton"
        );
        clickAction.performed += OnClickPerformed;
        clickAction.Enable();
    }

    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.performed -= OnClickPerformed;
            clickAction.Disable();
        }
    }

    private void GenerateGrid()
    {
        nodes = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, 0f, y * cellSize);
                GameObject tileGo = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                tileGo.name = $"Tile_{x}_{y}";
                Node node = new Node(x, y, true, tileGo);
                nodes[x, y] = node;
                tileToNode[tileGo] = node;
                SetTileMaterial(node, walkableMaterial);
            }
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;
        Ray ray =
            cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;
            if (tileToNode.TryGetValue(clicked, out Node node))
            {
                bool newWalkable = !node.walkable;
                SetWalkable(node, newWalkable);
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;
        return nodes[x, y];
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.y / cellSize);
        return GetNode(x, y);
    }

    public IEnumerable<Node> GetNeighbours(Node node, bool allowDiagonals = false)
    {
        int x = node.x;
        int y = node.y;
        yield return GetNode(x + 1, y);
        yield return GetNode(x - 1, y);
        yield return GetNode(x, y + 1);
        yield return GetNode(x, y - 1);
        if (allowDiagonals)
        {
            yield return GetNode(x + 1, y+1);
            yield return GetNode(x - 1, y-1);
            yield return GetNode(x+1,y + 1);
            yield return GetNode(x-1, y - 1);
        }
    }

    public void SetWalkable(Node node, bool walkable)
    {
        node.walkable = walkable;
        SetTileMaterial(node, walkable ? walkableMaterial : wallMaterial);
    }

    public void SetTileMaterial(Node node, Material material)
    {
        var renderer = node.tile.GetComponent<MeshRenderer>();
        if (renderer != null&& material !=null)
        {
            renderer.material = material;
        }
    }

    public void ResetTileMaterials()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetTileMaterial(GetNode(x, y), null);
            }
        }
    }
        
    public void ResetNodeCosts()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y].gCost = float.PositiveInfinity;
                nodes[x, y].hCost = 0f;
                nodes[x, y].parent = null;
            }
        }
    }
}

 