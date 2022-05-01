using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NoahNPCGen
{
    public class Proficiencies
    {
        static LoadAPI load = new LoadAPI();
        Queue<int> quantumQueue = load.LoadQuan();


        //checks API for a class's saving throws and proficiencies, making the ones that are, "checked" for input box
        public void GetProf(JObject classObj, JObject bgObj, ref List<Checkbox> saveChks, ref List<Checkbox> profChks, ref List<string> otherProf)
        {
            dynamic savThr = classObj["saving_throws"];
            saveChks.ElementAt(0).Check = ("str" == savThr[0]["index"].ToString() || "str" == savThr[1]["index"].ToString());
            saveChks.ElementAt(1).Check = ("dex" == savThr[0]["index"].ToString() || "dex" == savThr[1]["index"].ToString());
            saveChks.ElementAt(2).Check = ("con" == savThr[0]["index"].ToString() || "con" == savThr[1]["index"].ToString());
            saveChks.ElementAt(3).Check = ("int" == savThr[0]["index"].ToString() || "int" == savThr[1]["index"].ToString());
            saveChks.ElementAt(4).Check = ("wis" == savThr[0]["index"].ToString() || "wis" == savThr[1]["index"].ToString());
            saveChks.ElementAt(5).Check = ("cha" == savThr[0]["index"].ToString() || "cha" == savThr[1]["index"].ToString());

            var profSele = new List<string>();
            //check if object begins with "skill-" string as the API sorts proficiencies inconsistently
            dynamic correctProf = null;
            foreach (dynamic profPoss in classObj["proficiency_choices"])
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
                AddProficiency(chosenProf, profChks);
            }

            foreach (var prof in bgObj["starting_proficiencies"])
                AddProficiency(prof["index"].ToString(), profChks);

            profSele = new List<string>();
            //check if object begins with "skill-" string as the API sorts proficiencies inconsistently
            correctProf = null;
            foreach (dynamic profPoss in classObj["proficiency_choices"])
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
            foreach (dynamic weapProf in classObj["proficiencies"])
            {
                otherProf.Add(weapProf["name"].ToString());
            }
        }

        public string GetOtherProf(List<string> otherProf)
        {
            string result = "";
            foreach (string prof in otherProf)
                result += prof + ", ";
            return result.Substring(0, result.Length - 2);
        }

        public void AddProficiency(string prof, List<Checkbox> profChks)
        {
            switch (prof)
            {
                case "skill-acrobatics":
                    profChks.ElementAt(0).Check = true;
                    break;
                case "skill-animal-handling":
                    profChks.ElementAt(1).Check = true;
                    break;
                case "skill-arcana":
                    profChks.ElementAt(2).Check = true;
                    break;
                case "skill-athletics":
                    profChks.ElementAt(3).Check = true;
                    break;
                case "skill-deception":
                    profChks.ElementAt(4).Check = true;
                    break;
                case "skill-history":
                    profChks.ElementAt(5).Check = true;
                    break;
                case "skill-insight":
                    profChks.ElementAt(6).Check = true;
                    break;
                case "skill-intimidation":
                    profChks.ElementAt(7).Check = true;
                    break;
                case "skill-investigation":
                    profChks.ElementAt(8).Check = true;
                    break;
                case "skill-medicine":
                    profChks.ElementAt(9).Check = true;
                    break;
                case "skill-nature":
                    profChks.ElementAt(10).Check = true;
                    break;
                case "skill-perception":
                    profChks.ElementAt(11).Check = true;
                    break;
                case "skill-performance":
                    profChks.ElementAt(12).Check = true;
                    break;
                case "skill-persuasion":
                    profChks.ElementAt(13).Check = true;
                    break;
                case "skill-religion":
                    profChks.ElementAt(14).Check = true;
                    break;
                case "skill-sleight-of-hand":
                    profChks.ElementAt(15).Check = true;
                    break;
                case "skill-stealth":
                    profChks.ElementAt(16).Check = true;
                    break;
                case "skill-survival":
                    profChks.ElementAt(17).Check = true;
                    break;
            }
        }
    }
}
