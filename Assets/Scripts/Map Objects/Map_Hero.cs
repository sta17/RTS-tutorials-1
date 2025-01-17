using UnityEngine;

public class Map_Hero : Map_Unit
{

    [field: Header("Hero Properties")]
    [field: SerializeField] public int CurrentHeroExperience { get; private set; } = 0;
    [field: SerializeField] public int CurrentHeroLevel { get; private set; } = 1;

    #region Unity

    void OnValidate()
    {
        FixUnit();
        FixHero();
    }

    public new void OnBeforeSerialize()
    {
        FixUnit();
        FixHero();
    }

    public void FixHero()
    {
#if UNITY_EDITOR

#endif
    }

    #endregion

    #region Setup and Update

    #endregion

    #region Misc

    #endregion
}
