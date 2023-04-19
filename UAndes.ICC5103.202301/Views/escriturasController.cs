using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class EnajenanteClass
    {
        public int item { get; set; }
        public string rut { get; set; }
        public string porcentajeDerecho { get; set; }
        public bool porcentajeDerechoNoAcreditado { get; set; }
    }

    public class AdquirienteClass
    {
        public int item { get; set; }
        public string rut { get; set; }
        public decimal porcentajeDerecho { get; set; }
        public bool porcentajeDerechoNoAcreditado { get; set; }
    }
    public class EnajenantesList
    {
        public List<EnajenanteClass> enajenanteClass { get; set; }
    }

    public class AdquirienteList
    {
        public List<AdquirienteClass> adquirienteClass { get; set; }
    }
    public class AdquirienteVerificator
    {
        public bool CheckIfAnyAdquirienteWithoutDeclared(List<AdquirienteClass> adquirientes)
        {
            foreach(AdquirienteClass adquiriente in adquirientes)
            {
                if (adquiriente.porcentajeDerechoNoAcreditado == true)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckSumOfPercentages(List<AdquirienteClass> adquirientes)
        {
            decimal totalPercentage = 0;
            foreach(AdquirienteClass adquiriente in adquirientes)
            {
                totalPercentage += adquiriente.porcentajeDerecho;
            }
            if (totalPercentage == 100)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public decimal SumOfPercentages(List<AdquirienteClass> adquirientes)
        {
            decimal totalPercentage = 0;
            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                totalPercentage += adquiriente.porcentajeDerecho;
            }
            return totalPercentage;
        }
        public int AmountOfNonDeclaredAdquirientes(List<AdquirienteClass> adquirientes)
        {
            int nonDeclaredAdquirientesCount = 0;
            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                if (adquiriente.porcentajeDerechoNoAcreditado == true)
                {
                    nonDeclaredAdquirientesCount += 1;
                }
            }
            return nonDeclaredAdquirientesCount;
        }
        public decimal PostDeclarationAdquirientePercentage(AdquirienteClass adquiriente,int amountOfAdquirientes, decimal percentageSum)
        {
            int truncateValue = 100;
            decimal extraPercentage = Decimal.Truncate(truncateValue * percentageSum / amountOfAdquirientes)/ truncateValue;
            return adquiriente.porcentajeDerecho + extraPercentage;
        }
    }

    public class EscriturasController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

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
            var enajenantesActuales = db.Enajenante
                    .Where(b => b.NumeroAtencion == id)
                    .ToList();
            var adquirientesActuales = db.Adquiriente
                    .Where(c => c.NumeroAtencion == id)
                    .ToList();
            //System.Diagnostics.Debug.WriteLine(adquirientesActuales);
            foreach (var enajenanteActual in adquirientesActuales)
            {
                System.Diagnostics.Debug.WriteLine(enajenanteActual.ToString());
            }
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
        public ActionResult Create([Bind(Include = "NumeroAtencion,CNE,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumeroInscripcion")] Escritura escritura, string receivedEnajenantes, string receivedAdquirientes)
        {
            if (ModelState.IsValid)
            {
                db.Escritura.Add(escritura);
                string enajenanteOmissions = "regularizacion"
                List<EnajenanteClass> enajenantes;
                if (receivedEnajenantes!="" && escritura.CNE!= enajenanteOmissions)
                {
                    enajenantes = JsonConvert.DeserializeObject<List<EnajenanteClass>>(receivedEnajenantes);
                    foreach (var enajenante in enajenantes)
                    {
                        decimal porcentajeDerechoDecimal;
                        if (Decimal.TryParse(enajenante.porcentajeDerecho, out porcentajeDerechoDecimal))
                        {
                            Enajenante newEnajenante = new Enajenante
                            {
                                RunRut = enajenante.rut,
                                NumeroAtencion = escritura.NumeroAtencion,
                                PorcentajeDerecho = porcentajeDerechoDecimal,
                                DerechoNoAcreditado = enajenante.porcentajeDerechoNoAcreditado,
                            };
                            db.Enajenante.Add(newEnajenante);
                        }
                    }
                }  

                List<AdquirienteClass> adquirientes;
                if (receivedAdquirientes != "")
                {
                    decimal totalPercentage = 100;
                    decimal percentageRestTotal = 0;
                    adquirientes = JsonConvert.DeserializeObject<List<AdquirienteClass>>(receivedAdquirientes);
                    AdquirienteVerificator AdquirienteVerificator = new AdquirienteVerificator();
                    decimal sumOfPercentages = totalPercentage - AdquirienteVerificator.SumOfPercentages(adquirientes);
                    int nonDeclaredAdquirientes = AdquirienteVerificator.AmountOfNonDeclaredAdquirientes(adquirientes);
                    bool adquirientesWithoutAcreditedPercentages = AdquirienteVerificator.CheckIfAnyAdquirienteWithoutDeclared(adquirientes);
                    int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
                    int currentAñoVigenciaFinal = 0;

                    if (sumOfPercentages != percentageRestTotal && !adquirientesWithoutAcreditedPercentages)
                    {
                        System.Diagnostics.Debug.WriteLine(sumOfPercentages);
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    int updatedDate = escritura.FechaInscripcion.Year;
                    if (updatedDate < 2019)
                    {
                        updatedDate = 2019;
                    }

                    var sameYearMultipropietarios = db.Multipropietario
                        .Where(a => a.Comuna == escritura.Comuna)
                        .Where(b => b.Manzana == escritura.Manzana)
                        .Where(c => c.Predio == escritura.Predio)
                        .Where(d => d.AñoVigenciaInicial == updatedDate)
                        .ToList();

                    if (sameYearMultipropietarios.Count > 0)
                    {
                        if (sameYearMultipropietarios.First().NumeroInscripcion > currentInscriptionNumber)
                        {
                            return RedirectToAction("Index");
                        }
                        currentAñoVigenciaFinal = sameYearMultipropietarios.First().AñoVigenciaFinal;
                        foreach (var multipropietario in sameYearMultipropietarios)
                        {
                            db.Multipropietario.Remove(multipropietario);
                            db.SaveChanges();
                        }
                    }

                    var priorMultipropietarios = db.Multipropietario
                        .Where(a => a.Comuna == escritura.Comuna)
                        .Where(b => b.Manzana == escritura.Manzana)
                        .Where(c => c.Predio == escritura.Predio)
                        .Where(d => d.AñoVigenciaFinal == 0)
                        .Where(e => e.AñoVigenciaInicial < escritura.FechaInscripcion.Year)
                        .ToList();

                    if (priorMultipropietarios.Count > 0)
                    {
                        foreach (var multipropietario in priorMultipropietarios)
                        {
                            multipropietario.AñoVigenciaFinal = updatedDate - 1;
                            db.Entry(multipropietario).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    foreach (var adquiriente in adquirientes)
                    {
                        decimal adquirientePercentage = 0;
                        if (adquiriente.porcentajeDerechoNoAcreditado == true)
                        {
                            adquirientePercentage = AdquirienteVerificator.PostDeclarationAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
                        }
                        else
                        {
                            adquirientePercentage = adquiriente.porcentajeDerecho;
                        }
                        Adquiriente newAdquiriente = new Adquiriente
                        {
                            RunRut = adquiriente.rut,
                            NumeroAtencion = escritura.NumeroAtencion,
                            PorcentajeDerecho = adquirientePercentage,
                            DerechoNoAcreditado = adquiriente.porcentajeDerechoNoAcreditado,
                        };
                        db.Adquiriente.Add(newAdquiriente);

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
            Escritura escritura = db.Escritura.Find(id);
            db.Escritura.Remove(escritura);
            db.SaveChanges();
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
