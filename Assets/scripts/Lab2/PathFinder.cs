using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


    public class Pathfinder : MonoBehaviour
    {
        private GridManager _gridManager;
        private readonly float _stepCost = 1f;
        private InputAction _findPathAction;
        public List<Node> CalculatedPath;

        private void Awake()
        {
            _gridManager = GetComponent<GridManager>();
        }

        private void OnEnable()
        {
            _findPathAction = new InputAction(name: "findPath", type: InputActionType.Button, binding: "<Mouse>/rightButton");
            _findPathAction.performed += OnFindPathPerformed;
            _findPathAction.Enable();

        }
        
        private void OnDisable()
        {
            if (_findPathAction != null)
            {
                _findPathAction.performed -= OnFindPathPerformed;
                _findPathAction.Disable();
                _findPathAction.Dispose();
            }
        }


        private void OnFindPathPerformed(InputAction.CallbackContext context)
        {
           FindPath();
        }

        private void FindPath()
        {
             
            
            Node startNode = _gridManager.GetNode(0,0);
            Node goalNode = _gridManager.GetNode(_gridManager.Width - 1, _gridManager.Height - 1);
            
            for (int i = 0; i < _gridManager.Width; i++)
            {
                for (int j = 0; j < _gridManager.Height; j++)
                {
                    Node node = _gridManager.GetNode(i, j);
                    _gridManager.SetTileMaterial(node, !node.walkable ? _gridManager.wallMaterial : _gridManager.walkableMaterial);
                }
            }

            CalculatedPath = FindPath(startNode, goalNode);
            _gridManager.SetTileMaterial(startNode, _gridManager.startMaterial);
            _gridManager.SetTileMaterial(goalNode, _gridManager.goalMaterial);
        }

        private List<Node> FindPath(Node startNode, Node goalNode)
        {

            _gridManager.ResetNodeCosts();
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
       
            startNode.gCost = 0;
            startNode.hCost = HeuristicCost(startNode, goalNode);
            openSet.Add(startNode);
          
            
            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || (Mathf.Approximately(openSet[i].fCost, currentNode.fCost) && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                if (currentNode == goalNode)
                {
                    List<Node> path = new List<Node>();
                    
                    while (currentNode != null)
                    {
                        path.Add(currentNode);
                        _gridManager.SetTileMaterial(currentNode, _gridManager.pathMaterial);
                        if (currentNode == startNode)
                        {
                            break;
                        }
                        currentNode = currentNode.parent;
                    }
                    path.Reverse();
                    return path;
                }
               
                
                closedSet.Add(currentNode);
                openSet.Remove(currentNode);

                foreach (Node neighbour in _gridManager.GetNeighbours(currentNode))
                {
                    if (closedSet.Contains(neighbour) || neighbour == null || !neighbour.walkable)
                    {
                        continue;
                    }

                    float tentativeG = currentNode.gCost + _stepCost;
                    if (tentativeG < neighbour.gCost)
                    {
                        neighbour.parent = currentNode;
                        neighbour.gCost = tentativeG;
                        neighbour.hCost = HeuristicCost(neighbour, goalNode);
                    }

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
            
            return null;
        }

        private float HeuristicCost(Node a, Node b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return dx + dy;
        }
    }
