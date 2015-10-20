using System;

namespace model.Models
{
    public class LiaisonProjetEmploye
    {   
        public string LiaisonID { get; set; }
        public string ProjNom { get; set; }
        public bool Occupe { get; set; }

        public LiaisonProjetEmploye()
        {
            LiaisonID = String.Empty;
            ProjNom = String.Empty;
            Occupe = false;
        }
    }
}
