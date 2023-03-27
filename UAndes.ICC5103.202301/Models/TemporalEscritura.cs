using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UAndes.ICC5103._202301.Views;

namespace UAndes.ICC5103._202301.Models
{
    public class TemporalEscritura
    {
        public Escritura Escritura { get; set; }
        public List<Adquiriente> Adquirientes { get; set;}
        public List<Enajenante> Enajenantes { get;set; }
        public TemporalEscritura()
        {
            Adquirientes= new List<Adquiriente>() { new Adquiriente()};
            Enajenantes= new List<Enajenante>() { new Enajenante()};
        }
    }
}