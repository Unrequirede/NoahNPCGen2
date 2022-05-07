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
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.AcroForms;

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
            string intSav, string wisAbl, string wisMod, string wisSav, string chaAbl, string chaMod, string chaSav, string profBonus, string strSavThr, string dexSavThr,
            string conSavThr, string intSavThr, string wisSavThr, string chaSavThr, string dexAcrMod, string wisAniMod, string intArcMod, string strAthMod, string chaDecMod,
            string intHisMod, string wisInsMod, string chaIntMod, string intInvMod, string wisMedMod, string intNatMod, string wisPerMod, string chaPerfMod, string chaPersMod,
            string intRelMod, string dexSleMod, string dexSteMod, string wisSurMod, string passPerc, string otherProf, string acSco, string initSco, string speedSco, string maxHP,
            string currHP, string tempHP, string dieLevel, string hitDie, string dsSucc1, string dsSucc2, string dsSucc3, string dsFail1, string dsFail2, string dsFail3, string atk1Nam,
            string atk1Bon, string atk1Dam, string atk2Nam, string atk2Bon, string atk2Dam, string atk3Nam, string atk3Bon, string atk3Dam, string allEquipment, string persTrait,
            string idealTrait, string bondTrait, string flawTrait, string features, string dexAcr, string wisAni, string intArc, string strAth, string chaDec, string intHis,
            string wisIns, string chaInt, string intInv, string wisMed, string intNat, string wisPer, string chaPerf, string chaPers, string intRel, string dexSle, string dexSte,
            string wisSur)
        {

            AddPDF add = new AddPDF();
            // Open the file
            PdfDocument document = PdfReader.Open("CharacterSheet-FormFillable.pdf", PdfDocumentOpenMode.Modify);

            add.AddElem(ref document, "CharacterName", nameBox);
            add.AddElem(ref document, "PlayerName", playerBox);
            add.AddElem(ref document, "ClassLevel", classBox);
            add.AddElem(ref document, "Race ", raceBox);
            add.AddElem(ref document, "ClassLevel", classBox);
            add.AddElem(ref document, "Alignment", alignmentBox);
            add.AddElem(ref document, "XP", experienceBox);
            add.AddElem(ref document, "Background", backGBox);
            add.AddElem(ref document, "STR", strAbl);
            add.AddElem(ref document, "STRmod", strMod);
            add.AddElem(ref document, "DEX", dexAbl);
            add.AddElem(ref document, "DEXmod ", dexMod);
            add.AddElem(ref document, "CON", conAbl);
            add.AddElem(ref document, "CONmod", conMod);
            add.AddElem(ref document, "INT", intAbl);
            add.AddElem(ref document, "INTmod", intMod);
            add.AddElem(ref document, "WIS", wisAbl);
            add.AddElem(ref document, "WISmod", wisMod);
            add.AddElem(ref document, "CHA", chaAbl);
            add.AddElem(ref document, "CHamod", chaMod);
            add.AddElem(ref document, "ProfBonus", profBonus);
            add.AddElem(ref document, "ST Strength", strSavThr);
            add.AddElem(ref document, "ST Dexterity", dexSavThr);
            add.AddElem(ref document, "ST Constitution", conSavThr);
            add.AddElem(ref document, "ST Intelligence", intSavThr);
            add.AddElem(ref document, "ST Wisdom", wisSavThr);
            add.AddElem(ref document, "ST Charisma", chaSavThr);
            add.AddElem(ref document, "Check Box 11", strSav == "on"); //str saving throw check
            add.AddElem(ref document, "Check Box 12", dsSucc1 == "on"); //death save success 1
            add.AddElem(ref document, "Check Box 13", dsSucc2 == "on"); //death save success 2
            add.AddElem(ref document, "Check Box 14", dsSucc3 == "on"); //death save success 3
            add.AddElem(ref document, "Check Box 15", dsFail1 == "on"); //death save fail 1
            add.AddElem(ref document, "Check Box 16", dsFail2 == "on"); //death save fail 2
            add.AddElem(ref document, "Check Box 17", dsFail3 == "on"); //death save fail 3
            add.AddElem(ref document, "Check Box 18", dexSav == "on"); //dex saving throw check
            add.AddElem(ref document, "Check Box 19", conSav == "on"); //con saving throw check
            add.AddElem(ref document, "Check Box 20", intSav == "on"); //int saving throw check
            add.AddElem(ref document, "Check Box 21", wisSav == "on"); //wis saving throw check
            add.AddElem(ref document, "Check Box 22", chaSav == "on"); //cha saving throw check
            add.AddElem(ref document, "Check Box 23", dexAcr == "on"); //acrobatics check
            add.AddElem(ref document, "Check Box 24", wisAni == "on"); //animal handling check
            add.AddElem(ref document, "Check Box 25", intArc == "on"); //arcana check
            add.AddElem(ref document, "Check Box 26", strAth == "on"); //athletics check
            add.AddElem(ref document, "Check Box 27", chaDec == "on"); //deception check
            add.AddElem(ref document, "Check Box 28", intHis == "on"); //history check
            add.AddElem(ref document, "Check Box 29", wisIns == "on"); //insight check
            add.AddElem(ref document, "Check Box 30", chaInt == "on"); //intimidation check
            add.AddElem(ref document, "Check Box 31", intInv == "on"); //investigation check
            add.AddElem(ref document, "Check Box 32", wisMed == "on"); //medicine check
            add.AddElem(ref document, "Check Box 33", intNat == "on"); //nature check
            add.AddElem(ref document, "Check Box 34", wisPer == "on"); //perception check
            add.AddElem(ref document, "Check Box 35", chaPerf == "on"); //performance check
            add.AddElem(ref document, "Check Box 36", chaPers == "on"); //persuasion check
            add.AddElem(ref document, "Check Box 37", intRel == "on"); //religion check
            add.AddElem(ref document, "Check Box 38", dexSle == "on"); //sleight of hand check
            add.AddElem(ref document, "Check Box 39", dexSte == "on"); //stealth check
            add.AddElem(ref document, "Check Box 40", wisSur == "on"); //survival check
            add.AddElem(ref document, "Acrobatics", dexAcrMod);
            add.AddElem(ref document, "Animal", wisAniMod);
            add.AddElem(ref document, "Arcana", intArcMod);
            add.AddElem(ref document, "Athletics", strAthMod);
            add.AddElem(ref document, "Deception ", chaDecMod);
            add.AddElem(ref document, "History ", intHisMod);
            add.AddElem(ref document, "Insight", wisInsMod);
            add.AddElem(ref document, "Intimidation", chaIntMod);
            add.AddElem(ref document, "Investigation ", intInvMod);
            add.AddElem(ref document, "Medicine", wisMedMod);
            add.AddElem(ref document, "Nature", intNatMod);
            add.AddElem(ref document, "Perception ", wisPerMod);
            add.AddElem(ref document, "Performance", chaPerfMod);
            add.AddElem(ref document, "Persuasion", chaPersMod);
            add.AddElem(ref document, "Religion", intRelMod);
            add.AddElem(ref document, "SleightofHand", dexSleMod);
            add.AddElem(ref document, "Stealth ", dexSteMod);
            add.AddElem(ref document, "Survival", wisSurMod);
            add.AddElem(ref document, "Passive", passPerc);
            add.AddElem(ref document, "ProficienciesLang", otherProf);
            add.AddElem(ref document, "AC", acSco);
            add.AddElem(ref document, "Initiative", initSco);
            add.AddElem(ref document, "Speed", speedSco);
            add.AddElem(ref document, "HPMax", maxHP);
            add.AddElem(ref document, "HPCurrent", currHP);
            add.AddElem(ref document, "HPTemp", tempHP);
            add.AddElem(ref document, "HDTotal", dieLevel);
            add.AddElem(ref document, "HD", hitDie);
            add.AddElem(ref document, "Wpn Name", atk1Nam);
            add.AddElem(ref document, "Wpn1 AtkBonus", atk1Bon);
            add.AddElem(ref document, "Wpn1 Damage", atk1Dam);
            add.AddElem(ref document, "Wpn Name 2", atk2Nam);
            add.AddElem(ref document, "Wpn2 AtkBonus ", atk2Bon);
            add.AddElem(ref document, "Wpn2 Damage ", atk2Dam);
            add.AddElem(ref document, "Wpn Name 3", atk3Nam);
            add.AddElem(ref document, "Wpn3 AtkBonus  ", atk3Bon);
            add.AddElem(ref document, "Wpn3 Damage ", atk3Dam);
            add.AddElem(ref document, "Equipment", allEquipment);
            add.AddElem(ref document, "PersonalityTraits ", persTrait);
            add.AddElem(ref document, "Ideals", idealTrait);
            add.AddElem(ref document, "Bonds", bondTrait);
            add.AddElem(ref document, "Flaws", flawTrait);
            add.AddElem(ref document, "Features and Traits", features);

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, true);

                if (nameBox != "" && nameBox != null)
                    return File(stream.ToArray(), "application/pdf", $"{nameBox}.pdf");
                else
                    return File(stream.ToArray(), "application/pdf", $"Unnamed Character.pdf");
            }
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
