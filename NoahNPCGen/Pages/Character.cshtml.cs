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
        public static Generate gen = new Generate();
        public static Proficiencies prof = new Proficiencies();
        public static Items items = new Items();
        public static Personality pers = new Personality();
        public List<Checkbox> profChecks = ch.AbilityProf();
        public List<Checkbox> saveChecks = ch.SavingThro();
        public List<int> ablMods = new List<int>() { 0, 0, 0, 0, 0, 0 };
        public Queue<int> quantumQueue = load.LoadQuan();

        public void OnGetSingleOrder(string charName, string charRace, string charClass, string charSubClass, int charLevel, string charBackG, string charAlignment)
        {
            displayLevel = charLevel;
            coreAttr = new Dictionary<string, string>() { { "displayName", charName }, { "displayRace", charRace }, { "displayClass", charClass }, { "displaySubClass", charSubClass }, { "displayBackG", charBackG }, { "displayAlignment", charAlignment } };
            gen.RandomAttr(ref coreAttr, ref displayLevel, ref classObject, ref mcObject, ref scObject, ref scfObject, ref bgObject);
            displayHitDice = (int)classObject["hit_die"];
            gen.GetStats(ref coreAttr, mcObject, classObject, ref displayStr, ref displayDex, ref displayCon, ref displayInt, ref displayWis, ref displayCha);
            prof.GetProf(classObject, bgObject, ref saveChecks, ref profChecks, ref otherProf);
            gen.GetRaceInfo(coreAttr, ref profChecks, ref otherProf, ref featuresTraits, ref displaySpeed, ref displayStr, ref displayDex, ref displayCon, ref displayInt, ref displayWis, ref displayCha);
            GetAblMod();
            GetLevelInfo();
            items.GetEquipment(coreAttr, bgObject, ref itemSelect);
            items.GetCombat(itemSelect, otherProf, displayProf, coreAttr, displayLevel, ablMods, ref attackList, ref displayAC, ref displaySpeed, displayStr);
            pers.GetPersonality(bgObject, coreAttr, ref persTrait, ref persIdeal, ref persBonds, ref persFlaws);
            otherPro = prof.GetOtherProf(otherProf);
            
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

            return File(result.Result, "application/pdf", $"{nameBox}.pdf");
        }

        public string ReplaceIllegal(string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z]+", "");
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
            ablMods[0] = abls[0, 1];
            ablMods[1] = abls[1, 1];
            ablMods[2] = abls[2, 1];
            ablMods[3] = abls[3, 1];
            ablMods[4] = abls[4, 1];
            ablMods[5] = abls[5, 1];

        }
    }
}
