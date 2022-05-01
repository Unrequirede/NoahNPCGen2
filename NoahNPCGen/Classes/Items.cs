using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace NoahNPCGen
{
    public class Items
    {
        static LoadAPI load = new LoadAPI();
        Queue<int> quantumQueue = load.LoadQuan();

        //generates character equipment from starting-equipment from class
        public void GetEquipment(Dictionary<string, string> coreAt, JObject bgObj, ref List<DNDItem> itemSel)
        {
            dynamic charaEqu = load.LoadJObj("classes/" + coreAt["displayClass"].ToLower());
            foreach (dynamic item in bgObj["starting_equipment"])
                itemSel.Add(new DNDItem(item["equipment"]["index"].ToString(), item["equipment"]["name"].ToString(), (int)item["quantity"]));
            foreach (dynamic item in charaEqu["starting_equipment"])
                itemSel.Add(new DNDItem(item["equipment"]["index"].ToString(), item["equipment"]["name"].ToString(), (int)item["quantity"]));
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
                                itemSel.Add(new DNDItem(index, name, quantity));
                            }
                            else
                            {
                                List<dynamic> itemType = new List<dynamic>();
                                foreach (dynamic item in load.LoadJObj("equipment-categories/" + option["from"][rndChoice]["equipment_category"]["index"].ToString())["equipment"])
                                    itemType.Add(item);
                                for (int j = 0; j < (int)option["choose"]; j++)
                                {
                                    int ranItem = quantumQueue.Dequeue() % itemType.Count;
                                    itemSel.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
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
                                itemSel.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
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
                                    itemSel.Add(new DNDItem(index, name, quantity));
                                }
                                else
                                {
                                    List<dynamic> itemType = new List<dynamic>();
                                    foreach (dynamic item in load.LoadJObj("equipment-categories/" + option["from"][rndChoice][j.ToString()]["equipment_option"]["from"]["equipment_category"]["index"].ToString())["equipment"])
                                        itemType.Add(item);
                                    for (int k = 0; k < (int)option["from"][rndChoice][j.ToString()]["equipment_option"]["choose"]; k++)
                                    {
                                        int ranItem = quantumQueue.Dequeue() % itemType.Count;
                                        itemSel.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
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
                                    int ranItem = quantumQueue.Dequeue() % itemType.Count;
                                    itemSel.Add(new DNDItem(itemType.ElementAt(ranItem)["index"].ToString(), itemType.ElementAt(ranItem)["name"].ToString(), 1));
                                    k++;
                                }
                            }
                            j++;
                        }
                    }
                }
            }
        }

        //calculates weapon names, attack bonuses, and damage/type, as well as AC
        public void GetCombat(List<DNDItem> itemSel, List<string> otherPro, int disPro, Dictionary<string, string> coreAtt, int disLvl, List<int> ablMds, ref List<string[]> attackLst, ref int disAC, ref int disSpd, int disStr)
        {
            foreach (DNDItem item in itemSel)
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
                    foreach (string proficiency in otherPro)
                    {
                        if (proficiency == middleMan["weapon_category"].ToString() + " Weapons" || proficiency.ToLower() == middleMan["index"].ToString() + "s")
                        {
                            atkBonus += disPro;
                            break;
                        }
                    }
                    foreach (dynamic property in middleMan["properties"])
                    {
                        if (property["index"] == "finesse")
                            isFinesse = true;
                        if (property["index"] == "monk" && coreAtt["displayClass"] == "Monk")
                            isMonk = true;
                    }
                    if (isMonk)
                    {
                        int martArtDie = int.Parse(load.LoadJObj("classes/" + coreAtt["displayClass"].ToLower() + "/levels/" + disLvl)["class_specific"]["martial_arts"]["dice_value"].ToString());
                        if (int.Parse(middleMan["damage"]["damage_dice"].ToString().Split('d')[1]) < martArtDie)
                            atkDam = "1d" + martArtDie;
                        else
                            atkDam = middleMan["damage"]["damage_dice"].ToString();
                    }
                    else
                        atkDam = middleMan["damage"]["damage_dice"].ToString();
                    //if weapon can use dexterity and dex ability modifier is greater than strength
                    if ((isFinesse || coreAtt["displayClass"] == "Monk") && (ablMds.ElementAt(1) > ablMds.ElementAt(0)))
                    {
                        atkBonus += ablMds.ElementAt(1); //add dex to attack bonus
                        atkDam += " + " + ablMds.ElementAt(1); //add dex to damage
                    }
                    else
                    {
                        atkBonus += ablMds.ElementAt(0); //add str to attack bonus
                        atkDam += " + " + ablMds.ElementAt(0); //add str to damage
                    }
                    string[] atk = { item.Name, atkBonus.ToString(), atkDam + " " + damType };
                    attackLst.Add(atk);
                }

                if ("armor" == middleMan["equipment_category"]["index"].ToString() && "shield" != middleMan["index"].ToString())
                {
                    disAC = (int)middleMan["armor_class"]["base"];
                    if ((bool)middleMan["armor_class"]["dex_bonus"])
                    {
                        int dexMax = 99;
                        if (middleMan["armor_class"]["max_bonus"] != null)
                            dexMax = (int)middleMan["armor_class"]["max_bonus"];
                        if (dexMax < ablMds.ElementAt(1)) //if armor class has an maximum dex bonus that is less than dex mod
                            disAC += (int)middleMan["armor_class"]["max_bonus"];
                        else
                            disAC += ablMds.ElementAt(1); //add dex to AC
                    }
                    if (disStr < (int)middleMan["str_minimum"])
                    {
                        disSpd -= 10;
                    }
                }
                if (load.LoadJObj("classes/" + coreAtt["displayClass"].ToLower() + "/levels/1/features/")["results"][0]["index"].ToString() == "barbarian-unarmored-defense")
                    disAC = 10 + ablMds.ElementAt(1) + ablMds.ElementAt(2); // Armor class = Dex + Con
                if (load.LoadJObj("classes/" + coreAtt["displayClass"].ToLower() + "/levels/1/features/")["results"][1]["index"].ToString() == "monk-unarmored-defense")
                    disAC = 10 + ablMds.ElementAt(1) + ablMds.ElementAt(3); // Armor class = Dex + Wis
                if (disAC == -1)
                {
                    disAC = 10 + ablMds.ElementAt(1);
                }
            }
        }
    }
}
