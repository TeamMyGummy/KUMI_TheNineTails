using System.Collections.Generic;
using GameAbilitySystem;

namespace Data
{
    public class ASCState
    {
        public Dictionary<string, float> Attributes;
        public Dictionary<AbilityKey, AbilityName> GrantedAbilities;
    }
}