using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGW.DataAccess.Configuration
{
	public static class DatabaseHelper
	{
		public static List<string> GetProcedures(string nameFilter)
		{
			List<string> result = new List<string>();

			using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DataAccess.Properties.Settings.SGWConnectionString"].ConnectionString))
			{
				sql.Open();
				using (var command = sql.CreateCommand())
				{
					command.CommandText = string.Format("SELECT O.Name FROM SYSOBJECTS O WHERE O.Name LIKE '{0}%' AND XTYPE = 'P'", nameFilter);
					DataTable tb = new DataTable("Procedures");
					tb.Load(command.ExecuteReader());
					foreach (DataRow row in tb.Rows)
						result.Add(row[0].ToString());
				}
			}
			return result;
		}

		public static List<string> GetTables(string nameFilter)
		{
			List<string> result = new List<string>();

			using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DataAccess.Properties.Settings.SGWConnectionString"].ConnectionString))
			{
				sql.Open();
				using (var command = sql.CreateCommand())
				{
					command.CommandText = string.Format("SELECT O.Name FROM SYSOBJECTS O WHERE O.Name NOT LIKE '{0}%' AND O.Name NOT LIKE 'SYS%' AND XTYPE = 'U'", nameFilter);
					DataTable tb = new DataTable("Tables");
					tb.Load(command.ExecuteReader());
					foreach (DataRow row in tb.Rows)
						result.Add(row[0].ToString());
				}
			}
			return result;
		}

		public static List<KeyValuePair<string, string>> GetTableColumns(string tableName)
		{
			List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

			using (SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DataAccess.Properties.Settings.SGWConnectionString"].ConnectionString))
			{
				sql.Open();
				using (var command = sql.CreateCommand())
				{
					command.CommandText = string.Format("SELECT C.Name, C.Xtype FROM SYSOBJECTS O JOIN SYSCOLUMNS C ON C.ID = O.ID WHERE O.Name = '{0}' AND O.XTYPE = 'U'", tableName);
					DataTable tb = new DataTable("Columns");
					tb.Load(command.ExecuteReader());
					foreach (DataRow row in tb.Rows)
						result.Add(new KeyValuePair<string,string>(row[0].ToString(), row[1].ToString()));
				}
			}
			return result;
		}
	}
}
