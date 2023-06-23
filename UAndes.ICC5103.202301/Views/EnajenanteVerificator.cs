using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Models;

namespace UAndes.ICC5103._202301.Views
{
    public class EnajenanteVerificator
    {
        private const int CountValue = 1;
        private const int MinimumYear = 2019;

        public bool CheckNonDeclaredEnajenantes(List<LocalEnajenante> enajenantes)
        {
            foreach (LocalEnajenante enajenante in enajenantes)
            {
                if (enajenante.PorcentajeDerechoNoAcreditado == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool EnajenantesCheckSumOfPercentages(List<LocalEnajenante> enajenantes)
        {
            decimal totalPercentage = 0;
            foreach (LocalEnajenante enajenante in enajenantes)
            {
                totalPercentage += enajenante.PorcentajeDerecho;
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

        public decimal SumOfPercentages(List<LocalEnajenante> enajenantes)
        {
            decimal totalPercentage = 0;
            foreach (LocalEnajenante enajenante in enajenantes)
            {
                totalPercentage += enajenante.PorcentajeDerecho;
            }
            return totalPercentage;
        }

        public int NonDeclaredEnajenantesAmount(List<LocalEnajenante> enajenantes)
        {
            int nonDeclaredEnajenantesCount = 0;
            foreach (LocalEnajenante enajenante in enajenantes)
            {
                if (enajenante.PorcentajeDerechoNoAcreditado == true)
                {
                    nonDeclaredEnajenantesCount += CountValue;
                }
            }
            return nonDeclaredEnajenantesCount;
        }

        public decimal PostDeclarationEnajenantePercentage(LocalEnajenante enajenante, int amountOfEnajenantes, decimal percentageSum)
        {
            int truncateValue = 100;
            decimal extraPercentage = Decimal.Truncate(truncateValue * percentageSum / amountOfEnajenantes) / truncateValue;
            return enajenante.PorcentajeDerecho + extraPercentage;
        }

        public decimal GetEnajenantePercentage(LocalEnajenante enajenante, int nonDeclaredEnajenantes, decimal sumOfPercentages)
        {
            decimal adquirientePercentage;
            if (enajenante.PorcentajeDerechoNoAcreditado == true)
            {
                adquirientePercentage = PostDeclarationEnajenantePercentage(enajenante, nonDeclaredEnajenantes, sumOfPercentages);
            }
            else
            {
                adquirientePercentage = enajenante.PorcentajeDerecho;
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

}