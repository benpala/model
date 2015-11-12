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
        private IEmployeService _ServiceEmploye;
        private IProjetService _ServiceProjet;
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
            DataContext = this;

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
            if (Convert.ToSingle(lstRapportEmploye.SelectedItems.Count.ToString()) > 0)
            {
                if (chxProjetEmploye.IsChecked.Value || chxInfoGenerale.IsChecked.Value || chxDetailFinanciere.IsChecked.Value)
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
                        var layoutRectangle = new XRect(10, height += 15, page.Width , page.Height );
                        formatter.DrawString((emp.Prenom.ToString() + " " + emp.Nom.ToString()), font, XBrushes.Black, layoutRectangle);

                        if (chxInfoGenerale.IsChecked.Value)
                        {
                            layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                            formatter.DrawString(("Titre d'emploi : " + emp.Poste.ToString()), font, XBrushes.Black, layoutRectangle);
                            
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
                        }

                        if (chxDetailFinanciere.IsChecked.Value)
                        {
                            layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                            formatter.DrawString(("Saliare par l'heure : " + Math.Round(Convert.ToSingle(emp.Salaire), 2) + " $"), font, XBrushes.Black, layoutRectangle);
                            layoutRectangle = new XRect(20, height += 15, page.Width, page.Height);
                            formatter.DrawString(("Saliare par l'heures supplémentaire : " + Math.Round( Convert.ToSingle(emp.SalaireOver), 2) + " $"), font, XBrushes.Black, layoutRectangle);
                        }

                        if (chxProjetEmploye.IsChecked.Value)
                        {
                            Liaison = new List<LiaisonProjetEmploye>(_ServiceMysql.GetLiaison(emp.ID));
                            int count = 0;
                            int x = 10;
                            int y = height + 15;
                            if (Liaison.Count > 0)
                            {
                                layoutRectangle = new XRect(x, height += 15, page.Width, page.Height);
                                formatter.DrawString(("Projet(s) occupé(s) : "), font, XBrushes.Black, layoutRectangle);
                                foreach (LiaisonProjetEmploye liaison in Liaison)
                                {
                                    count++;
                                    if (count % 3 == 0)
                                    {
                                        x += 125;
                                        y -= 45;
                                    }
                                    layoutRectangle = new XRect(x, y += 15, page.Width, page.Height);
                                    formatter.DrawString(count + " : " + liaison.ProjNom.ToString(), font, XBrushes.Black, layoutRectangle);
                                }
                                height += 30;
                                if (y > height)
                                    height = y;
                            }
                        }
                        graph.DrawLine(XPens.Black, 10, height+=20 , 500, height);
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

        private void ChoisirTousEmploye(object sender, RoutedEventArgs e)
        {
            lstRapportEmploye.SelectAll();
            
        }

        private void GenererRapportFinancie(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("PDF");
        }

        private void ChoisirTousProjet(object sender, RoutedEventArgs e)
        {
            lstRapportProjet.SelectAll();
            
        }
    }
}
