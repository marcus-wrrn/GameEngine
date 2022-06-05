


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

        public BaseCharacterStats(uint maxHealth, uint health, int speed, float hitChance, float evasion, float criticalChance) {
            MaxHealth = maxHealth;
            Health = health;
            Speed = speed;
            HitChance = hitChance;
            Evasion = evasion;
            CriticalChance = criticalChance;
        }// end BaseCharacterStats()

        public BaseCharacterStats(uint health, int speed, float hitChance, float evasion, float criticalChance) : this(health, health, speed, hitChance, evasion, criticalChance) {}

    }// end BaseCharacterStats class
}