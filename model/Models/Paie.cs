using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model.Service.MySql;
using model.Service.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp;
using PdfSharp.Drawing.Layout;
using System.Diagnostics;
namespace model.Models
{
    public class Paie : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region INotifyPropertyChanged INotifyPropertyChanging
        public event PropertyChangedEventHandler PropertyChanged;

        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get { return PropertyChanged; }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get { return PropertyChanging; }
        }


        protected virtual void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        public static float salaireUn = 40000, 
                            salaireDeux = 44701, 
                            salaireTrois = 89401, 
                            salaireQuatre = 138585,
                            salaireParticulierUn = 41935,
                            salaireParticulierDeux =83865,
                            salaireParticulierTrois = 102040 ;
        public static float tauxUn = (float)0.2853,
                            tauxDeux = (float)0.3837,
                            tauxTrois = (float)0.4571,
                            tauxQuatre = (float)0.4997,
                            
                            tauxParticulierUn = (float)0.3253,
                            tauxParticulierDeux = (float)0.4237,
                            tauxParticulierTrois = (float)0.4746;

        public static float supp = 50;
        public virtual string ID { get; set; }
        public virtual string idEmploye { get; set; }
        public virtual string idPeriode {get; set;}
        public virtual string Periode { get; set; }
        public virtual string DateGenerationRapport { get; set; }
        // Comme l'employé à une liste d'heure nous savons ici c'est quoi les heures pour l'employé.
        public virtual string Nom { get; set; }
        public virtual string salaire { get; set; }
        private float montantbrute;
        public virtual float MontantBrute {
            get
            {
                return this.montantbrute;
            }
            set
            {
                if (this.montantbrute == value)
                {
                    return;
                }
                this.montantbrute = value;
                RaisePropertyChanged("MontantBrute");
            }
        }
        private float montantnet;
        public virtual float MontantNet {
            get 
            { 
                return this.montantnet;
            }
            set
            {
                if (this.montantnet == value)
                {
                    return;
                }
                this.montantnet = value;
                RaisePropertyChanged("MontantNet");
            }
        }
        public virtual float NombreHeure { get; set; }
        public virtual float NombreHeureSupp { get; set; }
        public virtual float MontantPrime { get; set; }
        public virtual float MontantIndemnite { get; set; }
        public virtual float MontantAllocations { get; set; }
        public virtual float MontantCommission { get; set; }
        public virtual float MontantPourboire { get; set; }
        public virtual string updatedetail { get; set; }
        public float getTauxFederal(float montantBrute, float heure, float heureSupp, String idPeriode) 
        {
            MySqlPaieService _service = new MySqlPaieService();
            DataTable Periode;
            Periode = _service.anciennePeriode(idPeriode);
            DateTime start = (DateTime)Periode.Rows[0][0],
                    end = (DateTime)Periode.Rows[0][1];

            TimeSpan t = end - start;
            Double days = t.TotalDays;
            days = 365 / (days + 1);

            return (tauxCombineQC_CA((days * montantBrute)));
        }
        public float tauxCombineQC_CA(double EstimatedAnnualSalary)
        {
            if (EstimatedAnnualSalary <= salaireUn)
            {
                return tauxUn;
            }
            else if (EstimatedAnnualSalary > salaireUn && EstimatedAnnualSalary < salaireDeux)
            {
                return tauxParticulierUn;
            }
            else if (EstimatedAnnualSalary >= salaireDeux && EstimatedAnnualSalary < salaireParticulierDeux)
            {
                return tauxDeux;
            }
            else if (EstimatedAnnualSalary >= salaireParticulierDeux && EstimatedAnnualSalary < salaireTrois)
            {
                return tauxParticulierDeux;
            }
            else if (EstimatedAnnualSalary >= salaireTrois && EstimatedAnnualSalary < salaireParticulierTrois)
            {
                return tauxTrois;
            }
            else if (EstimatedAnnualSalary >= salaireParticulierTrois && EstimatedAnnualSalary < salaireQuatre)
            {
                return tauxParticulierTrois;
            }
            else if (EstimatedAnnualSalary >= salaireQuatre)
            {
                return tauxQuatre;
            }
            else
            {
                return 0;
            }
        }
        public Paie()
        {
            ID = String.Empty;
            Periode = String.Empty;
            idPeriode = String.Empty;
            DateGenerationRapport = String.Empty;

            Nom = String.Empty;
            MontantBrute = 0;

            MontantNet = 0;
            NombreHeure = 0;

            NombreHeureSupp = 0;
            MontantPrime = 0;
            salaire = String.Empty;
            updatedetail= "Aucunes modifications";
            MontantIndemnite = 0;
            MontantAllocations = 0;
            MontantCommission = 0;
            MontantPourboire = 0;

        }
        // Fin de la classe de test.
        public void GenererPaies()
        {
            MySqlPaieService _service = new MySqlPaieService();
            DataTable Periode;

            float temps;
            try
            {
                // Aller chercher tous les employés 
                MySqlEmployeService _emService = new MySqlEmployeService();
                IList<Employe> emp = _emService.RetrieveAll();
                Periode = _service.PeriodeTemps();
                
                if (Periode.Rows.Count == 0)
                {
                    throw new Exception("Toutes les périodes de paies ont déjà été générer. Allez en rentré de nouvelle pour la nouvelle année.");
                }
                else
                {
                    DateTime start = (DateTime)Periode.Rows[0][0],
                    end = (DateTime)Periode.Rows[0][1];
                    DateTime date = DateTime.Now;
                    PeriodePaie tmpTime = new PeriodePaie(date, date);
                    if(tmpTime.Fin < end)
                    {
                        throw new Exception("La période que vous essayez de générer est en cours.");
                    }
                    float HeureSupp = 0;
                    int compteurTemps = 0;
                    int totalEmp = emp.Count;
                // Pour chaque employé aller chercher leur temps.
                    foreach (Employe em in emp)
                    {
                        {
                       
                            temps = _service.RetrieveCompteurs(em.ID, start, end);
                            // Vérification dans le cas ou personne n'aurais travailler dans cette période.
                            if(temps == 0)
                            {
                                compteurTemps++;
                                if(compteurTemps == totalEmp)
                                {
                                    StringBuilder mess = new StringBuilder();
                                    mess.Append("Aucune employé à travail durant la période. Cette période à été noté comme généré: ");
                                    mess.Append(start.ToString());
                                    mess.Append(end.ToString());
                                    if (!(_service.periodeGenere(start, end)))
                                    {
                                        throw new Exception("Problème avec l'état des périodes de paie, vérifier que votre période courrante est cohérente.");
                                    }
                                    throw new Exception(mess.ToString());
                                }  
                            }
                            else 
                            { 
                                if (temps > supp)
                                {
                                    HeureSupp = temps - supp;
                                    temps = temps - supp;
                                }
                        
                                float Brute = ((float)em.Salaire * temps) + ((float)em.SalaireOver * HeureSupp);
                                TimeSpan t = end - start;
                                Double days = t.TotalDays;
                                days = 365/(days+1);
                                double EstimatedAnnualSalary = days * Brute;
                                float Net = 0;
                                Net = Brute * (1 - tauxCombineQC_CA((double)(days * Brute)));

                                Paie tmpPaie = new Paie() { MontantBrute = Brute, MontantNet = Net, NombreHeure = temps, NombreHeureSupp = HeureSupp };
                                if (!(_service.periodeGenere(start, end)))
                                {
                                    throw new Exception("Problème avec l'état des périodes de paie, vérifier que votre période courrante est cohérente.");
                                }
                                // Appel  de la fonction de génération de paie.
                                if (!(_service.insertPaie(tmpPaie, start, end, em.ID)))
                                {
                                    throw new Exception("Impossible de générer la paie de " + em.Prenom + " , " + em.Nom + " correctements vérifier les dates de vos périodes de paies.");
                                }
                           }
                        }
                    }
                    // change la période paye à terminé.
                    throw new Exception("Réussite de la génération des paies pour la période de :" + (start.Date).ToString("d") + " au " + (end.Date).ToString("d"));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void generateSlipePay(Paie p){
            #region headerPDF
            int PDF_WIDTH = 250;
            int PDF_MARGIN_LEFT=20;
            int NEXT_MARGIN_TOP = 60;
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = p.Nom + "_Paie";
            PdfPage page;
            page = pdf.AddPage();
            page.Orientation = PageOrientation.Portrait;
            page.Size = PageSize.A4;
            XGraphics graph = XGraphics.FromPdfPage(page);
            List<LiaisonProjetEmploye> Liaison = new List<LiaisonProjetEmploye>();
            MySqlEmployeService _ServiceMysql = new MySqlEmployeService();
            XFont font = new XFont("Consolas", 10.0); //new XFont("Verdana", 20, XFontStyle.Bold);
            var formatter = new XTextFormatter(graph);
            XPen penn = new XPen(XColors.Black, 1);
            penn.DashStyle = XDashStyle.Dash;
           
            graph.DrawLine(penn, 0, 3, page.Height, 3);  
            graph.DrawRectangle(new XSolidBrush(XColor.FromArgb(95,95,95)), new XRect(0, 3, page.Height, 30));

            var layoutRectangle = new XRect(5, 12.5, page.Width, page.Height);
            formatter.DrawString("GEM-C Rapport de paie de : " + (p.Nom), font, XBrushes.White, layoutRectangle);
            layoutRectangle = new XRect(300, 12.5, page.Width, page.Height);
            formatter.DrawString(("Talon pour la période du : " + p.Periode), font, XBrushes.White, layoutRectangle);

            graph.DrawLine(penn, 0, 33, 800, 33);

            graph.DrawRectangle(new XSolidBrush(XColor.FromArgb(255, 243, 229)), new XRect(0, 34, page.Height, 255));
            layoutRectangle = new XRect(PDF_MARGIN_LEFT, 40, page.Width, page.Height);
            formatter.DrawString("Calcul du salaire :", font, XBrushes.Black, layoutRectangle);
            graph.DrawLine(penn, PDF_MARGIN_LEFT, 55, 150, 55);
            #endregion


          
            if(p.MontantCommission != 0)
            {
                drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font,layoutRectangle, formatter,"Montant commission : ", Math.Round(p.MontantCommission,2).ToString());
                NEXT_MARGIN_TOP+=10;
            }
            if (p.MontantIndemnite != 0)
            {
                drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Montant indemnite : ", Math.Round(p.MontantIndemnite,2).ToString());
                NEXT_MARGIN_TOP += 10;
            }
            if (p.MontantPourboire != 0)
            {
                drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Montant pourboire : ", Math.Round(p.MontantPourboire,2).ToString());
                NEXT_MARGIN_TOP += 10;
            }
            if (p.MontantPrime != 0)
            {
                drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Montant prime : ", Math.Round(p.MontantPrime,2).ToString());
                NEXT_MARGIN_TOP += 10;
            }
            if (p.MontantAllocations != 0)
            {
                drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Montant allocations : ", Math.Round(p.MontantAllocations, 2).ToString());
                NEXT_MARGIN_TOP += 10;
            }
            double lesmontants = Math.Round((p.MontantAllocations + p.MontantCommission + p.MontantIndemnite + p.MontantPourboire + p.MontantPrime), 2);
            if (lesmontants != 0)
            {
                drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "", "--------------", "");
                NEXT_MARGIN_TOP += 10;
                drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Total des montants :  ", lesmontants.ToString());
                NEXT_MARGIN_TOP += 30;
            }    
                
            // ce qui nou intéresse commence
            
            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Heure regulier : ", Math.Round(p.NombreHeure, 2).ToString(), " hr(s)");
            NEXT_MARGIN_TOP += 10;
            
            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Heure suplémentaire : ", Math.Round(p.NombreHeureSupp, 2).ToString(), " hr(s)");
            NEXT_MARGIN_TOP += 10;
            

            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, ("Taux rég : " + p.salaire.ToString() + "$ "), Math.Round((p.NombreHeureSupp*(Convert.ToDouble(p.salaire))),2).ToString());
            NEXT_MARGIN_TOP+=10; supp :
            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Taux supp :  " + Math.Round((Convert.ToDouble(p.salaire) * 1.5), 4).ToString() + " $", Math.Round((p.MontantBrute - lesmontants) - (p.NombreHeure * Convert.ToDouble(p.salaire)), 2).ToString());
            NEXT_MARGIN_TOP += 10;

            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "", "--------------", "");
            NEXT_MARGIN_TOP += 10;
            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Montant brute total :  ", Math.Round((p.MontantBrute), 2).ToString());
            NEXT_MARGIN_TOP += 10;
            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "", "--------------", "");
            NEXT_MARGIN_TOP += 10;
            
            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Montant impôt combiné (Féd. et prov.): ", "-" + (Math.Round(p.MontantBrute, 2) - Math.Round(p.MontantNet, 2)).ToString());
            NEXT_MARGIN_TOP += 20;

            graph.DrawRectangle(XPens.Black, XBrushes.LightSeaGreen, 5, NEXT_MARGIN_TOP-2.5, page.Width-10, page.Height-825);
            
            drawPDF(PDF_WIDTH, NEXT_MARGIN_TOP, PDF_MARGIN_LEFT, page, font, layoutRectangle, formatter, "Montant Net du : ", Math.Round(p.MontantNet, 2).ToString());

            graph.DrawLine(XPens.Black, 0, 280, 800, 280);     
            #region outputPDF
            StringBuilder b = new StringBuilder();
            b.Append("talon_");
            b.Append(p.Nom.ToString());
            b.Append("_");
            b.Append(String.Format("{0:M/d/yyyy}", Convert.ToDateTime(p.DateGenerationRapport)));
            b.Append(".pdf");
            string pdfFilename = b.ToString().Replace(" ", "_");
            pdf.Save(pdfFilename);
            Process.Start(pdfFilename);
            #endregion
        }
        public void drawPDF(int PDF_WIDTH, int NEXT_MARGIN_TOP, int PDF_MARGIN_LEFT, PdfPage page,  XFont font, XRect layoutRectangle, XTextFormatter formatter, string text, string ligne, string dollar = " $")
        {
            
            layoutRectangle = new XRect(PDF_MARGIN_LEFT, NEXT_MARGIN_TOP, page.Width, page.Height);
            formatter.DrawString((text), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(PDF_WIDTH, NEXT_MARGIN_TOP, page.Width, page.Height);
            formatter.DrawString((ligne + dollar), font, XBrushes.Black, layoutRectangle);
        }
    }

}
