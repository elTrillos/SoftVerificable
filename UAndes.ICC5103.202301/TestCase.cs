using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;
using UAndes.ICC5103._202301.Views;
using Moq;
using System.Web.Mvc;
using System.Data.Entity;

namespace UAndes.ICC5103._202301
{
    [TestFixture]
    public class TestCase
    {
        [TestCase]
        public void Test_CheckSumAdquirientesPercetanges_ReturnsCorrectSum()
        {
            AdquirienteVerificator verificator = new AdquirienteVerificator();
            List<LocalAdquiriente> adquirientelist = new List<LocalAdquiriente>
            {
                new LocalAdquiriente
                {
                    Item = 1,
                    Rut = "123456789",
                    PorcentajeDerecho = 70,
                    PorcentajeDerechoNoAcreditado = false
                },
                new LocalAdquiriente
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
        public void Test_CheckifPercentageSum_IsValid()
        {
            ValuesChecker checker = new ValuesChecker();
            List<LocalAdquiriente> adquirientelist = new List<LocalAdquiriente>
            {
                new LocalAdquiriente
                {
                    Item = 1,
                    Rut = "123456789",
                    PorcentajeDerecho = 70,
                    PorcentajeDerechoNoAcreditado = false
                },
                new LocalAdquiriente
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
        public void Test_GetLatestMultipropietarioYear_ReturnsLatestYear()
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
        public void Test_CheckIfEscrituraValuesAreValid_ReturnsFalse()
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
        public void Test_GetLatestInscriptionNumberOfYear()
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
        public void Test_NonDeclaredEnajenantesCount()
        {
            EnajenanteVerificator verificator = new EnajenanteVerificator();
            List<LocalEnajenante> enajenantes = new List<LocalEnajenante>
            {
                new LocalEnajenante
                {
                    Item = 1,
                    Rut = "123456789",
                    PorcentajeDerecho = 50,
                    PorcentajeDerechoNoAcreditado = false
                },
                new LocalEnajenante
                {
                    Item = 2,
                    Rut = "987654321",
                    PorcentajeDerecho = 30,
                    PorcentajeDerechoNoAcreditado = true
                },
                new LocalEnajenante
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
        public void Test_GetCorrectEnajenantePercentage()
        {
            EnajenanteVerificator verificator = new EnajenanteVerificator();
            LocalEnajenante enajenante = new LocalEnajenante
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
        public void Test_NonDeclaredAdquirientesAmount()
        {
            var adquirientes = new List<LocalAdquiriente>
            {
                new LocalAdquiriente { Item = 1, Rut = "12345678-9", PorcentajeDerecho = 50, PorcentajeDerechoNoAcreditado = true },
                new LocalAdquiriente { Item = 2, Rut = "98765432-1", PorcentajeDerecho = 30, PorcentajeDerechoNoAcreditado = false },
                new LocalAdquiriente { Item = 3, Rut = "54321678-9", PorcentajeDerecho = 20, PorcentajeDerechoNoAcreditado = true }
            };
            var expectedCount = 2;
            var adquirienteVerificator = new AdquirienteVerificator();
            var result = adquirienteVerificator.NonDeclaredAdquirientesAmount(adquirientes);

            Assert.AreEqual(expectedCount, result);
        }

        [TestCase]
        public void Test_AdquirientesCheckSumOfPercentages()
        {
            var adquirientes = new List<LocalAdquiriente>
            {
                new LocalAdquiriente { Item = 1, Rut = "12345678-9", PorcentajeDerecho = 50, PorcentajeDerechoNoAcreditado = false },
                new LocalAdquiriente { Item = 2, Rut = "98765432-1", PorcentajeDerecho = 30, PorcentajeDerechoNoAcreditado = false },
                new LocalAdquiriente { Item = 3, Rut = "54321678-9", PorcentajeDerecho = 20, PorcentajeDerechoNoAcreditado = false }
            };

            var adquirienteVerificator = new AdquirienteVerificator();
            var result = adquirienteVerificator.AdquirientesCheckSumOfPercentages(adquirientes);

            Assert.IsTrue(result);
        }

        [TestCase]
        public void Test_CheckIfDataIsValidCompraventa()
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

            Assert.IsTrue(result);
        }

        [TestCase]
        public void Test_GetAdquirientePercentage()
        {
            LocalAdquiriente adquiriente = new LocalAdquiriente
            {
                PorcentajeDerecho = 50,
                PorcentajeDerechoNoAcreditado = false
            };
            int nonDeclaredAdquirientes = 0;
            decimal sumOfPercentages = 0;

            AdquirienteVerificator verificator = new AdquirienteVerificator();
            decimal result = verificator.GetAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);

            Assert.AreEqual(50, result);
        }

        [TestCase]
        public void Test_GetUpdatedDateAdquiriente()
        {
            AdquirienteVerificator verificator = new AdquirienteVerificator();
            Escritura escritura = new Escritura
            {
                FechaInscripcion = new System.DateTime(2000, 1, 1)
            };

            int result = verificator.GetUpdatedDate(escritura);
            int minimumAllowedYear = 2019;

            Assert.AreEqual(minimumAllowedYear, result);
        }

        [TestCase]
        public void Test_PostDeclarationAdquirientePercentage_CorrectPercentage()
        {
            AdquirienteVerificator verificator = new AdquirienteVerificator();
            LocalAdquiriente adquiriente = new LocalAdquiriente
            {
                PorcentajeDerecho = 30
            };
            int amountOfAdquirientes = 5;
            decimal percentageSum = 21;

            int truncateValue = 100;
            decimal extraPercentage = Decimal.Truncate(truncateValue * percentageSum / amountOfAdquirientes) / truncateValue;
            decimal expectedPercentage = adquiriente.PorcentajeDerecho + extraPercentage;
            decimal result = verificator.PostDeclarationAdquirientePercentage(adquiriente, amountOfAdquirientes, percentageSum);

            Assert.AreEqual(expectedPercentage, result);
        }

        [TestCase]
        public void Test_CreateActionReturnsViewResult_NotNull()
        {
            var controller = new AdquirientesController();
            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_EditActionReturnsViewResult_NotNull()
        {
            var controller = new AdquirientesController();
            int id = 1; 
            var result = controller.Edit(id) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_DeleteActionReturnsViewResult_NotNull()
        {
            var controller = new AdquirientesController();
            int id = 1; 
            var result = controller.Delete(id) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_IndexActionReturnsViewResult_NotNull()
        {
            var controller = new AdquirientesController();
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_IndexActionReturnsAdquirienteList_NotNull()
        {
            var controller = new AdquirientesController();
            var result = controller.Index() as ViewResult;
            var model = result?.Model as List<Adquiriente>;

            Assert.IsNotNull(model);
        }

        [TestCase]
        public void Test_DetailsActionWithValidIdReturnsViewResult_NotNull()
        {
            var controller = new AdquirientesController();
            int validId = 1; 
            var result = controller.Details(validId) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_IndexActionReturnsViewResultWithEnajenanteList_NotNull()
        {
            var controller = new EnajenantesController();
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<Enajenante>>(result.Model);
        }

        [TestCase]
        public void Test_CreateEnajenanteActionReturnsViewResult_NotNull()
        {
            var controller = new EnajenantesController();
            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_MultipropietariosIndexActionReturnsViewResult_NotNull()
        {
            var controller = new MultipropietariosController();
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_MultipropietariosCreateActionReturnsViewResult_NotNull()
        {
            var controller = new MultipropietariosController();
            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase]
        public void Test_EnajenantesGetUpdatedDateReturnsUpdatedYear_GreaterThanMinimumYear()
        {
            var verificator = new EnajenanteVerificator();
            var escritura = new Escritura
            {
                FechaInscripcion = new System.DateTime(2022, 10, 15)
            };
            int expectedYear = 2022;
            int updatedDate = verificator.GetUpdatedDate(escritura);

            Assert.AreEqual(expectedYear, updatedDate);
        }
    }
}