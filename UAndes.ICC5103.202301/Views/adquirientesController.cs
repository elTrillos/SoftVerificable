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
    public class adquirientesController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: adquirientes
        public ActionResult Index()
        {
            var adquiriente = db.adquiriente.Include(a => a.escritura);
            return View(adquiriente.ToList());
        }

        // GET: adquirientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            adquiriente adquiriente = db.adquiriente.Find(id);
            if (adquiriente == null)
            {
                return HttpNotFound();
            }
            return View(adquiriente);
        }

        // GET: adquirientes/Create
        public ActionResult Create()
        {
            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne");
            return View();
        }

        // POST: adquirientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,numAtencion,runRut,porcentajeDerecho,derechoNoAcreditado")] adquiriente adquiriente)
        {
            if (ModelState.IsValid)
            {
                db.adquiriente.Add(adquiriente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne", adquiriente.numAtencion);
            return View(adquiriente);
        }

        // GET: adquirientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            adquiriente adquiriente = db.adquiriente.Find(id);
            if (adquiriente == null)
            {
                return HttpNotFound();
            }
            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne", adquiriente.numAtencion);
            return View(adquiriente);
        }

        // POST: adquirientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,numAtencion,runRut,porcentajeDerecho,derechoNoAcreditado")] adquiriente adquiriente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adquiriente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.numAtencion = new SelectList(db.escritura, "numAtencion", "cne", adquiriente.numAtencion);
            return View(adquiriente);
        }

        // GET: adquirientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            adquiriente adquiriente = db.adquiriente.Find(id);
            if (adquiriente == null)
            {
                return HttpNotFound();
            }
            return View(adquiriente);
        }

        // POST: adquirientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            adquiriente adquiriente = db.adquiriente.Find(id);
            db.adquiriente.Remove(adquiriente);
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
