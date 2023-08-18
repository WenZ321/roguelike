using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;

    [SerializeField] private string mapID;
    [SerializeField] private string sceneName;

    public string MapID { get => mapID; set => mapID = value; }
    public string SceneName { get => sceneName; set => sceneName = value; }

    [Header("Time")]
    [SerializeField] private float baseTime = 0.075f;
    [SerializeField] private float delayTime; //Read-only

    [Header("Entities")]
    [SerializeField] private List<Entity> entities;
    [SerializeField] private List<Actor> actors;

    [Header("Death")]
    [SerializeField] private Sprite deadSprite;
    public List<Entity> Entities { get => entities; }
    public List<Actor> Actors { get => actors; }
    public Sprite DeadSprite { get => deadSprite; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        SceneName = SceneManager.GetActiveScene().name;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    public void AddEntity(Entity entity)
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }
        entities.Add(entity);
    }

    public void InsertEntity(Entity entity, int index)
    {
        if (!entity.gameObject.activeSelf)
        {
            entity.gameObject.SetActive(true);
        }
        entities.Insert(index, entity);
    }

    public void RemoveEntity(Entity entity)
    {
        entity.gameObject.SetActive(false);
        entities.Remove(entity);
    }

    public void AddActor(Actor actor)
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

    public Actor GetActorAtLocation(Vector3 location)
    {
        foreach (Actor actor in actors)
        {
            if (actor.BlocksMovement && actor.transform.position == location)
            {
                return actor;
            }
        }
        return null;
    }

    private float SetTime() => baseTime / actors.Count;

    public void LoadData(GameData data)
    {
        GameState _gameData;

        string mapid;
        data.savedScenes.TryGetValue(SceneName, out mapid);
        if(mapid != null)
        {
            mapid = MapID;
            data.gameState.TryGetValue(MapID, out _gameData);
            if (_gameData != null)
            {
                StartCoroutine(LoadEntityStates(_gameData.Entities));
            }
        } else
        {

        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.savedScenes.ContainsKey(SceneName))
        {
            data.savedScenes.Remove(SceneName);
        }
        else
        {
            data.savedScenes.Add(SceneName, MapID);
        }

        if (data.gameState.ContainsKey(MapID))
        {
            data.gameState.Remove(MapID);
        } else
        {
            foreach (Item item in actors[0].Inventory.Items)
            {
                AddEntity(item);
            }

            GameState _gameState = new GameState(entities: entities.ConvertAll(x => x.SaveState()));

            foreach (Item item in actors[0].Inventory.Items)
            {
                RemoveEntity(item);
            }

            data.gameState.Add(MapID, _gameState);  
        }
    }

    private IEnumerator LoadEntityStates(List<EntityState> entityStates)
    {
        int entityState = 0;
        while (entityState < entityStates.Count)
        {
            yield return new WaitForEndOfFrame();



            if (entityStates[entityState].Type == EntityType.Actor)
            {
                ActorState actorState = entityStates[entityState] as ActorState;
                string entityName = entityStates[entityState].Name.Contains("Remains of") ?
                  entityStates[entityState].Name.Substring(entityStates[entityState].Name.LastIndexOf(' ') + 1) : entityStates[entityState].Name;

                if (entityName == "Player")
                {
                    actors[0].transform.position = entityStates[entityState].Position;
                    entityState++;
                    continue;
                }

                Actor actor = MapManager.instance.CreateEntity(entityName, actorState.Position).GetComponent<Actor>();

                actor.LoadState(actorState);
            }
            else if (entityStates[entityState].Type == EntityType.Item)
            {
                ItemState itemState = entityStates[entityState] as ItemState;

                string entityName = entityStates[entityState].Name.Contains("(E)") ?
                  entityStates[entityState].Name.Replace(" (E)", "") : entityStates[entityState].Name;

                if (itemState.Parent == "Player")
                {
                    entityState++;
                    continue;
                }

                Item item = MapManager.instance.CreateEntity(entityName, itemState.Position).GetComponent<Item>();

                item.LoadState(itemState);
            }

            entityState++;
        }
    }

    public void Reset(bool canRemovePlayer)
    {
        if (entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (entity.GetComponent<Player>())
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

