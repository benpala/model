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
                            formatter.DrawString("Information général", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(178, height + 20, page.Width, page.Height);
                            formatter.DrawString("-Titre emploi", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(178, height + 30, page.Width, page.Height);
                            formatter.DrawString("-'Date embauche'", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(178, height + 40, page.Width, page.Height);
                            formatter.DrawString("-(Status)", font, XBrushes.Black, layoutRectangle);
                        
                            graph.DrawRectangle(XPens.Black, new XRect(295, height + 5, 130, 60));
                            layoutRectangle = new XRect(298, height + 5, page.Width, page.Height);
                            formatter.DrawString("Détail financiés", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(298, height + 20, page.Width, page.Height);
                            formatter.DrawString("-Saliare par l'heures", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(298, height + 30, page.Width, page.Height);
                            formatter.DrawString("-(Saliare par l'heures", font, XBrushes.Black, layoutRectangle);

                            layoutRectangle = new XRect(298, height + 40, page.Width, page.Height);
                            formatter.DrawString(" supplémentaire)", font, XBrushes.Black, layoutRectangle);
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
                                int count = 0;
                                
                                if (Liaison.Count > 0)
                                {
                                    //Tri nom projet
                                    if ((bool)rbtTriProjetAZ.IsChecked)
                                        Liaison = Liaison.OrderBy(lst => lst.ProjNom).ToList();
                                    else if ((bool)rbtTriProjetZA.IsChecked)
                                        Liaison = Liaison.OrderByDescending(lst => lst.ProjNom).ToList();
                                    
                                    //Tri nb heure
                                    if ((bool)rbtTriNBheureASC.IsChecked)
                                        Liaison = Liaison.OrderBy(lst => lst.ProjNom).ToList();
                                    else if ((bool)rbtTriNBheureDESC.IsChecked)
                                        Liaison = Liaison.OrderByDescending(lst => lst.ProjNom).ToList();
                                    foreach (LiaisonProjetEmploye liaison in Liaison)
                                    {
                                        count++;
                                        layoutRectangle = new XRect(425, y += 15, page.Width, page.Height);
                                        formatter.DrawString(count + " - " + liaison.ProjNom.ToString(), font, XBrushes.Black, layoutRectangle);

                                        layoutRectangle = new XRect(605 , y, page.Width, page.Height);
                                        formatter.DrawString( _ServiceMysql.CompteursProjets(emp.ID,liaison.ProjNom) + " heures", font, XBrushes.Black, layoutRectangle);

                                    }
                                    if (y > height)
                                        height = y;
                                }
                            }
                            height += 30; 
                            graph.DrawLine(XPens.Black, 10, height+=20 , 800, height);

                        }
                        string pdfFilename = "RapportEmploye.pdf";
                        pdf.Save(pdfFilename);
                        Process.Start(pdfFilename);
                    }
                    else
                        MessageBox.Show("Aucune information choisi");
                }
                else
                    MessageBox.Show("Ancune employé selectionné");
            }
            catch (Exception E)
            {
                 MessageBox.Show("Le fichier est déja utilisé, veuillez le fermer pour le regénérer");
            }
        }

        private void ChoisirTousEmploye(object sender, RoutedEventArgs e)
        {
            lstRapportEmploye.SelectAll();
            
        }

        private void GenererRapportFinancie(object sender, RoutedEventArgs e)
        {
            try 
            {
                PeriodePaie tmp = new PeriodePaie(Convert.ToDateTime(dtDateDebut.Text), Convert.ToDateTime(dtDateFin.Text));
                float BruteTotal = 0;

                PdfDocument pdf = new PdfDocument();
                pdf.Info.Title = "Rapport Paie";
                PdfPage page;
                page = pdf.AddPage();
                page.Orientation = PageOrientation.Portrait;
                page.Size = PageSize.A4;
                XGraphics graph = XGraphics.FromPdfPage(page);

                XFont font = new XFont("Arial", 12.0); //new XFont("Verdana", 20, XFontStyle.Bold);
                var formatter = new XTextFormatter(graph);
                int height = 0;

                var layoutRectangle = new XRect(10, height += 15, page.Width, page.Height);

                foreach(Paie paie in Paie)
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
                        layoutRectangle = new XRect(10, height += 15, page.Width, page.Height);
                        formatter.DrawString((paie.DateGenerationRapport.ToString() + " " + paie.Nom.ToString()+" "+paie.ID), font, XBrushes.Black, layoutRectangle);
                        layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                        formatter.DrawString(("Montant Brute : " + Math.Round(Convert.ToSingle(paie.MontantBrute), 2) + " $"), font, XBrushes.Black, layoutRectangle);
                        BruteTotal = BruteTotal + paie.MontantBrute;
                    }

                    graph.DrawLine(XPens.Black, 10, height += 20, 500, height);
                }

                if(BruteTotal == 0)
                    throw new Exception("erreur");

                layoutRectangle = new XRect(10, height += 15, page.Width, page.Height);
                formatter.DrawString(("Cout Pour la période : " +tmp.Debut+" à "+tmp.Fin+" Montant : "+Math.Round(Convert.ToSingle(BruteTotal), 2) + " $"), font, XBrushes.Black, layoutRectangle);

                string pdfFilename = "RapportPaie.pdf";
                pdf.Save(pdfFilename);
                Process.Start(pdfFilename);
            }
            catch(Exception E)
            { 
                if(E.Message == "erreur")
                    MessageBox.Show("Aucune paie durant cette période");
                else if(E.Message == "Le processus ne peut pas accéder au fichier 'D:\\model\\model\\bin\\Debug\\RapportPaie.pdf', car il est en cours d'utilisation par un autre processus.")
                    MessageBox.Show("Le fichier est déja utilisé, veuillez le fermer pour le regenerer");
                else
                    MessageBox.Show("Veuillez entrer des dates");
                    
            }
        }

        private void ChoisirTousProjet(object sender, RoutedEventArgs e)
        {
            lstRapportProjet.SelectAll();
            
        }
    }
}
