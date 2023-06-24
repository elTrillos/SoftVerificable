using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class DatabaseQueries
    {
        public List<Multipropietario> GetSameYearMultipropietarios(Escritura escritura, int startDate, InscripcionesBrDbEntities db)
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

        public List<Multipropietario> GetPriorListMultipropietarios(Escritura escritura, InscripcionesBrDbEntities db)
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

        public List<Multipropietario> GetAllMultipropietarios(Escritura escritura, InscripcionesBrDbEntities db)
        {
            var multipropietarios = db.Multipropietario
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .ToList();
            return multipropietarios;
        }

        public List<Escritura> GetAllEscrituras(Escritura escritura, InscripcionesBrDbEntities db)
        {
            var escrituras = db.Escritura
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.Estado== "Vigente")
                .OrderBy(e => e.FechaInscripcion.Year)
                .ThenBy(f => f.NumeroInscripcion)
                .ToList();
            return escrituras;
        }

        public List<Enajenante> GetEscrituraEnajenantes(Escritura escritura, InscripcionesBrDbEntities db)
        {
            var enajenantes = db.Enajenante
                .Where(a => a.NumeroAtencion == escritura.NumeroAtencion)
                .ToList();
            return enajenantes;
        }

        public int GetEnajenantesCount(Escritura escritura, InscripcionesBrDbEntities db)
        {
            int enajenanteCount = db.Enajenante
                .Where(a => a.NumeroAtencion == escritura.NumeroAtencion)
                .Count();
            return enajenanteCount;
        }

        public List<Adquiriente> GetEscrituraAdquirientes(Escritura escritura, InscripcionesBrDbEntities db)
        {
            var adquirientes = db.Adquiriente
                .Where(a => a.NumeroAtencion == escritura.NumeroAtencion)
                .ToList();
            return adquirientes;
        }

        public int GetAdquirienteCount(Escritura escritura, InscripcionesBrDbEntities db)
        {
            int adquirienteCount = db.Adquiriente
                .Where(a => a.NumeroAtencion == escritura.NumeroAtencion)
                .Count();
            return adquirienteCount;
        }

        public decimal GetSumOfAllMultipropietariosPercentage(Escritura escritura, InscripcionesBrDbEntities db)
        {
            decimal sumOfAllMultipropietariosPercentage = db.Multipropietario
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.AñoVigenciaFinal == 0)
                .Sum(e => e.PorcentajeDerecho);
            return sumOfAllMultipropietariosPercentage;

        }

        public Escritura GetEscrituraFromMultipropietario(Multipropietario multipropietario, InscripcionesBrDbEntities db)
        {
            Escritura escritura = db.Escritura
                .Where(a => a.Comuna == multipropietario.Comuna)
                .Where(b => b.Manzana == multipropietario.Manzana)
                .Where(c => c.Predio == multipropietario.Predio)
                .Where(d => d.NumeroInscripcion == multipropietario.NumeroInscripcion.ToString())
                .First();
            return escritura;
        }
        public Escritura GetSameNumeroInscripcionEscritura(Escritura escritura, string numeroInscripcion, InscripcionesBrDbEntities db)
        {
            Escritura sameNumeroEscritura = db.Escritura
                .Where(a => a.Comuna == escritura.Comuna)
                .Where(b => b.Manzana == escritura.Manzana)
                .Where(c => c.Predio == escritura.Predio)
                .Where(d => d.FechaInscripcion.Year == escritura.FechaInscripcion.Year)
                .Where(e => e.NumeroInscripcion == numeroInscripcion)
                .Where(f => f.Estado == "No Vigente")
                .First();
            return sameNumeroEscritura;
        }
    }
}