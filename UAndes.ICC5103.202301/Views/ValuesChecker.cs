using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class ValuesChecker
    {
        public bool CheckIfEscrituraValuesAreValid(Escritura escritura)
        {
            if (escritura.NumeroInscripcion == null)
            {
                return false;
            }
            else if (escritura.Manzana == null)
            {
                return false;
            }
            else if (escritura.Predio == null)
            {
                return false;
            }
            else if (escritura.Comuna == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckIfSumOfPercentagesIsValid(Escritura escritura, List<AdquirienteClass> adquirientes)
        {
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfPercentages = 100 - adquirienteVerificator.SumOfPercentages(adquirientes);
            bool adquirientesWithoutAcreditedPercentages = adquirienteVerificator.CheckIfAnyAdquirienteWithoutDeclared(adquirientes);
            if (sumOfPercentages != 0 && !adquirientesWithoutAcreditedPercentages)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public int GetLatestMultipropietarioYear(List<Multipropietario> multipropietarios)
        {
            int year = 0;
            foreach (Multipropietario multipropietario in multipropietarios)
            {
                if (year < multipropietario.AñoVigenciaInicial)
                {
                    year = multipropietario.AñoVigenciaInicial;
                }
            }
            return year;
        }
        public int GetLatestInscriptionNumberOfYear(List<Multipropietario> multipropietarios, int year)
        {
            int inscriptionNumber = 0;
            foreach (Multipropietario multipropietario in multipropietarios)
            {
                if (multipropietario.AñoVigenciaInicial == year && multipropietario.NumeroInscripcion > inscriptionNumber)
                {
                    inscriptionNumber = multipropietario.NumeroInscripcion;
                }
            }
            return inscriptionNumber;
        }
        public bool CheckIfDataIsValidCompraventa(Escritura escritura, int updatedDate, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            try
            {
                List<Multipropietario> validMultipropietarios = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                int currentAñoVigenciaFinal = GetLatestMultipropietarioYear(validMultipropietarios);
                int currentMaxInscriptionNumber = GetLatestInscriptionNumberOfYear(validMultipropietarios, escritura.FechaInscripcion.Year);
                if (currentMaxInscriptionNumber > Int32.Parse(escritura.NumeroInscripcion) || currentAñoVigenciaFinal > escritura.FechaInscripcion.Year)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public decimal CalculateSumOfEnajenantesPercentagesDominios(List<EnajenanteClass> enajenantes, Escritura escritura, int updatedDate, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            decimal sumOfEnajenantesPercentage = 0;
            foreach (EnajenanteClass currentEnajenante in enajenantes)
            {
                var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, currentEnajenante.rut, db);
                sumOfEnajenantesPercentage += enajenanteMultipropietario.PorcentajeDerecho;
            }
            return sumOfEnajenantesPercentage;
        }
    }
}