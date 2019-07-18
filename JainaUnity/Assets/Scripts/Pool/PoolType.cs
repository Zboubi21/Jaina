namespace PoolTypes {
    [System.Serializable] public enum PoolType {
		EnemyType,
		SpellType,
        ObjectType,
	}
	[System.Serializable] public enum EnemyType {
		Zglorg_Jeune,
		Zglorg_Adulte,
        Zglorg_Conquérant,
        Butcher_Jeune,
		Butcher_Adulte,
        Butcher_Conquérant,
	}
    [System.Serializable] public enum SpellType {
		AutoAttack_Ice,
		AutoAttack_Fire,
		AutoAttack_Arcane,
        IceNoca,
        IceBuff,
        FireBalls,
        FireTrail,
        ArcaneProjectile1,
        ArcaneProjectile2,
        ArcaneProjectile3,
        ArcaneExplosion,
	}
    [System.Serializable] public enum ObjectType {
        LifePotion,
        ButcherCheckArea,
	}
}
