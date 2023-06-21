﻿using System;
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
                RunRut = adquiriente.rut,
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
                RunRut = enajenante.rut,
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
                RunRut = adquiriente.rut,
                NumeroAtencion = escritura.NumeroAtencion,
                PorcentajeDerecho = adquirientePercentage,
                DerechoNoAcreditado = adquiriente.porcentajeDerechoNoAcreditado,
            };
            db.Adquiriente.Add(newAdquiriente);
        }

        public void CreateEnajenante(Escritura escritura, EnajenanteClass enajenante, decimal enajenantePercentage, InscripcionesBrDbEntities db)
        {
            Enajenante newEnajenante = new Enajenante
            {
                RunRut = enajenante.rut,
                NumeroAtencion = escritura.NumeroAtencion,
                PorcentajeDerecho = enajenantePercentage,
                DerechoNoAcreditado = enajenante.porcentajeDerechoNoAcreditado
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

        public void CreateAdquirientesAndMultipropietariosForTraspaso(Escritura escritura, List<AdquirienteClass> adquirientes, decimal percentageToSplit, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.porcentajeDerecho * percentageToSplit / 100;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetMultipropietarioByRut(escritura, updatedDate, adquiriente.rut, db);
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho + adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietarioInscriptionNumber(multipropietario, Int32.Parse(escritura.NumeroInscripcion), db);

                }
                catch
                {
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
        }

        public void CreateAdquirienteAndMultipropietarioForDerechos(Escritura escritura, AdquirienteClass adquiriente, decimal percentageToSplit, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            CreateAdquiriente(escritura, adquiriente, percentageToSplit, db);
            CreateMultipropietario(escritura, adquiriente, percentageToSplit, currentInscriptionNumber, updatedDate, 0, db);
        }

        public void CreateAdquirienteAndMultipropietarioForDominios(Escritura escritura, List<AdquirienteClass> adquirientes, decimal percentageMultiplicator, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.porcentajeDerecho * percentageMultiplicator;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, adquiriente.rut, db);
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho + adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietarioInscriptionNumber(multipropietario, currentInscriptionNumber, db);
                }

                catch
                {
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
        }

        public void CreateMultipleEnajenantes(Escritura escritura, List<EnajenanteClass> enajenantes, InscripcionesBrDbEntities db)
        {
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                CreateEnajenante(escritura, enajenante, enajenante.porcentajeDerecho, db);
            }
        }
    }
}