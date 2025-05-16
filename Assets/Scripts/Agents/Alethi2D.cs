using UnityEngine;
using System.Collections;
public class Alethi2D : Agent2D
{
    public override void EngageCombat(Agent2D other)
    {
        other.TakeDamage(config.damage);
    }
    public override void CollectResource(ResourceBase2D resource)
    {
        if (resource is Gem2D)
        {
            // handle gem collection
        }
        else if (resource is Armor2D)
        {
            StartCoroutine(ApplyArmorBuff());
        }
    }

    private IEnumerator ApplyArmorBuff()
    {
        int originalDamage = config.damage;
        config.damage *= 10;
        yield return new WaitForSeconds(10f);
        config.damage = originalDamage;
    }

}
