using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NoahNPCGen
{
    public class Personality
    {
        static LoadAPI load = new LoadAPI();
        Queue<int> quantumQueue = load.LoadQuan();

        //Generates personality traits based on background and alignment
        public void GetPersonality(JObject bgObject, Dictionary<string, string> coreAttr, ref string pTrait, ref string pIdeal, ref string pBonds, ref string pFlaws)
        {
            dynamic backGround = bgObject;
            List<string> pTraits = new List<string>();
            foreach (dynamic persTrait in backGround["personality_traits"]["from"])
                pTraits.Add(persTrait.ToString());
            for (int i = 0; i < (int)backGround["personality_traits"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                pTrait += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
            pTraits.Clear();
            foreach (dynamic idealTrait in backGround["ideals"]["from"])
            {
                foreach (dynamic idealAlign in idealTrait["alignments"])
                {
                    if (idealAlign["name"].ToString() == coreAttr["displayAlignment"])
                        pTraits.Add(idealTrait["desc"].ToString());
                }
            }
            for (int i = 0; i < (int)backGround["ideals"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                pIdeal += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
            pTraits.Clear();
            foreach (dynamic bondTrait in backGround["bonds"]["from"])
                pTraits.Add(bondTrait.ToString());
            for (int i = 0; i < (int)backGround["bonds"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                pBonds += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
            pTraits.Clear();
            foreach (dynamic flawTrait in backGround["flaws"]["from"])
                pTraits.Add(flawTrait.ToString());
            for (int i = 0; i < (int)backGround["flaws"]["choose"]; i++)
            {
                int ranNum = quantumQueue.Dequeue() % pTraits.Count;
                pFlaws += pTraits.ElementAt(ranNum) + "\n";
                pTraits.RemoveAt(ranNum);
            }
        }
    }
}
