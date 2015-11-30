using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.Models
{
    public class Projet
    {
        public string ID { get; set; }
        public string nom { get; set; }
        public string dateun { get; set; }
        public string datedeux { get; set; }
        public int nbHeuresSimule { get; set; }
        public int nbHeuresReel { get; set;}
        public float prixSimulation { get; set;}
        public float prixReel { get; set;}
        public string etat { get; set;}
        public string dateTerminer { get; set;}
        public string dateAbandon { get; set;}
        public int nbEmploye { get; set;}
        public int nbRessourcesEstime { get; set;}
        public int nbHeureTravail { get; set;}
        public int nbQuart { get; set;}
        public bool joursOuvrable { get; set;}
    }
}
