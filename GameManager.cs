using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Time")]
    [SerializeField] private float baseTime = 0.075f;
    [SerializeField] private float delayTime; //Read-only

    [Header("Entities")]
    [SerializeField] private bool isPlayerTurn = true; //Read-only
    [SerializeField] private int actorNum = 0; //Read-only
    [SerializeField] private List<Entity> entities;
    [SerializeField] private List<Actor> actors;

    [Header("Death")]
    [SerializeField] private Sprite deadSprite;
    public bool IsPlayerTurn { get => isPlayerTurn; }
    public List<Entity> Entities { get => entities; }
    public List<Actor> Actors { get => actors; }
    public Sprite DeadSprite { get => deadSprite; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneState sceneState = SaveManager.instance.Save.Scenes.Find(x => x.FloorNumber == SaveManager.instance.CurrentFloor);     //Finds the scene that matches with the current scene saved from the sceneManager

        if (sceneState is not null)
        {
            LoadState(sceneState.GameState, true);      //Loads game if it finds one 
        }
        else
        {
            entities = new List<Entity>();
            actors = new List<Actor>();
        }
    }

    private void StartTurn()
    {
        if (actors[actorNum].GetComponent<Player>())        //If it is a player, set isPlayerTurn to true
        {
            isPlayerTurn = true;
        }
        else
        {
            if (actors[actorNum].AI != null)               //If the actor does have an AI component, we run the AI
            {
                actors[actorNum].AI.RunAI();
            }
            else
            {
                Action.WaitAction();                      //Basically ends turn 
            }
        }
    }

    public void EndTurn()
    {
        if (actors[actorNum].GetComponent<Player>())
        {
            isPlayerTurn = false;
        }

        if (actorNum == actors.Count - 1)
        {
            actorNum = 0;
        }
        else
        {
            actorNum++;
        }

        StartCoroutine(TurnDelay());
    }

    public IEnumerator TurnDelay()              //Basically like a timer 
    {
        yield return new WaitForSeconds(delayTime);
        StartTurn();
    }

    public void AddEntity(Entity entity)
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);  //Activates the actor if it is not activated
        }
        entities.Add(entity);                   //Adds it to the list of entities
    }

    public void InsertEntity(Entity entity, int index)  //Same as AddEntity
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);  
        }
        entities.Insert(index, entity);
    }

    public void RemoveEntity(Entity entity)
    {
        entity.gameObject.SetActive(false);  //Deactivates the entity
        entities.Remove(entity);             //Opposite of AddEntity
    }

    public void AddActor(Actor actor)       //These next 3 are self explanatory
    {
        actors.Add(actor);
        delayTime = SetTime();
    }

    public void InsertActor(Actor actor, int index)
    {
        actors.Insert(index, actor);
        delayTime = SetTime();
    }

    public void RemoveActor(Actor actor)
    {
        actors.Remove(actor);
        delayTime = SetTime();
    }

    public void RefreshPlayer()
    {
        actors[0].UpdateFieldOfView();  //Updates the player's POV
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        foreach (Actor actor in actors)                                             //Loops through all characters
        {
            if (actor.BlocksMovement && actor.transform.position == location)       //If the characters has a blocksMovement component and the location matches
            {
                return actor;
            }
        }
        return null;
    }

    private float SetTime() => baseTime / actors.Count;

    public GameState SaveState()
    {
        foreach (Item item in actors[0].Inventory.Items)        //Adds every item in the players inventory 
        {
            AddEntity(item);
        }

        GameState gameState = new GameState(entities: entities.ConvertAll(x => x.SaveState()));     //Then saves the current state 

        foreach (Item item in actors[0].Inventory.Items)
        {
            RemoveEntity(item);
        }

        return gameState;
    }

    public void LoadState(GameState state, bool canRemovePlayer)
    {
        isPlayerTurn = false; //Prevents player from moving during load

        Reset(canRemovePlayer);
        StartCoroutine(LoadEntityStates(state.Entities, canRemovePlayer));
    }

    private IEnumerator LoadEntityStates(List<EntityState> entityStates, bool canPlacePlayer)
    {
        int entityState = 0;
        while (entityState < entityStates.Count)
        {
            yield return new WaitForEndOfFrame();

            if (entityStates[entityState].Type == EntityState.EntityType.Actor)     //Checks if the entity is an actor
            {
                ActorState actorState = entityStates[entityState] as ActorState;

                string entityName = entityStates[entityState].Name.Contains("Remains of") ?
                  entityStates[entityState].Name.Substring(entityStates[entityState].Name.LastIndexOf(' ') + 1) : entityStates[entityState].Name;

                if (entityName == "Player" && !canPlacePlayer)
                {
                    actors[0].transform.position = entityStates[entityState].Position;
                    RefreshPlayer();
                    entityState++;
                    continue;
                }

                Actor actor = MapManager.instance.CreateEntity(entityName, actorState.Position).GetComponent<Actor>();

                actor.LoadState(actorState);                                        //Loads the actor 
            } 
            else if (entityStates[entityState].Type == EntityState.EntityType.Item) //Checks if the entity is an item
            {
                ItemState itemState = entityStates[entityState] as ItemState;

                string entityName = entityStates[entityState].Name.Contains("(E)") ?
                  entityStates[entityState].Name.Replace(" (E)", "") : entityStates[entityState].Name;

                if (itemState.Parent == "Player" && !canPlacePlayer)
                {
                    entityState++;
                    continue;
                }

                Item item = MapManager.instance.CreateEntity(entityName, itemState.Position).GetComponent<Item>();

                item.LoadState(itemState);                                         //Loads the item 
            }

            entityState++;
        }
        isPlayerTurn = true; //Allows player to move after load
    }

    public void Reset(bool canRemovePlayer)     //Removes every entity  
    {
        if (entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (!canRemovePlayer && entity.GetComponent<Player>())
                {
                    continue;
                }

                Destroy(entity.gameObject);
            }

            if (canRemovePlayer)
            {
                entities.Clear();
                actors.Clear();
            }
            else
            {
                entities.RemoveRange(1, entities.Count - 1);
                actors.RemoveRange(1, actors.Count - 1);
            }
        }
    }
}

[System.Serializable]
public class GameState
{
    [SerializeField] private List<EntityState> entities;

    public List<EntityState> Entities { get => entities; set => entities = value; }

    public GameState(List<EntityState> entities)
    {
        this.entities = entities;
    }
}