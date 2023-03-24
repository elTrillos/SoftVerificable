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
    public class AdquirientesController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: Adquirientes
        public ActionResult Index()
        {
            var adquiriente = db.Adquiriente.Include(a => a.Escritura);
            return View(adquiriente.ToList());
        }

        // GET: Adquirientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adquiriente adquiriente = db.Adquiriente.Find(id);
            if (adquiriente == null)
            {
                return HttpNotFound();
            }
            return View(adquiriente);
        }

        // GET: Adquirientes/Create
        public ActionResult Create()
        {
            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE");
            return View();
        }

        // POST: Adquirientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NumeroAtencion,RunRut,PorcentajeDerecho,DerechoNoAcreditado")] Adquiriente adquiriente)
        {
            if (ModelState.IsValid)
            {
                db.Adquiriente.Add(adquiriente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE", adquiriente.NumeroAtencion);
            return View(adquiriente);
        }

        // GET: Adquirientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adquiriente adquiriente = db.Adquiriente.Find(id);
            if (adquiriente == null)
            {
                return HttpNotFound();
            }
            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE", adquiriente.NumeroAtencion);
            return View(adquiriente);
        }

        // POST: Adquirientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NumeroAtencion,RunRut,PorcentajeDerecho,DerechoNoAcreditado")] Adquiriente adquiriente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adquiriente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NumeroAtencion = new SelectList(db.Escritura, "NumeroAtencion", "CNE", adquiriente.NumeroAtencion);
            return View(adquiriente);
        }

        // GET: Adquirientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adquiriente adquiriente = db.Adquiriente.Find(id);
            if (adquiriente == null)
            {
                return HttpNotFound();
            }
            return View(adquiriente);
        }

        // POST: Adquirientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Adquiriente adquiriente = db.Adquiriente.Find(id);
            db.Adquiriente.Remove(adquiriente);
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
