using System;
using System.Collections.Generic;
using System.Linq;
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
using model.Models;
using model.Service;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutProjet.xaml
    /// </summary>
    public partial class GestionProjetView : UserControl
    {
        private ProjetView _projet;

        public GestionProjetView()
        {
            InitializeComponent();
            DataContext = this;         
        }

        public GestionProjetView(IDictionary<string,object> parameters):this()
        {
            Projet = parameters["Projet"] as ProjetView;
        }

        

        /// <summary>
        /// Sets and gets the Propriete property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ProjetView Projet
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
                _projet = value;
            }
        }

        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<ProjetView>(new ProjetView());
        }

        private void EnregistrerProjet(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(txtNomProjet.Text + " à bien été enregistré." );
            //Enregistre les données et retourne à l'écran d'avant
            retourMenu(this,null);
        }
    }
}
