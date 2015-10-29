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
using model.Service;
using model.Service.MySql;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour detailPaie.xaml
    /// </summary>
    public partial class DetailPaieView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private Paie _Paie;
        public DetailPaieView()
        {
            InitializeComponent();
            DataContext= this;
        }
        public DetailPaieView(IDictionary<string,object> parametre):this()
        { 
            _Paie = parametre["paie"] as Paie;
        }
        
        public Paie Paies
        {
            get
            {
                return _Paie;
            }

            set
            {
                if (_Paie == value)
                {
                    return;
                }
                RaisePropertyChanging();
                _Paie = value;
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

        private void click_retour(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<ListePaieView>(new ListePaieView());
        }

        private void click_enregistre(object sender, RoutedEventArgs e)
        {
            // Envoie des champs en base de données afin de modifier les champs.

        }

        private void calculePrime(object sender, RoutedEventArgs e)
        {
            try
            {
                string p = prime.Text.Replace(".", ","); p.Trim();
                Paies.MontantBrute += (_Paie.MontantPrime = float.Parse(p));
                float taux = Paies.getTauxFederal(Paies.MontantBrute, Paies.NombreHeure, Paies.NombreHeureSupp, Paies.idPeriode);
                Paies.MontantNet = (Paies.MontantBrute * (1 - taux));
            }
            catch(Exception)
            {
                MessageBox.Show("Le montant entré est invalide! format accepté : 00.00");
            }
            
           
        }

        private void calculeIndemite(object sender, RoutedEventArgs e)
        {
            try
            {
                Paies.MontantBrute += (_Paie.MontantIndemnite = float.Parse(indemite.Text.Replace(".", ",")));
                float taux = Paies.getTauxFederal(Paies.MontantBrute, Paies.NombreHeure, Paies.NombreHeureSupp, Paies.idPeriode);
                Paies.MontantNet = (Paies.MontantBrute * (1 - taux));
            }
            catch(Exception)
            {
                MessageBox.Show("Le montant entré est invalide! format accepté : 00.00");
            }
        }

        private void calculeAllocation(object sender, RoutedEventArgs e)
        {
            try
            {
                Paies.MontantBrute += (_Paie.MontantAllocations = float.Parse(allocation.Text.Replace(".", ",")));
                float taux = Paies.getTauxFederal(Paies.MontantBrute, Paies.NombreHeure, Paies.NombreHeureSupp, Paies.idPeriode);
                Paies.MontantNet = (Paies.MontantBrute * (1 - taux));
            }
            catch(Exception)
            {
                MessageBox.Show("Le montant entré est invalide! format accepté : 00.00");
            }
        }

        private void calculePourboire(object sender, RoutedEventArgs e)
        {
            try
            {
                Paies.MontantBrute += (_Paie.MontantPourboire = float.Parse(pourboire.Text.Replace(".", ",")));
                float taux = Paies.getTauxFederal(Paies.MontantBrute, Paies.NombreHeure, Paies.NombreHeureSupp, Paies.idPeriode);
                Paies.MontantNet = (Paies.MontantBrute * (1 - taux));
            }
            catch(Exception)
            {
                MessageBox.Show("Le montant entré est invalide! format accepté : 00.00");
            }
        }

        private void calculeHeure(object sender, RoutedEventArgs e)
        {
        
            try
            {
                MySqlPaieService _service = new MySqlPaieService();

                float tauxHoraire = float.Parse(_service.tauxHorraire(Paies.idEmploye));
                float heureNormal = float.Parse(heure.Text.Replace(".", ","));
                Paies.MontantBrute += ((_Paie.NombreHeure = heureNormal)*tauxHoraire);
                float taux = Paies.getTauxFederal(Paies.MontantBrute, Paies.NombreHeure, Paies.NombreHeureSupp, Paies.idPeriode);
                Paies.MontantNet = (Paies.MontantBrute * (1 - taux));
            }
            catch (Exception)
            {
                MessageBox.Show("Le montant entré est invalide! format accepté : 00.00");
            }
        }

        private void calculeHeureSupp(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlPaieService _service = new MySqlPaieService();

                float tauxHoraire = float.Parse(_service.tauxHorraire(Paies.idEmploye));
                float heureNormal = float.Parse(heureSupp.Text.Replace(".", ","));
                Paies.MontantBrute += (float)((_Paie.NombreHeureSupp = heureNormal) * (tauxHoraire * 1.5));
                float taux = Paies.getTauxFederal(Paies.MontantBrute, Paies.NombreHeure, Paies.NombreHeureSupp, Paies.idPeriode);
                Paies.MontantNet = (Paies.MontantBrute * (1 - taux));
            }
            catch (Exception)
            {
                MessageBox.Show("Le montant entré est invalide! format accepté : 00.00");
            }
        }

    }
}
