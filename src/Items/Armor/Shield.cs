using System;

class Shield : Armor
{
    int BonusAC;
    public Shield ()
    {
        this.BonusAC = 2;
        this.Name = "Shield";
        this.ArmorType = new string[] {"shield"};
    }

    public override string ToString()
    {
        return $"{this.Name} +{this.BonusAC} AC";
    }
    
}
