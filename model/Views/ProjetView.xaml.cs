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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using model.Models;
using model.Models.Args;
using model.Service;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour projet.xaml
    /// </summary>
    public partial class ProjetView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private IProjetService _ServiceProjet;
        private IApplicationService _applicationService;

        public RetrieveProjetArgs RetrieveArgs { get; set;}
        private ObservableCollection<Projet> _projet = new ObservableCollection<Projet>();

        public ProjetView()
        {
            InitializeComponent();
            DataContext = this;
            RetrieveArgs = new RetrieveProjetArgs();
            _ServiceProjet = ServiceFactory.Instance.GetService<IProjetService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Projet = new ObservableCollection<Projet>(_ServiceProjet.retrieveAll());
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

        private void click_newProject(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<GestionProjetView>(new GestionProjetView(true));
        }

        private void click_modifierProject(object sender, RoutedEventArgs e)
        {
            var Projet = (Projet)((sender as Button).CommandParameter);

            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Dictionary<string,object> parametres = new Dictionary<string,object>() { { "Projet" , Projet} };
            applicationService.ChangeView<GestionProjetView>(new GestionProjetView(parametres));
            
        }

        private void txtRechercheID_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = Brushes.Black;
            tb.GotFocus -= txtRechercheID_GotFocus;
        }

        private void txtRechercheNomProjet_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = Brushes.Black;
            tb.GotFocus -= txtRechercheNomProjet_GotFocus;
        }

        private void txtRechercheNbHeures_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = Brushes.Black;
            tb.GotFocus -= txtRechercheNbHeures_GotFocus;
        }

        private void txtRecherche_textChanged(object sender, TextChangedEventArgs e)
        {
            List<string> arguments = new List<string>();
            List<string> donnees = new List<string>();
            int compteur = 0;
            try { 
                foreach (Control ctrl in ProjetGrid.Children)
                {
                    if (ctrl.GetType() == typeof(TextBox))
                    {
                        if (((TextBox)ctrl).Text != "ID" && ((TextBox)ctrl).Text != "Nom du projet" &&
                         ((TextBox)ctrl).Text != "Date de début" && ((TextBox)ctrl).Text != "Date de fin" && ((TextBox)ctrl).Text != "Nb heures" && ((TextBox)ctrl).Text != "")
                        {
                            arguments.Add(((TextBox)ctrl).Name.Substring(13));
                            donnees.Add(((TextBox)ctrl).Text);
                        }
                    }
                    if (ctrl.GetType() == typeof(CheckBox))
                    {
                        if (((CheckBox)ctrl).IsChecked == true)
                        {
                            arguments.Add("etat");
                            donnees.Add(((CheckBox)ctrl).Name.Substring(3));
                        }
                    }
                    if (ctrl.GetType() == typeof(DatePicker))
                    {
                        if (((DatePicker)ctrl).SelectedDate != null)
                        {
                            arguments.Add(ctrl.Name.Substring(13));
                            donnees.Add(((DatePicker)ctrl).SelectedDate.ToString());
                        }
                    }
                }
            } 
            catch
            {
                if(stackEtat1 != null &&  stackEtat2 != null)
                { 
                    foreach (Control ctrl in stackEtat1.Children)
                    {
                        if (ctrl.GetType() == typeof(CheckBox))
                        {
                            if (((CheckBox)ctrl).IsChecked == true)
                            {
                                arguments.Add("etat");
                                donnees.Add(((CheckBox)ctrl).Name.Substring(3));
                            }
                        }
                    }
                    foreach (Control ctrl in stackEtat2.Children)
                    {
                        if (ctrl.GetType() == typeof(CheckBox))
                        {
                            if (((CheckBox)ctrl).IsChecked == true)
                            {
                                arguments.Add("etat");
                                donnees.Add(((CheckBox)ctrl).Name.Substring(3));
                            }
                        }
                    } 
                }
            }
            if (arguments.Count == 0 && donnees.Count == 0)
            {
                foreach (Projet p in Projet)
                {
                    DataGridRow row = (DataGridRow)mygrid.ItemContainerGenerator.ContainerFromIndex(compteur);
                    row.Visibility = Visibility.Visible;
                    compteur++;
                }
            }
            else
            {
                foreach (Projet p in Projet)
                {
                    DataGridRow row = (DataGridRow)mygrid.ItemContainerGenerator.ContainerFromIndex(compteur);
                    int compteur2 = 0;
                    bool valide = true;
                    if (rboEt.IsChecked == false)
                        valide = false;

                    foreach (string args in arguments)
                    {
                        switch (args)
                        {
                            case "idProjet":
                                {
                                    if (rboEt.IsChecked == true)
                                    {
                                        if (p.ID.IndexOf(donnees[compteur2]) == -1)
                                            valide = false;
                                    }
                                    else
                                    {
                                        if (p.ID.IndexOf(donnees[compteur2]) != -1)
                                            valide = true;
                                    }
                                    break;
                                }
                            case "nom":
                                {
                                    if (rboEt.IsChecked == true)
                                    {
                                        if (p.nom.IndexOf(donnees[compteur2], StringComparison.CurrentCultureIgnoreCase) == -1)
                                            valide = false;
                                    }
                                    else
                                    {
                                        if (p.nom.IndexOf(donnees[compteur2], StringComparison.CurrentCultureIgnoreCase) != -1)
                                            valide = true;
                                    }
                                    break;
                                }
                            case "nbHeures":
                                {
                                    if (rboEt.IsChecked == true)
                                    {
                                        if (p.nbHeuresSimule.ToString().IndexOf(donnees[compteur2]) == -1 &&
                                                p.nbHeuresReel.ToString().IndexOf(donnees[compteur2]) == -1)
                                            valide = false;
                                    }
                                    else
                                    {
                                        if (p.nbHeuresSimule.ToString().IndexOf(donnees[compteur2]) != -1 &&
                                                p.nbHeuresReel.ToString().IndexOf(donnees[compteur2]) != -1)
                                            valide = true;
                                    }
                                    break;
                                }
                            case "etat":
                                {
                                    if (rboEt.IsChecked == true)
                                    {
                                        if (!donnees.Contains(p.etat))
                                            valide = false;
                                    }
                                    else
                                    {
                                        if (donnees.Contains(p.etat))
                                            valide = true;
                                    }
                                    break;
                                }
                            case "coutEstime":
                                {
                                    if (rboEt.IsChecked == true)
                                    {
                                        if (p.prixSimulation.ToString().IndexOf(donnees[compteur2]) == -1)
                                            valide = false;
                                    }
                                    else
                                    {
                                        if (p.prixSimulation.ToString().IndexOf(donnees[compteur2]) != -1)
                                            valide = true;
                                    }
                                    break;
                                }
                            case "coutReel":
                                {
                                    if (rboEt.IsChecked == true)
                                    {
                                        if (p.prixReel.ToString().IndexOf(donnees[compteur2]) == -1)
                                            valide = false;
                                    }
                                    else
                                    {
                                        if (p.prixReel.ToString().IndexOf(donnees[compteur2]) != -1)
                                            valide = true;
                                    }
                                    break;
                                }

                            default:
                                {
                                    if (dtpRecherche_dateDebut.SelectedDate != null && dtpRecherche_dateFin.SelectedDate != null)
                                    {
                                        if (compteur2 + 1 < donnees.Count && arguments[compteur2 + 1] != "etat" && arguments[compteur2 + 1] != "nbHeures")
                                        {
                                            DateTime d1 = Convert.ToDateTime(donnees[compteur2]);
                                            DateTime d2 = Convert.ToDateTime(donnees[compteur2 + 1]);
                                            DateTime targetDtDebut = Convert.ToDateTime(p.dateun);
                                            DateTime targetDtFin;
                                            if (p.datedeux != "Indéfini")
                                                targetDtFin = Convert.ToDateTime(p.datedeux);
                                            else
                                                targetDtFin = Convert.ToDateTime(new DateTime(9999, 01, 01));

                                            if (rboEt.IsChecked == true)
                                            {
                                                if (targetDtDebut.Ticks < d1.Ticks || targetDtDebut.Ticks > d2.Ticks && targetDtFin.Ticks < d1.Ticks || targetDtFin.Ticks > d2.Ticks)
                                                {
                                                    valide = false;
                                                }
                                            }
                                            else
                                            {
                                                if (targetDtDebut.Ticks > d1.Ticks && targetDtFin.Ticks < d2.Ticks)
                                                {
                                                    valide = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dtpRecherche_dateDebut.SelectedDate != null)
                                        {
                                            if (rboEt.IsChecked == true)
                                            {
                                                if (p.dateun.IndexOf(donnees[compteur2]) == -1)
                                                    valide = false;
                                            }
                                            else
                                            {
                                                if (p.dateun.IndexOf(donnees[compteur2]) != -1)
                                                    valide = true;
                                            }
                                        }
                                        if (dtpRecherche_dateFin.SelectedDate != null)
                                        {
                                            if (rboEt.IsChecked == true)
                                            {
                                                if (p.datedeux.IndexOf(donnees[compteur2]) == -1)
                                                    valide = false;
                                            }
                                            else
                                            {
                                                if (p.datedeux.IndexOf(donnees[compteur2]) != -1)
                                                    valide = false;
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                        if (!valide)
                            row.Visibility = Visibility.Collapsed;
                        else
                            row.Visibility = Visibility.Visible;
                        compteur2++;
                    }
                    compteur++;
                }
            }

        }

        private void changeDate(object sender, SelectionChangedEventArgs e)
        {
            txtRecherche_textChanged(null, null);
        }

        private void filtreEtat(object sender, RoutedEventArgs e)
        {
            txtRecherche_textChanged(null,null);
        }

        private void EffaceDate(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Back && ((DatePicker)sender).Text == "")
            {
                ((DatePicker)sender).SelectedDate = null;
                txtRecherche_textChanged(null, null);
            }
                
        }

        private void RefreshListe(object sender, RoutedEventArgs e)
        {
                int compteur = 0;
                foreach (Projet p in Projet)
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    DataGridRow row = (DataGridRow)mygrid.ItemContainerGenerator.ContainerFromIndex(compteur);

                        if (p.etat == "ABD")
                        {
                            var brush = (Brush)converter.ConvertFromString("#FF4D4D");
                            row.Background = brush;
                        }
                        if (p.etat == "SIM")
                        {
                            var brush = (Brush)converter.ConvertFromString("#99ddff");
                            row.Background = brush;
                        }
                        if (p.etat == "ECS")
                        {
                            var brush = (Brush)converter.ConvertFromString("#99ff99");
                            row.Background = brush;
                        }
                        if (p.etat == "TER")
                        {
                            var brush = (Brush)converter.ConvertFromString("#cccccc");
                            row.Background = brush;
                        }
                    compteur++;
                }
                compteur = 0; 
        }

        private void columnHeader_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if (columnHeader.Tag == null || columnHeader.Tag.ToString() == "Descending")
                columnHeader.Tag = "Ascending";
            else
                columnHeader.Tag = "Descending";
            if (columnHeader != null)
            {
                switch (columnHeader.Content.ToString())
                {
                    case "ID":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => int.Parse(i.ID)));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => int.Parse(i.ID)));

                            break;
                        }

                    case "Nom":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.nom));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.nom));
                            break;
                        }

                    case "Début":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.dateun));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.dateun));

                            break;
                        }

                    case "Fin":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.datedeux));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.datedeux));
                            break;
                        }

                    case "Temps Estimé":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.nbHeuresSimule));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.nbHeuresSimule));
                            break;
                        }

                    case "Temps réel":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.nbHeuresReel));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.nbHeuresReel));
                            break;
                        }

                    case "Coût estimé":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.prixSimulation));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.prixSimulation));
                            break;
                        }

                    case "Coût réel":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.prixReel));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.prixReel));
                            break;
                        }

                    case "Employés":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.nbEmploye));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.nbEmploye));
                            break;
                        }

                    case "État":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderBy(i => i.etat));
                            else
                                Projet = new ObservableCollection<Models.Projet>(_projet.OrderByDescending(i => i.etat));
                            break;
                        }
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    int compteur = 0;
                    foreach (Projet p in Projet)
                    {

                        var converter = new System.Windows.Media.BrushConverter();
                        DataGridRow row = (DataGridRow)mygrid.ItemContainerGenerator.ContainerFromIndex(compteur);

                        if (p.etat == "ABD")
                        {
                            var brush = (Brush)converter.ConvertFromString("#FF4D4D");
                            row.Background = brush;
                        }
                        if (p.etat == "SIM")
                        {
                            var brush = (Brush)converter.ConvertFromString("#99ddff");
                            row.Background = brush;
                        }
                        if (p.etat == "ECS")
                        {
                            var brush = (Brush)converter.ConvertFromString("#99ff99");
                            row.Background = brush;
                        }
                        if (p.etat == "TER")
                        {
                            var brush = (Brush)converter.ConvertFromString("#cccccc");
                            row.Background = brush;
                        }
                        compteur++;
                    }
                    compteur = 0;
                }));
            }
        }

   }
}
