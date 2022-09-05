using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableConstructor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Sql.CheckTableExist())
            {
                //Sql.ExecuteQuery(Tables.GetQueryGenerateTables());
                Reader.ReadQuery(ConfigurationManager.AppSettings["PathQueryConstruct"]);
            }
            Reader.ProcessDirectory(@"C:\Users\enriq\Downloads\Ulises2017-Sep-Dic\Diciembre");
            Console.ReadKey();


            //Steps::
            /*
             * Leer todos los archivos,  CHECK 
             * Verificar que exista la tabla principal CHECK
             * Si existe, verificar que los datos de la tabla principal no se repitan en cada archivo 
             * Si se repite, mandar aviso preguntando si se desea sobreescribir datos
             * Escribir datos tabla principal
             * Crear tablas secundarias(manualmente?)
             * Escribir datos tablas secundarias
             */
        }
    }
}
