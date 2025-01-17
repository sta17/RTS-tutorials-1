using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Map_Unit : Map_Object
{

    [Header("Player/Faction")]
    [SerializeField] public int playerID;
    [SerializeField] private bool DoNotusePlayerColour;
    [SerializeField] private Color unitColor;
    public Faction faction;

    [Header("Unit Properties")]
    [SerializeField] public SO_Unit unit;
    private float attackTimer;
    [SerializeField] private bool StartWithMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentMaxHealth;
    [SerializeField] private bool StartWithMaxAether;
    [SerializeField] private float currentAether;
    [SerializeField] private float currentMaxAether;
    [SerializeField] private int unitID;

    [Header("Misc")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform currentTarget;

    [Header("Unit UI")]
    [SerializeField] private UI_Healthbar healthbar;

    #region Unity

    void OnValidate()
    {
        FixUnit();
    }

    public void OnBeforeSerialize()
    {
        FixUnit();
    }

    public void FixUnit()
    {
#if UNITY_EDITOR
        if (Map_Object_Data != null)
        {
            unit = (SO_Unit)Map_Object_Data;
            this.gameObject.name = unit.DisplayName;
            if (StartWithMaxHealth)
            {

                currentHealth = unit.unitMaxHealth;
            }
        }
        if (faction != null)
        {
            if (!DoNotusePlayerColour)
            {
                unitColor = faction.playerColor;
                SetColorEditMode();
            }
            playerID = faction.playerID;
        }
#endif
    }

    #endregion

    #region Setup and Update

    public void Start()
    {
        //unitState.SetMapPresence(this);
        navAgent = GetComponent<NavMeshAgent>();

        if (!DoNotusePlayerColour)
        {
            unitColor = faction.playerColor;
            GetComponent<Renderer>().material.color = unitColor;
        }

        if (StartWithMaxHealth)
        {

            currentHealth = unit.unitMaxHealth;
            currentMaxHealth = currentHealth;
            healthbar.HideBar();
        }
    }

    public void Update()
    {
        if ((currentTarget != null))
        {
            if (currentTarget.CompareTag("Unit"))
            {
                navAgent.destination = currentTarget.position;
            }
            else if (currentTarget.CompareTag("Item"))
            {
                navAgent.destination = currentTarget.position;
            }
        }

        attackTimer += Time.deltaTime;
        if ((currentTarget != null))
        {
            if (currentTarget.CompareTag("Unit"))
            {
                navAgent.destination = currentTarget.position;

                var distance = (transform.position - currentTarget.position).magnitude;

                if (distance <= unit.attackRange)
                {
                    Attack();
                }
            }
            else if (currentTarget.CompareTag("Item"))
            {
                navAgent.destination = currentTarget.position;

                var itemModelHandler = currentTarget.GetComponent<Map_Item>();

                var distance = (itemModelHandler.transform.position - this.gameObject.transform.position).magnitude;

                if (distance <= itemModelHandler.getInteractRange())
                {
                    InteractWithItem(itemModelHandler);
                }
            }
        }
    }

    #endregion

    #region Actions and Movements

    public void InteractWithItem(Map_Item itemHandler)
    {
        var result = false;
        if (itemHandler.getItemMapType() == Item_Map_Type.Use_On_Pickup)
        {
            //result = itemHandler.HandlePickupType(this);
        }
        else
        {
            //result = AddItem(itemHandler.getItem());
        }
        if (result)
        {
            //itemHandler.ModelCleanUp();
        }
    }

    public void MoveUnit(Vector3 dest)
    {
        currentTarget = null;
        navAgent.destination = dest;
    }

    public void SetNewTarget(Transform enemy)
    {
        currentTarget = enemy;
    }

    public bool IsMoving()
    {
        if (!navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Attack()
    {
        if (attackTimer >= unit.attackSpeed)
        {
            UnitTakeDamage(this, currentTarget.GetComponent<Map_Unit>());
            attackTimer = 0;
        }
    }

    #region Take Damage

    public static void UnitTakeDamage(Map_Unit Attacking, Map_Unit Attacked)
    {
        var damage = Attacked.unit.attackDamage;

        Attacked.TakeDamage(Attacking, damage);
    }

    public void TakeDamage(Map_Unit enemy, float damage)
    {

        if (!healthbar.gameObject.activeInHierarchy)
        {
            healthbar.ShowBar();
        }
        if (currentHealth > 0)
        {
            StartCoroutine(Flasher());

            currentHealth -= damage;
            healthbar.SetHealth(unit.unitMaxHealth, currentHealth);
        }
        else
        {
            ZeroHealth();
            Debug.Log("Dead");
        }


    }

    IEnumerator Flasher()
    {
        var renderer = GetComponent<Renderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = Color.gray;
            yield return new WaitForSeconds(.05f);
            renderer.material.color = unitColor;
            yield return new WaitForSeconds(.05f);
        }
    }

    private void ZeroHealth()
    {
        //player.RaiseDeathChangeNotification(this);
        Destroy(this.gameObject);
    }

    #endregion


    #endregion

    #region Misc

    public void SetColor()
    {
        if (unitColor != null)
        {
            GetComponent<Renderer>().material.color = unitColor;
        }
    }

    public void SetColorEditMode()
    {
        if (unitColor != null)
        {
            GetComponent<Renderer>().sharedMaterial.color = unitColor;
        }
    }

    public float getCurrentAether()
    {
        return currentAether;
    }

    public float getCurrentMaxAether()
    {
        return currentMaxAether;
    }

    public float getCurrentHealth()
    {
        return currentHealth;
    }

    public float getCurrentMaxHealth()
    {
        return currentMaxHealth;
    }
    #endregion
}
