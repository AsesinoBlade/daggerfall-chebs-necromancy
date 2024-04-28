using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using UnityEngine;

namespace ChebsNecromancyMod.MinionSpawners
{
    public class SkeletonSpawner : MinionSpawner
    {
        private void Awake()
        {
            foeType = MobileTypes.SkeletalWarrior;
        }

        protected override void ScaleMinion(GameObject minion)
        {
            if (!minion.TryGetComponent(out DaggerfallEntityBehaviour daggerfallEntityBehaviour))
            {
                ChebsNecromancy.ChebError("Failed to scale minion - can't get DaggerfallEntityBehaviour");
                return;
            }

            mysticismLevel = mysticismLevel > 0 ? mysticismLevel : 1;
            magnitude = magnitude > 0 ? magnitude : 1;
            willpower = willpower > 0 ? willpower : 1;
            intelligence = intelligence > 0 ? intelligence : 1;

            // Vanilla Skeleton has 17-66 HP: https://en.uesp.net/wiki/Daggerfall:Skeletal_Warrior
            var minionEntity = daggerfallEntityBehaviour.Entity;
            var scaledHealth =
                mysticismLevel / 9  // +11 HP at 100 Mysticism
                + magnitude / 3     // +33 HP at 100 magnitude
                + intelligence / 7  // +14 HP at 100 int
                + willpower / 7     // +14 HP at 100 wil
                ;
            minionEntity.MaxHealth = scaledHealth;
            minionEntity.CurrentHealth = scaledHealth;
            var factor = magnitude / 400
                         + mysticismLevel / 400
                         + willpower / 400
                         + intelligence / 400;
            minionEntity.Level *= factor;
            // Give the skeleton a better weapon depending on the factor (no idea if this works)
            var maceMat = WeaponMaterialTypes.Iron;
            if (factor >= 0.9)
                maceMat = WeaponMaterialTypes.Daedric;
            else if (factor >= 0.8)
                maceMat = WeaponMaterialTypes.Ebony;
            else if (factor >= 0.7)
                maceMat = WeaponMaterialTypes.Orcish;
            else if (factor >= 0.6)
                maceMat = WeaponMaterialTypes.Adamantium;
            else if (factor >= 0.5)
                maceMat = WeaponMaterialTypes.Dwarven;
            else if (factor >= 0.4)
                maceMat = WeaponMaterialTypes.Silver;
            else if (factor >= 0.2)
                maceMat = WeaponMaterialTypes.Steel;
            var mace = ItemBuilder.CreateWeapon(Weapons.Mace, maceMat);
            // Scale damage - doesn't work. Min/max damage changes aren't reflected.
            // var mobileEnemy = minion.GetComponent<DaggerfallEnemy>().MobileUnit.Enemy;
            // mobileEnemy.MinDamage *= factor;
            // mobileEnemy.MaxDamage *= factor;
            // todo: localize
            if (showHUDMessage)
            {
                var msg = $"{foeType} created with {maceMat} {mace.shortName}!";
                DaggerfallUI.AddHUDText(msg);
            }
            minionEntity.ItemEquipTable.EquipItem(mace);
        }
    }
}