using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentMover : MonoBehaviour
{
    public GridManager gridManager;
    public Pathfinder pathfinder;
    public float moveSpeed = 3f;
    public float arriveDistance = 0.05f;

    private InputAction _followAction;
    
    private List<Node> _currentPath;
    private int _currentIndex;

    private void FollowPath(List<Node> path)
    {
        if (path == null || path.Count == 0)
        {
            _currentPath = null;
            _currentIndex = 0;
            return;
        }

        _currentPath = path;
        _currentIndex = 0;

        Continue();
    }
    
    private void OnEnable()
    {
        _followAction = new InputAction(
            name: "FollowPath",
            type: InputActionType.Button,
            binding: "<Keyboard>/r"
        );

        _followAction.performed += OnFollowPerformed;
        _followAction.Enable();
    }
    
    private void OnDisable()
    {
        if (_followAction != null)
        {
            _followAction.performed -= OnFollowPerformed;
            _followAction.Disable();
        }
    }
    
    private void OnFollowPerformed(InputAction.CallbackContext ctx)
    {
        StartFollowPath();
    }

    private void StartFollowPath()
    {
        FollowPath(pathfinder.CalculatedPath);
    }

    private void Update()
    {
        if (_currentPath == null || _currentPath.Count == 0)
        {
            return;
        }
        if (_currentIndex < 0 || _currentIndex >= _currentPath.Count)
        {
            Stop(); return;
        }

        Node targetNode = _currentPath[_currentIndex];
        Vector3 targetPos = NodeToWorldPosition(targetNode);

        // Move towards target
        Vector3 currentPos = transform.position;
        Vector3 nextPos = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
        transform.position = nextPos;

        if (Vector3.SqrMagnitude(transform.position - targetPos) <= arriveDistance * arriveDistance)
        {
            _currentIndex++;

            if (_currentIndex >= _currentPath.Count)
            {
                Stop();
            }
        }
    }

    private void Stop()
    {
        _currentPath = null;
        _currentIndex = 0;
    }

    private void Continue()
    {
        if (_currentPath == null || _currentPath.Count == 0) return;

        while (_currentIndex < _currentPath.Count)
        {
            Vector3 targetPos = NodeToWorldPosition(_currentPath[_currentIndex]);
            if (Vector3.SqrMagnitude(transform.position - targetPos) > arriveDistance * arriveDistance)
                break;

            _currentIndex++;
        }

        if (_currentIndex >= _currentPath.Count)
            Stop();
    }

    private Vector3 NodeToWorldPosition(Node node)
    {
        float cs = gridManager.CellSize;

        return new Vector3(node.x * cs, transform.position.y, node.y * cs);
    }
}
