using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;
using UAndes.ICC5103._202301.Views;

namespace UAndes.ICC5103._202301
{
    [TestFixture]
    public class TestCase
    {
        [TestCase]
        public void CheckSumAdquirientesPercetanges()
        {
            AdquirienteVerificator verificator = new AdquirienteVerificator();
            List<AdquirienteClass> adquirientelist = new List<AdquirienteClass>
            {
                new AdquirienteClass
                {
                    Item = 1,
                    Rut = "123456789",
                    PorcentajeDerecho = 70,
                    PorcentajeDerechoNoAcreditado = false
                },
                new AdquirienteClass
                {
                    Item = 2,
                    Rut = "987654321",
                    PorcentajeDerecho = 30,
                    PorcentajeDerechoNoAcreditado = false
                }
            };

            decimal expectedSum = 100;
            decimal actualSum = verificator.SumOfPercentages(adquirientelist);

            Assert.AreEqual(expectedSum, actualSum);
        }

        [TestCase]
        public void CheckifPercentageSumValid()
        {
            ValuesChecker checker = new ValuesChecker();
            List<AdquirienteClass> adquirientelist = new List<AdquirienteClass>
            {
                new AdquirienteClass
                {
                    Item = 1,
                    Rut = "123456789",
                    PorcentajeDerecho = 70,
                    PorcentajeDerechoNoAcreditado = false
                },
                new AdquirienteClass
                {
                    Item = 2,
                    Rut = "987654321",
                    PorcentajeDerecho = 30,
                    PorcentajeDerechoNoAcreditado = false
                }
            };

            bool expectedValidity = true; 
            bool actualValidity = checker.CheckIfSumOfPercentagesIsValid(adquirientelist);

            Assert.AreEqual(expectedValidity, actualValidity);
        }

        [TestCase]
        public void CheckLatestMultipropietarioYear()
        {
            ValuesChecker checker = new ValuesChecker();
            List<Multipropietario> multipropietarios = new List<Multipropietario>
            {
                new Multipropietario
                {
                    Id = 1,
                    Comuna = "VITACURA",
                    Manzana = "1",
                    Predio = "1",
                    RunRut = "123456789",
                    PorcentajeDerecho = 50,
                    Fojas = 1,
                    AñoInscripcion = 2020,
                    NumeroInscripcion = 1,
                    FechaInscripcion = DateTime.Now,
                    AñoVigenciaInicial = 2019,
                    AñoVigenciaFinal = 2023
                },
                new Multipropietario
                {
                    Id = 2,
                    Comuna = "LAS CONDES",
                    Manzana = "2",
                    Predio = "2",
                    RunRut = "987654321",
                    PorcentajeDerecho = 50,
                    Fojas = 1,
                    AñoInscripcion = 2018,
                    NumeroInscripcion = 1,
                    FechaInscripcion = DateTime.Now,
                    AñoVigenciaInicial = 2020,
                    AñoVigenciaFinal = 2021
                }
            };
            int expectedYear = 2020;
            int actualYear = checker.GetLatestMultipropietarioYear(multipropietarios);

            Assert.AreEqual(expectedYear, actualYear);
        }

        [TestCase]
        public void CheckIfEscrituraValuesAreValid()
        {
            ValuesChecker checker = new ValuesChecker();
            Escritura escritura = new Escritura
            {
                NumeroAtencion = 1,
                CNE = "SampleCNE",
                Comuna = null,
                Manzana = "SampleManzana",
                Predio = "SamplePredio",
                Fojas = 1,
                FechaInscripcion = DateTime.Now,
                NumeroInscripcion = "SampleNumeroInscripcion",
                Estado = "SampleEstado"
            };

            bool expectedResult = false;
            bool actualResult = checker.CheckIfEscrituraValuesAreValid(escritura);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase]
        public void GetLatestInscriptionNumberOfYear()
        {
            ValuesChecker checker = new ValuesChecker();
            List<Multipropietario> multipropietarios = new List<Multipropietario>
            {
                new Multipropietario
                {
                    Id = 1,
                    Comuna = "VITACURA",
                    Manzana = "1",
                    Predio = "1",
                    RunRut = "103394565",
                    PorcentajeDerecho = 50,
                    Fojas = 1,
                    AñoInscripcion = 2020,
                    NumeroInscripcion = 1,
                    FechaInscripcion = DateTime.Now,
                    AñoVigenciaInicial = 2020,
                    AñoVigenciaFinal = 2022
                },
                new Multipropietario
                {
                    Id = 2,
                    Comuna = "LAS CONDES",
                    Manzana = "2",
                    Predio = "2",
                    RunRut = "210339455",
                    PorcentajeDerecho = 50,
                    Fojas = 1,
                    AñoInscripcion = 2020,
                    NumeroInscripcion = 2,
                    FechaInscripcion = DateTime.Now,
                    AñoVigenciaInicial = 2020,
                    AñoVigenciaFinal = 2022
                },
                new Multipropietario
                {
                    Id = 3,
                    Comuna = "PUENTE ALTO",
                    Manzana = "3",
                    Predio = "3",
                    RunRut = "103394557",
                    PorcentajeDerecho = 50,
                    Fojas = 1,
                    AñoInscripcion = 2021,
                    NumeroInscripcion = 3,
                    FechaInscripcion = DateTime.Now,
                    AñoVigenciaInicial = 2021,
                    AñoVigenciaFinal = 2022
                }
            };

            int year = 2020;
            int expectedInscriptionNumber = 2;
            int actualInscriptionNumber = checker.GetLatestInscriptionNumberOfYear(multipropietarios, year);

            Assert.AreEqual(expectedInscriptionNumber, actualInscriptionNumber);
        }

        [TestCase]
        public void NonDeclaredEnajenantesCount()
        {
            EnajenanteVerificator verificator = new EnajenanteVerificator();
            List<EnajenanteClass> enajenantes = new List<EnajenanteClass>
            {
                new EnajenanteClass
                {
                    Item = 1,
                    Rut = "123456789",
                    PorcentajeDerecho = 50,
                    PorcentajeDerechoNoAcreditado = false
                },
                new EnajenanteClass
                {
                    Item = 2,
                    Rut = "987654321",
                    PorcentajeDerecho = 30,
                    PorcentajeDerechoNoAcreditado = true
                },
                new EnajenanteClass
                {
                    Item = 3,
                    Rut = "567891234",
                    PorcentajeDerecho = 20,
                    PorcentajeDerechoNoAcreditado = true
                }
            };

            int expectedCount = 2;
            int actualCount = verificator.NonDeclaredEnajenantesAmount(enajenantes);

            Assert.AreEqual(expectedCount, actualCount);
        }

        [TestCase]
        public void GetEnajenantePercentage()
        {
            EnajenanteVerificator verificator = new EnajenanteVerificator();
            EnajenanteClass enajenante = new EnajenanteClass
            {
                Item = 1,
                Rut = "123456789",
                PorcentajeDerecho = 25,
                PorcentajeDerechoNoAcreditado = true
            };

            int nonDeclaredEnajenantes = 2;
            decimal sumOfPercentages = 100;
            decimal expectedPercentage = 75;
            decimal actualPercentage = verificator.GetEnajenantePercentage(enajenante, nonDeclaredEnajenantes, sumOfPercentages);

            Assert.AreEqual(expectedPercentage, actualPercentage);
        }

        [TestCase]
        public void NonDeclaredAdquirientesAmount()
        {
            var adquirientes = new List<AdquirienteClass>
            {
                new AdquirienteClass { Item = 1, Rut = "12345678-9", PorcentajeDerecho = 50, PorcentajeDerechoNoAcreditado = true },
                new AdquirienteClass { Item = 2, Rut = "98765432-1", PorcentajeDerecho = 30, PorcentajeDerechoNoAcreditado = false },
                new AdquirienteClass { Item = 3, Rut = "54321678-9", PorcentajeDerecho = 20, PorcentajeDerechoNoAcreditado = true }
            };
            var expectedCount = 2;
            var adquirienteVerificator = new AdquirienteVerificator();
            var result = adquirienteVerificator.NonDeclaredAdquirientesAmount(adquirientes);

            Assert.AreEqual(expectedCount, result);
        }

        [TestCase]
        public void AdquirientesCheckSumOfPercentages()
        {
            var adquirientes = new List<AdquirienteClass>
            {
                new AdquirienteClass { Item = 1, Rut = "12345678-9", PorcentajeDerecho = 50, PorcentajeDerechoNoAcreditado = false },
                new AdquirienteClass { Item = 2, Rut = "98765432-1", PorcentajeDerecho = 30, PorcentajeDerechoNoAcreditado = false },
                new AdquirienteClass { Item = 3, Rut = "54321678-9", PorcentajeDerecho = 20, PorcentajeDerechoNoAcreditado = false }
            };

            var adquirienteVerificator = new AdquirienteVerificator();
            var result = adquirienteVerificator.AdquirientesCheckSumOfPercentages(adquirientes);

            Assert.IsTrue(result);
        }

        [TestCase]
        public void CheckIfDataIsValidCompraventa()
        {
            var escritura = new Escritura
            {
                NumeroInscripcion = "1",
                Comuna = "VITACURA",
                Manzana = "5",
                Predio = "5",
                FechaInscripcion = new DateTime(2023, 6, 22)
            };
            var updatedDate = 2023;
            var dbContext = new InscripcionesBrDbEntities();
            var valuesChecker = new ValuesChecker();
            var result = valuesChecker.CheckIfDataIsValidCompraventa(escritura, updatedDate, dbContext);

            Assert.IsFalse(result);
        }
    }
}