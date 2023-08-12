using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Fighter))]
public class HostileEnemy : AI
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private bool isFighting, attacking;

    private void OnValidate()
    {
        fighter = GetComponent<Fighter>();
        AStar = GetComponent<AStar>();
    }

    private void FixedUpdate()
    {
        RunAI();
    }

    public override void RunAI()
    {
        if (GetComponent<Actor>().IsAlive)
        {
            if (!fighter.Target)
            {                                          //Sets targets of hostile enemies to the player
                fighter.Target = GameManager.instance.Actors[0];
            }
            else if (fighter.Target && !fighter.Target.IsAlive)
            {
                fighter.Target = null;
            }

            if (fighter.Target)
            {
                Vector3Int targetPosition = MapManager.instance.FloorMap.WorldToCell(new Vector3Int(Mathf.FloorToInt(fighter.Target.transform.position.x), Mathf.FloorToInt(fighter.Target.transform.position.y), 0));  //Gets target position
                if (isFighting || GetComponent<Actor>().FieldOfView.Contains(targetPosition))
                {                           //If it is fighting and POV contains the target
                    if (!isFighting)
                    {
                        isFighting = true;
                    }

                    float targetDistance = Vector3.Distance(new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0), fighter.Target.transform.position);         //Distance between this AI and the player

                    if (targetDistance <= 1.5f)
                    {                                                                           //If it is in range                                                
                        StartCoroutine(Attacking());
                        return;
                    }
                    else
                    {                                                                                                //Else move towards target
                        MoveAlongPath(targetPosition);
                        return;
                    }
                }
            }
        }
    }

    private IEnumerator Attacking()
    {
        if (!attacking)
        {
            attacking = true;
            MeleeAction(GetComponent<Actor>(), fighter.Target);
            yield return new WaitForSeconds(1.5f);
            attacking = false;
        }
    }

    private void MeleeAction(Actor actor, Actor target)
    {
        int damage = actor.GetComponent<Fighter>().Power() - target.GetComponent<Fighter>().Defense();

        string attackDesc = $"{actor.name} attacks {target.name}";

        string colorHex = "";

        if (actor.GetComponent<Player>())
        {
            colorHex = "#ffffff"; // white
        }
        else
        {
            colorHex = "#d1a3a4"; // light red
        }

        if (damage > 0)
        {
            UIManager.instance.AddMessage($"{attackDesc} for {damage} hit points.", colorHex);
            target.GetComponent<Fighter>().Hp -= damage;
        }
        else
        {
            UIManager.instance.AddMessage($"{attackDesc} but does no damage.", colorHex);
        }
    }

    public override AIState SaveState() => new AIState(
      type: "HostileEnemy"
    );
}