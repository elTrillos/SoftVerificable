using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class CompraventaOperations
    {
        public void DerechosHandler(List<LocalEnajenante> enajenantes, List<LocalAdquiriente> adquirientes, Escritura escritura, int updatedDate, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();
            CreateClasses createClasses = new CreateClasses();
            LocalEnajenante enajenante = enajenantes[0];
            LocalAdquiriente adquiriente = adquirientes[0];
            decimal enajenanteTotalPercentage;
            decimal adquirientePercentage;

            try
            {
                Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.Rut, db);
                enajenanteTotalPercentage = multipropietario.PorcentajeDerecho;
                adquirientePercentage = adquiriente.PorcentajeDerecho * enajenanteTotalPercentage / 100;
                decimal sumOfEnajenantesPercentage = enajenanteTotalPercentage * (100 - enajenante.PorcentajeDerecho) / 100;
                multipropietariosModifications.UpdateOrCreateMultipropietarioForDerechos(multipropietario, escritura, enajenante, updatedDate, sumOfEnajenantesPercentage, db);
            }
            catch
            {
                enajenanteTotalPercentage = 100;
                adquirientePercentage = adquiriente.PorcentajeDerecho * enajenanteTotalPercentage / 100;
                decimal sumOfEnajenantesPercentage;
                bool emptyCheck=false;
                try
                {
                    sumOfEnajenantesPercentage = databaseQueries.SumOfAllMultipropietariosPercentage(escritura, db)-adquirientePercentage;
                }
                catch
                {
                    sumOfEnajenantesPercentage = enajenanteTotalPercentage * (100 - enajenante.PorcentajeDerecho) / 100;
                    emptyCheck = true;
                }
                System.Diagnostics.Debug.WriteLine("checkkk");
                System.Diagnostics.Debug.WriteLine(emptyCheck);
                if (emptyCheck)
                {
                    multipropietariosModifications.CreateMultipropietarioForDerechosFantasma(escritura, enajenante, updatedDate, sumOfEnajenantesPercentage, db);
                }
            }
            createClasses.CreateAdquirienteAndMultipropietarioForDerechos(escritura, adquiriente, adquirientePercentage, updatedDate, db);
        }
    }
}