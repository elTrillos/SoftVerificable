﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using UAndes.ICC5103._202301.Models;
using UAndes.ICC5103._202301.Views;

namespace UAndes.ICC5103._202301.Views
{
    public class EscriturasController : Controller
    {
        private const string EmptyInput = "";
        private const int InvalidValue = -1;
        private const string VigenteStatus = "Vigente";
        private const string NoVigenteStatus = "No Vigente";
        private const string EliminadoStatus = "Eliminado";

        readonly private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: Escrituras
        public ActionResult Index()
        {
            return View(db.Escritura.ToList());
        }

        // GET: Escrituras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Escritura escritura = db.Escritura.Find(id);
            if (escritura == null)
            {
                return HttpNotFound();
            }
            List<Enajenante> enajenantesActuales = db.Enajenante
                    .Where(b => b.NumeroAtencion == id)
                    .ToList();
            List<Adquiriente> adquirientesActuales = db.Adquiriente
                    .Where(c => c.NumeroAtencion == id)
                    .ToList();

            ViewBag.EnajenantesActuales = enajenantesActuales;
            ViewBag.AdquirientesActuales = adquirientesActuales;
            return View(escritura);
        }

        // GET: Escrituras/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Escrituras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NumeroAtencion,CNE,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumeroInscripcion,Estado")] Escritura escritura, string receivedEnajenantes, string receivedAdquirientes)
        {
            if (ModelState.IsValid)
            {
                escritura.Estado = VigenteStatus;
                ValuesChecker valuesChecker = new ValuesChecker();
                CreationOperations creationOperations = new CreationOperations();
                MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();
                const string regularizacion = "regularizacion";
                const string compraventa = "compraventa";
                if (!valuesChecker.CheckIfEscrituraValuesAreValid(escritura))
                {
                    return RedirectToAction("Create");
                }
                db.Escritura.Add(escritura);

                switch (escritura.CNE)
                {
                    case regularizacion:
                        if (receivedAdquirientes != EmptyInput)
                        {
                            AdquirienteVerificator AdquirienteVerificator = new AdquirienteVerificator();
                            List<LocalAdquiriente> adquirientes = JsonConvert.DeserializeObject<List<LocalAdquiriente>>(receivedAdquirientes);
                            int nonDeclaredAdquirientes = AdquirienteVerificator.NonDeclaredAdquirientesAmount(adquirientes);

                            if (!valuesChecker.CheckIfSumOfPercentagesIsValid(adquirientes))
                            {
                                return RedirectToAction("Create");
                            }
                            int updatedDate = AdquirienteVerificator.GetUpdatedDate(escritura);
                            int currentAñoVigenciaFinal = multipropietariosModifications.EliminateCurrentYearMultipropietarios(escritura, updatedDate, db);
                            if (currentAñoVigenciaFinal == InvalidValue) //if the year returned is -1, it means that there is no need to update or create anything.
                            {
                                return RedirectToAction("Index");
                            }
                            multipropietariosModifications.UpdateCurrentYearMultipropietarios(escritura, updatedDate, db);
                            creationOperations.CreateAdquirientes(escritura, adquirientes, db);
                            creationOperations.CreateMultipropietariosForRegularizacion(escritura, adquirientes, nonDeclaredAdquirientes, updatedDate, currentAñoVigenciaFinal, db);
                        }
                        break;

                    case compraventa:
                        if (receivedEnajenantes != EmptyInput && receivedAdquirientes != EmptyInput)
                        {
                            EnajenanteVerificator enajenanteVerificator = new EnajenanteVerificator();
                            List<LocalEnajenante> enajenantes = JsonConvert.DeserializeObject<List<LocalEnajenante>>(receivedEnajenantes);
                            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
                            List<LocalAdquiriente> adquirientes = JsonConvert.DeserializeObject<List<LocalAdquiriente>>(receivedAdquirientes);
                            DatabaseQueries databaseQueries = new DatabaseQueries();
                            CompraventaOperations compraventaOperations = new CompraventaOperations();
                            decimal sumOfEnajenantesPercentage = 0;
                            decimal sumOfPercentage = adquirienteVerificator.SumOfPercentages(adquirientes);
                            int updatedDate = enajenanteVerificator.GetUpdatedDate(escritura);
                            creationOperations.CreateMultipleEnajenantes(escritura, enajenantes, db);
                            creationOperations.CreateAdquirientes(escritura, adquirientes, db);
                            if (!valuesChecker.CheckIfDataIsValidCompraventa(escritura, updatedDate, db))
                            {
                                return RedirectToAction("Create");
                            }
                            int currentAñoVigenciaFinal = multipropietariosModifications.EliminateCurrentYearMultipropietarios(escritura, updatedDate, db);
                            if (currentAñoVigenciaFinal == InvalidValue) //if the year returned is -1, it means that there is no need to update or create anything.
                            {
                                return RedirectToAction("Index");
                            }

                            if (sumOfPercentage == 100)
                            {
                                bool fantasmaCheck = false;
                                int existingEnajenantesCount = 0;

                                foreach (LocalEnajenante enajenanteToCount in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenanteToCount.Rut, db);
                                        existingEnajenantesCount++;
                                    }
                                    catch
                                    {
                                        fantasmaCheck = true;
                                    }
                                }

                                if (fantasmaCheck)
                                {
                                    List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                    if (existingEnajenantesCount > 0)
                                    {
                                        sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 100, db);
                                        creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, 100-sumOfEnajenantesPercentage, updatedDate, 0, db);
                                        db.SaveChanges();
                                    }
                                    else if(existingEnajenantesCount == 0 && multipropietariosToUpdate.Count() == 0)
                                    {
                                        creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, 100, updatedDate, 0, db);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 100, db);
                                        creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, 100, updatedDate, 0, db);
                                        db.SaveChanges();
                                        multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                        int divisionValue = 2;

                                        foreach (Multipropietario multipropietarioUpdate in multipropietariosToUpdate)
                                        {
                                            multipropietariosModifications.UpdateMultipropietario(multipropietarioUpdate, multipropietarioUpdate.PorcentajeDerecho / divisionValue, db);
                                        }
                                    }
                                }
                                else
                                {
                                    sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 0, db);
                                    creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, sumOfEnajenantesPercentage, updatedDate, 0, db);
                                }
                            }
                            else if (sumOfPercentage < 100 && enajenantes.Count() == 1 && adquirientes.Count() == 1)
                            {
                                compraventaOperations.DerechosHandler(enajenantes, adquirientes, escritura, updatedDate, db);
                                db.SaveChanges();
                                multipropietariosModifications.UpdateMultipropietariosPercentageDerechos(escritura, updatedDate, db);
                            }
                            else
                            {
                                valuesChecker.CalculateSumOfEnajenantesPercentagesDominios(enajenantes, escritura, updatedDate, db);
                                multipropietariosModifications.UpdateMultipropietariosPorcentajesByPercentage(escritura, enajenantes, adquirientes, updatedDate, db);
                                bool fantasmaCheck = false;
                                List<LocalEnajenante> fantasmas = new List<LocalEnajenante>();
                                decimal porcentajeMultiplicator = 1;
                                db.SaveChanges();

                                foreach (LocalEnajenante enajenanteFantasma in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenanteFantasma.Rut, db);
                                    }
                                    catch
                                    {
                                        fantasmas.Add(enajenanteFantasma);
                                        fantasmaCheck = true;
                                    }
                                }

                                foreach (LocalEnajenante enajenante in enajenantes)
                                {
                                    try
                                    {
                                        multipropietariosModifications.UpdateOrCreateMultipropietarioForDominios(escritura, enajenante, porcentajeMultiplicator, sumOfEnajenantesPercentage, updatedDate, db);
                                    }
                                    catch { }
                                }
                                creationOperations.CreateAdquirienteAndMultipropietarioForDominios(escritura, adquirientes, updatedDate, db);
                                db.SaveChanges();

                                List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                decimal sumOfPercentages = 0;

                                foreach (Multipropietario multipropietario in multipropietariosToUpdate)
                                {
                                    sumOfPercentages += multipropietario.PorcentajeDerecho;
                                }
                                if (!fantasmaCheck)
                                {

                                    decimal updatePercentage = sumOfPercentages / 100;
                                    foreach (Multipropietario multipropietarioUpdate in multipropietariosToUpdate)
                                    {
                                        multipropietariosModifications.UpdateMultipropietario(multipropietarioUpdate, multipropietarioUpdate.PorcentajeDerecho / updatePercentage, db);
                                    }
                                }
                                else
                                {
                                    decimal percentageToShare = (100 - sumOfPercentages) / fantasmas.Count;
                                    foreach (LocalEnajenante fantasma in fantasmas)
                                    {
                                        creationOperations.CreateMultipropietarioWithEnajenante(escritura, fantasma, percentageToShare, Int32.Parse(escritura.NumeroInscripcion), updatedDate, 0, db);
                                    }
                                }

                            }
                        }
                        else
                        {
                            return RedirectToAction("Create");
                        }
                        break;
                    default:
                        break;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(escritura);
        }

        // GET: Escrituras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Escritura escritura = db.Escritura.Find(id);
            if (escritura == null)
            {
                return HttpNotFound();
            }
            return View(escritura);
        }

        // POST: Escrituras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NumeroAtencion,CNE,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumeroInscripcion")] Escritura escritura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(escritura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(escritura);
        }

        // GET: Escrituras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Escritura escritura = db.Escritura.Find(id);
            if (escritura == null)
            {
                return HttpNotFound();
            }
            return View(escritura);
        }

        // POST: Escrituras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Escritura escrituraToMod = db.Escritura.Find(id);
            escrituraToMod.Estado = EliminadoStatus;
            db.Entry(escrituraToMod).State = EntityState.Modified;
            db.SaveChanges();
            DatabaseQueries databaseQueries = new DatabaseQueries();
            try
            {
                Escritura escrituraToRecover = databaseQueries.GetSameNumeroInscripcionEscritura(escrituraToMod, escrituraToMod.NumeroInscripcion, db);
                escrituraToRecover.Estado = VigenteStatus;
                db.Entry(escrituraToRecover).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {

            }
            
            List<Multipropietario> allMultipropietarios = databaseQueries.GetAllMultipropietarios(escrituraToMod, db);
            foreach (Multipropietario multipropietario in allMultipropietarios)
            {
                db.Multipropietario.Remove(multipropietario);
                db.SaveChanges();
            }
            List<Escritura> allEscrituras = databaseQueries.GetAllEscrituras(escrituraToMod, db);
            foreach (Escritura testEsc in allEscrituras)
            {
                System.Diagnostics.Debug.WriteLine("xd");
                System.Diagnostics.Debug.WriteLine(testEsc.NumeroInscripcion);
                System.Diagnostics.Debug.WriteLine(testEsc.NumeroAtencion);
                System.Diagnostics.Debug.WriteLine(testEsc.FechaInscripcion.Year);
                System.Diagnostics.Debug.WriteLine(testEsc.Estado);
            }
            foreach (Escritura escritura in allEscrituras)
            {
                ValuesChecker valuesChecker = new ValuesChecker();
                CreationOperations creationOperations = new CreationOperations();
                MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();
                const string regularizacion = "regularizacion";
                const string compraventa = "compraventa";
                if (!valuesChecker.CheckIfEscrituraValuesAreValid(escritura))
                {
                    return RedirectToAction("Create");
                }
                int enajenantesCount = databaseQueries.GetEnajenantesCount(escritura, db);
                int adquirientesCount = databaseQueries.GetAdquirienteCount(escritura, db);
                
                switch (escritura.CNE)
                {
                    case regularizacion:
                        if (adquirientesCount != 0)
                        {
                            AdquirienteVerificator AdquirienteVerificator = new AdquirienteVerificator();
                            List<LocalAdquiriente> adquirientes = new List<LocalAdquiriente>();
                            List<Adquiriente> escrituraAdquirientes = databaseQueries.GetEscrituraAdquirientes(escritura, db);

                            foreach (Adquiriente escrituraToTransform in escrituraAdquirientes)
                            {
                                LocalAdquiriente newAdquiriente = new LocalAdquiriente
                                {
                                    Rut = escrituraToTransform.RunRut,
                                    PorcentajeDerecho = escrituraToTransform.PorcentajeDerecho,
                                    PorcentajeDerechoNoAcreditado = escrituraToTransform.DerechoNoAcreditado
                                };
                                adquirientes.Add(newAdquiriente);
                            }

                            int nonDeclaredAdquirientes = AdquirienteVerificator.NonDeclaredAdquirientesAmount(adquirientes);

                            if (!valuesChecker.CheckIfSumOfPercentagesIsValid(adquirientes))
                            {
                                return RedirectToAction("Create");
                            }
                            int updatedDate = AdquirienteVerificator.GetUpdatedDate(escritura);
                            int currentAñoVigenciaFinal = multipropietariosModifications.EliminateCurrentYearMultipropietarios(escritura, updatedDate, db);
                            if (currentAñoVigenciaFinal == InvalidValue)
                            {
                                return RedirectToAction("Index");
                            }
                            multipropietariosModifications.UpdateCurrentYearMultipropietarios(escritura, updatedDate, db);

                            creationOperations.CreateMultipropietariosForRegularizacion(escritura, adquirientes, nonDeclaredAdquirientes, updatedDate, currentAñoVigenciaFinal, db);
                        }
                        break;

                    case compraventa:
                        if (enajenantesCount != 0 && adquirientesCount != 0)
                        {
                            EnajenanteVerificator enajenanteVerificator = new EnajenanteVerificator();
                            List<LocalEnajenante> enajenantes = new List<LocalEnajenante>();
                            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
                            List<Enajenante> escrituraEnajenantes = databaseQueries.GetEscrituraEnajenantes(escritura, db);

                            foreach (Enajenante enajenanteToTransform in escrituraEnajenantes)
                            {
                                LocalEnajenante newEnajenante = new LocalEnajenante
                                {
                                    Rut = enajenanteToTransform.RunRut,
                                    PorcentajeDerecho = enajenanteToTransform.PorcentajeDerecho,
                                    PorcentajeDerechoNoAcreditado = enajenanteToTransform.DerechoNoAcreditado
                                };
                                enajenantes.Add(newEnajenante);
                            }

                            List<LocalAdquiriente> adquirientes = new List<LocalAdquiriente>();
                            List<Adquiriente> escrituraAdquirientes = databaseQueries.GetEscrituraAdquirientes(escritura, db);

                            foreach (Adquiriente adquirienteToTransform in escrituraAdquirientes)
                            {
                                LocalAdquiriente newAdquiriente = new LocalAdquiriente
                                {
                                    Rut = adquirienteToTransform.RunRut,
                                    PorcentajeDerecho = adquirienteToTransform.PorcentajeDerecho,
                                    PorcentajeDerechoNoAcreditado = adquirienteToTransform.DerechoNoAcreditado
                                };
                                adquirientes.Add(newAdquiriente);
                            }
                            CompraventaOperations compraventaOperations = new CompraventaOperations();
                            decimal sumOfEnajenantesPercentage = 0;
                            decimal sumOfPercentage = adquirienteVerificator.SumOfPercentages(adquirientes);
                            int updatedDate = enajenanteVerificator.GetUpdatedDate(escritura);
                            int currentAñoVigenciaFinal = multipropietariosModifications.EliminateCurrentYearMultipropietarios(escritura, updatedDate, db);
                            if (currentAñoVigenciaFinal == InvalidValue) //if the year returned is -1, it means that there is no need to update or create anything.
                            {
                                return RedirectToAction("Index");
                            }
                            if (!valuesChecker.CheckIfDataIsValidCompraventa(escritura, updatedDate, db))
                            {
                                return RedirectToAction("Create");
                            }

                            if (sumOfPercentage == 100)
                            {
                                bool fantasmaCheck = false;
                                int existingEnajenantesCount = 0;
                                foreach (LocalEnajenante enajenanteToCount in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenanteToCount.Rut, db);
                                        existingEnajenantesCount++;
                                    }
                                    catch
                                    {
                                        fantasmaCheck = true;
                                    }
                                }

                                if (fantasmaCheck)
                                {
                                    List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                    if (existingEnajenantesCount > 0)
                                    {
                                        sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 100, db);
                                        creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, 100 - sumOfEnajenantesPercentage, updatedDate, 0, db);
                                        db.SaveChanges();
                                    }
                                    else if (existingEnajenantesCount == 0 && multipropietariosToUpdate.Count() == 0)
                                    {
                                        creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, 100, updatedDate, 0, db);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 100, db);
                                        creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, 100, updatedDate, 0, db);
                                        db.SaveChanges();
                                        multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                        int divisionValue = 2;
                                        foreach (Multipropietario multipropietarioUpdate in multipropietariosToUpdate)
                                        {
                                            multipropietariosModifications.UpdateMultipropietario(multipropietarioUpdate, multipropietarioUpdate.PorcentajeDerecho / divisionValue, db);
                                        }
                                    }
                                }
                                else
                                {
                                    sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 0, db);
                                    creationOperations.CreateMultipropietariosForTraspaso(escritura, adquirientes, sumOfEnajenantesPercentage, updatedDate, 0, db);
                                }
                            }
                            else if (sumOfPercentage < 100 && enajenantes.Count() == 1 && adquirientes.Count() == 1)
                            {
                                compraventaOperations.DerechosHandler(enajenantes, adquirientes, escritura, updatedDate, db);
                                db.SaveChanges();
                                multipropietariosModifications.UpdateMultipropietariosPercentageDerechos(escritura, updatedDate, db);
                            }
                            else
                            {
                                valuesChecker.CalculateSumOfEnajenantesPercentagesDominios(enajenantes, escritura, updatedDate, db);
                                decimal porcentajeMultiplicator = multipropietariosModifications.UpdateMultipropietariosPorcentajesByPercentage(escritura, enajenantes, adquirientes, updatedDate, db);
                                bool fantasmaCheck = false;
                                List<LocalEnajenante> fantasmas = new List<LocalEnajenante>();
                                porcentajeMultiplicator = 1;
                                db.SaveChanges();

                                foreach (LocalEnajenante enajenanteFantasma in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenanteFantasma.Rut, db);
                                    }
                                    catch
                                    {
                                        fantasmas.Add(enajenanteFantasma);
                                        fantasmaCheck = true;
                                    }
                                }
                                foreach (LocalEnajenante enajenante in enajenantes)
                                {
                                    try
                                    {
                                        multipropietariosModifications.UpdateOrCreateMultipropietarioForDominios(escritura, enajenante, porcentajeMultiplicator, sumOfEnajenantesPercentage, updatedDate, db);
                                    }
                                    catch { }
                                }
                                creationOperations.CreateAdquirienteAndMultipropietarioForDominios(escritura, adquirientes, updatedDate, db);
                                db.SaveChanges();

                                List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                decimal sumOfPercentages = 0;
                                foreach (Multipropietario multipropietario in multipropietariosToUpdate)
                                {
                                    sumOfPercentages += multipropietario.PorcentajeDerecho;
                                }
                                if (!fantasmaCheck)
                                {

                                    decimal updatePercentage = sumOfPercentages / 100;
                                    foreach (Multipropietario multipropietarioUpdate in multipropietariosToUpdate)
                                    {
                                        multipropietariosModifications.UpdateMultipropietario(multipropietarioUpdate, multipropietarioUpdate.PorcentajeDerecho / updatePercentage, db);
                                    }
                                }
                                else
                                {
                                    decimal percentageToShare = (100 - sumOfPercentages) / fantasmas.Count;
                                    foreach (LocalEnajenante fantasma in fantasmas)
                                    {
                                        creationOperations.CreateMultipropietarioWithEnajenante(escritura, fantasma, percentageToShare, Int32.Parse(escritura.NumeroInscripcion), updatedDate, 0, db);
                                    }
                                }
                            }
                        }
                        else
                        {
                            return RedirectToAction("Create");
                        }
                        break;
                    default:
                        break;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
