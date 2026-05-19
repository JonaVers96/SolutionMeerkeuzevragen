using System;
using System.Collections.Generic;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Model;

namespace MeerkeuzevragenBL.Interfaces 
{
    public interface IMeerkeuzevragenFileProcessor {
        List<Vraag> LeesVragenBestand(string bestandsPad, Onderwerp onderwerp, Moeilijkheid standaardMoeilijkheid);
        void SchrijfToetsNaarBestand(Toets toets, string bestandsPad);
    }
}
