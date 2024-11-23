namespace FabulaUltimaHpMp
{
    public class Profile
    {
        public Profile(string name, int baseDex, int baseIns, int baseMig, int baseWil, int maxHp, int maxMp)
        {
            profileName = name;
            baseDexDice = baseDex;
            baseInsDice = baseIns;
            baseMigDice = baseMig;
            baseWilDice = baseWil;
            this.maxHp = maxHp;
            this.maxMp = maxMp;
        }
        public Profile()
        {
            profileName = "";
            baseDexDice = 0;
            baseInsDice = 0;
            baseMigDice = 0;
            baseWilDice = 0;
            maxHp = 0;
            maxMp = 0;
        }
        public string profileName { get; set; }
        public int baseDexDice { get; set; }
        public int baseInsDice { get; set; }
        public int baseMigDice { get; set; }
        public int baseWilDice { get; set; }
        public int maxHp { get; set; }
        public int maxMp { get; set; }


    }
}