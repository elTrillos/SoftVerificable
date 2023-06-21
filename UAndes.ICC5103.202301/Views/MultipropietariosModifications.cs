using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class MultipropietariosModifications
    {
        private const int InvalidValue = -1;

        public int EliminateCurrentYearMultipropietarios(Escritura escritura, int currentYear, InscripcionesBrDbEntities db)
        {
            //Returns AñoVigenciaFinal if the NumeroInscripcion of the new escrituras is more than the value of already existing escrituras of the same year
            int currentAñoVigenciaFinal = 0;
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var sameYearMultipropietarios = databaseQueries.SameYearMultipropietarios(escritura, currentYear, db);
            if (sameYearMultipropietarios.Count > 0)
            {
                if (sameYearMultipropietarios.First().NumeroInscripcion > currentInscriptionNumber)
                {
                    //if it is less than that, it returns -1
                    return InvalidValue;
                }
                currentAñoVigenciaFinal = sameYearMultipropietarios.First().AñoVigenciaFinal;
                foreach (Multipropietario multipropietario in sameYearMultipropietarios)
                {
                    db.Multipropietario.Remove(multipropietario);
                    db.SaveChanges();
                }
            }
            return currentAñoVigenciaFinal;
        }

        public void UpdateCurrentYearMultipropietarios(Escritura escritura, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var priorMultipropietarios = databaseQueries.PriorListMultipropietarios(escritura, db);
            if (priorMultipropietarios.Count > 0)
            {
                foreach (Multipropietario multipropietario in priorMultipropietarios)
                {
                    multipropietario.AñoVigenciaFinal = currentYear - 1;
                    db.Entry(multipropietario).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateCurrentYearMultipropietario(Multipropietario multipropietario, Escritura escritura, int updatedYear, InscripcionesBrDbEntities db)
        {
            multipropietario.AñoVigenciaFinal = updatedYear;
            db.Entry(multipropietario).State = EntityState.Modified;
            db.SaveChanges();
        }

        public decimal EliminateTranspasoMultipropietarios(Escritura escritura, int currentYear, List<EnajenanteClass> enajenantes, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            decimal sumOfEnajenantesPercentage = 0;
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                try
                {
                    var enajenanteMultipropietario = databaseQueries.GetMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
                    sumOfEnajenantesPercentage += enajenanteMultipropietario.PorcentajeDerecho;
                    db.Multipropietario.Remove(enajenanteMultipropietario);
                }
                catch
                {
                    var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
                    sumOfEnajenantesPercentage += enajenanteMultipropietario.PorcentajeDerecho;
                    enajenanteMultipropietario.AñoVigenciaFinal = currentYear - 1;
                    db.Entry(enajenanteMultipropietario).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return sumOfEnajenantesPercentage;
        }

        public void UpdateMultipropietarioPorcentaje(Escritura escritura, EnajenanteClass enajenante, decimal newPercentage, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            UpdateMultipropietario(enajenanteMultipropietario, enajenanteMultipropietario.PorcentajeDerecho - newPercentage, db);
        }

        public void UpdateMultipropietarioPorcentajeByMultiplication(Escritura escritura, EnajenanteClass enajenante, decimal percentageReason, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            UpdateMultipropietario(enajenanteMultipropietario, enajenanteMultipropietario.PorcentajeDerecho * percentageReason, db);
        }

        public void UpdateMultipropietario(Multipropietario multipropietario, decimal updatedPercentage, InscripcionesBrDbEntities db)
        {
            multipropietario.PorcentajeDerecho = updatedPercentage;
            db.Entry(multipropietario).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void UpdateMultipropietarioInscriptionNumber(Multipropietario multipropietario, int inscriptionNumber, InscripcionesBrDbEntities db)
        {
            multipropietario.NumeroInscripcion = inscriptionNumber;
            db.Entry(multipropietario).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void UpdateMultipropietarioDate(Multipropietario multipropietario, int date, InscripcionesBrDbEntities db)
        {
            multipropietario.AñoVigenciaInicial = date;
            db.Entry(multipropietario).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void EliminateSingleMultipropietario(Escritura escritura, EnajenanteClass enajenante, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            db.Multipropietario.Remove(enajenanteMultipropietario);
        }

        public void EliminateLatestMultipropietario(Escritura escritura, EnajenanteClass enajenante, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            db.Multipropietario.Remove(enajenanteMultipropietario);
        }

        public decimal UpdateMultipropietariosPorcentajesByPercentage(Escritura escritura, List<EnajenanteClass> enajenantes, List<AdquirienteClass> adquirientes, int updatedDate, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfEnajenantesPercentage = 0;
            foreach (EnajenanteClass currentEnajenante in enajenantes)
            {
                var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, currentEnajenante.rut, db);
                sumOfEnajenantesPercentage += enajenanteMultipropietario.PorcentajeDerecho;
            }
            decimal sumOfAdquirientesPercentages = adquirienteVerificator.SumOfPercentages(adquirientes);
            decimal porcentajeMultiplicator = 1;
            if (sumOfAdquirientesPercentages > sumOfEnajenantesPercentage)
            {
                porcentajeMultiplicator /= (sumOfAdquirientesPercentages - sumOfEnajenantesPercentage + 100) / 100;
                List<Multipropietario> allMultipropietarios = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                List<string> usedRuts = new List<string>();
                foreach (EnajenanteClass enajenante in enajenantes)
                {
                    usedRuts.Add(enajenante.rut);
                }
                foreach (Multipropietario multipropietario in allMultipropietarios)
                {
                    UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho * porcentajeMultiplicator, db);
                }
            }
            return porcentajeMultiplicator;
        }
        public void UpdateMultipropietariosPercentageDerechos(Escritura escritura, int updatedDate, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
            decimal sumOfEndPercentages = 0;
            foreach (Multipropietario multipropietarioToCheck in multipropietariosToUpdate)
            {
                sumOfEndPercentages += multipropietarioToCheck.PorcentajeDerecho;
            }
            foreach (Multipropietario multipropietarioToUpdate in multipropietariosToUpdate)
            {
                decimal updatedPercentage = 100 * multipropietarioToUpdate.PorcentajeDerecho / sumOfEndPercentages;
                UpdateMultipropietario(multipropietarioToUpdate, updatedPercentage, db);
            }
        }

        public void UpdateOrCreateMultipropietarioForDerechos(Multipropietario multipropietario, Escritura escritura, EnajenanteClass enajenante, int updatedDate, decimal sumOfEnajenantesPercentage, InscripcionesBrDbEntities db)
        {
            CreateClasses createClasses = new CreateClasses();
            if (multipropietario.AñoVigenciaInicial == updatedDate)
            {
                UpdateMultipropietario(multipropietario, sumOfEnajenantesPercentage, db);
                UpdateMultipropietarioInscriptionNumber(multipropietario, Int32.Parse(escritura.NumeroInscripcion), db);
            }
            else
            {
                UpdateCurrentYearMultipropietario(multipropietario, escritura, updatedDate, db);
                createClasses.CreateMultipropietarioWithEnajenante(escritura, enajenante, sumOfEnajenantesPercentage, Int32.Parse(escritura.NumeroInscripcion), updatedDate, 0, db);
            }
        }
        public void UpdateOrCreateMultipropietarioForDominios(Escritura escritura, EnajenanteClass enajenante, decimal porcentajeMultiplicator, decimal sumOfEnajenantesPercentage, int updatedDate, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            CreateClasses createClasses = new CreateClasses();
            try
            {
                Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db);
                decimal realEnajenantePercentage = multipropietario.PorcentajeDerecho;
                int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
                if (realEnajenantePercentage / porcentajeMultiplicator < enajenante.porcentajeDerecho)
                {
                    EliminateLatestMultipropietario(escritura, enajenante, updatedDate, db);
                    createClasses.CreateMultipropietarioWithEnajenante(escritura, enajenante, sumOfEnajenantesPercentage, currentInscriptionNumber, multipropietario.AñoVigenciaInicial, updatedDate - 1, db);
                    db.Multipropietario.Remove(multipropietario);
                }
                else
                {
                    if (multipropietario.AñoVigenciaInicial != updatedDate)
                    {
                        UpdateCurrentYearMultipropietario(multipropietario, escritura, updatedDate, db);
                    }
                    else
                    {
                        db.Multipropietario.Remove(multipropietario);
                    }
                    //UpdateCurrentYearMultipropietario(multipropietario, escritura, updatedDate, db);
                    decimal multipropietarioPercentage = (realEnajenantePercentage - enajenante.porcentajeDerecho) * porcentajeMultiplicator;
                    createClasses.CreateMultipropietarioWithEnajenante(escritura, enajenante, multipropietarioPercentage, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
            catch
            {
                return;
            }
        }
    }

}