using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableConstructor
{
    class Tables
    {
        public static DataTable mainTable = new DataTable();

        public static DataTable exiTable = new DataTable();
        public static DataTable recTable = new DataTable();
        public static DataTable recHeaderTable = new DataTable();
        public static DataTable recDetailTable = new DataTable();
        public static DataTable recDocumentsTable = new DataTable();
        public static DataTable vtaTable = new DataTable();
        public static DataTable vtaHeaderTable = new DataTable();
        public static DataTable vtaDetailTable = new DataTable();
        public static DataTable tqsTable = new DataTable();
        public static DataTable disTable = new DataTable();
        public static void GenerateTables(string name = "CVolumetrico")
        {

            mainTable.TableName = name;
            DataColumn IdColumn = mainTable.Columns.Add("Id", typeof(Int32));
            IdColumn.AutoIncrement = true;
            mainTable.Columns.Add("FechaCarga", typeof(DateTime));
            mainTable.Columns.Add("version", typeof(string));
            mainTable.Columns.Add("NombreArchivo", typeof(string));
            mainTable.Columns.Add("IdEstacion", typeof(string));
            mainTable.Columns.Add("FechaArchivo", typeof(DateTime));
            mainTable.Columns.Add("RFC", typeof(string));


            exiTable.TableName = "EXI";
            DataColumn IdColumnExi = exiTable.Columns.Add("Id", typeof(Int32));
            IdColumnExi.AutoIncrement = true;
            exiTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            exiTable.Columns.Add("numeroTanque", typeof(Int32));
            exiTable.Columns.Add("claveProductoPEMEX", typeof(string));
            exiTable.Columns.Add("volumenUtil", typeof(Int32));
            exiTable.Columns.Add("volumenFondaje", typeof(Int32));
            exiTable.Columns.Add("volumenAgua", typeof(Int32));
            exiTable.Columns.Add("volumenDisponible", typeof(Int32));
            exiTable.Columns.Add("volumenExtraccion", typeof(Int32));
            exiTable.Columns.Add("volumenRecepcion", typeof(Int32));
            exiTable.Columns.Add("temperatura", typeof(decimal));
            exiTable.Columns.Add("fechaYHoraEstaMedicion", typeof(DateTime));
            exiTable.Columns.Add("fechaYHoraMedicionAnterior", typeof(DateTime));

            recTable.TableName = "REC";
            DataColumn IdColumnRec = recTable.Columns.Add("Id", typeof(Int32));
            IdColumnRec.AutoIncrement = true;
            recTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            recTable.Columns.Add("TotalRecepciones", typeof(Int32));
            recTable.Columns.Add("TotalDocumentos", typeof(Int32));

            recHeaderTable.TableName = "RECCabecera";
            DataColumn IdColumnRecH = recHeaderTable.Columns.Add("Id", typeof(Int32));
            IdColumnRecH.AutoIncrement = true;
            recHeaderTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            recHeaderTable.Columns.Add("folioUnicoRecepcion", typeof(string));
            recHeaderTable.Columns.Add("claveProductoPEMEX", typeof(string));
            recHeaderTable.Columns.Add("folioUnicoRelacion", typeof(string));


            recDetailTable.TableName = "RECDetalle";
            DataColumn IdColumnRecD = recDetailTable.Columns.Add("Id", typeof(Int32));
            IdColumnRecD.AutoIncrement = true;
            recDetailTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            recDetailTable.Columns.Add("folioUnicoRelacion", typeof(string));
            recDetailTable.Columns.Add("folioUnicoRecepcion", typeof(string));
            recDetailTable.Columns.Add("numeroDeTanque", typeof(Int32));
            recDetailTable.Columns.Add("volumenInicialTanque", typeof(Int32));
            recDetailTable.Columns.Add("volumenFinalTanque", typeof(Int32));
            recDetailTable.Columns.Add("volumenRecepcion", typeof(Int32));
            recDetailTable.Columns.Add("temperatura", typeof(decimal));
            recDetailTable.Columns.Add("fechaYHoraRecepcion", typeof(DateTime));

            recDocumentsTable.TableName = "RECDocumentos";
            DataColumn IdColumnRecDD = recDocumentsTable.Columns.Add("Id", typeof(Int32));
            IdColumnRecDD.AutoIncrement = true;
            recDocumentsTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            recDocumentsTable.Columns.Add("folioUnicoRelacion", typeof(string));
            recDocumentsTable.Columns.Add("folioUnicoRecepcion", typeof(string));
            recDocumentsTable.Columns.Add("terminalAlmacenamientoYDistribucion", typeof(Int32));
            recDocumentsTable.Columns.Add("tipoDocumento", typeof(string));
            recDocumentsTable.Columns.Add("fechaDocumento", typeof(DateTime));
            recDocumentsTable.Columns.Add("folioDocumentoRecepcion", typeof(string));
            recDocumentsTable.Columns.Add("volumenDocumentadoPEMEX", typeof(Int32));
            recDocumentsTable.Columns.Add("claveVehiculo", typeof(string));

            vtaTable.TableName = "VTA";
            DataColumn IdColumnVta = vtaTable.Columns.Add("Id", typeof(Int32));
            IdColumnVta.AutoIncrement = true;
            vtaTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            vtaTable.Columns.Add("numTotalRegistrosDetalle", typeof(Int32));

            vtaHeaderTable.TableName = "VTACabecera";
            DataColumn IdColumnVtaH = vtaHeaderTable.Columns.Add("Id", typeof(Int32));
            IdColumnVtaH.AutoIncrement = true;
            vtaHeaderTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            vtaHeaderTable.Columns.Add("numeroTotalRegistrosDetalle", typeof(Int32));
            vtaHeaderTable.Columns.Add("numeroDispensario", typeof(Int32));
            vtaHeaderTable.Columns.Add("identificadorManguera", typeof(Int32));
            vtaHeaderTable.Columns.Add("claveProductoPEMEX", typeof(string));
            vtaHeaderTable.Columns.Add("sumatoriaVolumenDespachado", typeof(decimal));
            vtaHeaderTable.Columns.Add("sumatoriaVentas", typeof(decimal));

            vtaDetailTable.TableName = "VTADetalle";
            DataColumn IdColumnVtaD = vtaDetailTable.Columns.Add("Id", typeof(Int32));
            IdColumnVtaD.AutoIncrement = true;
            vtaDetailTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            vtaDetailTable.Columns.Add("tipoDeRegistro", typeof(string));
            vtaDetailTable.Columns.Add("numeroUnicoTransaccionVenta", typeof(string));
            vtaDetailTable.Columns.Add("numeroDispensario", typeof(Int32));
            vtaDetailTable.Columns.Add("identificadorManguera", typeof(string));
            vtaDetailTable.Columns.Add("claveProductoPEMEX", typeof(string));
            vtaDetailTable.Columns.Add("volumenDespachado", typeof(decimal));
            vtaDetailTable.Columns.Add("precioUnitarioProducto", typeof(decimal));
            vtaDetailTable.Columns.Add("importeTotalTransaccion", typeof(decimal));
            vtaDetailTable.Columns.Add("fechaYHoraTransaccionVenta", typeof(DateTime));


            tqsTable.TableName = "TQS";
            DataColumn IdColumnTqs = tqsTable.Columns.Add("Id", typeof(Int32));
            IdColumnTqs.AutoIncrement = true;
            tqsTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            tqsTable.Columns.Add("numeroTanque", typeof(Int32));
            tqsTable.Columns.Add("claveProductoPEMEX", typeof(string));
            tqsTable.Columns.Add("capacidadTotalTanque", typeof(Int32));
            tqsTable.Columns.Add("capacidadOperativaTanque", typeof(Int32));
            tqsTable.Columns.Add("capacidadUtilTanque", typeof(Int32));
            tqsTable.Columns.Add("capacidadFondajeTanque", typeof(Int32));
            tqsTable.Columns.Add("volumenMinimoOperacion", typeof(Int32));
            tqsTable.Columns.Add("estadoTanque", typeof(string));

            disTable.TableName = "DIS";
            DataColumn IdColumnDis = disTable.Columns.Add("Id", typeof(Int32));
            IdColumnDis.AutoIncrement = true;
            disTable.Columns.Add("IdCVolumetrico", typeof(Int32));
            disTable.Columns.Add("numeroDispensario", typeof(Int32));
            disTable.Columns.Add("identificadorManguera", typeof(string));
            disTable.Columns.Add("claveProductoPEMEX", typeof(string));

        }

        public static void CleanTables()
        {
            mainTable.Rows.Clear();
            exiTable.Rows.Clear();
            recTable.Rows.Clear();
            recHeaderTable.Rows.Clear();
            recDetailTable.Rows.Clear();
            recDocumentsTable.Rows.Clear();
            vtaTable.Rows.Clear();
            vtaDetailTable.Rows.Clear();
            vtaHeaderTable.Rows.Clear();
            tqsTable.Rows.Clear();
            disTable.Rows.Clear();
        }


        public static string GetQueryGenerateTables()
        {
            string query = @"CREATE TABLE CVolumetrico(  Id int IDENTITY(1,1) PRIMARY KEY,
                                               FechaCarga datetime,
				                               version varchar(100),
				                               NombreArchivo varchar(100),
				                               IdEstacion varchar(100),
				                               FechaArchivo datetime,
				                               RFC varchar(100))

                                                CREATE TABLE EXI(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                numeroTanque int,
                                                claveProductoPEMEX varchar(100),
                                                volumenUtil int,
                                                volumenFondaje int,
                                                volumenAgua int,
                                                volumenDisponible int,
                                                volumenExtraccion int,
                                                volumenRecepcion int,
                                                temperatura decimal(10,2),
                                                fechaYHoraEstaMedicion datetime,
                                                fechaYHoraMedicionAnterior datetime)

                                                CREATE TABLE REC(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                TotalRecepciones int,
                                                TotalDocumentos int)

                                                CREATE TABLE RECCabecera(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                folioUnicoRecepcion varchar(100),
                                                claveProductoPEMEX varchar(100),
                                                folioUnicoRelacion varchar(100))

                                                CREATE TABLE RECDetalle(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                folioUnicoRelacion varchar(100),
                                                folioUnicoRecepcion varchar(100),
                                                numeroDeTanque int,
                                                volumenInicialTanque int,
                                                volumenFinalTanque int,
                                                volumenRecepcion int,
                                                temperatura decimal(10,2),
                                                fechaYHoraRecepcion datetime)

                                                CREATE TABLE RECDocumentos(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                folioUnicoRelacion varchar(100),
                                                folioUnicoRecepcion varchar(100),
                                                terminalAlmacenamientoYDistribucion int,
                                                tipoDocumento varchar(100),
                                                fechaDocumento datetime,
                                                folioDocumentoRecepcion varchar(100),
                                                volumenDocumentadoPEMEX int,
                                                claveVehiculo varchar(100))

                                                CREATE TABLE VTA(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                numTotalRegistrosDetalle int)

                                                CREATE TABLE VTACabecera(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                numeroTotalRegistrosDetalle int,
                                                numeroDispensario int,
                                                identificadorManguera int,
                                                claveProductoPEMEX varchar(100),
                                                sumatoriaVolumenDespachado decimal(10,3),
                                                sumatoriaVentas decimal(10,3))

                                                CREATE TABLE VTADetalle(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                tipoDeRegistro varchar(100),
                                                numeroUnicoTransaccionVenta varchar(100),
                                                numeroDispensario int,
                                                identificadorManguera varchar(100),
                                                claveProductoPEMEX varchar(100),
                                                volumenDespachado decimal(10,3),
                                                precioUnitarioProducto decimal(10,3),
                                                importeTotalTransaccion decimal(10,3),
                                                fechaYHoraTransaccionVenta datetime)

                                                CREATE TABLE TQS(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                numeroTanque int,
                                                claveProductoPEMEX varchar(100),
                                                capacidadTotalTanque int, 
                                                capacidadOperativaTanque int,
                                                capacidadUtilTanque int,
                                                capacidadFondajeTanque int, 
                                                volumenMinimoOperacion int,
                                                estadoTanque varchar(100))

                                                CREATE TABLE DIS(  Id int IDENTITY(1,1) PRIMARY KEY,
                                                IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                                                numeroDispensario int,
                                                identificadorManguera varchar(100),
                                                claveProductoPEMEX varchar(100))";

            return query;
            
        }
    }
}
