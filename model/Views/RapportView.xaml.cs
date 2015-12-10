using model.Models;
using model.Models.Args;
using model.Service;
using model.Service.MySql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.Drawing;
using PdfSharp.Drawing.Layout;
using System.Configuration;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour RapportView.xaml
    /// </summary>
    public partial class RapportView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private ObservableCollection<Employe> _employe = new ObservableCollection<Employe>();
        private ObservableCollection<Projet> _projet = new ObservableCollection<Projet>();
        private ObservableCollection<Paie> _paie = new ObservableCollection<Paie>();
        private IEmployeService _ServiceEmploye;
        private IProjetService _ServiceProjet;
        private IPaiesService _ServicePaie;
        private IApplicationService _applicationService;

        private static readonly string nomEntreprise;

        public string NomEntreprice() { return nomEntreprise; }

        public RetrieveEmployeArgs RetrieveArgs { get; set; }

        public RapportView()
        {
            InitializeComponent();
            RetrieveArgs = new RetrieveEmployeArgs();
            _ServiceEmploye = ServiceFactory.Instance.GetService<IEmployeService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            
            Employe = new ObservableCollection<Employe>(_ServiceEmploye.RetrieveAll());
            _ServiceProjet = ServiceFactory.Instance.GetService<IProjetService>();
            Projet = new ObservableCollection<Projet>(_ServiceProjet.retrieveAll());
            _ServicePaie = ServiceFactory.Instance.GetService<IPaiesService>();
            Paie = new ObservableCollection<Paie>(_ServicePaie.RetrieveAll());
            DataContext = this;

        }

        static RapportView()
        {
            nomEntreprise = ConfigurationManager.AppSettings["nomEntreprise"].ToString();
        }

        #region get set 
        public ObservableCollection<Employe> Employe
        {
            get
            {
                return _employe;
            }
            set
            {
                if (_employe == value)
                {
                    return;
                }

                RaisePropertyChanging();
                _employe = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Projet> Projet
        {
            get
            {
                return _projet;
            }
            set
            {
                if (_projet == value)
                {
                    return;
                }

                RaisePropertyChanging();
                _projet = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Paie> Paie
        {
            get
            {
                return _paie;
            }
            set
            {
                if (_paie == value)
                {
                    return;
                }

                RaisePropertyChanging();
                _paie = value;
                RaisePropertyChanged();
            }
        }
        #endregion
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

        private void GenererRapportEmploye(object sender, RoutedEventArgs e)
        {
            List<Employe> lstEmp = new List<Employe>();
            //Tri nom employé
            foreach (Employe emp in lstRapportEmploye.SelectedItems)
                lstEmp.Add(emp);

            //Tri nom employé
            if((bool)rbtTriNomAZ.IsChecked)
                lstEmp = lstEmp.OrderBy(lst => lst.Nom).ToList();
            else if ((bool)rbtTriNomZA.IsChecked)
                lstEmp = lstEmp.OrderByDescending(lst => lst.Nom).ToList();

            //Tri salaire
            else if ((bool)rbtTriSalaireASC.IsChecked)// ASC 
                lstEmp = lstEmp.OrderBy(lst => lst.Salaire).ToList();
            else if ((bool)rbtTriSalaireDESC.IsChecked)// DESC
                lstEmp = lstEmp.OrderByDescending(lst => lst.Salaire).ToList();

            try 
            {
                if (Convert.ToSingle(lstEmp.Count.ToString()) > 0)
                {
                    if (chxProjetEmploye.IsChecked.Value || chxInfoGenerale.IsChecked.Value)
                    {
                        PdfDocument pdf = new PdfDocument();
                        pdf.Info.Title = "Rapport Employe";
                        PdfPage page;
                        page = pdf.AddPage();
                        page.Orientation = PageOrientation.Landscape;
                        page.Size = PageSize.A4;
                        XGraphics graph = XGraphics.FromPdfPage(page);
                        List<LiaisonProjetEmploye> Liaison = new List<LiaisonProjetEmploye>();
                        MySqlEmployeService _ServiceMysql = new MySqlEmployeService();
                        XFont font = new XFont("Consolas", 20.0); 
                        var formatter = new XTextFormatter(graph);
                        int height = 70;
                        XPen penn = new XPen(XColors.Black, 1);
                        penn.DashStyle = XDashStyle.Dash;

                        string date = DateTime.Now.ToString("dd/MM/yyyy");
                        //Entête
                        graph.DrawLine(penn, 3, 3, 850, 3);
                        graph.DrawRectangle(new XSolidBrush(XColor.FromArgb(95, 95, 95)), new XRect(0, 5, 850, 60));
                        var layoutRectangle = new XRect(5, 25, page.Width, page.Height);
                        formatter.DrawString("GEM-C Rapport des employés", font, XBrushes.Black, layoutRectangle);
                        layoutRectangle = new XRect(600, 25, page.Width, page.Height);
                        formatter.DrawString(("Date : " + date), font, XBrushes.Black, layoutRectangle);
                        graph.DrawLine(penn, 3, 67, 850, 67);
                        graph.DrawRectangle(new XSolidBrush(XColor.FromArgb(255, 243, 229)), new XRect(0, 70, 850, 600));

                        //Table 
                        font = new XFont("Consolas", 10.0);

                        graph.DrawRectangle(XPens.Black, new XRect(5, height + 5, 20, 60));
                        layoutRectangle = new XRect(7, height + 5, page.Width, page.Height);
                        formatter.DrawString("ID", font, XBrushes.Black, layoutRectangle);

                        graph.DrawRectangle(XPens.Black, new XRect(25, height + 5, 150, 60));
                        layoutRectangle = new XRect(27, height + 5, page.Width, page.Height);
                        formatter.DrawString("Nom", font, XBrushes.Black, layoutRectangle);

                        layoutRectangle = new XRect(27, height + 20, page.Width, page.Height);
                        formatter.DrawString("Prénom", font, XBrushes.Black, layoutRectangle);
                        
                        if (chxInfoGenerale.IsChecked.Value)
                        {
                            graph.DrawRectangle(XPens.Black, new XRect(175, height + 5, 120, 60));
                            layoutRectangle = new XRect(178, height + 5, page.Width, page.Height);
                            formatter.DrawString("Informations générales", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(178, height + 20, page.Width, page.Height);
                            formatter.DrawString("-Titre emploi", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(178, height + 30, page.Width, page.Height);
                            formatter.DrawString("-'Date embauche'", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(178, height + 40, page.Width, page.Height);
                            formatter.DrawString("-(Statut)", font, XBrushes.Black, layoutRectangle);
                        
                            graph.DrawRectangle(XPens.Black, new XRect(295, height + 5, 130, 60));
                            layoutRectangle = new XRect(298, height + 5, page.Width, page.Height);
                            formatter.DrawString("Détails financiés", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(298, height + 20, page.Width, page.Height);
                            formatter.DrawString("-Salaire" , font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(298, height + 30, page.Width, page.Height);
                            formatter.DrawString("-(Salaire avec les ", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(298, height + 40, page.Width, page.Height);
                            formatter.DrawString("heures supplémentaires)", font, XBrushes.Black, layoutRectangle);
                        }

                        if (chxProjetEmploye.IsChecked.Value)
                        {
                            graph.DrawRectangle(XPens.Black, new XRect(175, height + 5, 250, 60));
                            graph.DrawRectangle(XPens.Black, new XRect(425, height + 5, 170, 60));
                            layoutRectangle = new XRect(430, height + 5, page.Width, page.Height);
                            formatter.DrawString("Projet(s) occupé(s)", font, XBrushes.Black, layoutRectangle);
                        
                            graph.DrawRectangle(XPens.Black, new XRect(595, height + 5, 150, 60));
                            layoutRectangle = new XRect(605, height + 5, page.Width, page.Height);
                            formatter.DrawString("Temps travaillé", font, XBrushes.Black, layoutRectangle);
                        }
                        
                        height = 130;
                        foreach (Employe emp in lstEmp)
                        {
                            if (chxProjetEmploye.IsChecked.Value)
                            {
                                Liaison = new List<LiaisonProjetEmploye>(_ServiceMysql.GetLiaison(emp.ID));

                                if (15 * Liaison.Count + height > 530)
                                {
                                    page = pdf.AddPage();
                                    page.Orientation = PageOrientation.Landscape;
                                    graph = XGraphics.FromPdfPage(page);
                                    formatter = new XTextFormatter(graph);
                                    height = 0;
                                    graph.DrawRectangle(new XSolidBrush(XColor.FromArgb(255, 243, 229)), new XRect(0, 0, 850, 600));
                                }
                            }
                            else if (!chxProjetEmploye.IsChecked.Value && height > 550)
                            {
                                page = pdf.AddPage();
                                page.Orientation = PageOrientation.Landscape;
                                graph = XGraphics.FromPdfPage(page);
                                formatter = new XTextFormatter(graph);
                                height = 0;
                                graph.DrawRectangle(new XSolidBrush(XColor.FromArgb(255, 243, 229)), new XRect(0, 0, 850, 600));
                            }
                            
                            int y = height;

                            layoutRectangle = new XRect(7, height += 15 , page.Width, page.Height);
                            formatter.DrawString( emp.ID.ToString(), font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(27, height , page.Width, page.Height);
                            formatter.DrawString(emp.Nom.ToString(), font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(27  , height + 15 , page.Width, page.Height);
                            formatter.DrawString(emp.Prenom.ToString(), font, XBrushes.Black, layoutRectangle);
 
                            //Section info employé
                            if (chxInfoGenerale.IsChecked.Value)
                            {

                                layoutRectangle = new XRect(178, height , page.Width, page.Height);
                                formatter.DrawString(emp.Poste.ToString(), font, XBrushes.Black, layoutRectangle);

                                layoutRectangle = new XRect(178 , height + 15, page.Width, page.Height);
                                formatter.DrawString(("'" + _ServiceMysql.getDateEmbauche(emp.ID) + "'"), font, XBrushes.Black, layoutRectangle);

                                if (!emp.HorsFonction)
                                {
                                    layoutRectangle = new XRect(178 , height + 30, page.Width, page.Height);
                                    formatter.DrawString("(En service)", font, XBrushes.Black, layoutRectangle);
                                }
                                else
                                {
                                    layoutRectangle = new XRect(178 , height + 30, page.Width, page.Height);
                                    formatter.DrawString("(Hors service)", font, XBrushes.Black, layoutRectangle);
                                }

                                layoutRectangle = new XRect(298, height , page.Width, page.Height);
                                formatter.DrawString((Math.Round(Convert.ToSingle(emp.Salaire), 2) + " $"), font, XBrushes.Black, layoutRectangle);
                                layoutRectangle = new XRect(298, height + 15, page.Width, page.Height);
                                formatter.DrawString(("(" + Math.Round( Convert.ToSingle(emp.SalaireOver), 2) + " $)"), font, XBrushes.Black, layoutRectangle);
                            }
                            //Section projets des employés
                            if (chxProjetEmploye.IsChecked.Value)
                            {
                                Liaison = new List<LiaisonProjetEmploye>(_ServiceMysql.GetLiaison(emp.ID));
                                Dictionary<string, float> lstProjHeure = new Dictionary<string, float>();
                                foreach (LiaisonProjetEmploye liaison in Liaison)
                                    lstProjHeure.Add(liaison.ProjNom, Convert.ToInt32(_ServiceMysql.CompteursProjets(emp.ID, liaison.ProjNom)));
                                int count = 0;
                                
                                if (Liaison.Count > 0)
                                {
                                    //Tri nom projet
                                    if ((bool)rbtTriProjetAZ.IsChecked)
                                        lstProjHeure = (Dictionary<string, float>)lstProjHeure.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value); 
                                    else if ((bool)rbtTriProjetZA.IsChecked)
                                        lstProjHeure = (Dictionary<string, float>)lstProjHeure.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value); 
                                    
                                    //Tri nb heure
                                    if ((bool)rbtTriNBheureASC.IsChecked)//ASC
                                        lstProjHeure = (Dictionary<string, float>)lstProjHeure.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); 
                                    else if ((bool)rbtTriNBheureDESC.IsChecked)//DESC
                                        lstProjHeure = (Dictionary<string, float>)lstProjHeure.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value); 

                                    foreach (KeyValuePair<string, float> dict in lstProjHeure)
                                    {
                                        count++;
                                        layoutRectangle = new XRect(425, y += 15, page.Width, page.Height);
                                        formatter.DrawString(count + " - " + dict.Key.ToString(), font, XBrushes.Black, layoutRectangle);

                                        layoutRectangle = new XRect(605 , y, page.Width, page.Height);
                                        formatter.DrawString(dict.Value + " heures", font, XBrushes.Black, layoutRectangle);
                                    }
                                    if (y > height)
                                        height = y;
                                }
                            }
                            height += 30; 
                            graph.DrawLine(XPens.Black, 10, height+=20 , 800, height);

                        }
                        StringBuilder fileName = new StringBuilder();
                        fileName.Append("Rapport_Emp_");
                        fileName.Append(DateTime.Now.ToString());
                        fileName.Append(".pdf");
                        fileName.Replace(":", "");
                        fileName.Replace(" ", "_");
                        pdf.Save(fileName.ToString());
                        Process.Start(fileName.ToString());
                    }
                    else
                        MessageBox.Show("Aucune information choisie");
                }
                else
                    MessageBox.Show("Ancun employé sélectionné");
            }
            catch (Exception E)
            {
                 MessageBox.Show("Le fichier est déjà utilisé, veuillez le fermer pour le regénérer");
            }
        }

        private void ChoisirTousEmploye(object sender, RoutedEventArgs e)
        {
            lstRapportEmploye.SelectAll();
            
        }

        private void GenererRapportProjet(object sender, RoutedEventArgs e)
        {
            bool date = false;
            bool heure = false;
            bool cout = false;
            List<Projet> LstProjet = new List<Projet>();
            int NbPaie = 0;
            float BruteTotal = 0;
            int tailleCout = 0;
            int tailleHP = 0;
            int tailleHT = 0;

            List<Projet> lstProj = new List<Projet>();
            //Tri nom employé
            foreach (Projet pro in lstRapportProjet.SelectedItems)
            {
                lstProj.Add(pro);
                tailleCout = VerifeTaille(pro.prixSimulation.ToString().Length, tailleCout);
                tailleHP = VerifeTaille(pro.nbHeuresSimule.ToString().Length, tailleHP);
                tailleHT = VerifeTaille(pro.nbHeuresReel.ToString().Length, tailleHT);
            }

            try 
            {
                if(lstProj.Count() == 0)
                    throw new Exception("pasProg");              

                PdfDocument pdf = new PdfDocument();
                pdf.Info.Title = "Rapport Projet";
                PdfPage page;
                page = pdf.AddPage();
                page.Orientation = PageOrientation.Landscape;
                page.Size = PageSize.A4;
                XGraphics graph = XGraphics.FromPdfPage(page);

                XFont font = new XFont("Consolas", 12.0); //new XFont("Verdana", 20, XFontStyle.Bold);
                XTextFormatter formatter = new XTextFormatter(graph);
                int height = 0;

                if (dtDebut.Text.ToString() != "" && dtFin.Text.ToString() != "")
                    date = true;
                if (HeureMin.Text.ToString() != "" && HeureMax.Text.ToString() != "")
                    heure = true;
                if (CoutMin.Text.ToString() != "" && CoutMax.Text.ToString() != "")
                    cout = true;

                LstProjet = filtreRprojet(lstProj, date, heure, cout);

                XRect layoutRectangle = new XRect(10, height += 15, page.Width, page.Height);

                EnteteDPF(ref formatter, ref font, layoutRectangle);

                graph.DrawLine(XPens.Black, 10, 50, page.Width - 10, 50);

                ColonneA(ref page, ref graph, ref formatter, ref font, layoutRectangle, "PR");

                layoutRectangle = new XRect(10, height += 75, page.Width, page.Height);

                foreach (Projet p in LstProjet)
                {
                    CorpsProjet(ref page, ref formatter, ref font, layoutRectangle, ref height, p, tailleCout, tailleHP, tailleHT);
                    BruteTotal = BruteTotal + p.nbHeuresSimule;
                    NbPaie++;
                }
               
                if (LstProjet.Count() == 0)
                    throw new Exception("erreur");

                layoutRectangle = new XRect(10, page.Height - 70, page.Width, page.Height);
                formatter.DrawString(("Montant total : " + Math.Round(Convert.ToSingle(BruteTotal), 2) + " $" + "\t\t\tNombre de Projet : " + NbPaie.ToString()), font, XBrushes.Black, layoutRectangle);

                DateTime now = DateTime.Now;

                StringBuilder b = new StringBuilder();
                b.Append("RProjet_");
                b.Append(String.Format("{0:yyyy/MM/dd}", now));
                b.Append("_");
                b.Append(String.Format("{0:hh:mm:ss}", now));
                b.Append(".pdf");
                string pdfFilename = b.ToString().Replace(":", "");
                pdf.Save(pdfFilename);
                Process.Start(pdfFilename);
            }
            catch (Exception E)
            {
                if (E.Message == "erreur")
                    MessageBox.Show("Aucun projet n'a ces paramètres");
                else if(E.Message == "pasProg")
                    MessageBox.Show("Veuillez choisir un ou plusieurs projets");
                else
                    MessageBox.Show("Le fichier est déjà utilisé, veuillez le fermer pour le regenerer.");
            }
            
        }
        private List<Projet> filtreRprojet(List<Projet> listp, bool date = false, bool heures = false, bool cout = false)
        {
            if(date)
                listp = filtreAppliquerDate(listp);
            if(heures)
               listp = filtreAppliquerHeure(listp);
            if(cout)
               listp = filtreAppliquerCout(listp);

            return listp;
        }
        private List<Projet> filtreAppliquerDate(List<Projet> listp)
        {
            PeriodePaie tmp = new PeriodePaie(Convert.ToDateTime(dtDebut.ToString()), Convert.ToDateTime(dtFin.ToString()));

            List<Projet> returnlist = new List<Projet>();
            foreach (Projet p in listp)
            {
                if (Convert.ToDateTime(p.dateun.ToString()) >= tmp.Debut && Convert.ToDateTime(p.dateun.ToString()) <= tmp.Fin)
                    returnlist.Add(p);
            }
            return returnlist;
        }

        private List<Projet> filtreAppliquerHeure(List<Projet> listp)
        {
            List<Projet> returnlist = new List<Projet>();

            foreach (Projet p in listp)
            {
                if (p.nbHeuresSimule >= Convert.ToInt32(HeureMin.Text.ToString()) && p.nbHeuresSimule <= Convert.ToInt32(HeureMax.Text.ToString()))
                    returnlist.Add(p);
            }
            return returnlist;
        }

        private List<Projet> filtreAppliquerCout(List<Projet> listp)
        {
            List<Projet> returnlist = new List<Projet>();

            foreach (Projet p in listp)
            {
                if (p.prixSimulation >= Convert.ToInt32(CoutMin.Text.ToString()) && p.prixSimulation <= Convert.ToInt32(CoutMax.Text.ToString()))
                    returnlist.Add(p);
            }
            return returnlist;
        }

        private void ChoisirTousProjet(object sender, RoutedEventArgs e)
        {
            lstRapportProjet.SelectAll();

        }

        private void GenererRapportFinancie(object sender, RoutedEventArgs e)
        {
            int NbPaie = 0;
            int tailleB = 0;
            int tailleHS = 0;
            int tailleP = 0;
            int tailleI = 0;

            try
            {
                if (dtDateDebut.Text.ToString() == "" && dtDateFin.Text.ToString() == "")
                    throw new Exception("PasDate");
                if (chxA.IsChecked.Value){
                    if(chxB.IsChecked.Value)
                    {
                        if(chxC.IsChecked.Value)
                            throw new Exception("tropChx");
                        else
                            throw new Exception("tropChx");
                    }
                } else {
                    if (chxB.IsChecked.Value)
                    {
                        if (chxC.IsChecked.Value)
                            throw new Exception("tropChx");
                    }
                }

                if (chxA.IsChecked.Value || chxB.IsChecked.Value)
                {
                    PeriodePaie tmp = new PeriodePaie(Convert.ToDateTime(dtDateDebut.Text), Convert.ToDateTime(dtDateFin.Text));
                    float BruteTotal = 0;

                    PdfDocument pdf = new PdfDocument();
                    pdf.Info.Title = "Rapport Paie";
                    PdfPage page;
                    page = pdf.AddPage();
                    page.Orientation = PageOrientation.Landscape;
                    page.Size = PageSize.A4;
                    XGraphics graph = XGraphics.FromPdfPage(page);

                    XFont font = new XFont("Consolas", 12.0); //new XFont("Verdana", 20, XFontStyle.Bold);
                    XTextFormatter formatter = new XTextFormatter(graph);
                    int height = 0;

                    XRect layoutRectangle = new XRect(10, height += 15, page.Width, page.Height);

                    EnteteDPF(ref formatter, ref font, layoutRectangle);

                    graph.DrawLine(XPens.Black, 10, 50, page.Width - 10, 50);

                    if (chxA.IsChecked.Value)
                        ColonneA(ref page, ref graph, ref formatter, ref font, layoutRectangle);
                    else if (chxB.IsChecked.Value)
                        ColonneAD(ref page, ref graph, ref formatter, ref font, layoutRectangle);

                    layoutRectangle = new XRect(10, height += 75, page.Width, page.Height);

                    foreach (Paie paie in Paie)
                    {
                        tailleB = VerifeTaille(paie.MontantBrute.ToString().Length, tailleB);
                        tailleHS = VerifeTaille(paie.NombreHeureSupp.ToString().Length, tailleHS);
                        tailleP = VerifeTaille(paie.MontantPourboire.ToString().Length, tailleP);
                        tailleI = VerifeTaille(paie.MontantIndemnite.ToString().Length, tailleI);
                    }

                    foreach (Paie paie in Paie)
                    {
                        int tmpD = DateTime.Compare(Convert.ToDateTime(paie.DateGenerationRapport), tmp.Debut);
                        int tmpF = DateTime.Compare(Convert.ToDateTime(paie.DateGenerationRapport), tmp.Fin);

                        if (height > 700)
                        {
                            page = pdf.AddPage();
                            graph = XGraphics.FromPdfPage(page);
                            formatter = new XTextFormatter(graph);
                            height = 0;
                        }

                        if (tmpD > 0 && tmpF < 0)
                        {
                            if (chxA.IsChecked.Value)
                                CorpsAtomique(ref page, ref formatter, ref font, layoutRectangle, ref height, paie, tailleB);
                            else if (chxB.IsChecked.Value)
                                CorpsAtomiqueD(ref page, ref formatter, ref font, layoutRectangle, ref height, paie, tailleB, tailleHS, tailleP, tailleI);
                            BruteTotal = BruteTotal + paie.MontantBrute;
                            NbPaie++;
                        }

                    }

                    if (BruteTotal == 0)
                        throw new Exception("erreur");

                    layoutRectangle = new XRect(10, page.Height - 70, page.Width, page.Height);
                    formatter.DrawString(("Coût pour la période : " + String.Format("{0:yyyy-MM-dd}", tmp.Debut) + " à " + String.Format("{0:yyyy-MM-dd}", tmp.Fin) + "\t\t\tMontant total : " + Math.Round(Convert.ToSingle(BruteTotal), 2) + " $" + "\t\t\tNombre de Paie : " + NbPaie.ToString()), font, XBrushes.Black, layoutRectangle);

                    DateTime now = DateTime.Now;

                    StringBuilder b = new StringBuilder();
                    b.Append("RPaie_");
                    b.Append(String.Format("{0:yyyy/MM/dd}", now));
                    b.Append("_");
                    b.Append(String.Format("{0:hh:mm:ss}", now));
                    b.Append(".pdf");
                    string pdfFilename = b.ToString().Replace(":", "");
                    pdf.Save(pdfFilename);
                    Process.Start(pdfFilename);
                }
                else
                    throw new Exception("nochecked");
            }
            catch (Exception E)
            {
                if (E.Message == "erreur")
                    MessageBox.Show("Aucune paie durant cette période");
                else if (E.Message == "nochecked")
                    MessageBox.Show("Veuillez cocher atomique ou atomique détail pour afficher le rapport. L'option regrouper n'est pas disponible");
                else if (E.Message == "PasDate")
                    MessageBox.Show("Veuillez entrer des dates.");
                else if (E.Message == "tropChx")
                    MessageBox.Show("Veuillez choisir un seul choix à la fois.");
                else
                    MessageBox.Show("Le fichier est déjà utilisé, veuillez le fermer pour le regenerer.");
            }
        }

        
        private void EnteteDPF(ref XTextFormatter formatter, ref XFont font, XRect layoutRectangle)
        {
            DateTime now = DateTime.Now;

            layoutRectangle = new XRect(25, 25, 200, 25);
            formatter.DrawString((nomEntreprise + "\t\t\t\t\t Rapport de Paies"), font, XBrushes.Black, layoutRectangle);

            layoutRectangle = new XRect(300, 25, 300, 25);
            formatter.DrawString(("Date de Génération : " + now.ToString()), font, XBrushes.Black, layoutRectangle);
        }

        private void ColonneA(ref PdfPage page, ref XGraphics graph, ref XTextFormatter formatter, ref XFont font, XRect layoutRectangle, string PaieProjet = "PA")
        {
            //Étiquette
            if (PaieProjet == "PA")
            {
                graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 55, page.Width - 730, page.Height - 547);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 123, 55, page.Width - 638, page.Height - 547);
            }
            else
            {
                graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 55, page.Width - 638, page.Height - 547);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 215, 55, page.Width - 730, page.Height - 547);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 554, 55, page.Width - 730, page.Height - 547);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 667, 55, page.Width - 730, page.Height - 547);
            }
            graph.DrawRectangle(XPens.Black, XBrushes.White, 328, 55, page.Width - 730, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 441, 55, page.Width - 730, page.Height - 547);

            if (PaieProjet == "PA")
            {
                layoutRectangle = new XRect(15, 60, page.Width - 730, page.Height - 547);
                formatter.DrawString("Date Génération Paie", font, XBrushes.Black, layoutRectangle);
                layoutRectangle = new XRect(185, 60, page.Width - 638, page.Height - 547);
                formatter.DrawString("Nom Employé", font, XBrushes.Black, layoutRectangle);
            }
            else
            {
                layoutRectangle = new XRect(80, 60, page.Width - 730, page.Height - 547);
                formatter.DrawString("Nom du Projet", font, XBrushes.Black, layoutRectangle);
                layoutRectangle = new XRect(255, 60, page.Width - 638, page.Height - 547);
                formatter.DrawString("Coût", font, XBrushes.Black, layoutRectangle);
                layoutRectangle = new XRect(576, 60, page.Width - 730, page.Height - 547);
                formatter.DrawString("Date Début", font, XBrushes.Black, layoutRectangle);
                layoutRectangle = new XRect(692, 60, page.Width - 730, page.Height - 547);
                formatter.DrawString("Date Fin", font, XBrushes.Black, layoutRectangle);
            }
            layoutRectangle = new XRect(360, 60, page.Width - 730, page.Height - 547);
            if (PaieProjet == "PA")
                formatter.DrawString("ID Paie", font, XBrushes.Black, layoutRectangle);
            else
                formatter.DrawString("Nb heures Planifiées", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(470, 60, page.Width - 730, page.Height - 547);
            if (PaieProjet == "PA")
                formatter.DrawString("Montant Brut", font, XBrushes.Black, layoutRectangle);
            else
                formatter.DrawString("Nb heures Travaillées", font, XBrushes.Black, layoutRectangle);
            //Contenu
            if (PaieProjet == "PA")
            {
                graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 104, page.Width - 730, page.Height - 200);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 123, 104, page.Width - 638, page.Height - 200);
            }
            else
            {
                graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 104, page.Width - 638, page.Height - 200);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 215, 104, page.Width - 730, page.Height - 200);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 554, 104, page.Width - 730, page.Height - 200);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 667, 104, page.Width - 730, page.Height - 200);
            }
            graph.DrawRectangle(XPens.Black, XBrushes.White, 328, 104, page.Width - 730, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 441, 104, page.Width - 730, page.Height - 200);
        }

        private void ColonneAD(ref PdfPage page, ref XGraphics graph, ref XTextFormatter formatter, ref XFont font, XRect layoutRectangle)
        {
            //Étiquette
            graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 55, page.Width - 745, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 108, 55, page.Width - 670, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 281, 55, page.Width - 770, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 354, 55, page.Width - 760, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 437, 55, page.Width - 760, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 520, 55, page.Width - 760, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 603, 55, page.Width - 780, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 666, 55, page.Width - 760, page.Height - 547);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 749, 55, page.Width - 760, page.Height - 547);

            layoutRectangle = new XRect(15, 60, page.Width - 750, page.Height - 547);
            formatter.DrawString("Date Génération Paie", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(153, 60, page.Width - 638, page.Height - 547);
            formatter.DrawString("Nom Employé", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(292, 60, page.Width - 730, page.Height - 547);
            formatter.DrawString("ID Paie", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(356, 60, page.Width - 730, page.Height - 547);
            formatter.DrawString("Montant Brut", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(444, 60, page.Width - 750, page.Height - 547);
            formatter.DrawString("Heure supp", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(530, 60, page.Width - 638, page.Height - 547);
            formatter.DrawString("Pourboire", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(605, 60, page.Width - 730, page.Height - 547);
            formatter.DrawString("Indemnité", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(668, 60, page.Width - 730, page.Height - 547);
            formatter.DrawString("Taux horaire", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(751, 60, page.Width - 750, page.Height - 547);
            formatter.DrawString("Nombre heure", font, XBrushes.Black, layoutRectangle);
            //Contenu
            graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 104, page.Width - 745, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 108, 104, page.Width - 670, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 281, 104, page.Width - 770, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 354, 104, page.Width - 760, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 437, 104, page.Width - 760, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 520, 104, page.Width - 760, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 603, 104, page.Width - 780, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 666, 104, page.Width - 760, page.Height - 200);
            graph.DrawRectangle(XPens.Black, XBrushes.White, 749, 104, page.Width - 760, page.Height - 200);
        }

        private void CorpsAtomique(ref PdfPage page, ref XTextFormatter formatter, ref XFont font, XRect layoutRectangle, ref int height, Paie paie, int tB)
        {
            layoutRectangle = new XRect(30, height += 15, page.Width, page.Height);
            formatter.DrawString(String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(paie.DateGenerationRapport)), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(136, height, page.Width, page.Height);
            formatter.DrawString(paie.Nom.Substring(0, 1).ToString() + "." + paie.Nom.Substring(paie.Nom.IndexOf(' ')).ToString(), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(380, height, page.Width, page.Height);
            formatter.DrawString(paie.ID.ToString(), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(470, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tB+"}",Math.Round(Convert.ToSingle(paie.MontantBrute), 2)) + " $", font, XBrushes.Black, layoutRectangle);
        }

        private void CorpsProjet(ref PdfPage page, ref XTextFormatter formatter, ref XFont font, XRect layoutRectangle, ref int height, Projet projet, int tC, int tHP, int tHT)
        {
            layoutRectangle = new XRect(30, height += 15, page.Width, page.Height);
            formatter.DrawString(projet.nom, font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(260, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tC+"}",projet.prixSimulation.ToString()) + "$", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(360, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tHP+"}",projet.nbHeuresSimule.ToString()), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(470, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tHT+"}",projet.nbHeuresReel.ToString()), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(581, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(projet.dateun)), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(697, height, page.Width, page.Height);
            formatter.DrawString((projet.datedeux != "Indéfini"?String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(projet.datedeux)) : "Indéfini"), font, XBrushes.Black, layoutRectangle);
        }

        private void CorpsAtomiqueD(ref PdfPage page, ref XTextFormatter formatter, ref XFont font, XRect layoutRectangle, ref int height, Paie paie, int tB, int tHS, int tP, int tI)
        {
            layoutRectangle = new XRect(25, height += 15, page.Width, page.Height);
            formatter.DrawString(String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(paie.DateGenerationRapport)), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(116, height, page.Width, page.Height);
            formatter.DrawString(paie.Nom.Substring(0, 1).ToString() + "." + paie.Nom.Substring(paie.Nom.IndexOf(' ')).ToString(), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(310, height, page.Width, page.Height);
            formatter.DrawString(paie.ID.ToString(), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(362, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tB+"}",Math.Round(Convert.ToSingle(paie.MontantBrute), 2)) + "$", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(447, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tHS+"}",Math.Round(Convert.ToSingle(paie.NombreHeureSupp), 2).ToString()), font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(530, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tP+"}",Math.Round(Convert.ToSingle(paie.MontantPourboire), 2).ToString()) + "$", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(613, height, page.Width, page.Height);
            formatter.DrawString(String.Format("{0,"+tI+"}",Math.Round(Convert.ToSingle(paie.MontantIndemnite), 2).ToString()) + "$", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(676, height, page.Width, page.Height);
            formatter.DrawString(Math.Round(Convert.ToSingle(paie.salaire), 2).ToString() + "$/h", font, XBrushes.Black, layoutRectangle);
            layoutRectangle = new XRect(759, height, page.Width, page.Height);
            formatter.DrawString(Math.Round(Convert.ToSingle(paie.NombreHeure), 2).ToString(), font, XBrushes.Black, layoutRectangle);
        }

        private int VerifeTaille (int tObjet, int tComparer)
        {
            if (tObjet > tComparer)
                tComparer = tObjet;

            return tComparer;
        }
    }
}
