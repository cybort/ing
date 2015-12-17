﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    public class ExperienceRecord : RecordBase
    {
        public override string GroupName => "experience";

        int r_Admiral;
        Dictionary<int, int> r_Ships = new Dictionary<int, int>(100);

        internal ExperienceRecord(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(SessionService.Instance.Subscribe("api_port/port", _ =>
            {
                var rPort = KanColleGame.Current.Port;

                using (var rTransaction = Connection.BeginTransaction())
                {
                    if (r_Admiral != rPort.Admiral.Experience)
                    {
                        r_Admiral = rPort.Admiral.Experience;
                        InsertAdmiralRecord(rPort.Admiral.Experience);
                    }

                    foreach (var rShip in rPort.Ships.Values.Where(r => r.Experience > 0))
                    {
                        int rOldExperience;
                        if (!r_Ships.TryGetValue(rShip.ID, out rOldExperience))
                            r_Ships.Add(rShip.ID, rShip.Experience);
                        else
                            r_Ships[rShip.ID] = rShip.Experience;

                        if (rOldExperience != rShip.Experience)
                            InsertShipExperience(rShip.ID, rShip.Experience);
                    }

                    rTransaction.Commit();
                }
            }));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS admiral_experience(" +
                    "time INTEGER PRIMARY KEY NOT NULL, " +
                    "experience INTEGER NOT NULL);" +

                    "CREATE TABLE IF NOT EXISTS ship_experience(" +
                    "id INTEGER NOT NULL, " +
                    "time INTEGER NOT NULL, " +
                    "experience INTEGER NOT NULL, " +
                    "CONSTRAINT [] PRIMARY KEY(id, time)) WITHOUT ROWID;";

                rCommand.ExecuteNonQuery();
            }
        }
        protected override void Load()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT experience FROM admiral_experience ORDER BY time DESC LIMIT 1;";

                r_Admiral = Convert.ToInt32(rCommand.ExecuteScalar());
            }
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "SELECT id, experience FROM ship_experience GROUP BY id;";

                using (var rReader = rCommand.ExecuteReader())
                    while (rReader.Read())
                    {
                        var rID = Convert.ToInt32(rReader["id"]);
                        var rExperience = Convert.ToInt32(rReader["experience"]);

                        r_Ships.Add(rID, rExperience);
                    }
            }
        }

        void InsertAdmiralRecord(int rpExperience)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO admiral_experience(time, experience) " +
                    "VALUES(strftime('%s', 'now'), @experience);";
                rCommand.Parameters.AddWithValue("@experience", rpExperience);

                rCommand.ExecuteNonQuery();
            }
        }
        void InsertShipExperience(int rpID, int rpExperience)
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO ship_experience(id, time, experience) " +
                    "VALUES(@id, strftime('%s', 'now'), @experience);";
                rCommand.Parameters.AddWithValue("@id", rpID);
                rCommand.Parameters.AddWithValue("@experience", rpExperience);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
