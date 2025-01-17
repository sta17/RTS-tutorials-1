using UnityEditor;
using UnityEngine;

public enum PlayerTypes
{
    Human,
    AI,
    NPC
}

[System.Serializable]
public class Faction : MonoBehaviour
{
    [Header("Player Stats")]
    public string playerName;
    public Color playerColor;
    public int playerID;
    public PlayerTypes playerType;

    #region Unity

    void OnValidate()
    {
        FixFaction();
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
        FixFaction();
    }

    public void FixFaction()
    {
#if UNITY_EDITOR
            if (playerName != null)
            {
                this.gameObject.name = playerName;
            }
#endif
    }

    #endregion
}
