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

            _paie.Add(new Paie() { ID = "1000", UnEmploye = new Employe("Boby", "chose"), DateGenerationRapport = "2015-01-03" });
            _paie.Add(new Paie() { ID = "1001", UnEmploye = new Employe("Boby", "chose"), DateGenerationRapport = "2015-01-03" });
            _paie.Add(new Paie() { ID = "1002", UnEmploye = new Employe("Boby", "chose"), DateGenerationRapport = "2015-01-03" });
            _paie.Add(new Paie() { ID = "1002", UnEmploye = new Employe("Boby", "chose"), DateGenerationRapport = "2015-01-03" });
        }
        public IList<Paie> RetrieveAll()
        {
            return _paie;
        }
    }
}
