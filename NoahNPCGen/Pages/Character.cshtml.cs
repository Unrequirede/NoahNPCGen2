using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Kevsoft.PDFtk;
using System.Text.RegularExpressions;

namespace NoahNPCGen.Pages
{
    public class CharacterModel : PageModel
    {
        public int displayLevel = -1, displayExp = -1, displayStr = -1, displayDex = -1, displayCon = -1, displayInt = -1,
            displayWis = -1, displayCha = -1, displayProf = -1, displayHitDice = -1, displayHP = -1, displayAC = -1, displaySpeed = -1;
        public string otherPro, persTrait = "", persIdeal = "", persBonds = "", persFlaws = "", features="";

        JObject classObject, bgObject, mcObject, scObject;
        JArray scfObject;
        public List<string> otherProf = new List<string>();
        public List<string[]> attackList = new List<string[]>(), featuresTraits = new List<string[]>();
        public Dictionary<string, string> coreAttr;
        public List<DNDItem> itemSelect = new List<DNDItem>();
        public static Checkbox ch = new Checkbox();
        public static LoadAPI load = new LoadAPI();
        public static Generate gr = new Generate();
        public List<Checkbox> profChecks = ch.AbilityProf();
        public List<Checkbox> saveChecks = ch.SavingThro();
        public List<int> ablMods;
        public Queue<int> quantumQueue = load.LoadQuan();

        public class DNDItem
        {
            public string Index { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public DNDItem(string index, string name, int quantity)
            {
                Index = index;
                Name = name;
                Quantity = quantity;
            }
        }

        public void OnGetSingleOrder(string charName, string charRace, string charClass, string charSubClass, int charLevel, string charBackG, string charAlignment)
        {
            displayLevel = charLevel;
            coreAttr = new Dictionary<string, string>() { { "displayName", charName }, { "displayRace", charRace }, { "displayClass", charClass }, { "displaySubClass", charSubClass }, { "displayBackG", charBackG }, { "displayAlignment", charAlignment }, };
            gr.RandomAttr(ref coreAttr, ref displayLevel, ref classObject, ref mcObject, ref scObject, ref scfObject, ref bgObject);
            displayHitDice = (int)classObject["hit_die"];
            gr.GetStats(ref coreAttr, mcObject, classObject, ref displayStr, ref displayDex, ref displayCon, ref displayInt, ref displayWis, ref displayCha);
            GetProf();
            GetRaceInfo();
            ablMods = new List<int> {};
            GetAblMod();
            GetLevelInfo();
            GetEquipment();
            GetCombat();
            GetPersonality();
            otherPro = GetOtherProf();
            
        }


        public async Task<IActionResult> OnPost(string nameBox, string classBox, string backGBox, string playerBox, string raceBox, string alignmentBox, string experienceBox,
            string strAbl, string strMod, string strSav, string dexAbl, string dexMod, string dexSav, string conAbl, string conMod, string conSav, string intAbl, string intMod,
            string intSav, string wisAbl, string wisMod, string wisSav, string chaAbl, string chaMod, string chaSav, string profBonus, string strSaveThr, string dexSaveThr,
            string conSaveThr, string intSaveThr, string wisSaveThr, string chaSaveThr, string dexAcroMod, string wisAnimMod, string intArcMod, string strAthMod, string chaDecMod,
            string intHisMod, string wisInsMod, string chaIntMod, string intInvMod, string wisMedMod, string intNatMod, string wisPerMod, string chaPerfMod, string chaPersMod,
            string intRelMod, string dexSleMod, string dexSteMod, string wisSurMod, string passPerc, string otherProf, string acSco, string initSco, string speedSco, string maxHP,
            string currHP, string tempHP, string dieLevel, string hitDie, string dsSucc1, string dsSucc2, string dsSucc3, string dsFail1, string dsFail2, string dsFail3, string atk1Nam,
            string atk1Bon, string atk1Dam, string atk2Nam, string atk2Bon, string atk2Dam, string atk3Nam, string atk3Bon, string atk3Dam, string allEquipment, string persTrait,
            string idealTrait, string bondTrait, string flawTrait, string features, string dexAcr, string wisAni, string intArc, string strAth, string chaDec, string intHis,
            string wisIns, string chaInt, string intInv, string wisMed, string intNat, string wisPer, string chaPerf, string chaPers, string intRel, string dexSle, string dexSte,
            string wisSur)
        {
            var pdftk = new PDFtk();

            var pdfFile = await System.IO.File.ReadAllBytesAsync("CharacterSheet-FormFillable.pdf");

            var fieldData = new Dictionary<string, string>()
            {
                ["CharacterName"] = nameBox,
                ["ClassLevel"] = classBox,
                ["PlayerName"] = playerBox,
                ["Race "] = raceBox,
                ["Alignment"] = alignmentBox,
                ["XP"] = experienceBox,
                ["Background"] = backGBox,
                ["STR"] = strAbl,
                ["STRmod"] = strMod,
                ["DEX"] = dexAbl,
                ["DEXmod "] = dexMod,
                ["CON"] = conAbl,
                ["CONmod"] = conMod,
                ["INT"] = intAbl,
                ["INTmod"] = intMod,
                ["WIS"] = wisAbl,
                ["WISmod"] = wisMod,
                ["CHA"] = chaAbl,
                ["CHamod"] = chaMod,
                ["ProfBonus"] = profBonus,
                ["ST Strength"] = strSaveThr,
                ["ST Dexterity"] = dexSaveThr,
                ["ST Constitution"] = conSaveThr,
                ["ST Intelligence"] = intSaveThr,
                ["ST Wisdom"] = wisSaveThr,
                ["ST Charisma"] = chaSaveThr,
                ["Check Box 11"] = (strSav == "on") ? "Yes" : "Off", //str saving throw check
                ["Check Box 12"] = (dsSucc1 == "on") ? "Yes" : "Off", //death save success 1
                ["Check Box 13"] = (dsSucc2 == "on") ? "Yes" : "Off", //death save success 2
                ["Check Box 14"] = (dsSucc3 == "on") ? "Yes" : "Off", //death save success 3
                ["Check Box 15"] = (dsFail1 == "on") ? "Yes" : "Off", //death save fail 1
                ["Check Box 16"] = (dsFail2 == "on") ? "Yes" : "Off", //death save fail 2
                ["Check Box 17"] = (dsFail3 == "on") ? "Yes" : "Off", //death save fail 3
                ["Check Box 18"] = (dexSav == "on") ? "Yes" : "Off", //dex saving throw check
                ["Check Box 19"] = (conSav == "on") ? "Yes" : "Off", //con saving throw check
                ["Check Box 20"] = (intSav == "on") ? "Yes" : "Off", //int saving throw check
                ["Check Box 21"] = (wisSav == "on") ? "Yes" : "Off", //wis saving throw check
                ["Check Box 22"] = (chaSav == "on") ? "Yes" : "Off", //cha saving throw check
                ["Check Box 23"] = (dexAcr == "on") ? "Yes" : "off", //acrobatics check
                ["Check Box 24"] = (wisAni == "on") ? "Yes" : "off", //animal handling check
                ["Check Box 25"] = (intArc == "on") ? "Yes" : "off", //arcana check
                ["Check Box 26"] = (strAth == "on") ? "Yes" : "off", //athletics check
                ["Check Box 27"] = (chaDec == "on") ? "Yes" : "off", //deception check
                ["Check Box 28"] = (intHis == "on") ? "Yes" : "off", //history check
                ["Check Box 29"] = (wisIns == "on") ? "Yes" : "off", //insight check
                ["Check Box 30"] = (chaInt == "on") ? "Yes" : "off", //intimidation check
                ["Check Box 31"] = (intInv == "on") ? "Yes" : "off", //investigation check
                ["Check Box 32"] = (wisMed == "on") ? "Yes" : "off", //medicine check
                ["Check Box 33"] = (intNat == "on") ? "Yes" : "off", //nature check
                ["Check Box 34"] = (wisPer == "on") ? "Yes" : "off", //perception check
                ["Check Box 35"] = (chaPerf == "on") ? "Yes" : "off", //performance check
                ["Check Box 36"] = (chaPers == "on") ? "Yes" : "off", //persuasion check
                ["Check Box 37"] = (intRel == "on") ? "Yes" : "off", //religion check
                ["Check Box 38"] = (dexSle == "on") ? "Yes" : "off", //sleight of hand check
                ["Check Box 39"] = (dexSte == "on") ? "Yes" : "off", //stealth check
                ["Check Box 40"] = (wisSur == "on") ? "Yes" : "off", //survival check
                ["Acrobatics"] =  dexAcroMod,
                ["Animal"] = wisAnimMod,
                ["Arcana"] = intArcMod,
                ["Athletics"] = strAthMod,
                ["Deception "] = chaDecMod,
                ["History "] = intHisMod,
                ["Insight"] = wisInsMod,
                ["Intimidation"] = chaIntMod,
                ["Investigation "] = intInvMod,
                ["Medicine"] = wisMedMod,
                ["Nature"] = intNatMod,
                ["Perception "] = wisPerMod,
                ["Performance"] = chaPerfMod,
                ["Persuasion"] = chaPersMod,
                ["Religion"] = intRelMod,
                ["SleightofHand"] = dexSleMod,
                ["Stealth "] = dexSteMod,
                ["Survival"] = wisSurMod,
                ["Passive"] = passPerc,
                ["ProficienciesLang"] = otherProf,
                ["AC"] = acSco,
                ["Initiative"] = initSco,
                ["Speed"] = speedSco,
                ["HPMax"] = maxHP,
                ["HPCurrent"] = currHP,
                ["HPTemp"] = tempHP,
                ["HDTotal"] = dieLevel,
                ["HD"] = hitDie,
                ["Wpn Name"] = atk1Nam,
                ["Wpn1 AtkBonus"] = atk1Bon,
                ["Wpn1 Damage"] = atk1Dam,
                ["Wpn Name 2"] = atk2Nam,
                ["Wpn2 AtkBonus "] = atk2Bon,
                ["Wpn2 Damage "] = atk2Dam,
                ["Wpn Name 3"] = atk3Nam,
                ["Wpn3 AtkBonus  "] = atk3Bon,
                ["Wpn3 Damage "] = atk3Dam,
                ["Equipment"] = allEquipment,
                ["PersonalityTraits "] = persTrait,
                ["Ideals"] = idealTrait,
                ["Bonds"] = bondTrait,
                ["Flaws"] = flawTrait,
                ["Features and Traits"] = features
            };

            var result = await pdftk.FillFormAsync(pdfFile, fieldData, false, false);

            if (!result.Success)
                throw new Exception($"Oops: {result.StandardError}");

            return File(result.Result, "application/pdf", $"{Guid.NewGuid().ToString()}.pdf");
        }

        public string ReplaceIllegal(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z]+", "");
        }

        private void AddProficiency(string prof)
        {

            switch (prof)
            {
                case "skill-acrobatics":
                    profChecks.ElementAt(0).Check = true;
                    break;
                case "skill-animal-handling":
                    profChecks.ElementAt(1).Check = true;
                    break;
                case "skill-arcana":
                    profChecks.ElementAt(2).Check = true;
                    break;
                case "skill-athletics":
                    profChecks.ElementAt(3).Check = true;
                    break;
                case "skill-deception":
                    profChecks.ElementAt(4).Check = true;
                    break;
                case "skill-history":
                    profChecks.ElementAt(5).Check = true;
                    break;
                case "skill-insight":
                    profChecks.ElementAt(6).Check = true;
                    break;
                case "skill-intimidation":
                    profChecks.ElementAt(7).Check = true;
                    break;
                case "skill-investigation":
                    profChecks.ElementAt(8).Check = true;
                    break;
                case "skill-medicine":
                    profChecks.ElementAt(9).Check = true;
                    break;
                case "skill-nature":
                    profChecks.ElementAt(10).Check = true;
                    break;
                case "skill-perception":
                    profChecks.ElementAt(11).Check = true;
                    break;
                case "skill-performance":
                    profChecks.ElementAt(12).Check = true;
                    break;
                case "skill-persuasion":
                    profChecks.ElementAt(13).Check = true;
                    break;
                case "skill-religion":
                    profChecks.ElementAt(14).Check = true;
                    break;
                case "skill-sleight-of-hand":
                    profChecks.ElementAt(15).Check = true;
                    break;
                case "skill-stealth":
                    profChecks.ElementAt(16).Check = true;
                    break;
                case "skill-survival":
                    profChecks.ElementAt(17).Check = true;
                    break;
            }
        }

        //fills sheet with info from the character's D&D race
        private void GetRaceInfo()
        {
            dynamic race = load.LoadJObj("races/" + coreAttr["displayRace"].ToLower());
            displaySpeed = (int)race["speed"];
            foreach (dynamic bonus in race["ability_bonuses"])
                switch (bonus["ability_score"]["index"].ToString())
                {
                    case "str":
                        displayStr += (int)bonus["bonus"];
                        break;
                    case "dex":
                        displayStr += (int)bonus["bonus"];
                        break;
                    case "con":
                        displayStr += (int)bonus["bonus"];
                        break;
                    case "int":
                        displayStr += (int)bonus["bonus"];
                        break;
                    case "wis":
                        displayStr += (int)bonus["bonus"];
                        break;
                    case "cha":
                        displayStr += (int)bonus["bonus"];
                        break;
                    default:
                        Console.WriteLine("Something went wrong!");
                        break;
                }
            foreach (dynamic proficiency in race["starting_proficiencies"])
                AddProficiency(proficiency["index"].ToString());
            foreach (dynamic language in race["languages"])
                otherProf.Add(language["name"].ToString());
            foreach (dynamic trait in race["traits"])
                featuresTraits.Add( new string[] { trait["name"].ToString(), ArrayToDesc(load.LoadJObj(trait["url"].ToString().Substring(5))["desc"]) });
        }

        //Generates personality traits based on background and alignment
        private void GetPersonality()
        {
            dynamic backGround = bgObject;
            List<string> pTraits = new List<string>();
            foreach(dynamic persTrait in backGround["personality_traits"]["from"])
                pTraits.Add(persTrait.ToString());
            for (int i = 0; i < (int)backGround["personality_traits"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                persTrait += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
            pTraits.Clear();
            foreach(dynamic idealTrait in backGround["ideals"]["from"])
            {
                foreach(dynamic idealAlign in idealTrait["alignments"])
                {
                    if (idealAlign["name"].ToString() == coreAttr["displayAlignment"])
                        pTraits.Add(idealTrait["desc"].ToString());
                }
            }
            for (int i = 0; i < (int)backGround["ideals"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                persIdeal += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
            pTraits.Clear();
            foreach (dynamic bondTrait in backGround["bonds"]["from"])
                pTraits.Add(bondTrait.ToString());
            for (int i = 0; i < (int)backGround["bonds"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                persBonds += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
            pTraits.Clear();
            foreach (dynamic flawTrait in backGround["flaws"]["from"])
                pTraits.Add(flawTrait.ToString());
            for (int i = 0; i < (int)backGround["flaws"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                persFlaws += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
        }

        //calculates weapon names, attack bonuses, and damage/type, as well as AC
        private void GetCombat()
        {
            foreach (DNDItem item in itemSelect)
            {
                JObject middleMan = load.LoadJObj("equipment/" + item.Index);
                if ("weapon" == middleMan["equipment_category"]["index"].ToString())
                {
                    int atkBonus = 0;
                    string atkDam = "";
                    string damType = "";
                    if (middleMan["damage"]["damage_type"]["name"] != null)
                        damType = middleMan["damage"]["damage_type"]["name"].ToString();
                    else
                        break;
                    bool isFinesse = false, isMonk = false;
                    foreach (string proficiency in otherProf)
                    {
                        if (proficiency == middleMan["weapon_category"].ToString() + " Weapons" || proficiency.ToLower() == middleMan["index"].ToString() + "s")
                        {
                            atkBonus += displayProf;
                        }
                    }
                    foreach (dynamic property in middleMan["properties"])
                    {
                        if (property["index"] == "finesse")
                            isFinesse = true;
                        if (property["index"] == "monk" && coreAttr["displayClass"] == "Monk")
                            isMonk = true;
                    }
                    if (isMonk)
                    {
                        int martArtDie = int.Parse(load.LoadJObj("classes/" + coreAttr["displayClass"].ToLower() + " / levels/" + displayLevel)["class_specific"]["martial_arts"]["dice_value"].ToString());
                        if (int.Parse(middleMan["damage"]["damage_dice"].ToString().Split('d')[1]) < martArtDie)
                            atkDam = "1d" + martArtDie;
                        else
                            atkDam = middleMan["damage"]["damage_dice"].ToString();
                    }
                    else
                        atkDam = middleMan["damage"]["damage_dice"].ToString();
                    //if weapon can use dexterity and dex ability modifier is greater than strength
                    if ((isFinesse || coreAttr["displayClass"] == "Monk") && (ablMods.ElementAt(1) > ablMods.ElementAt(0)))
                    {
                        atkBonus += ablMods.ElementAt(1); //add dex to attack bonus
                        atkDam += " + " + ablMods.ElementAt(1); //add dex to damage
                    }
                    else
                    {
                        atkBonus += ablMods.ElementAt(0); //add str to attack bonus
                        atkDam += " + " + ablMods.ElementAt(0); //add str to damage
                    }
                    string[] atk = { item.Name, atkBonus.ToString(), atkDam + " " + damType};
                    attackList.Add(atk);
                }

                if ("armor" == middleMan["equipment_category"]["index"].ToString() && "shield" != middleMan["index"].ToString())
                {
                    displayAC = (int)middleMan["armor_class"]["base"];
                    if ((bool)middleMan["armor_class"]["dex_bonus"])
                    {
                        int dexMax = 99;
                        if (middleMan["armor_class"]["max_bonus"] != null)
                            dexMax = (int)middleMan["armor_class"]["max_bonus"];
                        if (dexMax < ablMods.ElementAt(1)) //if armor class has an maximum dex bonus that is less than dex mod
                            displayAC += (int)middleMan["armor_class"]["max_bonus"];
                        else
                            displayAC += ablMods.ElementAt(1); //add dex to AC
                    }
                    if (displayStr < (int)middleMan["str_minimum"])
                    {
                        displaySpeed -= 10;
                    }
                }
                if (load.LoadJObj("classes/" + coreAttr["displayClass"].ToLower() + "/levels/1/features/")["results"][0]["index"].ToString() == "barbarian-unarmored-defense")
                    displayAC = 10 + ablMods.ElementAt(1) + ablMods.ElementAt(2); // Armor class = Dex + Con
                if (load.LoadJObj("classes/" + coreAttr["displayClass"].ToLower() + "/levels/1/features/")["results"][1]["index"].ToString() == "monk-unarmored-defense")
                    displayAC = 10 + ablMods.ElementAt(1) + ablMods.ElementAt(3); // Armor class = Dex + Wis
                if (displayAC == -1)
                {
                   displayAC = 10 + ablMods.ElementAt(1);
                }
            }
        }

        //generates character equipment from starting-equipment from class
        private void GetEquipment()
        {
            dynamic charaEqu = load.LoadJObj("classes/" + coreAttr["displayClass"].ToLower());
            foreach (dynamic item in bgObject["starting_equipment"])
                itemSelect.Add(new DNDItem(item["equipment"]["index"].ToString(), item["equipment"]["name"].ToString(), (int)item["quantity"]));
            foreach (dynamic item in charaEqu["starting_equipment"])
                itemSelect.Add(new DNDItem(item["equipment"]["index"].ToString(), item["equipment"]["name"].ToString(), (int)item["quantity"]));
            foreach (JObject option in charaEqu["starting_equipment_options"]) //cycles through each choice the player makes
            {
                for (int i = 0; i < (int)option["choose"]; i++)
                {
                    JArray options = (JArray)option["from"];
                    int rndChoice = quantumQueue.Dequeue() % options.Count;
                    bool setOfItems = false, itemCategory = false, optionCategory = false;
                    //checks to see if the option is multiple items
                    setOfItems = option["from"][rndChoice]["0"] != null;
                    if (!setOfItems)
                    {
                        //checks to see if item in option is an option of a category such as "simple weapon" or "martial weapon"
                        optionCategory = option["from"][rndChoice]["equipment_option"] != null;
                        if (!optionCategory)
                        {
                            //checks to see if option itself is of a category such as holy symbol
                            itemCategory = option["from"][rndChoice]["equipment_category"] != null;
                            if (!itemCategory)
                            {
                                string index = option["from"][rndChoice]["equipment"]["index"].ToString();
                                string name = option["from"][rndChoice]["equipment"]["name"].ToString();
                                int quantity = (int)option["from"][rndChoice]["quantity"];
                                itemSelect.Add(new DNDItem(index, name, quantity));
                            }
                            else
                            {
                                List<dynamic> itemType = new List<dynamic>();
                                foreach (dynamic item in load.LoadJObj("equipment-categories/" + option["from"][rndChoice]["equipment_category"]["index"].ToString())["equipment"])
                                    itemType.Add(item);
                                for (int j = 0; j < (int)option["choose"]; j++)
                                {
                                    int ranItem = quantumQueue.Dequeue() % itemType.Count;
                                    itemSelect.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
                                }
                            }
                        }
                        else
                        {
                            List<dynamic> itemType = new List<dynamic>();
                            foreach (dynamic item in load.LoadJObj("equipment-categories/" + option["from"][rndChoice]["equipment_option"]["from"]["equipment_category"]["index"].ToString())["equipment"])
                                itemType.Add(item);
                            for (int j = 0; j < (int)option["from"][rndChoice]["equipment_option"]["choose"]; j++)
                            {
                                int ranItem = quantumQueue.Dequeue() % itemType.Count;
                                itemSelect.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
                            }
                        }

                    }
                    else
                    {
                        int j = 0;

                        while (option["from"][rndChoice][j.ToString()] != null)
                        {
                            //checks to see if item in option is an option of a category such as "simple weapon" or "martial weapon"
                            optionCategory = option["from"][rndChoice][j.ToString()]["equipment_option"] != null;
                            if (!optionCategory)
                            {
                                //checks to see if option itself is of a category such as holy symbol
                                itemCategory = option["from"][rndChoice]["equipment_category"] != null;
                                if (!itemCategory)
                                {
                                    string index = option["from"][rndChoice][j.ToString()]["equipment"]["index"].ToString();
                                    string name = option["from"][rndChoice][j.ToString()]["equipment"]["name"].ToString();
                                    int quantity = (int)option["from"][rndChoice][j.ToString()]["quantity"];
                                    itemSelect.Add(new DNDItem(index, name, quantity));
                                }
                                else
                                {
                                    List<dynamic> itemType = new List<dynamic>();
                                    foreach (dynamic item in load.LoadJObj("equipment-categories/" + option["from"][rndChoice][j.ToString()]["equipment_option"]["from"]["equipment_category"]["index"].ToString())["equipment"])
                                        itemType.Add(item);
                                    for (int k = 0; k < (int)option["from"][rndChoice][j.ToString()]["equipment_option"]["choose"]; k++)
                                    {
                                        int ranItem = quantumQueue.Dequeue() % itemType.Count;
                                        itemSelect.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
                                    }
                                }
                            }
                            else
                            {
                                List<dynamic> itemType = new List<dynamic>();
                                foreach (dynamic item in load.LoadJObj("equipment-categories/" + option["from"][rndChoice][j.ToString()]["equipment_option"]["from"]["equipment_category"]["index"].ToString())["equipment"])
                                    itemType.Add(item);

                                for (int k = 0; k < (int)option["from"][rndChoice][j.ToString()]["equipment_option"]["choose"]; k++)
                                {
                                    try
                                    {
                                        int ranItem = quantumQueue.Dequeue() % itemType.Count;
                                        itemSelect.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
                                        k++;
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("There was an error. It was: " + e);
                                    }
                                }
                            }
                            j++;
                        }
                    }
                }
            }
        }

        //prints items into equipment box
        public string AllItems()
        {
            string result = "";
            foreach (DNDItem item in itemSelect)
            {
                result += item.Name;
                result += " x" + item.Quantity + "\n";
            }
            return result;
        }

        //levels up the character
        private void GetLevelInfo()
        {
            int[] lvlToExp = { 0, 300, 900, 2700, 6500, 14000, 23000, 34000, 48000, 64000, 85000, 100000, 120000, 140000, 165000, 195000, 225000, 265000, 305000, 355000 };
            displayExp = lvlToExp[displayLevel - 1];
            displayProf = (int)load.LoadJObj("classes/" + coreAttr["displayClass"].ToLower() + "/levels/" + displayLevel.ToString())["prof_bonus"];
            featuresTraits.Add(new string[] { bgObject["feature"]["name"].ToString(), ArrayToDesc(bgObject["feature"]["desc"]) });
            for (int i = 1; i <= displayLevel; i++) {
                if (i == 1)
                {
                    displayHP = displayHitDice + ablMods.ElementAt(2);
                    Console.WriteLine("Base health: " + displayHP);
                }
                else
                {
                    int newHP = (quantumQueue.Dequeue() % displayHitDice) + ablMods.ElementAt(2);
                    if (newHP > 1)
                        displayHP += newHP;
                    else
                        displayHP += 1;
                }
                foreach (dynamic feature in load.LoadJObj("classes/" + coreAttr["displayClass"].ToLower() + "/levels/" + i.ToString())["features"])
                {
                    if (feature["name"] == "Ability Score Improvement")
                    {
                        AblScoImp(2);
                        GetAblMod();
                    }
                    else
                        featuresTraits.Add(new string[] { feature["name"].ToString(), ArrayToDesc(load.LoadJObj(feature["url"].ToString().Substring(5))["desc"]) });
                }
                List<int> subClassLevels = new List<int>();
                foreach (dynamic subClassFeature in scfObject)
                    subClassLevels.Add((int)subClassFeature["level"]);
                if (subClassLevels.Contains(i))
                    foreach (JObject feature in load.LoadJObj("subclasses/" + coreAttr["displaySubClass"].ToLower() + "/levels/" + i.ToString())["features"])
                        featuresTraits.Add(new string[] { feature["name"].ToString(), ArrayToDesc(load.LoadJObj(feature["url"].ToString().Substring(5))["desc"]) });
            }
        }

        private string ArrayToDesc(dynamic ja)
        {
            string result = "";
            foreach (dynamic line in ja)
            {
                result += line + "\n";
            }
            return result;
        }

        private void AblScoImp(int v)
        {
            for (int i = 0; i < v; i++)
            {
                bool changed = false;
                //highest stats assigned to multiclass prerequisites as best indicator of important abilities
                if (mcObject["prerequisites"] != null)
                    foreach (JObject preReq in mcObject["prerequisites"])
                        changed = AddScore(preReq["ability_score"]["index"].ToString());
                else
                    foreach (dynamic preReq in mcObject["prerequisite_options"]["from"])
                        changed = AddScore(preReq["ability_score"]["index"].ToString());
                if (!changed)
                {
                    //next highest stat assigned to spellcasting ability modifier (if any)
                    if (scObject != null)
                        changed = AddScore(scObject["spellcasting_ability"]["index"].ToString());
                }
                //next assigned to saving throws
                if (!changed)
                {
                    foreach (dynamic savThr in classObject["saving_throws"])
                        changed = AddScore(savThr["index"].ToString());
                }

                //rest assigned randomly, as they are usually subject to player preference
                if (!changed)
                {
                    string[] stats = { "str", "dex", "con", "int", "wis", "con" };
                    changed = AddScore(stats[quantumQueue.Dequeue() % 6]);
                }
            }
        }

        private bool AddScore(string stat)
        {

            switch (stat)
            {
                case "str":
                    if (displayStr < 20)
                    {
                        displayStr += 1;
                        return true;
                    }
                    break;
                case "dex":
                    if (displayDex < 20)
                    {
                        displayDex += 1;
                        return true;
                    }
                    break;
                case "con":
                    if (displayCon < 20)
                    {
                        displayCon += 1;
                        return true;
                    }
                    break;
                case "int":
                    if (displayInt < 20)
                    {
                        displayInt += 1;
                        return true;
                    }
                    break;
                case "wis":
                    if (displayWis < 20)
                    {
                        displayWis += 1;
                        return true;
                    }
                    break;
                case "cha":
                    if (displayCha < 20)
                    {
                        displayCha += 1;
                        return true;
                    }
                    break;
            }
            return false;
        }

        //checks API for a class's saving throws and proficiencies, making the ones that are, "checked" for input box
        private void GetProf()
        {
            dynamic savThr = classObject["saving_throws"];
            saveChecks.ElementAt(0).Check = ("str" == savThr[0]["index"].ToString() || "str" == savThr[1]["index"].ToString());
            saveChecks.ElementAt(1).Check = ("dex" == savThr[0]["index"].ToString() || "dex" == savThr[1]["index"].ToString());
            saveChecks.ElementAt(2).Check = ("con" == savThr[0]["index"].ToString() || "con" == savThr[1]["index"].ToString());
            saveChecks.ElementAt(3).Check = ("int" == savThr[0]["index"].ToString() || "int" == savThr[1]["index"].ToString());
            saveChecks.ElementAt(4).Check = ("wis" == savThr[0]["index"].ToString() || "wis" == savThr[1]["index"].ToString());
            saveChecks.ElementAt(5).Check = ("cha" == savThr[0]["index"].ToString() || "cha" == savThr[1]["index"].ToString());

            var profSele = new List<string>();
            //check if object begins with "skill-" string as the API sorts proficiencies inconsistently
            dynamic correctProf = null;
            foreach (dynamic profPoss in classObject["proficiency_choices"])
            {
                if (profPoss["from"][0]["index"].ToString().Substring(0, 6) == "skill-")
                    correctProf = profPoss;
            }

            //adds each proficiency choice from class to list
            foreach (var profChoice in correctProf["from"])
                profSele.Add(profChoice["index"].ToString());

            //randomly assigns skill proficiencies as they are usually user prefereence
            for (int i = 0; i < (int)correctProf["choose"]; i++)
            {
                string chosenProf = profSele.ElementAt(quantumQueue.Dequeue() % profSele.Count);
                AddProficiency(chosenProf);
            }

            foreach (var prof in bgObject["starting_proficiencies"])
                AddProficiency(prof["index"].ToString());

            profSele = new List<string>();
            //check if object begins with "skill-" string as the API sorts proficiencies inconsistently
            correctProf = null;
            foreach (dynamic profPoss in classObject["proficiency_choices"])
            {
                if (profPoss["from"][0]["index"].ToString().Substring(0, 6) != "skill-")
                {
                    correctProf = profPoss;
                    foreach (var profChoice in correctProf["from"])
                        profSele.Add(profChoice["name"].ToString());
                    //randomly assigns tool and instrument proficiencies as they are usually user prefereence
                    for (int i = 0; i < (int)correctProf["choose"]; i++)
                    {
                        otherProf.Add(profSele.ElementAt(quantumQueue.Dequeue() % profSele.Count));
                    }
                }
            }

            //adds weapon proficiency as well
            foreach (dynamic weapProf in classObject["proficiencies"])
            {
                otherProf.Add(weapProf["name"].ToString());
            }
        }

        public string GetOtherProf()
        {
            string result = "";
            foreach (string prof in otherProf)
                result += prof + ", ";
            return result.Substring(0, result.Length-2);
        }

        //gets the bonus of a skill the character has proficiency in
        public int ProfBonus(int abl, bool pro)
        {
            if (pro)
                return abl + displayProf;
            return abl;
        }
        public int ProfBonus(int abl, bool pro, int init)
        {
            if (pro)
                return abl + displayProf + init;
            return abl + init;
        }

        //calculates the ability modifiers based on D&D's equation
        public void GetAblMod()
        {
            int[,] abls = { { displayStr, 0 }, { displayDex, 0 }, { displayCon, 0 }, { displayInt, 0 }, { displayWis, 0 }, { displayCha, 0 } };

            for(int i = 0; i < 6; i++)
            {
                if (abls[i,0] > 10)
                     abls[i,1] = (abls[i, 0] - 10) / 2;
                else
                    abls[i,1] = (abls[i, 0] - 11) / 2;
            }
            ablMods.Add(abls[0, 1]);
            ablMods.Add(abls[1, 1]);
            ablMods.Add(abls[2, 1]);
            ablMods.Add(abls[3, 1]);
            ablMods.Add(abls[4, 1]);
            ablMods.Add(abls[5, 1]);

        }
    }
}
