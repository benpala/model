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
            applicationService.ChangeView<GestionProjetView>(new GestionProjetView());
        }

        private void click_modifierProject(object sender, RoutedEventArgs e)
        {
            var Projet = (Projet)((sender as Button).CommandParameter);

            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Dictionary<string,object> parametres = new Dictionary<string,object>() { { "Projet" , Projet} };
            applicationService.ChangeView<GestionProjetView>(new GestionProjetView(parametres));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var filtre = (TextBox)(sender as Button);
            MessageBox.Show(/*filtre.Text*/"");
        }

        private void btnRecherche_Click(object sender, RoutedEventArgs e)
        {
            Window Fenetre = new FenetreRecherche();
            Fenetre.ShowDialog();
        }
    }
}
