using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
sealed class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isEnemyColliding;
    [SerializeField] private bool isItemColliding;
    [SerializeField] private Actor targetEnemy;
    [SerializeField] private Item targetItem;
    Vector2 direction = Vector2.zero;

    private Controls controls;

    private InputAction move;
    private InputAction exit;
    private InputAction view;
    private InputAction pickup;
    private InputAction inventory;
    private InputAction drop;
    private InputAction confirm;
    private InputAction info;
    private InputAction attack;

    private Actor actor;

    private void Awake()
    {
        controls = new Controls();
        actor = GetComponent<Actor>();
    }

    private void OnEnable()
    {
        move = controls.Player.Movement;
        move.Enable();

        exit = controls.Player.Exit;
        exit.Enable();
        exit.performed += Exit;

        view = controls.Player.View;
        view.Enable();
        view.performed += View;

        pickup = controls.Player.Pickup;
        pickup.Enable();
        pickup.performed += Pickup;

        inventory = controls.Player.Inventory;
        inventory.Enable();
        inventory.performed += OnInventory;

        drop = controls.Player.Drop;
        drop.Enable();
        drop.performed += Drop;

        confirm = controls.Player.Confirm;
        confirm.Enable();
        confirm.performed += Confirm;

        info = controls.Player.Info;
        info.Enable();
        info.performed += Info;

        attack = controls.Player.Attack;
        attack.Enable();
        attack.performed += Attack;
    }

    private void OnDisable()
    {
        move.Disable();
        exit.Disable();
        view.Disable();
        pickup.Disable();
        inventory.Disable();
        drop.Disable();
        confirm.Disable();
        info.Disable();
    }

    private void Update()
    {
        direction = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {                                                                  //Allows player to move
        if (!UIManager.instance.IsMenuOpen)
        {
            if (GetComponent<Actor>().IsAlive)
            {
                rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
            }
        }
        actor.UpdateFieldOfView();
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CanAct())
            {
                if (isEnemyColliding && targetEnemy)
                {
                    MeleeAction(GetComponent<Actor>(), targetEnemy);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Actor>() != null)
        {
            isEnemyColliding = true;
            targetEnemy = collision.gameObject.GetComponent<Actor>();
            // Debug.Log($"EnemyColliding: {isEnemyColliding}");
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (targetEnemy != null)
        {
            isEnemyColliding = false;
            targetEnemy = null;
            // Debug.Log($"EnemyColliding: {isEnemyColliding}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Item>() != null)
        {
            isItemColliding = true;
            targetItem = collision.gameObject.GetComponent<Item>();
            // Debug.Log($"Colliding with item: {isItemColliding}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targetItem != null)
        {
            isItemColliding = false;
            targetItem = null;
            // Debug.Log($"Colliding with item: {isItemColliding}");
        }
    }

    private void Exit(InputAction.CallbackContext context)
    {    //When you press escape
        if (context.performed)
        {
            if (!UIManager.instance.IsEscapeMenuOpen && !UIManager.instance.IsMenuOpen)
            {  //If escape menu is not on and if menu is not open
                UIManager.instance.ToggleEscapeMenu();                                              //Turns on escape menu
            }
            else if (UIManager.instance.IsMenuOpen)
            {                                           //If menu is on 
                UIManager.instance.ToggleMenu();
            }
        }
    }

    public void View(InputAction.CallbackContext context)
    {                             //When you press V, for message history 
        if (context.performed)
        {
            if (!UIManager.instance.IsMenuOpen || UIManager.instance.IsMessageHistoryOpen)
            {  //If menu is off or if message history is open
                UIManager.instance.ToggleMessageHistory();                                      //Turn on/off message history
            }
        }
    }

    public void Pickup(InputAction.CallbackContext context)
    {                           //When you press G, picks up item 
        if (context.performed)
        {
            if (CanAct())
            {
                if (isItemColliding && targetItem)
                {
                    PickupAction(GetComponent<Actor>(), targetItem);
                }
            }
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {                        //When you press I, toggles inventory
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsInventoryOpen)
            {
                if (GetComponent<Inventory>().Items.Count > 0)
                {
                    UIManager.instance.ToggleInventory(GetComponent<Actor>());
                }
                else
                {
                    UIManager.instance.AddMessage("You have no items.", "#808080");
                }
            }
        }
    }

    public void Drop(InputAction.CallbackContext context)
    {                             //When you press D, toggles inventory for you to drop items
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsDropMenuOpen)
            {
                if (GetComponent<Inventory>().Items.Count > 0)
                {
                    UIManager.instance.ToggleDropMenu(GetComponent<Actor>());
                }
                else
                {
                    UIManager.instance.AddMessage("You have no items.", "#808080");
                }
            }
        }
    }

    public void Confirm(InputAction.CallbackContext context)
    {                                                           //Pressing enter 
        if (context.performed)
        {
            if (CanAct())
            {
                TakeStairsAction(GetComponent<Actor>());
            }
        }
    }

    public void Info(InputAction.CallbackContext context)
    {                             //When you press C, it pulls up the character info
        if (context.performed)
        {
            if (CanAct() || UIManager.instance.IsCharacterInformationMenuOpen)
            {
                UIManager.instance.ToggleCharacterInformationMenu(GetComponent<Actor>());
            }
        }
    }


    private bool CanAct()
    {
        if (UIManager.instance.IsMenuOpen || !GetComponent<Actor>().IsAlive)
        {    //If target mode is on, menus are open, or the entity is not alive
            return false;                                                                         //It cannot act
        }
        else
        {
            return true;                                                                          //Else it can
        }
    }



    private void TakeStairsAction(Actor actor)
    {                                        //For going between levels
        Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(new Vector3(Mathf.Floor(actor.transform.position.x) + 0.5f, Mathf.Floor(actor.transform.position.y) + 0.5f, 0));    //Checks players position
        string tileName = MapManager.instance.FloorMap.GetTile(pos).name;                       //Gets tile at players position

        if (tileName != MapManager.instance.UpStairsTile.name && tileName != MapManager.instance.DownStairsTile.name)
        {     //Sees if it is a stair tile
            UIManager.instance.AddMessage("There are no stairs here.", "#0da2ff");
            return;
        }
        
        if (SaveManager.instance.CurrentFloor == 1 && tileName == MapManager.instance.UpStairsTile.name)
        {  //Can't go back a level if you are on level 1
            UIManager.instance.AddMessage("A mysterious force prevents you from going back.", "#0da2ff");
            return;
        }

        SaveManager.instance.SaveGame();
        SaveManager.instance.CurrentFloor += tileName == MapManager.instance.UpStairsTile.name ? -1 : 1;

        if (SaveManager.instance.Save.Scenes.Exists(x => x.FloorNumber == SaveManager.instance.CurrentFloor))
        {
            SaveManager.instance.LoadScene(false);
        }
        else
        {
            GameManager.instance.Reset(false);
            MapManager.instance.GenerateDungeon();
        }

        UIManager.instance.AddMessage("You take the stairs.", "#0da2ff");
        UIManager.instance.SetDungeonFloorText(SaveManager.instance.CurrentFloor);
    }

    private void PickupAction(Actor actor, Item item)
    {
        if (actor.Inventory.Items.Count >= actor.Inventory.Capacity)
        {
            UIManager.instance.AddMessage($"Your inventory is full.", "#808080");
            return;
        }
        else
        {
            actor.Inventory.Add(item);
        }
        UIManager.instance.AddMessage($"You picked up the {item.name}!", "#FFFFFF");
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
}

