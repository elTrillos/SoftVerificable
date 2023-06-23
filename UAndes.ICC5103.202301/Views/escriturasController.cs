using System;
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
                escritura.Estado = "Vigente";
                ValuesChecker valuesChecker = new ValuesChecker();
                CreateClasses createClasses = new CreateClasses();
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
                            createClasses.CreateAdquirientes(escritura, adquirientes, db);
                            createClasses.CreateMultipropietariosForRegularizacion(escritura, adquirientes, nonDeclaredAdquirientes, updatedDate, currentAñoVigenciaFinal, db);
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
                            createClasses.CreateMultipleEnajenantes(escritura, enajenantes, db);
                            createClasses.CreateAdquirientes(escritura, adquirientes, db);
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
                                foreach (LocalEnajenante enajenanteToCount in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenanteToCount.Rut, db);
                                    }
                                    catch
                                    {
                                        fantasmaCheck = true;
                                    }
                                }
                                System.Diagnostics.Debug.WriteLine("fantasmaCheck");
                                System.Diagnostics.Debug.WriteLine(fantasmaCheck);

                                if (fantasmaCheck)
                                {
                                    List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                    if (multipropietariosToUpdate.Count() > 0)
                                    {
                                        sumOfEnajenantesPercentage = databaseQueries.SumOfAllMultipropietariosPercentage(escritura, db);
                                        multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 100, db);
                                        createClasses.CreateAdquirientesAndMultipropietariosForTraspaso(escritura, adquirientes, 100, updatedDate, 0, db);
                                        db.SaveChanges();
                                        decimal updatePercentage = (100+sumOfEnajenantesPercentage) / 100;
                                        multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                        foreach (Multipropietario multipropietario1 in multipropietariosToUpdate)
                                        {
                                            multipropietariosModifications.UpdateMultipropietario(multipropietario1, multipropietario1.PorcentajeDerecho / updatePercentage, db);
                                        }
                                    }
                                    else
                                    {
                                        createClasses.CreateAdquirientesAndMultipropietariosForTraspaso(escritura, adquirientes, 100, updatedDate, 0, db);
                                    }
                                }
                                else
                                {
                                    sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 0, db);
                                    createClasses.CreateAdquirientesAndMultipropietariosForTraspaso(escritura, adquirientes, sumOfEnajenantesPercentage, updatedDate, 0, db);
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
                                foreach (LocalEnajenante enajenante1 in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante1.Rut, db);
                                    }
                                    catch
                                    {
                                        fantasmas.Add(enajenante1);
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
                                System.Diagnostics.Debug.WriteLine("poggie woggie uwu xddd");
                                createClasses.CreateAdquirienteAndMultipropietarioForDominios(escritura, adquirientes, updatedDate, db);
                                db.SaveChanges();

                                List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                decimal sumOfPercentages = 0;
                                foreach (Multipropietario multipropietario in multipropietariosToUpdate)
                                {
                                    System.Diagnostics.Debug.WriteLine(multipropietario.PorcentajeDerecho);
                                    sumOfPercentages += multipropietario.PorcentajeDerecho;
                                }
                                if (!fantasmaCheck)
                                {

                                    decimal updatePercentage = sumOfPercentages / 100;
                                    foreach (Multipropietario multipropietario1 in multipropietariosToUpdate)
                                    {
                                        multipropietariosModifications.UpdateMultipropietario(multipropietario1, multipropietario1.PorcentajeDerecho / updatePercentage, db);
                                    }
                                }
                                else
                                {
                                    decimal percentageToShare = (100 - sumOfPercentages) / fantasmas.Count;
                                    foreach (LocalEnajenante fantasma in fantasmas)
                                    {
                                        createClasses.CreateMultipropietarioWithEnajenante(escritura, fantasma, percentageToShare, Int32.Parse(escritura.NumeroInscripcion), updatedDate, 0, db);
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
            escrituraToMod.Estado = "No Vigente";
            db.Entry(escrituraToMod).State = EntityState.Modified;
            db.SaveChanges();
            DatabaseQueries databaseQueries = new DatabaseQueries();
            List<Multipropietario> allMultipropietarios = databaseQueries.GetAllMultipropietarios(escrituraToMod, db);
            foreach (Multipropietario multipropietario in allMultipropietarios)
            {
                db.Multipropietario.Remove(multipropietario);
                db.SaveChanges();
            }
            List<Escritura> allEscrituras = databaseQueries.GetAllEscrituras(escrituraToMod, db);
            foreach (Escritura escritura in allEscrituras)
            {
                ValuesChecker valuesChecker = new ValuesChecker();
                CreateClasses createClasses = new CreateClasses();
                MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();
                const string regularizacion = "regularizacion";
                const string compraventa = "compraventa";
                if (!valuesChecker.CheckIfEscrituraValuesAreValid(escritura))
                {
                    return RedirectToAction("Create");
                }
                int enajenantesCount = databaseQueries.EnajenantesCount(escritura, db);
                int adquirientesCount = databaseQueries.AdquirienteCount(escritura, db);
                
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

                            createClasses.CreateMultipropietariosForRegularizacion(escritura, adquirientes, nonDeclaredAdquirientes, updatedDate, currentAñoVigenciaFinal, db);
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
                                foreach (LocalEnajenante enajenante1 in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante1.Rut, db);
                                    }
                                    catch
                                    {
                                        fantasmaCheck = true;
                                    }
                                }

                                if (fantasmaCheck)
                                {
                                    List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                    if (multipropietariosToUpdate.Count() > 0)
                                    {
                                        foreach (Multipropietario multipropietario in multipropietariosToUpdate)
                                        {
                                            multipropietario.PorcentajeDerecho /= multipropietariosToUpdate.Count();
                                            db.SaveChanges();
                                        }
                                        sumOfEnajenantesPercentage = 100;
                                        multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, sumOfEnajenantesPercentage, db);
                                        createClasses.CreateAdquirientesAndMultipropietariosForTraspaso(escritura, adquirientes, sumOfEnajenantesPercentage, updatedDate, sumOfEnajenantesPercentage, db);
                                    }
                                    else
                                    {
                                        sumOfEnajenantesPercentage = 100;
                                        createClasses.CreateAdquirientesAndMultipropietariosForTraspaso(escritura, adquirientes, sumOfEnajenantesPercentage, updatedDate, sumOfEnajenantesPercentage, db);
                                    }
                                }
                                else
                                {
                                    sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, 0, db);
                                    createClasses.CreateAdquirientesAndMultipropietariosForTraspaso(escritura, adquirientes, sumOfEnajenantesPercentage, updatedDate, 0, db);
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
                                foreach (LocalEnajenante enajenante1 in enajenantes)
                                {
                                    try
                                    {
                                        var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante1.Rut, db);
                                    }
                                    catch
                                    {
                                        fantasmas.Add(enajenante1);
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
                                System.Diagnostics.Debug.WriteLine("poggie woggie uwu xddd");
                                createClasses.CreateAdquirienteAndMultipropietarioForDominios(escritura, adquirientes, updatedDate, db);
                                db.SaveChanges();

                                List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                decimal sumOfPercentages = 0;
                                foreach (Multipropietario multipropietario in multipropietariosToUpdate)
                                {
                                    System.Diagnostics.Debug.WriteLine(multipropietario.PorcentajeDerecho);
                                    sumOfPercentages += multipropietario.PorcentajeDerecho;
                                }
                                if (!fantasmaCheck)
                                {

                                    decimal updatePercentage = sumOfPercentages / 100;
                                    foreach (Multipropietario multipropietario1 in multipropietariosToUpdate)
                                    {
                                        multipropietariosModifications.UpdateMultipropietario(multipropietario1, multipropietario1.PorcentajeDerecho / updatePercentage, db);
                                    }
                                }
                                else
                                {
                                    decimal percentageToShare = (100 - sumOfPercentages) / fantasmas.Count;
                                    foreach (LocalEnajenante fantasma in fantasmas)
                                    {
                                        createClasses.CreateMultipropietarioWithEnajenante(escritura, fantasma, percentageToShare, Int32.Parse(escritura.NumeroInscripcion), updatedDate, 0, db);
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
