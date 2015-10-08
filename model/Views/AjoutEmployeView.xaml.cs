using System;
using System.Collections.Generic;
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
using model.Models;
using model.Service;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutEmploye.xaml
    /// </summary>
    public partial class AjoutEmployeView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public AjoutEmployeView()
        {
            InitializeComponent();
           
        }
       
        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void AjouterEmploye(object sender, RoutedEventArgs e)
        {/*
            //{ID = "00",Prenom = "Pei", Nom = "Li", Photo = "01", Poste = "Chef", Salaire = "40$"}
            Employe employe = new Employe("5002", txtAjoutNom.Text, txtAjoutPrenom.Text, txtAjoutPoste.Text, txtAjoutSalaire.Text); 
            //Enregistre les données et retourne à l'écran d'avant
            EmployeService _serviceEmploye = new EmployeService();
            _serviceEmploye.addOneEmploye(employe);
            retourMenu(this,null);*/
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

        private void textSeulement(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

            // If tab is presses, then the focus must go to the
            // next control.
            if (e.Key == Key.Tab)
            {
                e.Handled = false;
            }
        }

        private void numSeulement(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 ||
                e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Decimal)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

            // If tab is presses, then the focus must go to the
            // next control.
            if (e.Key == Key.Tab)
            {
                e.Handled = false;
            }
        }



    }
}
