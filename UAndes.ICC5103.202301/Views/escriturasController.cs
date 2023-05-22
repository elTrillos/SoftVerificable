using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Pkcs;
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
        public decimal porcentajeDerecho { get; set; }
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

    public class ValuesChecker
    {
        public bool CheckIfEscrituraValuesAreValid(Escritura escritura)
        {
            if (escritura.NumeroInscripcion == null)
            {
                return false;
            }
            else if (escritura.Manzana == null)
            {
                return false;
            }
            else if (escritura.Predio == null)
            {
                return false;
            }
            else if (escritura.Comuna == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckIfSumOfPercentagesIsValid(Escritura escritura, List<AdquirienteClass> adquirientes)
        {
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfPercentages = 100 - adquirienteVerificator.SumOfPercentages(adquirientes);
            bool adquirientesWithoutAcreditedPercentages = adquirienteVerificator.CheckIfAnyAdquirienteWithoutDeclared(adquirientes);
            if (sumOfPercentages != 0 && !adquirientesWithoutAcreditedPercentages)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public class CreateClasses
    {
        public void CreateMultipropietario(Escritura escritura, AdquirienteClass adquiriente, decimal adquirientePercentage, int currentInscriptionNumber, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
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
        
        public void CreateMultipropietarioWithEnajenante(Escritura escritura, EnajenanteClass enajenante, decimal enajenantePercentage, int currentInscriptionNumber, int currentAñoVigenciaInicial, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            Multipropietario newMultipropietario = new Multipropietario
            {
                Comuna = escritura.Comuna,
                Manzana = escritura.Manzana,
                Predio = escritura.Predio,
                RunRut = enajenante.rut,
                PorcentajeDerecho = enajenantePercentage,
                Fojas = escritura.Fojas,
                AñoInscripcion = escritura.FechaInscripcion.Year,
                NumeroInscripcion = currentInscriptionNumber,
                FechaInscripcion = escritura.FechaInscripcion,
                AñoVigenciaInicial = currentAñoVigenciaInicial,
                AñoVigenciaFinal = currentAñoVigenciaFinal,
            };
            db.Multipropietario.Add(newMultipropietario);
        }

        public void CreateAdquiriente(Escritura escritura, AdquirienteClass adquiriente, decimal adquirientePercentage, InscripcionesBrDbEntities db)
        {
            Adquiriente newAdquiriente = new Adquiriente
            {
                RunRut = adquiriente.rut,
                NumeroAtencion = escritura.NumeroAtencion,
                PorcentajeDerecho = adquirientePercentage,
                DerechoNoAcreditado = adquiriente.porcentajeDerechoNoAcreditado,
            };
            db.Adquiriente.Add(newAdquiriente);
        }

        public void CreateEnajenante(Escritura escritura, EnajenanteClass enajenante, decimal enajenantePercentage, InscripcionesBrDbEntities db)
        {
            Enajenante newEnajenante = new Enajenante
            {
                RunRut = enajenante.rut,
                NumeroAtencion = escritura.NumeroAtencion,
                PorcentajeDerecho = enajenantePercentage,
                DerechoNoAcreditado = enajenante.porcentajeDerechoNoAcreditado
            };

            db.Enajenante.Add(newEnajenante);
        }

        public void CreateAdquirientesAndMultipropietarios(Escritura escritura, List<AdquirienteClass> adquirientes, int nonDeclaredAdquirientes, int updatedDate, int currentAñoVigenciaFinal, InscripcionesBrDbEntities db)
        {
            AdquirienteVerificator adquirienteVerificator = new AdquirienteVerificator();
            decimal sumOfPercentages = 100 - adquirienteVerificator.SumOfPercentages(adquirientes);
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquirienteVerificator.GetAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
                CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, currentAñoVigenciaFinal, db);
            }
        }

        public void CreateAdquirientesAndMultipropietariosForTraspaso(Escritura escritura, List<AdquirienteClass> adquirientes, decimal percentageToSplit, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.porcentajeDerecho* percentageToSplit/100 ;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetMultipropietarioByRut(escritura, updatedDate, adquiriente.rut, db);
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho + adquirientePercentage,db);

                }
                catch
                {
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, 0, db);
                }  
            }
        }
        
        public void CreateAdquirienteAndMultipropietarioForDerechos(Escritura escritura, AdquirienteClass adquiriente, decimal percentageToSplit, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            CreateAdquiriente(escritura, adquiriente, percentageToSplit, db);
            CreateMultipropietario(escritura, adquiriente, percentageToSplit, currentInscriptionNumber, updatedDate, 0, db);
        }

        public void CreateAdquirienteAndMultipropietarioForDominios(Escritura escritura, List<AdquirienteClass> adquirientes , decimal percentageMultiplicator, int updatedDate, InscripcionesBrDbEntities db)
        {
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();

            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                decimal adquirientePercentage = adquiriente.porcentajeDerecho * percentageMultiplicator;
                try
                {
                    Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, adquiriente.rut, db);
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    multipropietariosModifications.UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho + adquirientePercentage, db);
                }

                catch
                {
                    CreateAdquiriente(escritura, adquiriente, adquirientePercentage, db);
                    CreateMultipropietario(escritura, adquiriente, adquirientePercentage, currentInscriptionNumber, updatedDate, 0, db);
                }
            }
        }

        public void CreateMultipleEnajenantes(Escritura escritura, List<EnajenanteClass> enajenantes, InscripcionesBrDbEntities db)
        {
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                CreateEnajenante(escritura, enajenante, enajenante.porcentajeDerecho, db);
            }
        }
    }

    public class DatabaseQueries
    {
        public List<Multipropietario> SameYearMultipropietarios(Escritura escritura, int startDate, InscripcionesBrDbEntities db)
        {
            var sameYearMultipropietarios = db.Multipropietario
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.AñoVigenciaInicial == startDate)
                .ToList();
            return sameYearMultipropietarios;
        }

        public Multipropietario GetMultipropietarioByRut(Escritura escritura, int startDate, string rut, InscripcionesBrDbEntities db)
        {
            var multipropietario = db.Multipropietario
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.AñoVigenciaInicial == startDate)
                .Where(d => d.RunRut == rut)
                .First();
            return multipropietario;
        }

        public Multipropietario GetLatestMultipropietarioByRut(Escritura escritura, int startDate, string rut, InscripcionesBrDbEntities db)
        {
            var multipropietario = db.Multipropietario
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.AñoVigenciaInicial <= startDate)
                .Where(d => d.RunRut == rut)
                .OrderByDescending(e => e.AñoVigenciaInicial)
                .OrderByDescending(f => f.NumeroInscripcion)
                .First();
            return multipropietario;
        }

        public List<Multipropietario> PriorListMultipropietarios(Escritura escritura, InscripcionesBrDbEntities db)
        {
            var priorMultipropietarios = db.Multipropietario
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.AñoVigenciaFinal == 0)
                .Where(e => e.AñoVigenciaInicial < escritura.FechaInscripcion.Year)
                .ToList();
            return priorMultipropietarios;
        }

        public List<Multipropietario> GetAllValidMultipropietarios(Escritura escritura, int date, InscripcionesBrDbEntities db)
        {
            var priorMultipropietarios = db.Multipropietario
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.AñoVigenciaFinal == 0)
                .Where(e => e.AñoVigenciaInicial <= date)
                .ToList();
            return priorMultipropietarios;
        }
    }

    public class MultipropietariosModifications
    {
        private const int InvalidValue = -1;

        public int EliminateCurrentYearMultipropietarios(Escritura escritura, int currentYear, InscripcionesBrDbEntities db)
        {
            //Returns AñoVigenciaFinal if the NumeroInscripcion of the new escrituras is more than the value of already existing escrituras of the same year
            int currentAñoVigenciaFinal = 0;
            int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var sameYearMultipropietarios = databaseQueries.SameYearMultipropietarios(escritura, currentYear, db);
            if (sameYearMultipropietarios.Count > 0)
            {
                if (sameYearMultipropietarios.First().NumeroInscripcion > currentInscriptionNumber)
                {
                    //if it is less than that, it returns -1
                    return InvalidValue;
                }
                currentAñoVigenciaFinal = sameYearMultipropietarios.First().AñoVigenciaFinal;
                foreach (Multipropietario multipropietario in sameYearMultipropietarios)
                {
                    db.Multipropietario.Remove(multipropietario);
                    db.SaveChanges();
                }
            }
            return currentAñoVigenciaFinal;
        }

        public void UpdateCurrentYearMultipropietarios(Escritura escritura, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var priorMultipropietarios = databaseQueries.PriorListMultipropietarios(escritura,db);
            if (priorMultipropietarios.Count > 0)
            {
                foreach (Multipropietario multipropietario in priorMultipropietarios)
                {
                    multipropietario.AñoVigenciaFinal = currentYear - 1;
                    db.Entry(multipropietario).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateCurrentYearMultipropietario(Multipropietario multipropietario, Escritura escritura, int updatedYear, InscripcionesBrDbEntities db)
        {
            multipropietario.AñoVigenciaFinal = updatedYear;
            db.Entry(multipropietario).State = EntityState.Modified;
            db.SaveChanges();
        }

        public decimal EliminateTranspasoMultipropietarios(Escritura escritura, int currentYear, List<EnajenanteClass> enajenantes, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            decimal sumOfEnajenantesPercentage = 0;
            foreach(EnajenanteClass enajenante in enajenantes)
            {
                try
                {
                    var enajenanteMultipropietario = databaseQueries.GetMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
                    sumOfEnajenantesPercentage += enajenanteMultipropietario.PorcentajeDerecho;
                    db.Multipropietario.Remove(enajenanteMultipropietario);
                }
                catch
                {
                    //System.Diagnostics.Debug.WriteLine("error1");
                    var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
                    sumOfEnajenantesPercentage += enajenanteMultipropietario.PorcentajeDerecho;
                    enajenanteMultipropietario.AñoVigenciaFinal = currentYear - 1;
                    db.Entry(enajenanteMultipropietario).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return sumOfEnajenantesPercentage;
        }

        public void UpdateMultipropietarioPorcentaje(Escritura escritura, EnajenanteClass enajenante,decimal newPercentage,int currentYear,InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            UpdateMultipropietario(enajenanteMultipropietario, enajenanteMultipropietario.PorcentajeDerecho - newPercentage, db);
        }

        public void UpdateMultipropietarioPorcentajeByMultiplication(Escritura escritura, EnajenanteClass enajenante, decimal percentageReason, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            UpdateMultipropietario(enajenanteMultipropietario, enajenanteMultipropietario.PorcentajeDerecho * percentageReason, db);
        }

        public void UpdateMultipropietario(Multipropietario multipropietario,decimal updatedPercentage, InscripcionesBrDbEntities db)
        {
            multipropietario.PorcentajeDerecho = updatedPercentage;
            db.Entry(multipropietario).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void EliminateSingleMultipropietario(Escritura escritura, EnajenanteClass enajenante, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            db.Multipropietario.Remove(enajenanteMultipropietario);
        }

        public void EliminateLatestMultipropietario(Escritura escritura, EnajenanteClass enajenante, int currentYear, InscripcionesBrDbEntities db)
        {
            DatabaseQueries databaseQueries = new DatabaseQueries();
            var enajenanteMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, currentYear, enajenante.rut, db);
            db.Multipropietario.Remove(enajenanteMultipropietario);
        }
    }

    public class AdquirienteVerificator
    {
        private const int CountValue = 1;
        private const int MinimumYear = 2019;

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

        public bool AdquirientesCheckSumOfPercentages(List<AdquirienteClass> adquirientes)
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

        public int NonDeclaredAdquirientesAmount(List<AdquirienteClass> adquirientes)
        {
            int nonDeclaredAdquirientesCount = 0;
            foreach (AdquirienteClass adquiriente in adquirientes)
            {
                if (adquiriente.porcentajeDerechoNoAcreditado == true)
                {
                    nonDeclaredAdquirientesCount += CountValue;
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

        public decimal GetAdquirientePercentage(AdquirienteClass adquiriente, int nonDeclaredAdquirientes, decimal sumOfPercentages)
        {
            decimal adquirientePercentage = 0;
            if (adquiriente.porcentajeDerechoNoAcreditado == true)
            {
                adquirientePercentage = PostDeclarationAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
            }
            else
            {
                adquirientePercentage = adquiriente.porcentajeDerecho;
            }
            return adquirientePercentage;
        }

        public int GetUpdatedDate(Escritura escritura)
        {
            int minimumAño = MinimumYear;
            int updatedDate = escritura.FechaInscripcion.Year;
            if (updatedDate < minimumAño)
            {
                updatedDate = minimumAño;
            }
            return updatedDate;
        }
    }

    public class EnajenanteVerificator
    {
        private const int CountValue = 1;
        private const int MinimumYear = 2019;

        public bool CheckNonDeclaredEnajenantes(List<EnajenanteClass> enajenantes)
        {
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                if (enajenante.porcentajeDerechoNoAcreditado == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool EnajenantesCheckSumOfPercentages(List<EnajenanteClass> enajenantes)
        {
            decimal totalPercentage = 0;
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                totalPercentage += enajenante.porcentajeDerecho;
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

        public decimal SumOfPercentages(List<EnajenanteClass> enajenantes)
        {
            decimal totalPercentage = 0;
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                totalPercentage += enajenante.porcentajeDerecho;
            }
            return totalPercentage;
        }

        public int NonDeclaredEnajenantesAmount(List<EnajenanteClass> enajenantes)
        {
            int nonDeclaredEnajenantesCount = 0;
            foreach (EnajenanteClass enajenante in enajenantes)
            {
                if (enajenante.porcentajeDerechoNoAcreditado == true)
                {
                    nonDeclaredEnajenantesCount += CountValue;
                }
            }
            return nonDeclaredEnajenantesCount;
        }

        public decimal PostDeclarationEnajenantePercentage(EnajenanteClass enajenante, int amountOfEnajenantes, decimal percentageSum)
        {
            int truncateValue = 100;
            decimal extraPercentage = Decimal.Truncate(truncateValue * percentageSum / amountOfEnajenantes) / truncateValue;
            return enajenante.porcentajeDerecho + extraPercentage;
        }

        public decimal GetEnajenantePercentage(EnajenanteClass enajenante, int nonDeclaredEnajenantes, decimal sumOfPercentages)
        {
            decimal adquirientePercentage = 0;
            if (enajenante.porcentajeDerechoNoAcreditado == true)
            {
                adquirientePercentage = PostDeclarationEnajenantePercentage(enajenante, nonDeclaredEnajenantes, sumOfPercentages);
            }
            else
            {
                adquirientePercentage = enajenante.porcentajeDerecho;
            }
            return adquirientePercentage;
        }

        public int GetUpdatedDate(Escritura escritura)
        {
            int updatedDate = escritura.FechaInscripcion.Year;
            if (updatedDate < MinimumYear)
            {
                updatedDate = MinimumYear;
            }
            return updatedDate;
        }
    }

    public class EscriturasController : Controller
    {
        private const string EmptyInput = "";
        private const int InvalidValue = -1;

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
        public ActionResult Create([Bind(Include = "NumeroAtencion,CNE,Comuna,Manzana,Predio,Fojas,FechaInscripcion,NumeroInscripcion")] Escritura escritura, string receivedEnajenantes, string receivedAdquirientes)
        {
            if (ModelState.IsValid)
            {
                ValuesChecker valuesChecker = new ValuesChecker();
                CreateClasses createClasses = new CreateClasses();
                MultipropietariosModifications multipropietariosModifications = new MultipropietariosModifications();

                if (!valuesChecker.CheckIfEscrituraValuesAreValid(escritura)){
                    return RedirectToAction("Create");
                }
                db.Escritura.Add(escritura);

                switch (escritura.CNE)
                {
                    case "regularizacion":
                        if (receivedAdquirientes != EmptyInput)
                        {
                            AdquirienteVerificator AdquirienteVerificator = new AdquirienteVerificator();
                            List<AdquirienteClass> adquirientes = JsonConvert.DeserializeObject<List<AdquirienteClass>>(receivedAdquirientes);
                            int nonDeclaredAdquirientes = AdquirienteVerificator.NonDeclaredAdquirientesAmount(adquirientes);

                            if (!valuesChecker.CheckIfSumOfPercentagesIsValid(escritura, adquirientes))
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

                            createClasses.CreateAdquirientesAndMultipropietarios(escritura, adquirientes, nonDeclaredAdquirientes, updatedDate, currentAñoVigenciaFinal, db);
                        }
                        break;
                    case "compraventa":
                        
                        if (receivedEnajenantes != EmptyInput && receivedAdquirientes != EmptyInput)
                        {
                            EnajenanteVerificator EnajenanteVerificator = new EnajenanteVerificator();
                            List<EnajenanteClass> enajenantes = JsonConvert.DeserializeObject<List<EnajenanteClass>>(receivedEnajenantes);
                            AdquirienteVerificator AdquirienteVerificator = new AdquirienteVerificator();
                            List<AdquirienteClass> adquirientes = JsonConvert.DeserializeObject<List<AdquirienteClass>>(receivedAdquirientes);
                            DatabaseQueries databaseQueries = new DatabaseQueries();
                            decimal sumOfEnajenantesPercentage = 0;
                            decimal adquirientePercentage = 0;
                            decimal sumOfPercentage = AdquirienteVerificator.SumOfPercentages(adquirientes);
                            int updatedDate = EnajenanteVerificator.GetUpdatedDate(escritura);
                            createClasses.CreateMultipleEnajenantes(escritura, enajenantes, db);
                            if (sumOfPercentage == 100)
                            {
                                sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, db);
                                createClasses.CreateAdquirientesAndMultipropietariosForTraspaso(escritura, adquirientes, sumOfEnajenantesPercentage, updatedDate, db);
                            }
                            else if (sumOfPercentage !=100 && enajenantes.Count() == 1 && adquirientes.Count()==1)
                            {
                                EnajenanteClass enajenante = enajenantes[0];
                                AdquirienteClass adquiriente = adquirientes[0];
                                
                                try
                                {
                                    Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db);
                                    decimal enajenanteTotalPercentage = multipropietario.PorcentajeDerecho;
                                    sumOfEnajenantesPercentage = enajenanteTotalPercentage * (100-enajenante.porcentajeDerecho) / 100;
                                    adquirientePercentage = adquiriente.porcentajeDerecho * enajenanteTotalPercentage/100;
                                    //multipropietariosModifications.UpdateMultipropietarioPorcentaje(escritura, enajenante, sumOfEnajenantesPercentage, updatedDate, db);
                                    multipropietariosModifications.UpdateMultipropietario(multipropietario, sumOfEnajenantesPercentage, db);
                                }
                                catch
                                {
                                    Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db);
                                    decimal enajenanteTotalPercentage = multipropietario.PorcentajeDerecho;
                                    adquirientePercentage = adquiriente.porcentajeDerecho * enajenanteTotalPercentage/100;
                                    sumOfEnajenantesPercentage = enajenanteTotalPercentage * (100 - enajenante.porcentajeDerecho) / 100;
                                    multipropietariosModifications.UpdateCurrentYearMultipropietario(multipropietario, escritura, updatedDate-1, db);
                                    createClasses.CreateMultipropietarioWithEnajenante(escritura, enajenante, sumOfEnajenantesPercentage, multipropietario.NumeroInscripcion, updatedDate, 0, db);
                                }
                                
                                createClasses.CreateAdquirienteAndMultipropietarioForDerechos(escritura, adquiriente, adquirientePercentage, updatedDate, db);
                                db.SaveChanges();
                                List<Multipropietario> multipropietariosToUpdate = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                decimal sumOfEndPercentages = 0;
                                foreach (Multipropietario multipropietarioToCheck in multipropietariosToUpdate)
                                {
                                    System.Diagnostics.Debug.WriteLine("Checkshit");
                                    System.Diagnostics.Debug.WriteLine(multipropietarioToCheck.PorcentajeDerecho);
                                    sumOfEndPercentages += multipropietarioToCheck.PorcentajeDerecho;
                                }
                                System.Diagnostics.Debug.WriteLine(sumOfEndPercentages);
                                foreach (Multipropietario multipropietarioToUpdate in multipropietariosToUpdate)
                                {
                                    System.Diagnostics.Debug.WriteLine("Checkshit");
                                    System.Diagnostics.Debug.WriteLine(multipropietarioToUpdate.PorcentajeDerecho);
                                    decimal updatedPercentage = 100 * multipropietarioToUpdate.PorcentajeDerecho / sumOfEndPercentages;
                                    multipropietariosModifications.UpdateMultipropietario(multipropietarioToUpdate, updatedPercentage, db);
                                }
                            }
                            else
                            {
                                sumOfEnajenantesPercentage = multipropietariosModifications.EliminateTranspasoMultipropietarios(escritura, updatedDate, enajenantes, db);
                                decimal sumOfAdquirientesPercentages = AdquirienteVerificator.SumOfPercentages(adquirientes);
                                decimal porcentajeMultiplicator = 1;
                                if (sumOfAdquirientesPercentages > sumOfEnajenantesPercentage)
                                {
                                    porcentajeMultiplicator /= (sumOfAdquirientesPercentages - sumOfEnajenantesPercentage + 100) / 100;
                                    List<Multipropietario> allMultipropietarios = databaseQueries.GetAllValidMultipropietarios(escritura, updatedDate, db);
                                    List<string> usedRuts = new List<string>();
                                    foreach (EnajenanteClass enajenante in enajenantes)
                                    {
                                        usedRuts.Add(enajenante.rut);
                                    }
                                    foreach (Multipropietario multipropietario in allMultipropietarios)
                                    {
                                        System.Diagnostics.Debug.WriteLine("rut");
                                        System.Diagnostics.Debug.WriteLine(multipropietario.RunRut);
                                        //if (!usedRuts.Contains(multipropietario.RunRut))
                                        //{
                                        System.Diagnostics.Debug.WriteLine("yes");
                                        System.Diagnostics.Debug.WriteLine(multipropietario.RunRut);
                                        multipropietariosModifications.UpdateMultipropietario(multipropietario, multipropietario.PorcentajeDerecho * porcentajeMultiplicator, db);
                                        //}
                                    }
                                }
                                db.SaveChanges();
                                foreach (EnajenanteClass enajenante in enajenantes)
                                {
                                    //System.Diagnostics.Debug.WriteLine("asdwwe");
                                    Multipropietario multipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db);
                                    System.Diagnostics.Debug.WriteLine(multipropietario.PorcentajeDerecho);
                                    decimal realEnajenantePercentage = 0;
                                    try
                                    {
                                        realEnajenantePercentage = databaseQueries.GetMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db).PorcentajeDerecho;
                                        System.Diagnostics.Debug.WriteLine(realEnajenantePercentage);
                                    }
                                    catch
                                    {
                                        realEnajenantePercentage = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db).PorcentajeDerecho;
                                    }
                                    int currentInscriptionNumber = Int32.Parse(escritura.NumeroInscripcion);
                                    if (realEnajenantePercentage/ porcentajeMultiplicator < enajenante.porcentajeDerecho)
                                    {
                                        Multipropietario lastMultipropietario = databaseQueries.GetLatestMultipropietarioByRut(escritura, updatedDate, enajenante.rut, db);
                                        multipropietariosModifications.EliminateLatestMultipropietario(escritura, enajenante, updatedDate, db);
                                        createClasses.CreateMultipropietarioWithEnajenante(escritura, enajenante, sumOfEnajenantesPercentage, currentInscriptionNumber, lastMultipropietario.AñoVigenciaInicial,updatedDate-1 , db);
                                        db.Multipropietario.Remove(lastMultipropietario);

                                    }
                                    else
                                    {
                                        //multipropietariosModifications.UpdateMultipropietarioPorcentaje(escritura, enajenante, enajenante.porcentajeDerecho * porcentajeMultiplicator, updatedDate, db);
                                        multipropietariosModifications.UpdateCurrentYearMultipropietario(multipropietario, escritura, updatedDate - 1, db);
                                        decimal multipropietarioPercentage = (realEnajenantePercentage - enajenante.porcentajeDerecho) * porcentajeMultiplicator;
                                        createClasses.CreateMultipropietarioWithEnajenante(escritura, enajenante, multipropietarioPercentage, currentInscriptionNumber, updatedDate, 0, db);
                                    }
                                }
                                createClasses.CreateAdquirienteAndMultipropietarioForDominios(escritura, adquirientes, porcentajeMultiplicator, updatedDate, db);
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
