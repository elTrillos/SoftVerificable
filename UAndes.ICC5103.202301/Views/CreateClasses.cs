using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class CreateClasses
    {
        public void CreateMultipropietario(Escritura escritura, AdquirienteClass adquiriente, decimal adquirientePercentage, int currentInscriptionNumber, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
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

        public void CreateMultipropietarioWithEnajenante(Escritura escritura, EnajenanteClass enajenante, decimal enajenantePercentage, int currentInscriptionNumber, int currentAñoVigenciaInicial, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
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


        public void CreateAdquiriente(Escritura escritura, AdquirienteClass adquiriente, decimal adquirientePercentage, InscripcionesBrDbEntities db)
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

        public void CreateEnajenante(Escritura escritura, EnajenanteClass enajenante, decimal enajenantePercentage, InscripcionesBrDbEntities db)
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

        public void CreateAdquirientesAndMultipropietarios(Escritura escritura, List<AdquirienteClass> adquirientes, int nonDeclaredAdquirientes, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfPercentages = 100 - adquirienteVerificator.SumOfPercentages(adquirientes);
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquirienteVerificator.GetAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
                CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, currentAñoVigenciaFinal, db);
            }
        }
        public void CreateMultipropietariosForRegularizacion(Escritura escritura, List<AdquirienteClass> adquirientes, int nonDeclaredAdquirientes, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfPercentages = 100 - adquirienteVerificator.SumOfPercentages(adquirientes);
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquirienteVerificator.GetAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
                CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, currentAñoVigenciaFinal, db);
            }
        }
        public void CreateAdquirientes(Escritura escritura, List<AdquirienteClass> adquirientes, InscripcionesBrDbEntities db)
        {
            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                CreateAdquiriente(escritura, adquiriente, adquiriente.PorcentajeDerecho, db);
            }
        }

        public void CreateEnajenantes(Escritura escritura, List<EnajenanteClass> enajenantes, InscripcionesBrDbEntities db)
        {
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                CreateEnajenante(escritura, enajenante, enajenante.PorcentajeDerecho, db);
            }
        }

        public void CreateAdquirientesAndMultipropietariosForTraspaso(Escritura escritura, List<AdquirienteClass> adquirientes, decimal percentageToSplit, int updatedDate, decimal percentageOver, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();
            decimal multipler = 100 / (100 + percentageOver);
            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.PorcentajeDerecho * percentageToSplit / 100;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetMultipropietarioByRut(escritura, updatedDate, adquiriente.Rut, db);
                    //CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, (multipropietario.PorcentajeDerecho + adquirientePercentage)* multipler, db);
                    multipropietariosModifications.UpdateMultipropietarioInscriptionNumber(multipropietario, Int32.Parse(escritura.NumeroInscripcion), db);

                }
                catch
                {
                    //CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage* multipler, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
        }

        public void CreateAdquirienteAndMultipropietarioForDerechos(Escritura escritura, AdquirienteClass adquiriente, decimal percentageToSplit, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            //CreateAdquiriente(escritura, adquiriente, percentageToSplit, db);
            CreateMultipropietario(escritura, adquiriente, percentageToSplit, currentInscriptionNumber, updatedDate, 0, db);
        }

        public void CreateAdquirienteAndMultipropietarioForDominios(Escritura escritura, List<AdquirienteClass> adquirientes, decimal percentageMultiplicator, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.PorcentajeDerecho;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, adquiriente.Rut, db);
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    System.Diagnostics.Debug.WriteLine("okdda");
                    System.Diagnostics.Debug.WriteLine(multipropietario.PorcentajeDerecho + adquirientePercentage);
                    System.Diagnostics.Debug.WriteLine(multipropietario.PorcentajeDerecho);
                    System.Diagnostics.Debug.WriteLine(adquirientePercentage);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho + adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietarioInscriptionNumber(multipropietario, currentInscriptionNumber, db);
                }

                catch
                {
                    //CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
        }

        public void CreateMultipleEnajenantes(Escritura escritura, List<EnajenanteClass> enajenantes, InscripcionesBrDbEntities db)
        {
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                CreateEnajenante(escritura, enajenante, enajenante.PorcentajeDerecho, db);
            }
        }
    }
}