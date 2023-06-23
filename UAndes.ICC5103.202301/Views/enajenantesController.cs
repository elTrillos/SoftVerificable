using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class EnajenantesController : Controller
    {
        readonly private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: Enajenantes
        public ActionResult Index()
        {
            var enajenante = db.Enajenante.Include(e => e.Escritura);
            return View(enajenante.ToList());
        }

        // GET: Enajenantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enajenante enajenante = db.Enajenante.Find(id);
            if (enajenante == null)
            {
                return HttpNotFound();
            }
            return View(enajenante);
        }

        // GET: Enajenantes/Create
        public ActionResult Create()
        {
            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE");
            return View();
        }

        // POST: Enajenantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,NumeroAtencion,RunRut,PorcentajeDerecho,DerechoNoAcreditado")] Enajenante enajenante)
        {
            if (ModelState.IsValid)
            {
                db.Enajenante.Add(enajenante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE", enajenante.NumeroAtencion);
            return View(enajenante);
        }

        // GET: Enajenantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enajenante enajenante = db.Enajenante.Find(id);
            if (enajenante == null)
            {
                return HttpNotFound();
            }
            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE", enajenante.NumeroAtencion);
            return View(enajenante);
        }

        // POST: Enajenantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,NumeroAtencion,RunRut,PorcentajeDerecho,DerechoNoAcreditado")] Enajenante enajenante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enajenante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE", enajenante.NumeroAtencion);
            return View(enajenante);
        }

        // GET: Enajenantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enajenante enajenante = db.Enajenante.Find(id);
            if (enajenante == null)
            {
                return HttpNotFound();
            }
            return View(enajenante);
        }

        // POST: Enajenantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enajenante enajenante = db.Enajenante.Find(id);
            db.Enajenante.Remove(enajenante);
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
