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
        public int nbHeures { get; set; }
        public bool abandonne { get; set;}
        public float prixSimulation { get; set;}

        public Projet()
        {

        }
    }
}
