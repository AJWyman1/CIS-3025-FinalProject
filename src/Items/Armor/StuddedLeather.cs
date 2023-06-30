using System;

class StuddedLeather : Armor
{
    public StuddedLeather ()
    {
        this.ArmorType = new string[] {"light armor"};
        this.StealthDisadvantage = false;
        this.DexModifier = true;
        this.ArmorClass = 12;
    }
}