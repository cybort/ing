﻿using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Equipment : RawDataWrapper<RawEquipment>, IID
    {
        public static Equipment Dummy { get; } = new Equipment(new RawEquipment() { ID = -1, EquipmentID = -1 });

        public int ID => RawData.ID;

        public EquipmentInfo Info { get; private set; }

        public int Level => RawData.Level;
        public string LevelText => Level == 0 ? string.Empty : Level == 10 ? "★max" : "★+" + Level;

        public bool IsLocked => RawData.IsLocked;

        public int Proficiency => RawData.Proficiency;
        public string ProficiencyText => Proficiency == 0 ? string.Empty : "+" + Proficiency;

        public string FullName
        {
            get
            {
                var rBuilder = new StringBuilder();

                rBuilder.Append(Info.Name);

                if (Level > 0)
                    rBuilder.Append(' ').Append(LevelText);

                if (Proficiency > 0)
                    rBuilder.Append(' ').Append(ProficiencyText);

                return rBuilder.ToString();
            }
        }

        internal Equipment(RawEquipment rpRawData) : base(rpRawData)
        {
            EquipmentInfo rInfo;
            if (KanColleGame.Current.MasterInfo.Equipments.TryGetValue(rpRawData.EquipmentID, out rInfo))
                Info = rInfo;
            else
                Info = EquipmentInfo.Dummy;
        }

        protected override void OnRawDataUpdated()
        {
            if (Info == null || Info.ID != RawData.EquipmentID)
            {
                EquipmentInfo rInfo;
                if (!KanColleGame.Current.MasterInfo.Equipments.TryGetValue(RawData.EquipmentID, out rInfo))
                    rInfo = EquipmentInfo.Dummy;

                Info = rInfo;
            }

            OnPropertyChanged(nameof(Info));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(LevelText));
            OnPropertyChanged(nameof(IsLocked));
            OnPropertyChanged(nameof(Proficiency));
            OnPropertyChanged(nameof(ProficiencyText));
        }

        public override string ToString() => $"ID = {ID}, Name = \"{Info.Name}\", Level = {Level}{(Proficiency == 0 ? string.Empty : $" Proficiency = {Proficiency}")}";
    }
}
