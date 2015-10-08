using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Models;

namespace model.Service
{
    public interface IPaiesService
    {
        IList<Paie> RetrieveAll();
    }
}
