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

        Zglorgette_Jeune,
		Zglorgette_Adulte,
        Zglorgette_Conquérant,

        Zglorg_GrandConquérant,
        Butcher_GrandConquérant,
        Zglorgette_GrandConquérant,
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
        AutoAttack_Zglorgette,
        Spell_Zglorgette,
	}
    [System.Serializable] public enum ObjectType {
        LifePotion,
        ButcherCheckArea,
	}
}
