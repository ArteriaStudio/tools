using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace LigareBook
{
	public class ProfileProvider
	{
		ProfileProvider()
		{
		}

		public static bool Save(ProfileData pProfile)
		{
			var m_pConnectionString = "Data Source=" + pProfile.m_pProfilePath;

			using (var pConnection = new SqliteConnection(m_pConnectionString))
			using (var pCommand = pConnection.CreateCommand())
			{
				pConnection.Open();

				pCommand.CommandText = @"CREATE TABLE UserParameters (Item TEXT, Value TEXT);";
				pCommand.ExecuteNonQuery();

				pCommand.CommandText = String.Format("INSERT INTO UserParameters (Item, Value) VALUES ('DatabaseServer', '{0}')", pProfile.m_pDatabaseServer);
				pCommand.ExecuteNonQuery();

				pCommand.Connection = pConnection;
			}


			return (true);
		}

		public static ProfileData Load()
		{
			ProfileData pProfileData = new ProfileData();

			return (pProfileData);
		}

	}
}
