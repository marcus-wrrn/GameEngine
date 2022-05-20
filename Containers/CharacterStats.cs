


namespace Containers {
    public interface IStats {
        uint MaxHealth { get; set; }
        uint Health { get; set; }
        int Speed { get; set; }
        float HitChance { get; set; }
        float Evasion { get; set; }
        float CriticalChance { get; set; }
    }// end IStats

    public class BaseCharacterStats : IStats {
        public uint MaxHealth { get; set; }
        public uint Health { get; set; }
        public int Speed { get; set; }
        public float HitChance { get; set; }
        public float Evasion { get; set; }
        public float CriticalChance { get; set; }

        public BaseCharacterStats(uint health, int speed, float hitChance, float evasion, float criticalChance) {
            MaxHealth = health;
            Health = MaxHealth;
            Speed = speed;
            HitChance = hitChance;
            Evasion = evasion;
            CriticalChance = criticalChance;
        }// end constructor

    }// end BaseCharacterStats class
}