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
    public class escriturasController : Controller
    {
        private InscripcionesBrDbEntities db = new InscripcionesBrDbEntities();

        // GET: escrituras
        public ActionResult Index()
        {
            return View(db.escritura.ToList());
        }

        // GET: escrituras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            escritura escritura = db.escritura.Find(id);
            if (escritura == null)
            {
                return HttpNotFound();
            }
            return View(escritura);
        }

        // GET: escrituras/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: escrituras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "numAtencion,cne,comuna,manzana,predio,fojas,fechaInscripcion,numInscripcion")] escritura escritura)
        {
            if (ModelState.IsValid)
            {
                db.escritura.Add(escritura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(escritura);
        }

        // GET: escrituras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            escritura escritura = db.escritura.Find(id);
            if (escritura == null)
            {
                return HttpNotFound();
            }
            return View(escritura);
        }

        // POST: escrituras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "numAtencion,cne,comuna,manzana,predio,fojas,fechaInscripcion,numInscripcion")] escritura escritura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(escritura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(escritura);
        }

        // GET: escrituras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            escritura escritura = db.escritura.Find(id);
            if (escritura == null)
            {
                return HttpNotFound();
            }
            return View(escritura);
        }

        // POST: escrituras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            escritura escritura = db.escritura.Find(id);
            db.escritura.Remove(escritura);
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
