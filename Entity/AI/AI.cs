using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class AI : MonoBehaviour
{
    [SerializeField] private AStar aStar;                                                     //Path finding 
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;


    public AStar AStar { get => aStar; set => aStar = value; }

    private void OnValidate() => aStar = GetComponent<AStar>();

    public virtual void RunAI() { }

    public void MoveAlongPath(Vector3Int targetPosition)
    {                                     //Moving towards target based on calculated path of the AStar
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(new Vector3(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f, 0));;
        Vector2 direction = aStar.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
    }

    public virtual AIState SaveState() => new AIState();
}

[System.Serializable]
public class AIState
{
    [SerializeField] private string type;

    public string Type { get => type; set => type = value; }

    public AIState(string type = "")
    {
        this.type = type;
    }
}
