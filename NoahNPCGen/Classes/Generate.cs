using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NoahNPCGen
{
    public class Generate
    {
        static LoadAPI load = new LoadAPI();
        Queue<int> quantumQueue = load.LoadQuan();

        public void RandomAttr(ref Dictionary<string, string> coreA, ref int disL, ref JObject classObj, ref JObject mcObj, ref JObject scObj, ref JArray scfObj, ref JObject bgObj)
        {
            if (coreA["displayRace"] == "Random")
            {
                List<string> raceList = new List<string>();
                foreach (dynamic raceName in load.LoadJObj("races")["results"])
                    raceList.Add(raceName["name"].ToString());
                coreA["displayRace"] = raceList.ElementAt(quantumQueue.Dequeue() % raceList.Count);
            }
            if (coreA["displayClass"] == "Random")
            {
                List<string> classList = new List<string>();
                foreach (dynamic className in load.LoadJObj("classes")["results"])
                    classList.Add(className["name"].ToString());
                coreA["displayClass"] = classList.ElementAt(quantumQueue.Dequeue() % classList.Count);
            }
            classObj = load.LoadJObj("classes/" + coreA["displayClass"].ToLower());
            mcObj = load.LoadJObj("classes/" + coreA["displayClass"].ToLower() + "/multi-classing/");
            if (load.LoadJObj("classes/" + coreA["displayClass"].ToLower())["spellcasting"] != null)
                scObj = load.LoadJObj("classes/" + coreA["displayClass"].ToLower() + "/spellcasting/");
            scfObj = load.LoadJArr("subclasses/" + coreA["displayClass"].ToLower() + "/levels/");
            if (coreA["displayClass"] == "Random")
            {
                List<string> subClassList = new List<string>();
                foreach (dynamic subClassName in classObj["subclasses"])
                    subClassList.Add(subClassName["name"].ToString());
                coreA["displayClass"] = subClassList.ElementAt(quantumQueue.Dequeue() % subClassList.Count);
            }
            if (coreA["displayBackG"] == "Random")
            {
                List<string> backGList = new List<string>();
                foreach (dynamic backGName in load.LoadJObj("backgrounds")["results"])
                    backGList.Add(backGName["name"].ToString());
                coreA["displayBackG"] = backGList.ElementAt(quantumQueue.Dequeue() % backGList.Count);
            }
            bgObj = load.LoadJObj("backgrounds/" + coreA["displayBackG"].ToLower());
            if (coreA["displayAlignment"] == "Random")
            {
                List<string> alignmentList = new List<string>();
                foreach (dynamic alignmentName in load.LoadJObj("alignments")["results"])
                    alignmentList.Add(alignmentName["name"].ToString());
                coreA["displayAlignment"] = alignmentList.ElementAt(quantumQueue.Dequeue() % alignmentList.Count);
            }
            if (disL == 0)
                disL = quantumQueue.Dequeue() % 20 + 1;
        }

        //rolls six stats, one for each ability score, and assigns them based off class characteristics
        public void GetStats(ref Dictionary<string, string> coreA, JObject mcObject, JObject classObject, ref int streng, ref int dexter, ref int consti, ref int intell, ref int wisdom, ref int charis)
        {
            int[] rolledStats = new int[6];
            for (int i = 0; i < 6; i++)
            {
                int[] statRoll = { quantumQueue.Dequeue() % 6, quantumQueue.Dequeue() % 6, quantumQueue.Dequeue() % 6, quantumQueue.Dequeue() % 6 };
                rolledStats[i] = statRoll.Sum() - statRoll.Min();
            }

            //highest stats assigned to multiclass prerequisites as best indicator of important abilities
            if (mcObject["prerequisites"] != null)
                foreach (JObject ability in mcObject["prerequisites"])
                    FillStat(ability["ability_score"]["index"].ToString(), rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
            if (mcObject["prerequisite_options"] != null)
                foreach (JObject ability in mcObject["prerequisite_options"]["from"])
                    FillStat(ability["ability_score"]["index"].ToString(), rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);

            //next highest stat assigned to ability modifier (if any)
            if (load.LoadJObj("classes/" + coreA["displayClass"].ToLower())["spellcasting"] != null)
                FillStat(load.LoadJObj("classes/" + coreA["displayClass"].ToLower() + "/spellcasting/")["spellcasting_ability"]["index"].ToString(), rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);

            //next assigned to saving throws
            if (load.LoadJObj("classes/" + coreA["displayClass"].ToLower())["saving_throws"][0] != null)
                FillStat(classObject["saving_throws"][0]["index"].ToString(), rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
            if (load.LoadJObj("classes/" + coreA["displayClass"].ToLower())["saving_throws"][1] != null)
                FillStat(classObject["saving_throws"][1]["index"].ToString(), rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);

            //rest assigned randomly, as they are usually subject to player preference
            FillStat("str", rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
            FillStat("dex", rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
            FillStat("con", rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
            FillStat("int", rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
            FillStat("wis", rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
            FillStat("cha", rolledStats, ref streng, ref dexter, ref consti, ref intell, ref wisdom, ref charis);
        }

        //assigns highest stat in an array to the stat named in the string
        private void FillStat(string stat, int[] statArr, ref int displayStr, ref int displayDex, ref int displayCon, ref int displayInt, ref int displayWis, ref int displayCha)
        {
            if ("str" == stat && displayStr == -1)
            {
                displayStr = statArr.Max();
                statArr[statArr.ToList().IndexOf(displayStr)] = 0;
            }
            if ("dex" == stat && displayDex == -1)
            {
                displayDex = statArr.Max();
                statArr[statArr.ToList().IndexOf(displayDex)] = 0;
            }
            if ("con" == stat && displayCon == -1)
            {
                displayCon = statArr.Max();
                statArr[statArr.ToList().IndexOf(displayCon)] = 0;
            }
            if ("int" == stat && displayInt == -1)
            {
                displayInt = statArr.Max();
                statArr[statArr.ToList().IndexOf(displayInt)] = 0;
            }
            if ("wis" == stat && displayWis == -1)
            {
                displayWis = statArr.Max();
                statArr[statArr.ToList().IndexOf(displayWis)] = 0;
            }
            if ("cha" == stat && displayCha == -1)
            {
                displayCha = statArr.Max();
                statArr[statArr.ToList().IndexOf(displayCha)] = 0;
            }
        }

        //fills sheet with info from the character's D&D race
        public void GetRaceInfo(Dictionary<string, string> coreAttr, ref List<Checkbox> profChecks, ref List<string> otherProf, ref List<string[]> featuresTraits, ref int displaySpeed, ref int streng, ref int dexter, ref int consti, ref int intell, ref int wisdom, ref int charis)
        {
            Proficiencies prof = new Proficiencies();
            dynamic race = load.LoadJObj("races/" + coreAttr["displayRace"].ToLower());
            displaySpeed = (int)race["speed"];
            foreach (dynamic bonus in race["ability_bonuses"])
                switch (bonus["ability_score"]["index"].ToString())
                {
                    case "str":
                        streng += (int)bonus["bonus"];
                        break;
                    case "dex":
                        dexter += (int)bonus["bonus"];
                        break;
                    case "con":
                        consti += (int)bonus["bonus"];
                        break;
                    case "int":
                        intell += (int)bonus["bonus"];
                        break;
                    case "wis":
                        wisdom += (int)bonus["bonus"];
                        break;
                    case "cha":
                        charis += (int)bonus["bonus"];
                        break;
                }
            foreach (dynamic proficiency in race["starting_proficiencies"])
                prof.AddProficiency(proficiency["index"].ToString(), profChecks);
            foreach (dynamic language in race["languages"])
                otherProf.Add(language["name"].ToString());
            foreach (dynamic trait in race["traits"])
                featuresTraits.Add(new string[] { trait["name"].ToString(), ArrayToDesc(load.LoadJObj(trait["url"].ToString().Substring(5))["desc"]) });
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
    }
}
