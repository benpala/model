using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Models;

namespace model
{
    public interface IProjetService
    {
        IList<Projet> retrieveAll();
        IList<Projet> retrieveAll(List<string> args,List<string> donnees);
        void create(Projet p);
    }
}
