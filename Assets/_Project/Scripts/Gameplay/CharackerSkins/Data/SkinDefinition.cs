using System;
using UnityEngine;

[Serializable]
public struct SkinDefinition
{
    public SkinIdType Id;
    public string ChildNameUnderPlayerModel;
    public string RewardedAdvId;
    public string DisplayNameLocKey;
    public GameObject PickupVisualPrefab;
}