using System.Collections.Generic;

public static class StatsManager
{
    public static float currentHp = 100;
    public static float maxHp = 100;
    public static List<Character_Movement.PowerUp> myUpgrades = new List<Character_Movement.PowerUp>();
    public static float ulti1Stacks;

    public static void CopyStats(Character_Movement myChar)
    {
        myChar.GetComponent<Health>().currentHP = currentHp;
        myChar.GetComponent<Health>().maxHP = maxHp;
        myChar.myUpgrades = myUpgrades;
    }

    public static void SaveStats(Character_Movement myChar)
    {
        if (currentHp == 0) return;

        currentHp = myChar.GetComponent<Health>().currentHP;
        maxHp = myChar.GetComponent<Health>().maxHP;
        myUpgrades = myChar.myUpgrades;
    }

}
