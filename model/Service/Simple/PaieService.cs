using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Models;

namespace model.Service
{
    
    public class PaieService : IPaiesService
    {
        private IList<Paie> _paie;
        public PaieService()
        {
            _paie = new List<Paie>();
            _paie.Add(new Paie() { ID = "1000", 
                                   DateGenerationRapport = "2015-01-03", 
                                   UnEmploye = new Employe("Benjamin", "Laverdure"), 
                                   MontantBrute= (float)50.25, 
                                   MontantNet= (float)60.24, 
                                   NombreHeure=3, 
                                   NombreHeureSupp=0,
                                   MontantPrime=(float)0.00, 
                                   MontantIndemites=(float)0.00,
                                   MontantAllocations=(float)0.00,
                                   MontantPourboire=(float)0.00 });
            _paie.Add(new Paie()
                                {
                                    ID = "1000",
                                    DateGenerationRapport = "2015-01-03",
                                    UnEmploye = new Employe("Alain", "Martel"),
                                    MontantBrute = (float)50.25,
                                    MontantNet = (float)60.24,
                                    NombreHeure = 3,
                                    NombreHeureSupp = 0,
                                    MontantPrime = (float)0.00,
                                    MontantIndemites = (float)0.00,
                                    MontantAllocations = (float)0.00,
                                    MontantPourboire = (float)0.00
                                });
           
        }
        public IList<Paie> RetrieveAll()
        {
            return _paie;
        }
    }
}
