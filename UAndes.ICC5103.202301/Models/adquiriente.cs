
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace UAndes.ICC5103._202301.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Adquiriente
{

    public int Id { get; set; }

    public int NumeroAtencion { get; set; }

    public string RunRut { get; set; }

    public decimal PorcentajeDerecho { get; set; }

    public bool DerechoNoAcreditado { get; set; }



    public virtual Escritura Escritura { get; set; }

}

}
