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
    public class enajenantesController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: enajenantes
        public ActionResult Index()
        {
            var enajenante = db.enajenante.Include(e => e.escritura);
            return View(enajenante.ToList());
        }

        // GET: enajenantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            enajenante enajenante = db.enajenante.Find(id);
            if (enajenante == null)
            {
                return HttpNotFound();
            }
            return View(enajenante);
        }

        // GET: enajenantes/Create
        public ActionResult Create()
        {
            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne");
            return View();
        }

        // POST: enajenantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,numAtencion,runRut,porcentajeDerecho,derechoNoAcreditado")] enajenante enajenante)
        {
            if (ModelState.IsValid)
            {
                db.enajenante.Add(enajenante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne", enajenante.numAtencion);
            return View(enajenante);
        }

        // GET: enajenantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            enajenante enajenante = db.enajenante.Find(id);
            if (enajenante == null)
            {
                return HttpNotFound();
            }
            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne", enajenante.numAtencion);
            return View(enajenante);
        }

        // POST: enajenantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,numAtencion,runRut,porcentajeDerecho,derechoNoAcreditado")] enajenante enajenante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enajenante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne", enajenante.numAtencion);
            return View(enajenante);
        }

        // GET: enajenantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            enajenante enajenante = db.enajenante.Find(id);
            if (enajenante == null)
            {
                return HttpNotFound();
            }
            return View(enajenante);
        }

        // POST: enajenantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            enajenante enajenante = db.enajenante.Find(id);
            db.enajenante.Remove(enajenante);
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
