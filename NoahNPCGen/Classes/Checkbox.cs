namespace NoahNPCGen
{
    using System.Collections.Generic;

    public class Checkbox
    {
        private int ability;
        private string boxName;
        private string modName;
        private string labelName;
        private bool check;

        public int Ability { get => ability; set => ability = value; }
        public string BoxName { get => boxName; set => boxName = value; }
        public string ModName { get => modName; set => modName = value; }
        public string LabelName { get => labelName; set => labelName = value; }
        public bool Check { get => check; set => check = value; }

        public Checkbox()
        {

        }
        private Checkbox(int abl, string box, string mod, string label, bool check)
        {
            this.ability = abl;
            this.boxName = box;
            this.modName = mod;
            this.labelName = label;
            this.check = check;
        }
        public int ProfBonus(int abl, bool pro, int prof)
        {
            if (pro)
                return abl + prof;
            return abl;
        }
        public List<Checkbox> AbilityProf()
        {
            return new List<Checkbox>()
        {
            new Checkbox(1, "DexAcr", "DexAcrMod", "Acrobatics", false),
            new Checkbox(4, "WisAni", "WisAniMod", "Animal Handling", false),
            new Checkbox(3, "IntArc", "IntArcMod", "Arcana", false),
            new Checkbox(0, "StrAth", "StrAthMod", "Athletics", false),
            new Checkbox(5, "ChaDec", "ChaDecMod", "Deception", false),
            new Checkbox(3, "IntHis", "IntHisMod", "History", false),
            new Checkbox(4, "WisIns", "WisInsMod", "Insight", false),
            new Checkbox(3, "ChaInt", "ChaIntMod", "Intimidation", false),
            new Checkbox(3, "IntInv", "IntInvMod", "Investigation", false),
            new Checkbox(4, "WisMed", "WisMedMod", "Medicine", false),
            new Checkbox(3, "IntNat", "IntNatMod", "Nature", false),
            new Checkbox(4, "WisPer", "WisPerMod", "Perception", false),
            new Checkbox(5, "ChaPerf", "ChaPerfMod", "Performance", false),
            new Checkbox(5, "ChaPers", "ChaPersMod", "Persuasion", false),
            new Checkbox(3, "IntRel", "IntRelMod", "Religion", false),
            new Checkbox(1, "DexSle", "DexSleMod", "Sleight of Hand", false),
            new Checkbox(1, "DexSte", "DexSteMod", "Stealth", false),
            new Checkbox(4, "WisSur", "WisSurMod", "Survival", false)
        };
        }

        public List<Checkbox> SavingThro()
        {
            return new List<Checkbox>()
        {
            new Checkbox(0, "StrSav", "StrSavThr", "Strength Saving Throw", false),
            new Checkbox(1, "DexSav", "DexSavThr", "Dexterity Saving Throw", false),
            new Checkbox(2, "ConSav", "ConSavThr", "Constitution Saving Throw", false),
            new Checkbox(3, "IntSav", "IntSavThr", "Intelligence Saving Throw", false),
            new Checkbox(4, "WisSav", "WisSavThr", "Wisdom Saving Throw", false),
            new Checkbox(5, "ChaSav", "ChaSavThr", "Charisma Saving Throw", false),
        };
        }
    }
}


