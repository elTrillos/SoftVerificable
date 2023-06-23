using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UAndes.ICC5103._202301.Views
{
    public class LocalAdquiriente
    {
        public int Item { get; set; }
        public string Rut { get; set; }
        public decimal PorcentajeDerecho { get; set; }
        public bool PorcentajeDerechoNoAcreditado { get; set; }
    }
}