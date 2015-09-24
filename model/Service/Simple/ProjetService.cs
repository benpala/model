using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Models;

namespace model.Service.Simple
{
    public class ProjetService : IProjetService
    {
       private IList<Projet> _projet; 

       public ProjetService()
       {
           _projet = new List<Projet>();

           _projet.Add(new Projet() { ID = "1000", nom = "Garage INC", dateun = "2015-01-03", datedeux = "2015-12-03" , nbHeures = 200 , abandonne = false});
           _projet.Add(new Projet() { ID = "1001", nom = "Android Power", dateun = "2015-01-03", datedeux = "2015-12-03", nbHeures = 150, abandonne = false });
           _projet.Add(new Projet() { ID = "1002", nom = "Apple Shit", dateun = "2015-01-03", datedeux = "2015-12-03", nbHeures = 100, abandonne = false });
           _projet.Add(new Projet() { ID = "1002", nom = "etc", dateun = "2015-01-03", datedeux = "2015-12-03", nbHeures = 0, abandonne = true});
       }

       public IList<Projet> retrieveAll()
       {
           return _projet;
       }
    }
}
