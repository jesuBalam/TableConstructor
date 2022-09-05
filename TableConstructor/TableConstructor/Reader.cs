using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TableConstructor
{
    class Reader
    {
        public const string MASTER_NAME_TABLE = "CVolumetrico";
        public static void ProcessDirectory(string targetDirectory)
        {
            Tables.GenerateTables();
            DirectoryInfo directoryInfo = new DirectoryInfo(targetDirectory);
            FileInfo[] fileEntries = directoryInfo.GetFiles();
            //foreach(FileInfo file in fileEntries)
            //{
            //    ConvertToJson(file.FullName, file.Name);
            //}
            ConvertToJson(fileEntries[1].FullName, fileEntries[1].Name);
        }

        public static void ConvertToJson(string path, string filename)
        {

            Tables.CleanTables();

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            JObject json = JObject.Parse(jsonText);
            string version = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.@version").ToString();
            string claveEstacionServicio = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.@claveEstacionServicio").ToString();
            string rfc = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.@rfc").ToString();
            string fechaFounded = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.@fechaYHoraCorte").ToString().Replace("T", " ");
            fechaFounded = fechaFounded.Substring(0, 19);
            DateTime fechaCorte = DateTime.Parse(fechaFounded, CultureInfo.InvariantCulture);
            bool canWrite = true;
            bool isModified = false;
            //Check if exist to overwriting: Else inser new values
            if(Sql.ExecuteQueryScalar(Sql.GetQueryFindValueMasterTable(version, claveEstacionServicio, rfc, fechaFounded)) >= 1)
            {
                //Exists!
                Console.WriteLine("Estos datos existen. Desea sobreescribirlo? y/n");
                var response = Console.ReadKey();
                if(response.Key == ConsoleKey.Y)
                {
                    isModified = true;
                    canWrite = true;
                }
                else
                {
                    canWrite = false;
                }
            }
            else
            {
                canWrite = true;
                isModified = false;
                
            }

            if (canWrite)
            {
                DateTime myDateTime = DateTime.ParseExact(DateTime.Now.Date.ToString("yyyy/MM/dd HH:mm:ss"), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

                DataRow row = Tables.mainTable.NewRow();
                row["FechaCarga"] = myDateTime;
                row["version"] = version;
                row["NombreArchivo"] = filename;
                row["IdEstacion"] = claveEstacionServicio;
                row["FechaArchivo"] = fechaCorte;
                row["RFC"] = rfc;
                Tables.mainTable.Rows.Add(row);
                if (isModified)
                {
                    Tables.mainTable.AcceptChanges();
                    foreach(DataRow r in Tables.mainTable.Rows)
                    {
                        r.SetModified();
                    }
                }
                Sql.InjectData(Tables.mainTable);
                
                string idVol = Sql.ExecuteQueryReader(Sql.GetQueryFindValueMasterTableFields(version, claveEstacionServicio, rfc, fechaFounded), 0);
                
                #region secondaryTables
                foreach (var exi in json.SelectToken("controlesvolumetricos:ControlesVolumetricos.controlesvolumetricos:EXI"))
                {
                    DataRow rowExi = Tables.exiTable.NewRow();
                    rowExi["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                    rowExi["numeroTanque"] = (int)exi.SelectToken("@numeroTanque");
                    rowExi["claveProductoPEMEX"] = exi.SelectToken("@claveProductoPEMEX").ToString();
                    rowExi["volumenUtil"] = (int)exi.SelectToken("@volumenUtil");
                    rowExi["volumenFondaje"] = (int)exi.SelectToken("@volumenFondaje");
                    rowExi["volumenAgua"] = (int)exi.SelectToken("@volumenAgua");
                    rowExi["volumenDisponible"] = (int)exi.SelectToken("@volumenDisponible");
                    rowExi["volumenExtraccion"] = (int)exi.SelectToken("@volumenExtraccion");
                    rowExi["volumenRecepcion"] = (int)exi.SelectToken("@volumenRecepcion");
                    rowExi["temperatura"] = (decimal)exi.SelectToken("@temperatura");
                    rowExi["fechaYHoraEstaMedicion"] = DateTime.Parse(exi.SelectToken("@fechaYHoraEstaMedicion").ToString());
                    rowExi["fechaYHoraMedicionAnterior"] = DateTime.Parse(exi.SelectToken("@fechaYHoraMedicionAnterior").ToString());
                    Tables.exiTable.Rows.Add(rowExi);
                }

                var rec = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.controlesvolumetricos:REC");

                DataRow rowRec = Tables.recTable.NewRow();
                rowRec["IdCVolumetrico"] = Convert.ToInt32(idVol);
                rowRec["TotalRecepciones"] = (int)rec.SelectToken("@totalRecepciones");
                rowRec["TotalDocumentos"] = (int)rec.SelectToken("@totalDocumentos");
                Tables.recTable.Rows.Add(rowRec);

                if ((int)rec.SelectToken("@totalRecepciones") > 0)
                {
                    List<int?> idList = new List<int?>();
                    foreach (var recH in rec.SelectToken("controlesvolumetricos:RECCabecera"))
                    {
                        DataRow rowRecH = Tables.recHeaderTable.NewRow();
                        rowRecH["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                        rowRecH["folioUnicoRecepcion"] = recH.SelectToken("@folioUnicoRecepcion").ToString();
                        rowRecH["claveProductoPEMEX"] = recH.SelectToken("@claveProductoPEMEX").ToString();
                        rowRecH["folioUnicoRelacion"] = recH.SelectToken("@folioUnicoRelacion").ToString();
                        Tables.recHeaderTable.Rows.Add(rowRecH);
                    }
                    if (isModified)
                    {
                        Tables.recHeaderTable.AcceptChanges();
                        foreach (DataRow r in Tables.recHeaderTable.Rows) r.SetModified();
                    }
                    Sql.InjectData(Tables.recHeaderTable);

                    foreach(DataRow rowHeader in Tables.recHeaderTable.Rows)
                    {
                        string idHeader = Sql.ExecuteQueryReader(Sql.GetQueryFindRecCabeceraId(Convert.ToInt32(rowHeader["IdCVolumetrico"]), rowHeader["folioUnicoRecepcion"].ToString(), rowHeader["claveProductoPEMEX"].ToString(), rowHeader["folioUnicoRelacion"].ToString()), 0);
                        idList.Add(idHeader != string.Empty ? Convert.ToInt32(idHeader) : (int?)null);
                    }

                    int counter = 0;
                    foreach (var recD in rec.SelectToken("controlesvolumetricos:RECDetalle"))
                    {
                        DataRow rowRecD = Tables.recDetailTable.NewRow();
                        rowRecD["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                        rowRecD["IdCabecera"] = Convert.ToInt32(idList[counter]);
                        rowRecD["folioUnicoRelacion"] = recD.SelectToken("@folioUnicoRelacion").ToString();
                        rowRecD["folioUnicoRecepcion"] = recD.SelectToken("@folioUnicoRecepcion").ToString();
                        rowRecD["numeroDeTanque"] = (int)recD.SelectToken("@numeroDeTanque");
                        rowRecD["volumenInicialTanque"] = (int)recD.SelectToken("@volumenInicialTanque");
                        rowRecD["volumenFinalTanque"] = (int)recD.SelectToken("@volumenFinalTanque");
                        rowRecD["volumenRecepcion"] = (int)recD.SelectToken("@volumenRecepcion");
                        rowRecD["temperatura"] = (decimal)recD.SelectToken("@temperatura");
                        rowRecD["fechaYHoraRecepcion"] = DateTime.Parse(recD.SelectToken("@fechaYHoraRecepcion").ToString());
                        Tables.recDetailTable.Rows.Add(rowRecD);
                        counter++;
                    }

                }

                if ((int)rec.SelectToken("@totalDocumentos") > 0)
                {
                    foreach (var recD in rec.SelectToken("controlesvolumetricos:RECDocumentos"))
                    {
                        DataRow rowRecD = Tables.recDocumentsTable.NewRow();
                        rowRecD["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                        rowRecD["folioUnicoRelacion"] = recD.SelectToken("@folioUnicoRelacion").ToString();
                        rowRecD["folioUnicoRecepcion"] = recD.SelectToken("@folioUnicoRecepcion").ToString();
                        rowRecD["terminalAlmacenamientoYDistribucion"] = (int)recD.SelectToken("@terminalAlmacenamientoYDistribucion");
                        rowRecD["tipoDocumento"] = recD.SelectToken("@tipoDocumento").ToString();
                        rowRecD["fechaDocumento"] = DateTime.Parse(recD.SelectToken("@fechaDocumento").ToString());
                        rowRecD["folioDocumentoRecepcion"] = recD.SelectToken("@folioDocumentoRecepcion").ToString();
                        rowRecD["volumenDocumentadoPEMEX"] = (int)recD.SelectToken("@volumenDocumentadoPEMEX");
                        rowRecD["claveVehiculo"] = recD.SelectToken("@claveVehiculo").ToString();
                        Tables.recDocumentsTable.Rows.Add(rowRecD);

                    }
                }

                var vta = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.controlesvolumetricos:VTA");

                DataRow rowVta = Tables.vtaTable.NewRow();
                rowVta["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                rowVta["numTotalRegistrosDetalle"] = (int)vta.SelectToken("@numTotalRegistrosDetalle");
                Tables.vtaTable.Rows.Add(rowVta);


                if ((int)vta.SelectToken("@numTotalRegistrosDetalle") > 0)
                {
                    List<int?> idList = new List<int?>();
                    foreach (var vtaH in vta.SelectToken("controlesvolumetricos:VTACabecera"))
                    {
                        DataRow rowVtaH = Tables.vtaHeaderTable.NewRow();
                        rowVtaH["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                        rowVtaH["numeroTotalRegistrosDetalle"] = (int)vtaH.SelectToken("@numeroTotalRegistrosDetalle");
                        rowVtaH["numeroDispensario"] = (int)vtaH.SelectToken("@numeroDispensario");
                        rowVtaH["identificadorManguera"] = (int)vtaH.SelectToken("@identificadorManguera");
                        rowVtaH["claveProductoPEMEX"] = vtaH.SelectToken("@claveProductoPEMEX").ToString();
                        rowVtaH["sumatoriaVolumenDespachado"] = (decimal)vtaH.SelectToken("@sumatoriaVolumenDespachado");
                        rowVtaH["sumatoriaVentas"] = (decimal)vtaH.SelectToken("@sumatoriaVentas");
                        Tables.vtaHeaderTable.Rows.Add(rowVtaH);

                    }
                    if (isModified)
                    {
                        Tables.vtaHeaderTable.AcceptChanges();
                        foreach (DataRow r in Tables.vtaHeaderTable.Rows) r.SetModified();
                    }
                    Sql.InjectData(Tables.vtaHeaderTable);
                    foreach (DataRow rowHeader in Tables.vtaHeaderTable.Rows)
                    {
                        string idHeader = Sql.ExecuteQueryReader(Sql.GetQueryFindVtaCabeceraId(Convert.ToInt32(rowHeader["IdCVolumetrico"]), (int)rowHeader["numeroTotalRegistrosDetalle"], (int)rowHeader["numeroDispensario"], (int)rowHeader["identificadorManguera"], rowHeader["claveProductoPEMEX"].ToString(), (decimal)rowHeader["sumatoriaVolumenDespachado"],(decimal)rowHeader["sumatoriaVentas"]), 0);
                        idList.Add(idHeader != string.Empty ? Convert.ToInt32(idHeader) : (int?)null);
                    }

                    int counter = 0;
                    foreach (var vtaD in vta.SelectToken("controlesvolumetricos:VTADetalle"))
                    {
                        DataRow rowVtaD = Tables.vtaDetailTable.NewRow();
                        rowVtaD["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                        rowVtaD["IdCabecera"] = Convert.ToInt32(idList[counter]);
                        rowVtaD["tipoDeRegistro"] = vtaD.SelectToken("@tipoDeRegistro").ToString();
                        rowVtaD["numeroUnicoTransaccionVenta"] = vtaD.SelectToken("@numeroUnicoTransaccionVenta").ToString();
                        rowVtaD["numeroDispensario"] = (int)vtaD.SelectToken("@numeroDispensario");
                        rowVtaD["identificadorManguera"] = vtaD.SelectToken("@identificadorManguera").ToString();
                        rowVtaD["claveProductoPEMEX"] = vtaD.SelectToken("@claveProductoPEMEX").ToString();
                        rowVtaD["volumenDespachado"] = (decimal)vtaD.SelectToken("@volumenDespachado");
                        rowVtaD["precioUnitarioProducto"] = (decimal)vtaD.SelectToken("@precioUnitarioProducto");
                        rowVtaD["importeTotalTransaccion"] = (decimal)vtaD.SelectToken("@importeTotalTransaccion");
                        rowVtaD["fechaYHoraTransaccionVenta"] = DateTime.Parse(vtaD.SelectToken("@fechaYHoraTransaccionVenta").ToString());
                        Tables.vtaDetailTable.Rows.Add(rowVtaD);
                        counter++;
                    }
                }

                var tqs = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.controlesvolumetricos:TQS");

                foreach (var tqsE in tqs)
                {
                    DataRow rowTqs = Tables.tqsTable.NewRow();
                    rowTqs["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                    rowTqs["numeroTanque"] = (int)tqsE.SelectToken("@numeroTanque");
                    rowTqs["claveProductoPEMEX"] = tqsE.SelectToken("@claveProductoPEMEX").ToString();
                    rowTqs["capacidadTotalTanque"] = (int)tqsE.SelectToken("@capacidadTotalTanque");
                    rowTqs["capacidadOperativaTanque"] = (int)tqsE.SelectToken("@capacidadOperativaTanque");
                    rowTqs["capacidadUtilTanque"] = (int)tqsE.SelectToken("@capacidadUtilTanque");
                    rowTqs["capacidadFondajeTanque"] = (int)tqsE.SelectToken("@capacidadFondajeTanque");
                    rowTqs["volumenMinimoOperacion"] = (int)tqsE.SelectToken("@volumenMinimoOperacion");
                    rowTqs["estadoTanque"] = tqsE.SelectToken("@estadoTanque").ToString();
                    Tables.tqsTable.Rows.Add(rowTqs);

                }

                var dis = json.SelectToken("controlesvolumetricos:ControlesVolumetricos.controlesvolumetricos:DIS");

                foreach (var disE in dis)
                {
                    DataRow rowDis = Tables.disTable.NewRow();
                    rowDis["IdCVolumetrico"] = Convert.ToInt32(idVol);//myDateTime; //TODO GET IDCVOLUM!!!
                    rowDis["numeroDispensario"] = (int)disE.SelectToken("@numeroDispensario");
                    rowDis["identificadorManguera"] = disE.SelectToken("@identificadorManguera").ToString();
                    rowDis["claveProductoPEMEX"] = disE.SelectToken("@claveProductoPEMEX").ToString();
                    Tables.disTable.Rows.Add(rowDis);

                }

                #endregion

                if (isModified)
                {
                    Tables.exiTable.AcceptChanges();
                    Tables.recTable.AcceptChanges();
                    
                    Tables.recDetailTable.AcceptChanges();
                    Tables.recDocumentsTable.AcceptChanges();
                    Tables.vtaTable.AcceptChanges();
                    
                    Tables.vtaDetailTable.AcceptChanges();
                    Tables.tqsTable.AcceptChanges();
                    Tables.disTable.AcceptChanges();
                    foreach (DataRow r in Tables.exiTable.Rows) r.SetModified();
                    foreach (DataRow r in Tables.recTable.Rows) r.SetModified();                    
                    foreach (DataRow r in Tables.recDetailTable.Rows) r.SetModified();
                    foreach (DataRow r in Tables.recDocumentsTable.Rows) r.SetModified();
                    foreach (DataRow r in Tables.vtaTable.Rows) r.SetModified();
                    
                    foreach (DataRow r in Tables.vtaDetailTable.Rows) r.SetModified();
                    foreach (DataRow r in Tables.tqsTable.Rows) r.SetModified();
                    foreach (DataRow r in Tables.disTable.Rows) r.SetModified();
                }

                Sql.InjectData(Tables.exiTable);
                Sql.InjectData(Tables.recTable);                
                Sql.InjectData(Tables.recDetailTable);
                Sql.InjectData(Tables.recDocumentsTable);
                Sql.InjectData(Tables.vtaTable);                
                Sql.InjectData(Tables.vtaDetailTable);
                Sql.InjectData(Tables.tqsTable);
                Sql.InjectData(Tables.disTable);

                Console.WriteLine("Process Done");
            }
        }


        public static void ReadQuery(string path)
        {
            string script = File.ReadAllText(path);
            string regexSemicolon = ";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
            string regexGo = @"^\s*GO\s*$";
            Console.WriteLine("Reading data");
            IEnumerable<string> commandStrings = Regex.Split(script, regexSemicolon, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Console.WriteLine("Executing querys");           
            foreach (string commandString in commandStrings)
            {
                if (!string.IsNullOrWhiteSpace(commandString.Trim()))
                {
                    Sql.ExecuteQuery(commandString);
                }
            }
            
        }
    }
}
