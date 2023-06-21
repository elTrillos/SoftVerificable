﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
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
}