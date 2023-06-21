using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class CompraventaOperations
    {
        public void DerechosHandler(List<EnajenanteClass> enajenantes, List<AdquirienteClass> adquirientes, Escritura escritura, int updatedDate, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();
            CreateClasses createClasses = new CreateClasses();
            EnajenanteClass enajenante = enajenantes[0];
            AdquirienteClass adquiriente = adquirientes[0];
            decimal enajenanteTotalPercentage = 0;
            decimal adquirientePercentage = 0;
            try
            {
                Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db);
                enajenanteTotalPercentage = multipropietario.PorcentajeDerecho;
                adquirientePercentage = adquiriente.porcentajeDerecho * enajenanteTotalPercentage / 100;
                decimal sumOfEnajenantesPercentage = enajenanteTotalPercentage * (100 - enajenante.porcentajeDerecho) / 100;
                multipropietariosModifications.UpdateOrCreateMultipropietarioForDerechos(multipropietario, escritura, enajenante, updatedDate, sumOfEnajenantesPercentage, db);
            }
            catch
            {
                enajenanteTotalPercentage = 100;
                adquirientePercentage = adquiriente.porcentajeDerecho * enajenanteTotalPercentage / 100;
            }

            createClasses.CreateAdquirienteAndMultipropietarioForDerechos(escritura, adquiriente, adquirientePercentage, updatedDate, db);
        }
    }
}