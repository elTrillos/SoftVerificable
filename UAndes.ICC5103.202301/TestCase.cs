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
                    item = 1,
                    rut = "123456789",
                    porcentajeDerecho = 70,
                    porcentajeDerechoNoAcreditado = false
                },
                new AdquirienteClass
                {
                    item = 2,
                    rut = "987654321",
                    porcentajeDerecho = 30,
                    porcentajeDerechoNoAcreditado = false
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
                    item = 1,
                    rut = "123456789",
                    porcentajeDerecho = 70,
                    porcentajeDerechoNoAcreditado = false
                },
                new AdquirienteClass
                {
                    item = 2,
                    rut = "987654321",
                    porcentajeDerecho = 30,
                    porcentajeDerechoNoAcreditado = false
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
                    item = 1,
                    rut = "123456789",
                    porcentajeDerecho = 50,
                    porcentajeDerechoNoAcreditado = false
                },
                new EnajenanteClass
                {
                    item = 2,
                    rut = "987654321",
                    porcentajeDerecho = 30,
                    porcentajeDerechoNoAcreditado = true
                },
                new EnajenanteClass
                {
                    item = 3,
                    rut = "567891234",
                    porcentajeDerecho = 20,
                    porcentajeDerechoNoAcreditado = true
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
                item = 1,
                rut = "123456789",
                porcentajeDerecho = 25,
                porcentajeDerechoNoAcreditado = true
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
                new AdquirienteClass { item = 1, rut = "12345678-9", porcentajeDerecho = 50, porcentajeDerechoNoAcreditado = true },
                new AdquirienteClass { item = 2, rut = "98765432-1", porcentajeDerecho = 30, porcentajeDerechoNoAcreditado = false },
                new AdquirienteClass { item = 3, rut = "54321678-9", porcentajeDerecho = 20, porcentajeDerechoNoAcreditado = true }
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
                new AdquirienteClass { item = 1, rut = "12345678-9", porcentajeDerecho = 50, porcentajeDerechoNoAcreditado = false },
                new AdquirienteClass { item = 2, rut = "98765432-1", porcentajeDerecho = 30, porcentajeDerechoNoAcreditado = false },
                new AdquirienteClass { item = 3, rut = "54321678-9", porcentajeDerecho = 20, porcentajeDerechoNoAcreditado = false }
            };

            var adquirienteVerificator = new AdquirienteVerificator();
            var result = adquirienteVerificator.AdquirientesCheckSumOfPercentages(adquirientes);

            Assert.IsTrue(result);
        }
    }
}