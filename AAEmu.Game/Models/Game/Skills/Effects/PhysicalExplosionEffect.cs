using System;
using AAEmu.Game.Models.Game.Skills.Templates;
using AAEmu.Game.Models.Game.Units;

namespace AAEmu.Game.Models.Game.Skills.Effects
{
    public class PhysicalExplosionEffect : EffectTemplate
    {
        public float Radius { get; set; }
        public float HoleSize { get; set; }
        public float Pressure { get; set; }

        public override bool OnActionTime => false;

        public override void Apply(Unit caster, SkillAction casterObj, BaseUnit target, SkillAction targetObj, CastAction castObj,
            Skill skill, DateTime time)
        {
            _log.Debug("PhysicalExplosionEffect");
        }
    }
}