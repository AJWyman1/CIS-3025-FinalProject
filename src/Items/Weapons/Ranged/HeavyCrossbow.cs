class HeavyCrossbow : Weapon
{
    public HeavyCrossbow()
    {
        this.DamageType = "piercing";
        this.Name = "Heavy Crossbow";
        this.isRanged = true;
        this.WeaponType = new string[] {"heavy crossbow", "martial weapon"};
        this.Sides = 10;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 't';
    }
}