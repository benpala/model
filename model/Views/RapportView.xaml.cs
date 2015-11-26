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
            try 
            {
                if (Convert.ToSingle(lstRapportEmploye.SelectedItems.Count.ToString()) > 0)
                {
                    if (chxProjetEmploye.IsChecked.Value || chxInfoGenerale.IsChecked.Value)
                    {
                        PdfDocument pdf = new PdfDocument();
                        pdf.Info.Title = "Rapport Employe";
                        PdfPage page;
                        page = pdf.AddPage();
                        page.Orientation = PageOrientation.Portrait;
                        page.Size = PageSize.A4;
                        XGraphics graph = XGraphics.FromPdfPage(page);
                        List<LiaisonProjetEmploye> Liaison = new List<LiaisonProjetEmploye>();
                        MySqlEmployeService _ServiceMysql = new MySqlEmployeService();


                        XFont font = new XFont("Arial", 12.0); //new XFont("Verdana", 20, XFontStyle.Bold);
                        var formatter = new XTextFormatter(graph);
                        int height = 0;
                        foreach (Employe emp in lstRapportEmploye.SelectedItems)
                        {
                    
                        if (height > 700)
                            {
                                page = pdf.AddPage();
                                graph = XGraphics.FromPdfPage(page);
                                formatter = new XTextFormatter(graph);
                                height = 0;
                            }
                            int y = height;
                            var layoutRectangle = new XRect(10, height += 15, page.Width , page.Height );
                            formatter.DrawString((emp.Prenom.ToString() + " " + emp.Nom.ToString()), font, XBrushes.Black, layoutRectangle);
                            //Section info employé
                            if (chxInfoGenerale.IsChecked.Value)
                            {
                                layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                                formatter.DrawString(("Titre d'emploi : " + emp.Poste.ToString()), font, XBrushes.Black, layoutRectangle);

                                layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                                formatter.DrawString(("Employeur : " + _ServiceMysql.getEmployeur(emp.ID)), font, XBrushes.Black, layoutRectangle);

                                layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                                formatter.DrawString(("Date embauche : " + _ServiceMysql.getDateEmbauche(emp.ID)), font, XBrushes.Black, layoutRectangle);

                                if (!emp.HorsFonction)
                                {
                                    layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                                    formatter.DrawString("Status : En service", font, XBrushes.Black, layoutRectangle);
                                }
                                else
                                {
                                    layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                                    formatter.DrawString("Status : Hors service", font, XBrushes.Black, layoutRectangle);
                                }
                            
                                layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                                formatter.DrawString(("Saliare par l'heure : " + Math.Round(Convert.ToSingle(emp.Salaire), 2) + " $"), font, XBrushes.Black, layoutRectangle);
                                layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                                formatter.DrawString(("Saliare par l'heures supplémentaire : " + Math.Round( Convert.ToSingle(emp.SalaireOver), 2) + " $"), font, XBrushes.Black, layoutRectangle);
                            }
                            //Section projets des employés
                            if (chxProjetEmploye.IsChecked.Value)
                            {
                                Liaison = new List<LiaisonProjetEmploye>(_ServiceMysql.GetLiaison(emp.ID));
                                int count = 0;
                                int x = 300;
                                if (Liaison.Count > 0)
                                {
                                    layoutRectangle = new XRect(x, y += 15, page.Width, page.Height);
                                    formatter.DrawString(("Projet(s) occupé(s) : "), font, XBrushes.Black, layoutRectangle);
                                    layoutRectangle = new XRect(x + 180, y , page.Width, page.Height);
                                    formatter.DrawString(("Temps travaillé : "), font, XBrushes.Black, layoutRectangle);
                                    foreach (LiaisonProjetEmploye liaison in Liaison)
                                    {
                                        count++;
                                        layoutRectangle = new XRect(x, y += 15, page.Width, page.Height);
                                        formatter.DrawString(count + " : " + liaison.ProjNom.ToString(), font, XBrushes.Black, layoutRectangle);

                                        layoutRectangle = new XRect(x + 180, y , page.Width, page.Height);
                                        formatter.DrawString( _ServiceMysql.CompteursProjets(emp.ID,liaison.ProjNom) + " heures", font, XBrushes.Black, layoutRectangle);

                                    }
                                    height += 30;
                                    if (y > height)
                                        height = y;
                                }
                            }
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
                if (E.Message == "erreur")
                    MessageBox.Show("Aucune paie durant cette période");
                else if (E.Message == "Le processus ne peut pas accéder au fichier 'D:\\model\\model\\bin\\Debug\\RapportEmploye.pdf', car il est en cours d'utilisation par un autre processus.")
                    MessageBox.Show("Le fichier est déja utilisé, veuillez le fermer pour le regénérer");
                else
                    MessageBox.Show("Veuillez entrer des dates");

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
                page.Orientation = PageOrientation.Landscape;
                page.Size = PageSize.A4;
                XGraphics graph = XGraphics.FromPdfPage(page);

                XFont font = new XFont("Consolas", 12.0); //new XFont("Verdana", 20, XFontStyle.Bold);
                XTextFormatter formatter = new XTextFormatter(graph);
                int height = 0;

                XRect layoutRectangle = new XRect(10, height += 15, page.Width, page.Height);

                EnteteDPF(ref formatter, ref font, layoutRectangle, tmp);

                graph.DrawLine(XPens.Black, 10, 50, page.Width - 10, 50);

                MessageBox.Show(page.Height.ToString());
                MessageBox.Show(page.Width.ToString());

                //Étiquette
                graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 55, page.Width - 730, page.Height - 547);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 215, 55, page.Width - 638, page.Height - 547);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 420, 55, page.Width - 636, page.Height - 547);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 627, 55, page.Width - 638, page.Height - 547);

                /*layoutRectangle = new XRect(25, 60, page.Width - 447, page.Height - 820);
                formatter.DrawString("Date Génération Paie", font, XBrushes.Black, layoutRectangle);
                layoutRectangle = new XRect(185, 60, page.Width - 447, page.Height - 820);
                formatter.DrawString("Nom Employé", font, XBrushes.Black, layoutRectangle);
                layoutRectangle = new XRect(345, 60, page.Width - 447, page.Height - 820);
                formatter.DrawString("ID Paie", font, XBrushes.Black, layoutRectangle);
                layoutRectangle = new XRect(474, 60, page.Width - 447, page.Height - 820);
                formatter.DrawString("Montant Brut", font, XBrushes.Black, layoutRectangle);*/
                //Contenu
                graph.DrawRectangle(XPens.Black, XBrushes.White, 10, 104, page.Width - 730, page.Height - 200);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 215, 104, page.Width - 638, page.Height - 200);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 420, 104, page.Width - 636, page.Height - 200);
                graph.DrawRectangle(XPens.Black, XBrushes.White, 627, 104, page.Width - 638, page.Height - 200);

                layoutRectangle = new XRect(10, height += 75, page.Width, page.Height);

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
                        layoutRectangle = new XRect(30, height += 15, page.Width, page.Height);
                        formatter.DrawString(String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(paie.DateGenerationRapport)), font, XBrushes.Black, layoutRectangle);
                        layoutRectangle = new XRect(260, height, page.Width, page.Height);
                        formatter.DrawString(paie.Nom.ToString(), font, XBrushes.Black, layoutRectangle);
                        layoutRectangle = new XRect(510, height, page.Width, page.Height);
                        formatter.DrawString(paie.ID.ToString(), font, XBrushes.Black, layoutRectangle);
                        layoutRectangle = new XRect(700, height , page.Width, page.Height);
                        formatter.DrawString(Math.Round(Convert.ToSingle(paie.MontantBrute), 2) + " $", font, XBrushes.Black, layoutRectangle);
                        BruteTotal = BruteTotal + paie.MontantBrute;
                    }

                }

                if (BruteTotal == 0)
                    throw new Exception("erreur");

                layoutRectangle = new XRect(10, height += 15, page.Width, page.Height);
                formatter.DrawString(("Cout Pour la période : " + tmp.Debut + " à " + tmp.Fin + " Montant : " + Math.Round(Convert.ToSingle(BruteTotal), 2) + " $"), font, XBrushes.Black, layoutRectangle);

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

        private void EnteteDPF(ref XTextFormatter formatter, ref XFont font, XRect layoutRectangle, PeriodePaie tmp)
        {
            DateTime now = DateTime.Now;

            layoutRectangle = new XRect(25, 25, 200, 25);
            formatter.DrawString((nomEntreprise + "\t\t\t\t\t Rapport de Paies"), font, XBrushes.Black, layoutRectangle);

            layoutRectangle = new XRect(300, 25, 300, 25);
            formatter.DrawString(("Date de Génération : " + now.ToString()), font, XBrushes.Black, layoutRectangle);
        }
    }
}
