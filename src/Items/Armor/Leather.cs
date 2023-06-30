using System;

class Leather : Armor
{
    public Leather ()
    {
        this.ArmorType = new string[] {"light armor", "light nonmetal armor"};
        this.StealthDisadvantage = false;
        this.DexModifier = true;
        this.ArmorClass = 14;
    }
}