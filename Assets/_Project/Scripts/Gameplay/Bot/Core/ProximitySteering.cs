using System.Collections.Generic;
using UnityEngine;

public class ProximitySteering
{
    public Vector3 ComputeSeparation(BotAgent self, IReadOnlyCollection<BotAgent> allBots, float minDistance)
    {
        if (allBots == null || allBots.Count <= 1 || minDistance <= 0f)
            return Vector3.zero;

        Vector3 result = Vector3.zero;
        Vector3 selfPosition = self.Controller.transform.position;

        foreach (BotAgent other in allBots)
        {
            if (other == null || other == self)
                continue;
            
            Vector3 delta = selfPosition - other.Controller.transform.position;
            delta.y = 0f;
            
            float magnitude = delta.magnitude;
            
            if (magnitude > 0f && magnitude < minDistance)
            {
                float strength = 1f - Mathf.Clamp01(magnitude / minDistance);
                
                result += (delta / Mathf.Max(magnitude, 0.0001f)) * strength;
            }
        }

        return result;
    }
}