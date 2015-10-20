using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.Service
{
    public interface ILiaisonProjetService
    {
        IList<ILiaisonProjetService> GetLiaison();
    }
}
