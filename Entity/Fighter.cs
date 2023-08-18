using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Fighter : MonoBehaviour
{
    private string Id;
    private FighterState _fighterState;
    
    [SerializeField] private int maxHp, hp, baseDefense, basePower;
    [SerializeField] private Actor target;

    private void Awake()
    {
        Id = gameObject.GetComponent<Entity>().EntityID;
    }

    public int Hp
    {                                   //Sets up the hp of the actor
        get => hp; set
        {
            hp = Mathf.Max(0, Mathf.Min(value, maxHp));

            if (GetComponent<Player>())
            {
                UIManager.instance.SetHealth(hp, maxHp);
            }

            if (hp == 0)
                Die();
        }
    }

    public int MaxHp
    {                            //Sets up the max hp of the actor
        get => maxHp; set
        {
            maxHp = value;
            if (GetComponent<Player>())
            {
                UIManager.instance.SetHealthMax(maxHp);
            }
        }
    }
    //States for defense and power 
    public int BaseDefense { get => baseDefense; set => baseDefense = value; }
    public int BasePower { get => basePower; set => basePower = value; }
    public Actor Target { get => target; set => target = value; }

    public int Power()
    {
        return basePower + PowerBonus();
    }

    public int Defense()
    {
        return baseDefense + DefenseBonus();
    }

    public int DefenseBonus()
    {
        if (GetComponent<Equipment>() is not null)
        {
            return GetComponent<Equipment>().DefenseBonus();
        }

        return 0;
    }

    public int PowerBonus()
    {
        if (GetComponent<Equipment>() is not null)
        {
            return GetComponent<Equipment>().PowerBonus();
        }

        return 0;
    }

    private void Start()
    {
        if (GetComponent<Player>())
        {                   //If actor is a player
            UIManager.instance.SetHealthMax(maxHp);
            UIManager.instance.SetHealth(hp, maxHp);
        }
    }

    public void Die()
    {                               //Dying for both player and mobs
        if (GetComponent<Actor>().IsAlive)
        {
            if (GetComponent<Player>())
            {
                UIManager.instance.AddMessage("You died!", "#ff0000"); //Red
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            else
            {
                GameManager.instance.Actors[0].GetComponent<Level>().AddExperience(GetComponent<Level>().XPGiven); //Give XP to player
                UIManager.instance.AddMessage($"{name} is dead!", "#ffa500"); //Light Orange
            }
            GetComponent<Actor>().IsAlive = false;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.DeadSprite;
        spriteRenderer.color = new Color(191, 0, 0, 1);
        spriteRenderer.sortingOrder = 0;

        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        name = $"Remains of {name}";
        GetComponent<Actor>().BlocksMovement = false;
        if (!GetComponent<Player>())
        {
            GameManager.instance.RemoveActor(this.GetComponent<Actor>());
        }
    }

    public int Heal(int amount)
    {                 //Self explanatory
        if (hp == maxHp)
        {
            return 0;
        }

        int newHPValue = hp + amount;

        if (newHPValue > maxHp)
        {
            newHPValue = maxHp;
        }

        int amountRecovered = newHPValue - hp;
        Hp = newHPValue;
        return amountRecovered;
    }

    public FighterState SaveState() => new FighterState(
        maxHp: maxHp,
        hp: hp,
        defense: baseDefense,
        power: basePower,
        target: target != null ? target.name : null
      );

    public void LoadState(FighterState state)
    {
        maxHp = state.MaxHp;
        hp = state.Hp;
        baseDefense = state.Defense;
        basePower = state.Power;
        target = GameManager.instance.Actors.Find(a => a.name == state.Target);
    }

}

