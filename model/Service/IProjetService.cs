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
    }
}
