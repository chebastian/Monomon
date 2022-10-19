using Monomon.Mons;
using System.Threading.Tasks;

namespace Monomon.Battle
{
    public enum AttackType
    { 
        Tackle,
        Slash,
        Swipe,
        Wrap,
        Growl
    } 

    public record MonStatus(int attack, int defense, int speed);
    public record BattleCommand();
    public record AttackCommand(AttackType attackType, MonStatus stat) : BattleCommand();
    public record Tackle(MonStatus stat) : AttackCommand(AttackType.Tackle,stat);
    public record Slash(MonStatus stat) : AttackCommand(AttackType.Slash,stat with { attack = stat.attack + 12 });
    public record Swipe(MonStatus stat) : AttackCommand(AttackType.Swipe,stat with { attack = stat.attack + 7 });
    public record Growl(MonStatus stat) : AttackCommand(AttackType.Growl,stat with { attack = 0 });

    public record PotionCommand(int hpRestore) : BattleCommand();
    public record SwapMonCommand(Mobmon from, Mobmon to) : BattleCommand();
}