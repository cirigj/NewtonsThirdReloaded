using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JBirdEngine;

public interface IKillable {

    void Kill ();

}

public interface IDamagable {

    void TakeDamage (float dmg, bool fromProjectile, Vector3 dmgPos);
    void TakeDamage (float dmg);

}

public interface IShootable : IDamagable {

    void Interact (Projectile proj);
    float CalculateProjectileDamageReduction (float dmg);

}

public interface ICollidable : IDamagable {

    Vector3 GetVelocity ();
    void SetVelocity (Vector3 vel);
    float GetMass ();
    float GetDamage (float momentumDiff);
    float CalculateCollisionDamageReduction (float dmg);
    Vector3 GetPosition ();
    float GetElasticity ();

}

public static class ICollidableExtensions {

    public static void HandleCollision (this ICollidable me, ICollidable target) {
        // Reference frame: target is standing still (this makes calculations easier)
        // Adjust velocities to fit the reference frame
        Vector3 otherInitialVelocity = target.GetVelocity();
        Vector3 initialVelocity = me.GetVelocity() - otherInitialVelocity;
        // Target will be sent directly away from my center of mass
        Vector3 directionTowardsOther = (target.GetPosition() - me.GetPosition()).normalized;
        // Get my initial kinetic energy
        // My KE is equal to the total KE of the system, because the target is stationary in this reference frame
        float initialKineticEnergy = (0.5f * me.GetMass() * me.GetVelocity().sqrMagnitude);
        // How much KE is transferred to the target depends on how direct the hit was
        float kEPercentage = Vector3.Dot(directionTowardsOther, initialVelocity.normalized);
        float transferredKE = initialKineticEnergy * kEPercentage;
        // Elasticity determines how much KE is converted into damage and how much becomes recoil
        float elasticityAvg = (me.GetElasticity() + target.GetElasticity()) / 2f;
        float recoilKE = transferredKE * elasticityAvg;
        float convertedKE = transferredKE - recoilKE;
        // Get the new speed of the target from the recoil KE and set velocity in the direction of impact
        float theirSpeed = Mathf.Sqrt(Mathf.Abs(2f * recoilKE * kEPercentage) / me.GetMass());
        Vector3 theirNewVelocity = directionTowardsOther * theirSpeed;
        // Calculate my new velocity using the conservation of momentum formula
        Vector3 myNewVelocity = initialVelocity - ((target.GetMass() * theirNewVelocity) / me.GetMass());
        // Set velocities of both bodies, shifting back to the global reference frame
        me.SetVelocity(myNewVelocity + otherInitialVelocity);
        target.SetVelocity(theirNewVelocity + otherInitialVelocity);
        // Take damage based on the converted KE
        me.TakeDamage(me.CalculateCollisionDamageReduction(target.GetDamage(convertedKE)), false, VectorHelper.Midpoint(me.GetPosition(), target.GetPosition()));
        target.TakeDamage(target.CalculateCollisionDamageReduction(me.GetDamage(convertedKE)), false, VectorHelper.Midpoint(me.GetPosition(), target.GetPosition()));
    }

}
