using UnityEngine;

public class KnifeBehavior : ProjectileWeaponController
{

    
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        transform.position += direction * weaponData.Speed * Time.deltaTime;// Set the mnovement of Knife
    }
}
