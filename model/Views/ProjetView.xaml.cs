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
using System.Windows.Shapes;
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
        private ObservableCollection<Projet> projetCopie = new ObservableCollection<Projet>();

        public ProjetView()
        {
            InitializeComponent();
            DataContext = this;
            RetrieveArgs = new RetrieveProjetArgs();
            _ServiceProjet = ServiceFactory.Instance.GetService<IProjetService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Projet = new ObservableCollection<Projet>(_ServiceProjet.retrieveAll());
            foreach(Projet p in Projet)
            {
                if(p.etat == "ABD")
                { 
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString("#FFFFFF90");
                    mygrid.RowBackground = brush;
                }
                if (p.etat == "SIM")
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString("#3406FF");
                    mygrid.RowBackground = brush;
                }
                if (p.etat == "ENC")
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString("#06FF2F");
                    mygrid.RowBackground = brush;
                }
                if (p.etat == "END")
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString("#FF060E");
                    mygrid.RowBackground = brush;
                }
            }
            projetCopie = Projet;
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
            applicationService.ChangeView<GestionProjetView>(new GestionProjetView());
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

        //private void txtRechercheDateDebut_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox tb = (TextBox)sender;
        //    tb.Text = string.Empty;
        //    tb.Foreground = Brushes.Black;
        //    tb.GotFocus -= txtRechercheDateDebut_GotFocus;
        //}

        //private void txtRechercheDateFin_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox tb = (TextBox)sender;
        //    tb.Text = string.Empty;
        //    tb.Foreground = Brushes.Black;
        //    tb.GotFocus -= txtRechercheDateFin_GotFocus;
        //}

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
            }

            if(arguments.Count == 0 && donnees.Count == 0)
            {
                Projet = projetCopie;
            }
            else 
            {
                Projet = new ObservableCollection<Projet>(_ServiceProjet.retrieveAll(arguments,donnees));     
            }
        }

        private void txtRechercheNbHeures_textChanged(object sender, TextChangedEventArgs e)
        {
            // cherche dans l'observable collection et affiche ceux qui sont valide

        }

        private void dtpDebutDate_DateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

   }
}
