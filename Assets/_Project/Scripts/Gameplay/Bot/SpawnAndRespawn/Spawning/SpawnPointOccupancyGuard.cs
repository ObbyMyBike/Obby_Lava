using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SpawnPointOccupancyGuard
{
    private readonly ActiveBots activeBots;
    private readonly SpawnedPlayerAccessor playerAccessor;
    private readonly CharacterController prefabController;

    private readonly List<SpawnAreaReservation> reservations = new List<SpawnAreaReservation>(8);
    
    private readonly float extraHorizontalClearance;
    private readonly float extraVerticalClearance;
    private readonly float reservationTtlSeconds;
    
    public SpawnPointOccupancyGuard(ActiveBots activeBots, [InjectOptional] SpawnedPlayerAccessor playerAccessor,
        CharacterController prefabController, float extraHorizontalClearance, float extraVerticalClearance,
        float reservationTtlSeconds)
    {
        this.activeBots = activeBots;
        this.playerAccessor = playerAccessor;
        this.prefabController = prefabController;

        this.extraHorizontalClearance = Mathf.Max(0f, extraHorizontalClearance);
        this.extraVerticalClearance = Mathf.Max(0f, extraVerticalClearance);
        this.reservationTtlSeconds = Mathf.Max(0.01f, reservationTtlSeconds);
    }

    public void Update(float deltaTime)
    {
        if (reservations.Count == 0)
            return;

        for (int i = reservations.Count - 1; i >= 0; i--)
        {
            SpawnAreaReservation spawnAreaReservation = reservations[i];
            spawnAreaReservation.RemainingSeconds -= deltaTime;

            if (spawnAreaReservation.RemainingSeconds <= 0f)
                reservations.RemoveAt(i);
            else
                reservations[i] = spawnAreaReservation;
        }
    }

    public bool IsAreaFree(Vector3 spawnPosition)
    {
        if (prefabController == null)
            return true;

        Bounds candidate = ComputeCandidateBounds(spawnPosition);
        
        for (int i = 0; i < reservations.Count; i++)
            if (reservations[i].Bounds.Intersects(candidate))
                return false;
        
        CharacterController playerController = playerAccessor != null ? playerAccessor.CharacterController : null;
        
        if (playerController != null && playerController.enabled && playerController.gameObject.activeInHierarchy)
        {
            if (playerController.bounds.Intersects(candidate))
                return false;
        }
        
        if (activeBots?.Active != null)
        {
            foreach (BotAgent bot in activeBots.Active)
            {
                if (bot == null)
                    continue;

                CharacterController botController = bot.Controller;
                
                if (botController == null || !bot.gameObject.activeInHierarchy)
                    continue;

                if (botController.bounds.Intersects(candidate))
                    return false;
            }
        }

        return true;
    }

    public void Reserve(Vector3 spawnPosition)
    {
        if (prefabController == null)
            return;

        SpawnAreaReservation spawnAreaReservation = new SpawnAreaReservation
        {
            Bounds = ComputeCandidateBounds(spawnPosition),
            RemainingSeconds = reservationTtlSeconds
        };

        reservations.Add(spawnAreaReservation);
    }

    private Bounds ComputeCandidateBounds(Vector3 spawnPosition)
    {
        float radius = prefabController.radius + extraHorizontalClearance;
        float height = prefabController.height + extraVerticalClearance * 2f;

        Vector3 centerWorld = spawnPosition + prefabController.center + new Vector3(0f, extraVerticalClearance, 0f);
        Vector3 size = new Vector3(radius * 2f, height, radius * 2f);
        
        return new Bounds(centerWorld, size);
    }
}