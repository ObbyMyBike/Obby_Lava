using System.Collections.Generic;
using UnityEngine;

public static class BotRandomNamesProvider
{
    private static readonly string[] allNicknames = new string[]
    {
        "CoolGamer42","xXShadowNinjaXx","PixelFox77","BuilderMaxYT","EpicDude999",
        "RainbowSlayer12","KittyPlayz34","ProMasterX","OofLord88","BlazeFire07",
        "ThunderKid101","IceDragon55","FunnyBaconMan","SpeedyNoob22","CraftyWolf19",
        "DarkKnight420","PopcornKing33","RobloxianXxX","HyperBlast21","SneakyCat99",
        "ZombieHunter64","BananaBoy07","LuckyDuck88","MegaCoderX","GhostlyNub12",
        "LavaKing77","TurboPenguin9","NinjaPanda42","GalaxyGirl101","BaconHairPro",
        "SharkByte77","FlamingTaco22","DragonSlayerX","PotatoQueen15","AstroNova99",
        "ChillBro27","EpicUnicornXx","SneakAttack11","RoboNerd2025","LightningKid44",
        "CookieBlaster9","SpookyGhost77","ProBaconatorX","CyberKnight22","CoolSloth88",
        "BuilderBot303","PixelatedNub12","CrystalWolf77","MemeOverlordXx","ToxicGamer420",
        "NoobMaster69","GoldenPickle77","xXDogeMemeXx","CreeperKid999","FrostyPenguin11",
        "RobloxHero101","PizzaWarrior88","DragonFruitXx","GalaxyNub420","UltraKnight55",
        "HappySlime77","DarkWizard22","DerpyCat3000","FastBuilder09","BanHammerPro",
        "CrazyKoala12","EpicSharkXxX","DiamondBacon77","ThunderNub101","LavaPenguinX",
        "CaptainMeme88","RoboBuilder22","ProPizzaMan77","ShadowWolfXx","LightningGirl07",
        "MasterNoobX","TofuSlayer11","FluffyPanda99","AstroDoggo88","SneakyNoodleX",
        "MegaBanana42","ChocoNub777","BuilderQueenX","TrollKing999","ZombiePenguinXx",
        "EpicOof123","PandaKnight77","TurboBaconX","PixelNerd99","FunnyPickle44",
        "SpicyDragon77","RoboSlothXx","IceCreamNub12","TacoWizard99","LaserKid2025",
        "DarkUnicornX","Noobinator77","CosmicFox101","BaconOverlordXx","HyperDuck88"
    };

    private static List<string> shuffledNicknames = new List<string>(allNicknames);
    private static int currentIndex = 0;

    public static string GetNextNickname()
    {
        if (shuffledNicknames.Count == 0)
            return null;

        if (currentIndex >= shuffledNicknames.Count)
        {
            Shuffle();
            currentIndex = 0;
        }

        string nextNick = shuffledNicknames[currentIndex];
        currentIndex++;
        return nextNick;
    }

    private static void Shuffle()
    {
        for (int i = shuffledNicknames.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = shuffledNicknames[i];
            shuffledNicknames[i] = shuffledNicknames[j];
            shuffledNicknames[j] = temp;
        }
    }
}
