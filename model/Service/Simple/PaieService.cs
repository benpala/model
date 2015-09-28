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

            _paie.Add(new Paie() { ID = "1000", DateGenerationRapport = "2015-01-03", UnEmploye = new Employe("Boby", "chose"), MontantBrute= (float)50.25, MontantNet= (float)60.24  });
           
        }
        public IList<Paie> RetrieveAll()
        {
            return _paie;
        }
    }
}
