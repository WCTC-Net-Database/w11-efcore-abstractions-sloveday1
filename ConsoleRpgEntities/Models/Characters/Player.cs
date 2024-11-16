using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters
{
    public class Player : ITargetable, IPlayer
    {
        public int Experience { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }
        public virtual Equipment Equipment { get; set; }
        

        public void Attack(ITargetable target, Equipment equipment)
        {
            // Player-specific attack logic
            Console.WriteLine($"{Name} attacks {target.Name} with a {equipment.Weapon.Name} dealing {equipment.Weapon.Attack}!");
            Console.WriteLine($"{Name} is wearing {Equipment!.Armor.Name} and reduces damage by {Equipment.Armor.Defense}");
        }

        public void UseAbility(IAbility ability, ITargetable target)
        {
            if (Abilities.Contains(ability))
            {
                ability.Activate(this, target);
            }
            else
            {
                Console.WriteLine($"{Name} does not have the ability {ability.Name}!");
            }
        }
    }
}
