﻿using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class ShipTypeInfo
    {
        public TextTranslationGroup Name { get; } = new TextTranslationGroup();

        partial void CreateDummy()
        {
            Name.Origin = owner.Localization?.GetUnlocalized("ShipTypeName", Id.ToString());
            Name.Translation = owner.Localization?.GetLocalized("ShipTypeName", Id.ToString());
        }

        partial void UpdateCore(IRawShipTypeInfo raw)
        {
            if (Name.Origin == null)
            {
                Name.Origin = raw.Name;
                NotifyPropertyChanged(nameof(Name));
            }
            availableEquipmentTypes.Query = raw.AvailableEquipmentTypes.Select(x => owner.EquipmentTypes[x]);
        }

        public override string ToString() => $"ShipTypeInfo {Id}: {Name.Origin}";
    }
}
