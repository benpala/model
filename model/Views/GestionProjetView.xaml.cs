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
        const float SALAIREADMIN = 50;
        const float SALAIRESENIOR = 40;
        const float SALAIREJUNIOR = 30;
        bool siCreation = false;
        float HeureDepart = 0;
        MySqlProjetService Requete = new MySqlProjetService();

        public GestionProjetView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public GestionProjetView(bool creation)
        {
            siCreation = creation;
            _projet = new Projet();
            _projet.ID = Requete.dernierId();
            _projet.dateun = DateTime.Now.ToString();
            _projet.etat = "SIM";
            InitializeComponent();
            DataContext = this;
            AutoriserChampDroit(rboSIM,null);
            dtDateFin.DisplayDateStart = dtDateDebut.SelectedDate;
            gridEmployeProjet.Visibility = Visibility.Hidden; 
            lblEmployesProjet.Visibility = Visibility.Hidden;
            rboEND.IsEnabled = false;
        }

        public GestionProjetView(IDictionary<string,object> parameters):this()
        {
            Projet = parameters["Projet"] as Projet;
            if(Projet.etat != "SIM")
            {
                if (Projet.etat == "END")
                {
                    dtDateDebut.IsEnabled = false;
                    dtDateFin.IsEnabled = false;
                    txtNomProjet.IsEnabled = false;
                }
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
                lblNbHeureAdmin.Visibility = Visibility.Hidden;
                lblNbHeureJunior.Visibility = Visibility.Hidden;
                lblNbHeureSenior.Visibility = Visibility.Hidden;
                txtNbHeureAdmin.Visibility = Visibility.Hidden;
                txtNbHeureJunior.Visibility = Visibility.Hidden;
                txtNbHeureSenior.Visibility = Visibility.Hidden;
               
            }
            else
            {
                gridEmployeProjet.Visibility = Visibility.Hidden;
                lblEmployesProjet.Visibility = Visibility.Hidden;
            }

            if (Projet.etat == "ABD")
            {
                rboABD.IsChecked = true;
                rboSIM.IsEnabled = false;
            }

            if (Projet.etat == "ECS")
            {
                rboECS.IsChecked = true;
                rboSIM.IsEnabled = false;
            }

            if (Projet.etat == "END")
            {
                rboEND.IsChecked = true;
                rboSIM.IsEnabled = false;
            }
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
            bool estValide = true;
                try
                {
                    _projet.ID = txtID.Text;
                    if (txtNomProjet.Text == "" || ValiderNomProjet() && siCreation)
                    {
                        estValide = false;
                        lblNomProj.Foreground = Brushes.Red;
                    }
                    _projet.nom = txtNomProjet.Text;
                    _projet.dateun = "'" + dtDateDebut.SelectedDate.ToString() + "'";
                    if(_projet.dateun == "''")
                    {
                        estValide = false;
                        lblDateDebut.Foreground = Brushes.Red;
                    }
                    else
                    {
                        lblDateDebut.Foreground = Brushes.Black;
                    }
                    _projet.datedeux = "'" + dtDateFin.SelectedDate.ToString() + "'";
                    if(_projet.datedeux == "''")
                    {
                        _projet.datedeux = "'0001-01-01 00:00:00'";
                    }
                    _projet.prixSimulation = 0;
                    _projet.prixReel = 0;
                    _projet.nbHeuresReel = 0;
                    _projet.nbHeuresSimule = 0;

                    if (rboABD.IsChecked == true)
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
                        int nombreTexte = txtPrixEstimation.Text.Length;
                        if(nombreTexte != 0 && txtNbrHeuresEstime.Text != "")
                        { 
                            string prixEstime = txtPrixEstimation.Text.Substring(0, nombreTexte - 1);
                            float prixSimule = float.Parse(prixEstime);
                            _projet.prixSimulation = prixSimule;
                            _projet.nbHeuresSimule = int.Parse(txtNbrHeuresEstime.Text);
                        }
                        else 
                        {
                            estValide = false;
                        }
                        if (txtNbrHeuresEstime.Text == "")
                            lblNbrHeuresEstime.Foreground = Brushes.Red;
                        else
                            lblNbrHeuresEstime.Foreground = Brushes.Black;

                        if (txtRessourcesAdmin.Text == "")
                            lblRessourcesAdmin.Foreground = Brushes.Red;
                        else
                            lblRessourcesAdmin.Foreground = Brushes.Black;

                        if (txtRessourcesJunior.Text == "")
                            lblRessourcesJunior.Foreground = Brushes.Red;
                        else
                            lblRessourcesJunior.Foreground = Brushes.Black;
                        if (txtRessourcesSenior.Text == "")
                            lblRessourcesSenior.Foreground = Brushes.Red;
                        else
                            lblRessourcesSenior.Foreground = Brushes.Black;
                    }
                    else
                    {
                        lblNbrHeuresEstime.Foreground = Brushes.Black;
                        lblRessourcesAdmin.Foreground = Brushes.Black;
                        lblRessourcesJunior.Foreground = Brushes.Black;
                        lblRessourcesSenior.Foreground = Brushes.Black;
                    }
                    if (rboEND.IsChecked == true)
                    {
                        _projet.etat = "END";
                    }
                    if (siCreation && estValide)
                    {
                        Requete.create(_projet);
                    }
                    else
                    {
                        if(estValide)
                            Requete.modifier(_projet);
                    }
                    if(estValide)
                    { 
                        MessageBox.Show(txtNomProjet.Text + " à bien été enregistré.");
                        retourMenu(this, null);
                    }
                    else
                    {
                        MessageBox.Show("Le projet n'a pas été enregistré.");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Le projet n'a pas été enregistré.");
                }
        }

        private void CalculerPrix(object sender, TextChangedEventArgs e)
        {
            float prixEstimer = 0;

            if(siCreation && _projet.etat == "SIM")
            {
                float tempsSenior = (txtNbHeureSenior.Text == ""?0:float.Parse(txtNbHeureSenior.Text)) / (txtRessourcesSenior.Text == ""?1:float.Parse(txtRessourcesSenior.Text));
                float tempsJunior = (txtNbHeureJunior.Text == ""?0:float.Parse(txtNbHeureJunior.Text)) /(txtRessourcesJunior.Text == ""?1:float.Parse(txtRessourcesJunior.Text));
                float tempsAdmin = (txtNbHeureAdmin.Text == ""?0:float.Parse(txtNbHeureAdmin.Text)) / (txtRessourcesAdmin.Text == ""?1:float.Parse(txtRessourcesAdmin.Text));
                float tempsEstimer = 0;

                tempsEstimer = tempsSenior + tempsJunior + tempsAdmin;

                if (dtDateFin.Tag != "Changer")
                    txtNbrHeuresEstime.Text = ((txtNbHeureAdmin.Text == "" ? 0 : float.Parse(txtNbHeureAdmin.Text)) + (txtNbHeureJunior.Text == "" ? 0 : float.Parse(txtNbHeureJunior.Text)) + (txtNbHeureSenior.Text == "" ? 0 : float.Parse(txtNbHeureSenior.Text))).ToString();
                else
                {
                    if ((txtNbHeureAdmin.Text == "" ? 0 : float.Parse(txtNbHeureAdmin.Text)) + (txtNbHeureJunior.Text == "" ? 0 : float.Parse(txtNbHeureJunior.Text)) +
                        (txtNbHeureSenior.Text == "" ? 0 : float.Parse(txtNbHeureSenior.Text)) != float.Parse(txtNbrHeuresEstime.Text) && siCreation)
                    {
                        lblNbHeureAdmin.Foreground = Brushes.Red;
                        lblNbHeureSenior.Foreground = Brushes.Red;
                        lblNbHeureJunior.Foreground = Brushes.Red;
                    }
                    else
                    {
                        lblNbHeureAdmin.Foreground = Brushes.Black;
                        lblNbHeureSenior.Foreground = Brushes.Black;
                        lblNbHeureJunior.Foreground = Brushes.Black;
                    }
                }


                if (txtRessourcesAdmin.Text != "" && txtNbrHeuresEstime.Text != "")
                {
                    prixEstimer += ((txtRessourcesAdmin.Text == ""?0:float.Parse(txtRessourcesAdmin.Text)) * SALAIREADMIN) * tempsAdmin/*(txtNbHeureAdmin.Text == ""?0:float.Parse(txtNbHeureAdmin.Text))*/;
                }

                if (txtRessourcesSenior.Text != "" && txtNbrHeuresEstime.Text != "")
                {
                    prixEstimer += ((txtRessourcesSenior.Text == ""?0:float.Parse(txtRessourcesSenior.Text)) * SALAIRESENIOR) * tempsSenior/*(txtNbHeureSenior.Text == ""?0:float.Parse(txtNbHeureSenior.Text))*/;
                }

                if (txtRessourcesJunior.Text != "" && txtNbrHeuresEstime.Text != "")
                {
                    prixEstimer += ((txtRessourcesJunior.Text == ""?0:float.Parse(txtRessourcesJunior.Text)) * SALAIREJUNIOR) * tempsJunior/*(txtNbHeureJunior.Text == ""?0:float.Parse(txtNbHeureJunior.Text))*/;
                }

                if (txtNbrHeuresEstime.Text != "" && txtNbrHeuresEstime.Text != "0" && dtDateFin.Tag != "Changer")
                {
                    DateTime dtDebut = Convert.ToDateTime(dtDateDebut.SelectedDate);
                    dtDebut = dtDebut.AddDays(tempsEstimer / 7);
                    dtDateFin.Tag = "Prog";
                    dtDateFin.SelectedDate = dtDebut;
                    dtDateFin.Tag = "";
                }

                txtPrixEstimation.Text = prixEstimer.ToString() + "$";
            }
        }

        private void CalculerTempsEstime(object sender, SelectionChangedEventArgs e)
        {
            if (txtNbrHeuresEstime.IsEnabled != false)
            {
                if (dtDateFin.Tag != "Prog" || dtDateFin.Tag == null)
                {

                    if (dtDateFin.SelectedDate < dtDateDebut.SelectedDate && siCreation)
                    {
                        MessageBox.Show("Date de fin plus petite que la date de début");
                        dtDateFin.SelectedDate = dtDateDebut.SelectedDate;
                    }
                    else
                    {
                        if (sender != null)
                        {
                            if (((DatePicker)sender).Name == "dtDateFin")
                                dtDateFin.Tag = "Changer";
                        }

                        DateTime dtFin = Convert.ToDateTime(dtDateFin.SelectedDate);
                        DateTime dtDebut = Convert.ToDateTime(dtDateDebut.SelectedDate);
                        double heure = (dtFin - dtDebut).TotalHours / 24 * 7;

                        if (dtDateFin.SelectedDate != null)
                        {
                            if (txtNbrHeuresEstime.Text != ((int)Math.Round(heure)).ToString())
                            {
                                txtNbrHeuresEstime.Text = ((int)Math.Round(heure)).ToString();
                            }

                            if ((txtNbHeureAdmin.Text == "" ? 0 : float.Parse(txtNbHeureAdmin.Text)) + (txtNbHeureJunior.Text == "" ? 0 : float.Parse(txtNbHeureJunior.Text)) +
                                 (txtNbHeureSenior.Text == "" ? 0 : float.Parse(txtNbHeureSenior.Text)) != float.Parse(txtNbrHeuresEstime.Text) && siCreation)
                            {
                                lblNbHeureAdmin.Foreground = Brushes.Red;
                                lblNbHeureSenior.Foreground = Brushes.Red;
                                lblNbHeureJunior.Foreground = Brushes.Red;
                            }
                            else
                            {
                                lblNbHeureAdmin.Foreground = Brushes.Black;
                                lblNbHeureSenior.Foreground = Brushes.Black;
                                lblNbHeureJunior.Foreground = Brushes.Black;
                            }
                        }
                    }
                }
                else
                {
                    txtNbrHeuresEstime.Text = ((txtNbHeureAdmin.Text == "" ? 0 : float.Parse(txtNbHeureAdmin.Text)) + (txtNbHeureJunior.Text == "" ? 0 : float.Parse(txtNbHeureJunior.Text)) +
                                 (txtNbHeureSenior.Text == "" ? 0 : float.Parse(txtNbHeureSenior.Text))).ToString();
                }
            }
                 
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text,sender);
        }

        private bool IsTextAllowed(string text, object sender)
        {
            if (((TextBox)sender).Name == "txtNomProjet")
            {
                Regex regex = new Regex("[^a-zA-Z0-9-ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÇçÌÍÎÏìíîïÙÚÛÜùúûüÿÑñ]+");
                return !regex.IsMatch(text);
            }
            else
            {
               Regex regex = new Regex("[^0-9]");
                return !regex.IsMatch(text);
            }
        }

        private void ValidationNomProjet(object sender, RoutedEventArgs e)
        {
            bool existe = false;
            if(siCreation || txtNomProjet.Text != _projet.nom)
            { 
                existe = ValiderNomProjet();
                if(existe)
                {
                    lblNomProj.Foreground = Brushes.Red;
                    MessageBox.Show("Nom du projet déjà existant");
                }
                else if (txtNomProjet.Text == "")
                {
                    lblNomProj.Foreground = Brushes.Red;
                }
                else
                {
                    lblNomProj.Foreground = Brushes.Black;
                }
            }
        }

        private bool ValiderNomProjet()
        {
            bool existe;
            existe = Requete.Existe(txtNomProjet.Text);
            return existe;
        }

        private void AutoriserChampDroit(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                if (((RadioButton)sender).IsChecked == true)
                {
                    lblRessourcesAdmin.Foreground = Brushes.Black;
                    lblRessourcesSenior.Foreground = Brushes.Black;
                    lblRessourcesJunior.Foreground = Brushes.Black;
                    txtRessourcesAdmin.IsEnabled = true;
                    txtRessourcesSenior.IsEnabled = true;
                    txtRessourcesJunior.IsEnabled = true;
                    lblPrixEstimation.Foreground = Brushes.Black;
                    txtPrixEstimation.IsEnabled = true;
                    lblNbrHeuresEstime.Foreground = Brushes.Black;
                    txtNbrHeuresEstime.IsEnabled = true;
                    lblNbHeureAdmin.Foreground = Brushes.Black;
                    lblNbHeureJunior.Foreground = Brushes.Black;
                    lblNbHeureSenior.Foreground = Brushes.Black;
                    txtNbHeureAdmin.IsEnabled = true;
                    txtNbHeureJunior.IsEnabled = true;
                    txtNbHeureSenior.IsEnabled = true;
                }
                else
                {
                    lblRessourcesAdmin.Foreground = Brushes.Gray;
                    lblRessourcesSenior.Foreground = Brushes.Gray;
                    lblRessourcesJunior.Foreground = Brushes.Gray;
                    txtRessourcesAdmin.IsEnabled = false;
                    txtRessourcesSenior.IsEnabled = false;
                    txtRessourcesJunior.IsEnabled = false;
                    lblPrixEstimation.Foreground = Brushes.Gray;
                    txtPrixEstimation.IsEnabled = false;
                    lblNbrHeuresEstime.Foreground = Brushes.Gray;
                    txtNbrHeuresEstime.IsEnabled = false;
                    lblNbHeureAdmin.Foreground = Brushes.Gray;
                    lblNbHeureJunior.Foreground = Brushes.Gray;
                    lblNbHeureSenior.Foreground = Brushes.Gray;
                    txtNbHeureAdmin.IsEnabled = false;
                    txtNbHeureJunior.IsEnabled = false;
                    txtNbHeureSenior.IsEnabled = false;
                }
            }
        }

        private void EffaceDate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && ((DatePicker)sender).Text == "")
            {
                if(dtDateDebut.Text == "")
                {
                    dtDateDebut.SelectedDate = DateTime.Now;
                }
                ((DatePicker)sender).Tag = "";
                lblNbHeureAdmin.Foreground = Brushes.Black;
                lblNbHeureSenior.Foreground = Brushes.Black;
                lblNbHeureJunior.Foreground = Brushes.Black;
                txtNbrHeuresEstime.Text = ((txtNbHeureAdmin.Text == "" ? 0 : float.Parse(txtNbHeureAdmin.Text)) + (txtNbHeureJunior.Text == "" ? 0 : float.Parse(txtNbHeureJunior.Text)) +
             (txtNbHeureSenior.Text == "" ? 0 : float.Parse(txtNbHeureSenior.Text))).ToString();
                CalculerTempsEstime(null, null);
            }
        }
    }
}
