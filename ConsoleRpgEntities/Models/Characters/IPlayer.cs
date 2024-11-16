using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters;

public interface IPlayer
{
    int Id { get; set; }
    string Name { get; set; }

    ICollection<Ability> Abilities { get; set; }
    Equipment Equipment { get; set; }

    void Attack(ITargetable target, Equipment equipment);
    void UseAbility(IAbility ability, ITargetable target);


}
