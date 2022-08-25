					CREATE TABLE CVolumetrico(  Id int IDENTITY(1,1) PRIMARY KEY,
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
                    folioUnicoRelacion varchar(100) UNIQUE)

                    CREATE TABLE RECDetalle(  Id int IDENTITY(1,1) PRIMARY KEY,
                    IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                    folioUnicoRelacion varchar(100) FOREIGN KEY REFERENCES RECCabecera(folioUnicoRelacion),
                    folioUnicoRecepcion varchar(100),
                    numeroDeTanque int,
                    volumenInicialTanque int,
                    volumenFinalTanque int,
                    volumenRecepcion int,
                    temperatura decimal(10,2),
                    fechaYHoraRecepcion datetime)

                    CREATE TABLE RECDocumentos(  Id int IDENTITY(1,1) PRIMARY KEY,
                    IdCVolumetrico int FOREIGN KEY REFERENCES CVolumetrico(Id),
                    folioUnicoRelacion varchar(100) FOREIGN KEY REFERENCES RECCabecera(folioUnicoRelacion),
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
                    claveProductoPEMEX varchar(100))