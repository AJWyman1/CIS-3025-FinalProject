class LightCrossbow : Weapon
{
    public LightCrossbow()
    {
        this.DamageType = "piercing";
        this.Name = "Light Crossbow";
        this.isRanged = true;
        this.WeaponType = new string[] {"light crossbow", "simple weapon"};
        this.Sides = 8;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 't';
    }
}