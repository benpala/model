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
using Xceed.Wpf.Toolkit;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutProjet.xaml
    /// </summary>
    public partial class GestionProjetView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private Projet _projet;
        private ObservableCollection<ProjetEmploye> _ProjetEmploye;
        const float SALAIREADMIN = 50;
        const float SALAIRESENIOR = 40;
        const float SALAIREJUNIOR = 30;
        bool siCreation = false;
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
            rboRessGen.IsChecked = true;
            txtDateTerminerOuAbandonner.Visibility = Visibility.Hidden;
            canvas.Visibility = Visibility.Hidden;
        }

        public GestionProjetView(IDictionary<string,object> parameters):this()
        {
            Projet = parameters["Projet"] as Projet;
            
            if(Projet.etat != "SIM")
            {
                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(Requete.getEmployeProjet(Projet.ID));
                if (Projet.etat == "TER")
                {
                    lblDateTerminerOuAbandonner.Content = "Projet terminé le";
                    txtDateTerminerOuAbandonner.Text = Projet.dateTerminer;
                    rboEND.IsChecked = true;
                    rboSIM.IsEnabled = false;
                    dtDateDebut.IsEnabled = false;
                    dtDateFin.IsEnabled = false;
                    txtNomProjet.IsEnabled = false;
                    dtDateDebut.IsEnabled= false;
                    dtDateFin.IsEnabled = false;
                    lblEtat.Visibility = Visibility.Collapsed;
                    lblEtatA.Visibility = Visibility.Collapsed;
                    lblEtatS.Visibility = Visibility.Collapsed;
                    lblEtatE.Visibility = Visibility.Collapsed;
                    lblEtatC.Visibility = Visibility.Collapsed;
                    rboABD.Visibility = Visibility.Collapsed;
                    rboECS.Visibility = Visibility.Collapsed;
                    rboSIM.Visibility = Visibility.Collapsed;
                    rboEND.Visibility = Visibility.Collapsed;
                    btnEnregistrer.Visibility = Visibility.Collapsed;
                }
                if (Projet.etat == "ABD")
                {
                    rboABD.IsChecked = true;
                    rboSIM.IsEnabled = false;
                    rboEND.IsEnabled = false;
                    lblDateTerminerOuAbandonner.Content = "Projet abandonné le";
                    txtDateTerminerOuAbandonner.Text = Projet.dateAbandon;
                }

                if (Projet.etat == "ECS")
                {
                    rboECS.IsChecked = true;
                    rboSIM.IsEnabled = false;
                    lblDateTerminerOuAbandonner.Visibility = Visibility.Hidden;
                    txtDateTerminerOuAbandonner.Visibility = Visibility.Hidden;
                    
                }
                rboRessGen.IsEnabled = false;
                lblRessourcesAdmin.Visibility = Visibility.Hidden;
                lblJours.Visibility = Visibility.Hidden;
                lblNonOuvrable.Visibility = Visibility.Hidden;
                lblOuvrable.Visibility = Visibility.Hidden;
                txtJourNon.Visibility = Visibility.Hidden;
                txtJourOuvr.Visibility = Visibility.Hidden;
                txtRessourcesAdmin.Visibility = Visibility.Hidden;
                lblPrixEstimation.Visibility = Visibility.Hidden;   
                txtPrixEstimation.Visibility = Visibility.Hidden;
                lblNbrHeuresEstime.Visibility = Visibility.Hidden;
                //txtNbrHeuresEstime.Visibility = Visibility.Hidden;
                //txtNbrHeuresEstimeOuvrable.Visibility = Visibility.Hidden;
                lblNbrHeuresEstimeOuvrable.Visibility = Visibility.Hidden;
                txtEstimation.Visibility = Visibility.Hidden;
                lblHeuresEstime.Visibility = Visibility.Hidden;
                DockChoixRessources.Visibility = Visibility.Hidden;
                txtHeure.Text = _projet.nbHeuresReel.ToString();
                txtCout.Text = _projet.prixReel.ToString() + "$";
                txtTempsEstime.Text = _projet.nbHeuresSimule.ToString();
                if(_projet.datedeux.ToString() != "Indéfini" && Projet.etat != "ABD")
                {       
                    DateTime d1 = Convert.ToDateTime(_projet.datedeux);
                    DateTime d2 = DateTime.Now;
                    try
                    { 
                        d2 = Convert.ToDateTime(_projet.dateTerminer);
                    }
                    catch
                    {
                        
                    }
                    if(Projet.etat == "TER" || (d1- d2).TotalDays < 0)
                        txtRetard.Text = Convert.ToInt32((d1 - d2).TotalDays).ToString();
                    else
                        txtRetard.Text = "0";
                    if (int.Parse(txtRetard.Text) >= 0)
                        txtRetard.Background = Brushes.Green;
                    else
                        txtRetard.Background = Brushes.Red;   
                }
                else
                {
                    txtRetard.Text = "NaN";
                }
            }
            else
            {
                canvas.Visibility = Visibility.Hidden;
                gridEmployeProjet.Visibility = Visibility.Hidden;
                lblEmployesProjet.Visibility = Visibility.Hidden;
                rboSIM.IsChecked = true;
                if(Projet.joursOuvrable) 
                    rboRessGen.IsChecked = true;
                else
                    rboRessGen2.IsChecked = true;
                rboEND.IsEnabled = false;
                lblDateTerminerOuAbandonner.Visibility = Visibility.Hidden;
                txtDateTerminerOuAbandonner.Visibility = Visibility.Hidden;
                txtPrixEstimation.Text = _projet.prixSimulation.ToString() + "$";
                txtRessourcesAdmin.Text = _projet.nbRessourcesEstime.ToString();
                nbHeureJour.Text = _projet.nbHeureTravail.ToString();
                nbQuart.Text = _projet.nbQuart.ToString();
                txtEstimation.Text = _projet.nbHeuresSimule.ToString();
                if(_projet.joursOuvrable)
                    rboRessGen.IsChecked = true;
                else
                    rboRessGen2.IsChecked = true;


                if(_projet.dateun != "indéfini" && _projet.datedeux != "indéfini")
                {
                    CalculerTempsEstime(null,null);
                }
            }
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


        public ObservableCollection<ProjetEmploye> ProjetEmployeList
        {
            get
            {
                return _ProjetEmploye;
            }
            set
            {
                if (_ProjetEmploye == value)
                {
                    return;
                }

                RaisePropertyChanging();
                _ProjetEmploye = value;
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

        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<ProjetView>(new ProjetView());
        }

        private void EnregistrerProjet(object sender, RoutedEventArgs e)
        {
            bool estValide = true;
            string date = "";
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
                    if(_projet.dateun == "''" && _projet.etat != "SIM")
                    {
                        estValide = false;
                        lblDateDebut.Foreground = Brushes.Red;
                    }
                    else
                    {
                        lblDateDebut.Foreground = Brushes.Black;
                    }
                    if(_projet.dateun == "''" && _projet.etat == "SIM")
                    {
                        _projet.dateun = "'0001-01-01 00:00:00'";
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
                        date = DateTime.Now.ToString("d");
                    }
                    if (rboECS.IsChecked == true)
                    {
                        _projet.etat = "ECS";
                    }
                    if (rboSIM.IsChecked == true)
                    {
                        _projet.etat = "SIM";
                        int nombreTexte = txtPrixEstimation.Text.Length;
                        if (nombreTexte != 0 && txtNbrHeuresEstime.Text != "")
                        {
                            string prixEstime = txtPrixEstimation.Text.Substring(0, nombreTexte - 1);
                            float prixSimule = float.Parse(prixEstime);
                            _projet.prixSimulation = prixSimule;
                            if (txtNbrHeuresEstime.Text != "Infini")
                                _projet.nbHeuresSimule = int.Parse(txtNbrHeuresEstime.Text);
                        }
                        else
                        {
                            _projet.nbHeuresSimule = 0;
                            _projet.prixSimulation = 0;
                        }
                        _projet.nbHeureTravail = int.Parse(nbHeureJour.Text.ToString());
                        _projet.nbQuart = int.Parse(nbQuart.Text.ToString());
                        _projet.nbRessourcesEstime = int.Parse(txtRessourcesAdmin.Text.ToString());
                        if(rboRessGen.IsChecked == true)
                            _projet.joursOuvrable = true;
                        else
                            _projet.joursOuvrable = false;
                    }

                    if (rboEND.IsChecked == true)
                    {
                        _projet.etat = "TER";
                        var result = System.Windows.MessageBox.Show("Avertissement! Si un projet est terminé, vous ne pourrez plus revenir en arrière. Êtes-vous certain que le projet est terminé?","Confirmation",MessageBoxButton.YesNo,MessageBoxImage.Warning).ToString();
                        if(result == "No")
                        {
                            return;
                        }
                        if(result == "Yes")
                        {
                            date = DateTime.Now.ToString("d");
                        }
                    }
                    if (siCreation && estValide)
                    {
                        Requete.create(_projet);
                    }
                    else
                    {
                        if(estValide)
                        {
                            Requete.modifier(_projet,date);
                        }
                    }
                    if(estValide)
                    { 
                        System.Windows.MessageBox.Show(txtNomProjet.Text + " à bien été enregistré.");
                        retourMenu(this, null);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Le projet n'a pas été enregistré.");
                    }
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Le projet n'a pas été enregistré.");
                }
        }

        private void CalculerPrix(object sender, TextChangedEventArgs e)
        {
            float prixEstimer = 0;
            try { 
                if(txtPrixEstimation != null && txtJourOuvr.Text != "" && txtJourNon.Text != "")
                {
                    prixEstimer = int.Parse(txtRessourcesAdmin.Text.ToString()) * 40 * int.Parse(nbHeureJour.Text.ToString()) * (rboRessGen.IsChecked == true ? int.Parse(txtJourOuvr.Text.ToString()) : int.Parse(txtJourNon.Text.ToString()));
                    txtPrixEstimation.Text = prixEstimer.ToString() + "$";
                    EstimationDuChampsManquant(sender, null);
                }
            }
            catch
            {

            }
        }

        private void CalculerTempsEstime(object sender, SelectionChangedEventArgs e)
        {
            if(txtNbrHeuresEstime != null)
            { 
                if (nbHeureJour.Text != "" && dtDateFin.Text != "")
                {
                    if (dtDateFin.Tag == null || dtDateFin.Tag.ToString() != "Prog")
                    {

                        if (dtDateFin.SelectedDate < dtDateDebut.SelectedDate && siCreation)
                        {
                            System.Windows.MessageBox.Show("Date de fin plus petite que la date de début");
                            dtDateFin.SelectedDate = dtDateDebut.SelectedDate;
                        }
                        else
                        {
                            if (sender != null && sender.GetType().ToString() == "DatePicker")
                            {
                                if (((DatePicker)sender).Name == "dtDateFin")
                                    dtDateFin.Tag = "Changer";
                            }

                            DateTime dtFin = Convert.ToDateTime(dtDateFin.SelectedDate);
                            DateTime dtDebut = Convert.ToDateTime(dtDateDebut.SelectedDate);
                            //int nombreJour = int.Parse((dtFin - dtDebut).TotalDays.ToString()) + 1;
                            int nombreJour = Convert.ToInt32((dtFin - dtDebut).TotalDays)+1;
                            int nombreJourOuvrable = 0;

                            for (DateTime date = dtDebut; date <= dtFin; date = date.AddDays(1))
                            {
                                if (date.DayOfWeek.ToString() != "Saturday" && date.DayOfWeek.ToString() != "Sunday")
                                    nombreJourOuvrable++;

                            }

                            double heure = nombreJour * int.Parse(nbHeureJour.Text);
                            double heureOuvrable = nombreJourOuvrable * int.Parse(nbHeureJour.Text);

                            txtJourOuvr.Text = nombreJourOuvrable.ToString();
                            if (nombreJour > 0)
                                txtJourNon.Text = nombreJour.ToString();
                            else
                                txtJourNon.Text = "0";

                            if (dtDateFin.SelectedDate != null)
                            {
                                if (txtNbrHeuresEstime.Text != ((int)Math.Round(heure)).ToString())
                                {
                                    string heures = ((int)Math.Round(heure)).ToString();
                                    txtNbrHeuresEstime.Text = heures;
                                    txtNbrHeuresEstimeOuvrable.Text = ((int)Math.Round(heureOuvrable)).ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    txtNbrHeuresEstime.Text = "Infini";
                    txtNbrHeuresEstimeOuvrable.Text = "Infini";
                }
                if(sender != null)
                { 
                    if(sender.GetType().ToString() == "DatePicker")
                    {
                        EstimationDuChampsManquant(sender, null);
                    }
                }
            }                 
        }

        new private void PreviewTextInput(object sender, TextCompositionEventArgs e)
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
                    System.Windows.MessageBox.Show("Nom du projet déjà existant");
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
                ChangerEtat(sender,null);
                if (((RadioButton)sender).IsChecked == true)
                    DockSimule.IsEnabled = true;
                else
                    DockSimule.IsEnabled = false;
            }
        }

        private void EffaceDate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && ((DatePicker)sender).Text == "" /*&& _projet.etat != "SIM"*/)
            {
                if(dtDateDebut.Text == "")
                {
                    dtDateDebut.SelectedDate = DateTime.Now;
                }
                ((DatePicker)sender).Tag = "";
                CalculerTempsEstime(null, null);
            }
        }

        private void ValidationNombre(object sender, KeyEventArgs e)
        {
            Xceed.Wpf.Toolkit.IntegerUpDown txt = sender as Xceed.Wpf.Toolkit.IntegerUpDown;
            int number;
            if (txt != null)
            {
                if(txt.Text != null)
                { 
                    if (int.TryParse(txt.Text, out number))
                    {
                        // à modifier
                        switch(txt.Name)
                        {
                            case "nbHeureJour":
                                {
                                    if (number > 24)
                                    {
                                        txt.Text = "24";
                                    }


                                    break;
                                }
                            case "nbQuart":
                                {
                                    if(number > (24/nbHeureJour.Value))
                                    {
                                        txt.Text = Math.Floor((decimal)(24/nbHeureJour.Value)).ToString();
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        if (txt.Text.Count() != 0)
                            txt.Text = txt.Text.Substring(0,txt.Text.Count()-1);
                        if(txt.Text == "")
                            txt.Text = "0";
                    }
                }
            }
        }

        private void CalculerPrixKey(object sender, KeyEventArgs e)
        {
            ValidationNombre(sender,e);
            CalculerPrix(sender,null);
            EstimationDuChampsManquant(sender, null);
        }

        private void CalculerPrix1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ValidationNombre(sender, null);
            CalculerTempsEstime(sender,null);
            CalculerPrix(sender,null);
            EstimationDuChampsManquant(sender, null);
        }

        private void ChangerEtat(object sender, RoutedEventArgs e)
        {
            if(!siCreation && dtDateDebut.ToString() != "")
            { 
                RadioButton rb = sender as RadioButton;
                if(rb != null)
                { 
                    switch(rb.Name.ToString())
                    {
                        case "rboSIM":
                        {
                            _projet.etat = "SIM";
                            break;
                        }
                        case "rboECS":
                        {
                            if(_projet.etat == "SIM")
                            {
                                _projet.dateun = DateTime.Now.ToString();
                                dtDateFin.SelectedDate = DateTime.Now.AddHours(_projet.nbHeuresSimule);
                                dtDateDebut.SelectedDate = DateTime.Now;
                                rboSIM.IsEnabled = false;
                            }
                            _projet.etat = "ECS";
                            break;
                        }
                        case "rboEND":
                        {
                            _projet.etat = "TER";
                            break;
                        }
                        case "rboABD":
                        {
                            _projet.etat = "ABD";
                            break;
                        }
                    }
                    EffaceDate(dtDateDebut, new KeyEventArgs(Keyboard.PrimaryDevice,
                Keyboard.PrimaryDevice.ActiveSource,
                0, Key.Back));
              }
          }
        }

        private void columnHeader_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if (columnHeader.Tag.ToString() == "" || columnHeader.Tag.ToString() == "Ascending")
                columnHeader.Tag = "Descending";
            else
                columnHeader.Tag = "Ascending";
            if (columnHeader != null)
            {
                switch (columnHeader.Content.ToString())
                {
                    case "ID":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderBy(i => int.Parse(i.ID)));
                            else
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderByDescending(i => int.Parse(i.ID)));

                            break;
                        }
                    case "Nom":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderBy(i => i.Nom));
                            else
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderByDescending(i => i.Nom));

                            break;
                        }
                    case "Poste":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderBy(i => i.Poste));
                            else
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderByDescending(i => i.Poste));

                            break;
                        }
                    case "Heures":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderBy(i => i.Heure));
                            else
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderByDescending(i => i.Heure));

                            break;
                        }
                    case "Coût":
                        {
                            if (columnHeader.Tag.ToString() == "Ascending")
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderBy(i => int.Parse(i.Cout)));
                            else
                                ProjetEmployeList = new ObservableCollection<ProjetEmploye>(_ProjetEmploye.OrderByDescending(i => int.Parse(i.Cout)));

                            break;
                        }
                }
            }
        }

        private void EstimerChampsManquantRb(object sender, RoutedEventArgs e)
        {
            EstimationDuChampsManquant(sender,null);
        }

        private void EstimationDuChampsManquant(object sender, RoutedEvent e)
        {
            //if (Projet.etat == "SIM" && txtNbrHeuresEstime != null && !siCreation)
            //{ 
                    try
                    { 
                        if(/*txtNbrHeuresEstime.Text != "" && txtRessourcesAdmin.Text != "0" && dtDateFin.SelectedDate.ToString() ==  ""*/ rboDateFin.IsChecked == true) 
                        {
                            // on calcule la date de fin
                            DateTime dateFin = (DateTime)dtDateDebut.SelectedDate;
                            if(rboRessGen.IsChecked == true)
                            {
                                // Calculer nombre de fds
                                dateFin = dateFin.AddDays((nbHeureJour.Value < Convert.ToInt32(txtEstimation.Text.ToString())?Math.Floor(float.Parse(txtEstimation.Text) / float.Parse(nbHeureJour.Text) / float.Parse(nbQuart.Text) / int.Parse(txtRessourcesAdmin.Text)):0));
                                dateFin = dateFin.AddDays(CalculerJourNonOuvrable());
                                dtDateFin.SelectedDate = dateFin;
                            }
                            else
                            {
                                dateFin = dateFin.AddDays((nbHeureJour.Value < Convert.ToInt32(txtEstimation.Text.ToString())?Math.Floor(float.Parse(txtEstimation.Text) / float.Parse(nbHeureJour.Text) / float.Parse(nbQuart.Text) / int.Parse(txtRessourcesAdmin.Text)):0));
                                dtDateFin.SelectedDate = dateFin;
                            }
                        }
                        if (/*txtEstimation.Text != "" && dtDateFin.Text != ""*/ rboEmploye.IsChecked == true)
                        {
                            float nbRessources = 0;
                            // on cherche le nombre de ressources
                            if(rboRessGen.IsChecked == true)
                            {
                                nbRessources = float.Parse(txtEstimation.Text) / (float.Parse(txtJourOuvr.Text) * float.Parse(nbHeureJour.Text) * float.Parse(nbQuart.Text));
                                if(nbRessources < 1)
                                {
                                    nbRessources = 1;
                                }
                                txtRessourcesAdmin.Text = Math.Ceiling(nbRessources).ToString();
                            }
                            else
                            {
                                nbRessources = float.Parse(txtEstimation.Text) / (float.Parse(txtJourNon.Text) * float.Parse(nbHeureJour.Text) * float.Parse(nbQuart.Text));
                                if (nbRessources < 1)
                                {
                                    nbRessources = 1;
                                }
                                txtRessourcesAdmin.Text = Math.Ceiling(nbRessources).ToString();
                            }
                        }
                        if(/*dtDateFin.Text != "" && txtRessourcesAdmin.Text != "0"*/ rboHeure.IsChecked == true)
                        {
                            // on calcule le nombre d'heures
                            float nbHeure= 0;
                            if(rboRessGen.IsChecked == true)
                            {
                                nbHeure = float.Parse(txtJourOuvr.Text) * float.Parse(nbHeureJour.Text) * float.Parse(nbQuart.Text) * float.Parse(txtRessourcesAdmin.Text);
                                if (nbHeure < 1)
                                {
                                    nbHeure = 1;
                                }
                                txtEstimation.Text = Math.Round(nbHeure,2).ToString();
                            }
                            else
                            {
                                nbHeure = float.Parse(txtJourNon.Text) * float.Parse(nbHeureJour.Text) * float.Parse(nbQuart.Text) * float.Parse(txtRessourcesAdmin.Text);
                                if (nbHeure < 1)
                                {
                                    nbHeure = 1;
                                }
                                txtEstimation.Text = Math.Round(nbHeure, 2).ToString();
                            }
                        }
                    }
                    catch
                    {
                        
                    }
            //}
        }

        private int CalculerJourNonOuvrable()
        {
            DateTime dtFin = Convert.ToDateTime(dtDateFin.SelectedDate);
            DateTime dtDebut = Convert.ToDateTime(dtDateDebut.SelectedDate);
            //int nombreJour = int.Parse((dtFin - dtDebut).TotalDays.ToString()) + 1;
            int nombreJour = Convert.ToInt32((dtFin - dtDebut).TotalDays) + 1;
            int nombreJourOuvrable = 0;

            for (DateTime date = dtDebut; date <= dtFin; date = date.AddDays(1))
            {
                if (date.DayOfWeek.ToString() == "Saturday" && date.DayOfWeek.ToString() == "Sunday")
                    nombreJourOuvrable++;

            }

            return nombreJourOuvrable;
        }
    }
}
