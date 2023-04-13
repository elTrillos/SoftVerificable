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
        public string porcentajeDerecho { get; set; }
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
                System.Diagnostics.Debug.WriteLine(escritura.Fojas);
                System.Diagnostics.Debug.WriteLine(receivedEnajenantes);
                System.Diagnostics.Debug.WriteLine(escritura.CNE);
                List<EnajenanteClass> enajenantes;
                if (receivedEnajenantes!="" && escritura.CNE!="regularizacion")
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
                    adquirientes = JsonConvert.DeserializeObject<List<AdquirienteClass>>(receivedAdquirientes);
                    foreach (var adquiriente in adquirientes)
                    {
                        decimal porcentajeDerechoDecimal;
                        if (Decimal.TryParse(adquiriente.porcentajeDerecho, out porcentajeDerechoDecimal))
                        {
                            Adquiriente newAdquiriente = new Adquiriente
                            {
                                RunRut = adquiriente.rut,
                                NumeroAtencion = escritura.NumeroAtencion,
                                PorcentajeDerecho = porcentajeDerechoDecimal,
                                DerechoNoAcreditado = adquiriente.porcentajeDerechoNoAcreditado,
                            };
                            db.Adquiriente.Add(newAdquiriente);

                            var multipropietariosAnteriores = db.Multipropietario
                            .Where(a => a.Comuna == escritura.Comuna)
                            .Where(b => b.Manzana == escritura.Manzana)
                            .Where(c => c.Predio == escritura.Predio)
                            .Where(d => d.AñoVigenciaFinal == 0)
                            .ToList();

                            if (multipropietariosAnteriores.Count > 0)
                            {
                                foreach (var enajenanteActual in multipropietariosAnteriores)
                                {
                                    System.Diagnostics.Debug.WriteLine(enajenanteActual.ToString());
                                    enajenanteActual.AñoVigenciaFinal = escritura.FechaInscripcion.Year - 1;
                                    db.Entry(enajenanteActual).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            

                            Multipropietario newMultipropietario = new Multipropietario
                            {
                                Comuna = escritura.Comuna,
                                Manzana = escritura.Manzana,
                                Predio = escritura.Predio,
                                RunRut = adquiriente.rut,
                                PorcentajeDerecho = porcentajeDerechoDecimal,
                                Fojas = escritura.Fojas,
                                AñoInscripcion = escritura.FechaInscripcion.Year,
                                NumeroInscripcion = escritura.NumeroAtencion,
                                FechaInscripcion = escritura.FechaInscripcion,
                                AñoVigenciaInicial = escritura.FechaInscripcion.Year,
                            };
                            db.Multipropietario.Add(newMultipropietario);
                            
                        }
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
