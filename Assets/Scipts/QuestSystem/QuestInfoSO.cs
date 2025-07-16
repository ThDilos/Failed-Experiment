using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObject/QuestInfoSO Info")]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public int levelRequirement;
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int itemReward;
    public int gaugeReward;

    // ensure the id is always the name of the Scriptable Object asset
    private void OnValidate()
    {
        #if UNITY_EDITOR
                id = this.name;
                UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
