using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableConstructor
{
    class Sql
    {
        public static bool CheckTableExist(string tableName = Reader.MASTER_NAME_TABLE)
        {
            SqlConnection connection = new SqlConnection(string.Format("Data Source={0};database={1};Integrated Security=SSPI; User ID={2};Password={3}", ConfigurationManager.AppSettings["ServerDatabase"], ConfigurationManager.AppSettings["Database"], ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Pass"]));
            connection.Open();
            SqlCommand cmd = new SqlCommand(@"IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table) SELECT 1 ELSE SELECT 0", connection);
            cmd.Parameters.Add("@table", SqlDbType.NVarChar).Value = tableName;
            int exists = (int)cmd.ExecuteScalar();
            connection.Close();
            return exists == 1;
            
        }

        public static void ExecuteQuery(string query)
        {
            SqlConnection connection = new SqlConnection(string.Format("Data Source={0};database={1};Integrated Security=SSPI; User ID={2};Password={3}", ConfigurationManager.AppSettings["ServerDatabase"], ConfigurationManager.AppSettings["Database"], ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Pass"]));
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                int result = command.ExecuteNonQuery();
                
                Console.WriteLine("Completed query" + result);
            }
            connection.Close();
        }

        public static string ExecuteQueryReader(string query, int indexResponse)
        {
            SqlConnection connection = new SqlConnection(string.Format("Data Source={0};database={1};Integrated Security=SSPI; User ID={2};Password={3}", ConfigurationManager.AppSettings["ServerDatabase"], ConfigurationManager.AppSettings["Database"], ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Pass"]));
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                SqlDataReader result = command.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        Console.WriteLine("{0}", result.GetValue(indexResponse));
                        return result.GetValue(indexResponse).ToString();
                    }
                }
            }
            connection.Close();
            return string.Empty;
        }


        public static int ExecuteQueryScalar(string query)
        {
            int result = 0;
            SqlConnection connection = new SqlConnection(string.Format("Data Source={0};database={1};Integrated Security=SSPI; User ID={2};Password={3}", ConfigurationManager.AppSettings["ServerDatabase"], ConfigurationManager.AppSettings["Database"], ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Pass"]));
            connection.Open();
            using (var command = new SqlCommand(query, connection))
            {
                result = (int)command.ExecuteScalar();

                Console.WriteLine("Completed query" + result);
            }
            connection.Close();
            return result;
        }


        public static string GetQueryCreateTable(DataTable table)
        {
            string sqlsc;
            sqlsc = "CREATE TABLE " + table.TableName + "(";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";
                string columnType = table.Columns[i].DataType.ToString();
                switch (columnType)
                {
                    case "System.Int32":
                        sqlsc += " int ";
                        break;
                    case "System.Int64":
                        sqlsc += " bigint ";
                        break;
                    case "System.Int16":
                        sqlsc += " smallint";
                        break;
                    case "System.Byte":
                        sqlsc += " tinyint";
                        break;
                    case "System.Decimal":
                        sqlsc += " decimal ";
                        break;
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.String":
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
        }

        public static void InjectData(DataTable table)
        {

            SqlConnection connection = new SqlConnection(string.Format("Data Source={0}; database={1}; Integrated Security=SSPI; User ID={2}; Password={3}", ConfigurationManager.AppSettings["ServerDatabase"], ConfigurationManager.AppSettings["Database"], ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Pass"]));
            connection.Open();

            SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
            bulkCopy.DestinationTableName = table.TableName;
            bulkCopy.WriteToServer(table);

            connection.Close();
            Console.WriteLine("Data Injected");
        }

        public static string GetQueryFindValueMasterTable(string version, string idEstacion, string rfc, string fileDate)
        {
            string query = $"SELECT COUNT(*) FROM {Reader.MASTER_NAME_TABLE} where version = '{version}' and IdEstacion='{idEstacion}' and RFC='{rfc}' and FechaArchivo='{fileDate}'";
            return query;
        }

        public static string GetQueryFindValueMasterTableFields(string version, string idEstacion, string rfc, string fileDate)
        {
            string query = $"SELECT * FROM {Reader.MASTER_NAME_TABLE} where version = '{version}' and IdEstacion='{idEstacion}' and RFC='{rfc}' and FechaArchivo='{fileDate}'";            
            return query;
        }

        public static string GetQueryFindRecCabeceraId(int idVol, string folioUnicoRecepcion, string claveProductoPEMEX, string folioUnicoRelacion)
        {
            string query = $"SELECT * FROM RECCabecera where Id={idVol} and folioUnicoRecepcion = '{folioUnicoRecepcion}' and claveProductoPEMEX='{claveProductoPEMEX}' and folioUnicoRelacion='{folioUnicoRelacion}'";
            return query;
        }

        public static string GetQueryFindVtaCabeceraId(int idVol, int numReg, int numDis, int idMang, string pemexId, decimal volSum, decimal sellsVol)
        {
            string query = $"SELECT * FROM RECCabecera where Id={idVol} and numeroTotalRegistrosDetalle = {numReg} and numeroDispensario={numDis} and identificadorManguera={idMang} and claveProductoPEMEX='{pemexId}'  and sumatoriaVolumenDespachado={volSum} and sumatoriaVentas={sellsVol}";
            return query;
        }
    }
}
