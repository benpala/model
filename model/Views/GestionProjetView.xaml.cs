using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using model.Service.MySql;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutProjet.xaml
    /// </summary>
    public partial class GestionProjetView : UserControl
    {
        private Projet _projet;
        const float SALAIREADMIN = 75;
        const float SALAIRESENIOR = 50;
        const float SALAIREJUNIOR = 25;
        MySqlProjetService Requete = new MySqlProjetService();

        public GestionProjetView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public GestionProjetView(bool creation)
        {
            InitializeComponent();
            DataContext = this;
            _projet = new Projet();
            _projet.ID = Requete.dernierId();
            _projet.dateun = DateTime.Now.ToString();
            dtDateFin.DisplayDateStart = dtDateDebut.SelectedDate;
            gridEmployeProjet.Visibility = Visibility.Hidden; 
            lblEmployesProjet.Visibility = Visibility.Hidden;
        }

        public GestionProjetView(IDictionary<string,object> parameters):this()
        {
            lblRessourcesAdmin.Visibility = Visibility.Hidden;
            lblRessourcesSenior.Visibility = Visibility.Hidden;
            lblRessourcesJunior.Visibility = Visibility.Hidden;
            txtRessourcesAdmin.Visibility = Visibility.Hidden;
            txtRessourcesSenior.Visibility = Visibility.Hidden;
            txtRessourcesJunior.Visibility = Visibility.Hidden;
            lblPrixEstimation.Visibility = Visibility.Hidden;
            txtPrixEstimation.Visibility = Visibility.Hidden;
            lblNbrHeuresEstime.Visibility = Visibility.Hidden;
            txtNbrHeuresEstime.Visibility = Visibility.Hidden;

            Projet = parameters["Projet"] as Projet;
            if (Projet.etat == "ABD")
                rboABD.IsChecked = true;
            if (Projet.etat == "ECS")
                rboECS.IsChecked = true;
            if (Projet.etat == "END")
                rboEND.IsChecked = true;
            if (Projet.etat == "SIM")
                rboSIM.IsChecked = true;
        }

        

        /// <summary>
        /// Sets and gets the Propriete property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Projet Projet
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
            int nombreTexte = txtPrixEstimation.Text.Length;
            string prixEstime = txtPrixEstimation.Text.Substring(0,nombreTexte-1);
            float prixSimule = float.Parse(prixEstime);
            
            // créer l'objet projet puis insérer en bd
            try 
            { 
                _projet.ID = txtID.Text;
                _projet.nom = txtNomProjet.Text;
                _projet.dateun = "'" + dtDateDebut.SelectedDate.ToString() + "'";
                _projet.datedeux = "'" + dtDateFin.SelectedDate.ToString() + "'";
                _projet.prixSimulation = prixSimule;
                _projet.prixReel = 0;
                _projet.nbHeuresReel = 0;
                _projet.nbHeuresSimule = int.Parse(txtNbrHeuresEstime.Text);
                if(rboABD.IsChecked == true)
                {
                    _projet.etat = "ABD";
                }
                if (rboECS.IsChecked == true)
                {
                    _projet.etat = "ECS";
                }
                if (rboSIM.IsChecked == true)
                {
                    _projet.etat = "SIM";
                }
                if (rboEND.IsChecked == true)
                {
                    _projet.etat = "END";
                }
                Requete.create(_projet);
                MessageBox.Show(txtNomProjet.Text + " à bien été enregistré.");
                retourMenu(this, null);
            } catch(Exception)
            {
                MessageBox.Show(txtNomProjet.Text + " n'à pas été enregistré.");
            }
        }

        private void CalculerPrix(object sender, TextChangedEventArgs e)
        {
            float prixEstimer = 0;
            
            if(txtRessourcesAdmin.Text != "" && txtNbrHeuresEstime.Text != "")
            {
             prixEstimer += (float.Parse(txtRessourcesAdmin.Text) * SALAIREADMIN) * float.Parse(txtNbrHeuresEstime.Text);
            }

            if(txtRessourcesSenior.Text != "" && txtNbrHeuresEstime.Text != "")
            {
                prixEstimer += (float.Parse(txtRessourcesSenior.Text) * SALAIRESENIOR) * float.Parse(txtNbrHeuresEstime.Text);
            }

            if(txtRessourcesJunior.Text != "" && txtNbrHeuresEstime.Text != "")
            {
                prixEstimer += (float.Parse(txtRessourcesJunior.Text) * SALAIREJUNIOR) * float.Parse(txtNbrHeuresEstime.Text);
            }

            if(txtNbrHeuresEstime.Text != "")
            {
                DateTime dtDebut = Convert.ToDateTime(dtDateDebut.SelectedDate);
                dtDebut = dtDebut.AddDays(double.Parse(txtNbrHeuresEstime.Text)/7);
                dtDateFin.SelectedDate = dtDebut;
            }

            txtPrixEstimation.Text = prixEstimer.ToString() + "$";
        }

        private void CalculerTempsEstime(object sender, SelectionChangedEventArgs e)
        {

            if (dtDateFin.SelectedDate < dtDateDebut.SelectedDate)
            {
                MessageBox.Show("Date de fin plus petite que la date de début");
                dtDateFin.SelectedDate = dtDateDebut.SelectedDate;
            }
            else 
            { 
                DateTime dtFin = Convert.ToDateTime(dtDateFin.SelectedDate);
                DateTime dtDebut = Convert.ToDateTime(dtDateDebut.SelectedDate);
                double heure = (dtFin - dtDebut).TotalHours/24*7;
                
                if(txtNbrHeuresEstime.Text != ((int)Math.Round(heure)).ToString())
                {
                    txtNbrHeuresEstime.Text = ((int)Math.Round(heure)).ToString();
                }
            }
                 
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void ValiderNomProjet(object sender, RoutedEventArgs e)
        {
            bool existe;
            existe = Requete.Existe(txtNomProjet.Text);
            if(existe || txtNomProjet.Text == "")
            {
               lblNomProj.Foreground = Brushes.Red;
               MessageBox.Show("Nom du projet déjà existant");
            }
            else
            {
                lblNomProj.Foreground = Brushes.Black;
            }
        }
    }
}
