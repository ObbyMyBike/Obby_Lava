using UnityEngine;

public delegate void OnUiClicked();

public delegate void OnPressed();

public delegate void OnUsed();

public delegate void OnCountChange(ItemType type, int count, Sprite icon);

public delegate void OnEmpty(ItemType type);

public delegate void OnPlacementModeChange(bool enabled);

public delegate void OnRespawnRequest();

public delegate void OnCheckpointActivate(Transform checkpoint);

public delegate void OnSkinUnlock(SkinIdType skinId);

public delegate void OnSkinApply(SkinIdType skinId);

public delegate void OnGoldChanged(int value);

public delegate void OnItemPurchased(ItemType type);

public delegate void OnRewardedAdComplete();

public delegate void OnExpired(ItemType type);

public delegate void OnEffectLaunch(ItemType type, Sprite icon, float durationSeconds);

public delegate void OnLavaCountdownDisplay(float seconds);

public delegate void OnLavaHeightChange(float height);

public delegate void OnLavaPhaseChange(LavaPhaseType phase, int stepIndex);

public delegate void OnLavaPreCountdownDisplay(float seconds);

public delegate void OnLavaRiseComplete(float finalHeight);

public delegate void OnLavaRiseStart();

public delegate void OnDie();

public delegate void OnHealthChange(int current, int max);

public delegate void OnPush();

public delegate void OnClimbEnter(Transform ladderFacing);

public delegate void OnClimbExit(Transform ladderFacing);

public delegate void OnClimbSpeedChange(float signedSpeed);

public delegate void OnClimbStateChange(bool isClimbing);

public delegate void OnPlayerSpawn(Transform playerTransform);