class Longbow : Weapon
{
    public Longbow()
    {
        this.DamageType = "piercing";
        this.Name = "Longbow";
        this.isRanged = true;
        this.WeaponType = new string[] {"Longbow", "martial weapon"};
        this.Sides = 8;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 'b';
    }
}