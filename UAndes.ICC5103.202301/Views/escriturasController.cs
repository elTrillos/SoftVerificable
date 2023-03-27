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
        public ActionResult Create([Bind(Include = "NumeroAtencion,CNE,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumeroInscripcion")] Escritura escritura, string emajenantesTableJson, string adquirientesTableJson)
        {
            if (ModelState.IsValid)
            {
                db.Escritura.Add(escritura);
                System.Diagnostics.Debug.WriteLine("test");
                System.Diagnostics.Debug.WriteLine(emajenantesTableJson);
                System.Diagnostics.Debug.WriteLine(adquirientesTableJson);

                var enajenantesJson = JsonConvert.DeserializeObject<List<EnajenanteClass>>(emajenantesTableJson);
                System.Diagnostics.Debug.WriteLine(enajenantesJson);
                foreach (var emajenante in enajenantesJson)
                {
                    System.Diagnostics.Debug.WriteLine(emajenante.item.ToString());
                    decimal porcentajeDerechoDecimal;
                    if(Decimal.TryParse(emajenante.porcentajeDerecho, out porcentajeDerechoDecimal))
                    {
                        Enajenante enajenante = new Enajenante
                        {
                            RunRut = emajenante.rut,
                            NumeroAtencion = escritura.NumeroAtencion,
                            PorcentajeDerecho = porcentajeDerechoDecimal,
                            DerechoNoAcreditado = emajenante.porcentajeDerechoNoAcreditado,

                        };
                        db.Enajenante.Add(enajenante);
                    }
                    
                }

                var adquirientesJson = JsonConvert.DeserializeObject<List<AdquirienteClass>>(adquirientesTableJson);
                System.Diagnostics.Debug.WriteLine(adquirientesJson);
                foreach (var adquirienteVar in adquirientesJson)
                {
                    System.Diagnostics.Debug.WriteLine(adquirienteVar.item.ToString());
                    decimal porcentajeDerechoDecimal;
                    if (Decimal.TryParse(adquirienteVar.porcentajeDerecho, out porcentajeDerechoDecimal))
                    {
                        Adquiriente adquiriente = new Adquiriente
                        {
                            RunRut = adquirienteVar.rut,
                            NumeroAtencion = escritura.NumeroAtencion,
                            PorcentajeDerecho = porcentajeDerechoDecimal,
                            DerechoNoAcreditado = adquirienteVar.porcentajeDerechoNoAcreditado,

                        };
                        db.Adquiriente.Add(adquiriente);

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
