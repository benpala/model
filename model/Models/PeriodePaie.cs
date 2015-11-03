using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.Models
{
    public class PeriodePaie
    {
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
       
        public PeriodePaie(DateTime debut, DateTime fin)
        {
            Debut = convertToRealPeriodeTime(debut, true);
            Fin = convertToRealPeriodeTime(fin, false);
        }
        public DateTime convertToRealPeriodeTime(DateTime oneDate, bool param)
        {
            //True est pour un début
            if(param)
            {
                return (oneDate = (oneDate.Date + new TimeSpan(00,00,00)));
            }
            else if(!param) // false est pour la fin.
            {
                return (oneDate = (oneDate.Date + new TimeSpan(23, 59, 59)));
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
