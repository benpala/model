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

        public Projet()
        {

        }

        public void CalculerNbHeureSimule()
        {

        }
    }
}
