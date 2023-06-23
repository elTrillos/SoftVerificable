using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class CreationOperations
    {
        public void CreateMultipropietario(Escritura escritura, LocalAdquiriente adquiriente, decimal adquirientePercentage, int currentInscriptionNumber, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            Multipropietario newMultipropietario = new Multipropietario
            {
                Comuna = escritura.Comuna,
                Manzana = escritura.Manzana,
                Predio = escritura.Predio,
                RunRut = adquiriente.Rut,
                PorcentajeDerecho = adquirientePercentage,
                Fojas = escritura.Fojas,
                AñoInscripcion = escritura.FechaInscripcion.Year,
                NumeroInscripcion = currentInscriptionNumber,
                FechaInscripcion = escritura.FechaInscripcion,
                AñoVigenciaInicial = updatedDate,
                AñoVigenciaFinal = currentAñoVigenciaFinal,
            };
            db.Multipropietario.Add(newMultipropietario);
        }

        public void CreateMultipropietarioWithEnajenante(Escritura escritura, LocalEnajenante enajenante, decimal enajenantePercentage, int currentInscriptionNumber, int currentAñoVigenciaInicial, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            Multipropietario newMultipropietario = new Multipropietario
            {
                Comuna = escritura.Comuna,
                Manzana = escritura.Manzana,
                Predio = escritura.Predio,
                RunRut = enajenante.Rut,
                PorcentajeDerecho = enajenantePercentage,
                Fojas = escritura.Fojas,
                AñoInscripcion = escritura.FechaInscripcion.Year,
                NumeroInscripcion = currentInscriptionNumber,
                FechaInscripcion = escritura.FechaInscripcion,
                AñoVigenciaInicial = currentAñoVigenciaInicial,
                AñoVigenciaFinal = currentAñoVigenciaFinal,
            };
            db.Multipropietario.Add(newMultipropietario);
        }

        public void CreateAdquiriente(Escritura escritura, LocalAdquiriente adquiriente, decimal adquirientePercentage, InscripcionesBrDbEntities db)
        {
            Adquiriente newAdquiriente = new Adquiriente
            {
                RunRut = adquiriente.Rut,
                NumeroAtencion = escritura.NumeroAtencion,
                PorcentajeDerecho = adquirientePercentage,
                DerechoNoAcreditado = adquiriente.PorcentajeDerechoNoAcreditado,
            };
            db.Adquiriente.Add(newAdquiriente);
        }

        public void CreateEnajenante(Escritura escritura, LocalEnajenante enajenante, decimal enajenantePercentage, InscripcionesBrDbEntities db)
        {
            Enajenante newEnajenante = new Enajenante
            {
                RunRut = enajenante.Rut,
                NumeroAtencion = escritura.NumeroAtencion,
                PorcentajeDerecho = enajenantePercentage,
                DerechoNoAcreditado = enajenante.PorcentajeDerechoNoAcreditado
            };

            db.Enajenante.Add(newEnajenante);
        }

        public void CreateAdquirientesAndMultipropietarios(Escritura escritura, List<LocalAdquiriente> adquirientes, int nonDeclaredAdquirientes, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfPercentages = 100 - adquirienteVerificator.SumOfPercentages(adquirientes);
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);

            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquirienteVerificator.GetAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
                CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, currentAñoVigenciaFinal, db);
            }
        }

        public void CreateMultipropietariosForRegularizacion(Escritura escritura, List<LocalAdquiriente> adquirientes, int nonDeclaredAdquirientes, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfPercentages = 100 - adquirienteVerificator.SumOfPercentages(adquirientes);
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);

            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquirienteVerificator.GetAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
                CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, currentAñoVigenciaFinal, db);
            }
        }

        public void CreateAdquirientes(Escritura escritura, List<LocalAdquiriente> adquirientes, InscripcionesBrDbEntities db)
        {
            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                CreateAdquiriente(escritura, adquiriente, adquiriente.PorcentajeDerecho, db);
            }
        }

        public void CreateEnajenantes(Escritura escritura, List<LocalEnajenante> enajenantes, InscripcionesBrDbEntities db)
        {
            foreach (LocalEnajenante enajenante in enajenantes)
            {
                CreateEnajenante(escritura, enajenante, enajenante.PorcentajeDerecho, db);
            }
        }

        public void CreateMultipropietariosForTraspaso(Escritura escritura, List<LocalAdquiriente> adquirientes, decimal percentageToSplit, int updatedDate, decimal percentageOver, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();
            decimal multipler = 100 / (100 + percentageOver);
            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.PorcentajeDerecho * percentageToSplit / 100;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetMultipropietarioByRut(escritura, updatedDate, adquiriente.Rut, db);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, (multipropietario.PorcentajeDerecho + adquirientePercentage)* multipler, db);
                    multipropietariosModifications.UpdateMultipropietarioInscriptionNumber(multipropietario, Int32.Parse(escritura.NumeroInscripcion), db);

                }
                catch
                {
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage* multipler, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
        }

        public void CreateMultipropietarioForDerechos(Escritura escritura, LocalAdquiriente adquiriente, decimal percentageToSplit, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            CreateMultipropietario(escritura, adquiriente, percentageToSplit, currentInscriptionNumber, updatedDate, 0, db);
        }

        public void CreateAdquirienteAndMultipropietarioForDominios(Escritura escritura, List<LocalAdquiriente> adquirientes, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();

            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.PorcentajeDerecho;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, adquiriente.Rut, db);
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho + adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietarioInscriptionNumber(multipropietario, currentInscriptionNumber, db);
                }

                catch
                {
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
        }

        public void CreateMultipleEnajenantes(Escritura escritura, List<LocalEnajenante> enajenantes, InscripcionesBrDbEntities db)
        {
            foreach (LocalEnajenante enajenante in enajenantes)
            {
                CreateEnajenante(escritura, enajenante, enajenante.PorcentajeDerecho, db);
            }
        }
    }
}