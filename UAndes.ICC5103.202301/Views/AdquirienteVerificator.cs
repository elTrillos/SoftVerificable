﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class AdquirienteVerificator
    {
        private const int CountValue = 1;
        private const int MinimumYear = 2019;

        public bool CheckIfAnyAdquirienteWithoutDeclared(List<LocalAdquiriente> adquirientes)
        {
            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                if (adquiriente.PorcentajeDerechoNoAcreditado == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AdquirientesCheckSumOfPercentages(List<LocalAdquiriente> adquirientes)
        {
            decimal totalPercentage = 0;
            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                totalPercentage += adquiriente.PorcentajeDerecho;
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

        public decimal SumOfPercentages(List<LocalAdquiriente> adquirientes)
        {
            decimal totalPercentage = 0;
            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                totalPercentage += adquiriente.PorcentajeDerecho;
            }
            return totalPercentage;
        }

        public int NonDeclaredAdquirientesAmount(List<LocalAdquiriente> adquirientes)
        {
            int nonDeclaredAdquirientesCount = 0;
            foreach (LocalAdquiriente adquiriente in adquirientes)
            {
                if (adquiriente.PorcentajeDerechoNoAcreditado == true)
                {
                    nonDeclaredAdquirientesCount += CountValue;
                }
            }
            return nonDeclaredAdquirientesCount;
        }

        public decimal PostDeclarationAdquirientePercentage(LocalAdquiriente adquiriente, int amountOfAdquirientes, decimal percentageSum)
        {
            int truncateValue = 100;
            decimal extraPercentage = Decimal.Truncate(truncateValue * percentageSum / amountOfAdquirientes) / truncateValue;
            return adquiriente.PorcentajeDerecho + extraPercentage;
        }

        public decimal GetAdquirientePercentage(LocalAdquiriente adquiriente, int nonDeclaredAdquirientes, decimal sumOfPercentages)
        {
            decimal adquirientePercentage;
            if (adquiriente.PorcentajeDerechoNoAcreditado == true)
            {
                adquirientePercentage = PostDeclarationAdquirientePercentage(adquiriente, nonDeclaredAdquirientes, sumOfPercentages);
            }
            else
            {
                adquirientePercentage = adquiriente.PorcentajeDerecho;
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

}