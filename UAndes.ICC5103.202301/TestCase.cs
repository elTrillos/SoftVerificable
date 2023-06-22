using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Views;

namespace UAndes.ICC5103._202301
{
    [TestFixture]
    public class TestCase
    {
        [TestCase]
        public void ValidPercentage()
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
    }
}