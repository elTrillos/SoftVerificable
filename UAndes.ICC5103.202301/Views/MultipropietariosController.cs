using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class MultipropietariosController : Controller
    {
        readonly private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: Multipropietarios
        public ActionResult Index()
        {
            return View(db.Multipropietario.ToList());
        }

        // GET: Multipropietarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multipropietario multipropietario = db.Multipropietario.Find(id);
            if (multipropietario == null)
            {
                return HttpNotFound();
            }
            return View(multipropietario);
        }

        // GET: Multipropietarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Multipropietarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Comuna,Manzana,Predio,RunRut,PorcentajeDerecho,Fojas,AñoInscripcion,NumeroInscripcion,FechaInscripcion,AñoVigenciaInicial,AñoVigenciaFinal")] Multipropietario multipropietario)
        {
            if (ModelState.IsValid)
            {
                db.Multipropietario.Add(multipropietario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(multipropietario);
        }

        // GET: Multipropietarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multipropietario multipropietario = db.Multipropietario.Find(id);
            if (multipropietario == null)
            {
                return HttpNotFound();
            }
            return View(multipropietario);
        }

        // POST: Multipropietarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Comuna,Manzana,Predio,RunRut,PorcentajeDerecho,Fojas,AñoInscripcion,NumeroInscripcion,FechaInscripcion,AñoVigenciaInicial,AñoVigenciaFinal")] Multipropietario multipropietario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(multipropietario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(multipropietario);
        }

        // GET: Multipropietarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multipropietario multipropietario = db.Multipropietario.Find(id);
            if (multipropietario == null)
            {
                return HttpNotFound();
            }
            return View(multipropietario);
        }

        // POST: Multipropietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Multipropietario multipropietario = db.Multipropietario.Find(id);
            db.Multipropietario.Remove(multipropietario);
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
