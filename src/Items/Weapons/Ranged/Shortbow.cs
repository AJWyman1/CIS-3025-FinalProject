class Shortbow : Weapon
{
    public Shortbow()
    {
        this.DamageType = "piercing";
        this.Name = "Shortbow";
        this.isRanged = true;
        this.WeaponType = new string[] {"Shortbow", "simple weapon"};
        this.Sides = 6;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 'b';
    }
}