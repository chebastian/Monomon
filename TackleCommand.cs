using System.Threading.Tasks;

namespace Monomon.Battle
{
    public enum AttackType
    { 
        Tackle,
        Slash,
        Wrap,
    } 

    public record MonStatus(int attack, int defense, int speed);
    public record AttackCommand(AttackType attackType, MonStatus stat);
}