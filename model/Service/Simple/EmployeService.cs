using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Models;

namespace model.Service.Simple
{
    public class EmployeService : IEmployeService
    {
        private IList<Employe> _Employe;
        public EmployeService()
        { 
            _Employe = new List<Employe>();

            _Employe.Add(new Employe {ID = "00",Prenom = "Pei", Nom = "Li", Photo = "01", Poste = "Chef", Salaire = "40$"});
            _Employe.Add(new Employe { ID = "01", Prenom = "Théo", Nom = "xxx", Photo = "02", Poste = "Chef", Salaire = "50$" });
            _Employe.Add(new Employe { ID = "02", Prenom = "Ben", Nom = "ooo", Photo = "03", Poste = "Chef", Salaire = "60$" });
            _Employe.Add(new Employe { ID = "03", Prenom = "Jac", Nom = "zzz", Photo = "04", Poste = "Chef", Salaire = "70$" });
        }
        public IList<Employe> RetrieveAll()
        {
            return _Employe;
        }
    }
}
