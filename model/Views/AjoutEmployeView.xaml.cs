using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using model.Service;
using model.Service.MySql;
using System;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutEmploye.xaml
    /// </summary>
    public partial class AjoutEmployeView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        MySqlEmployeService _ServiceMysql = new MySqlEmployeService();
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
        {
            //Vérification si le même nom et prénom de l'employé existe ou non
            if(!_ServiceMysql.ExisteEmploye(txtAjoutNom.Text, txtAjoutPrenom.Text))
            {
                _ServiceMysql.AjoutUnEmploye(txtAjoutNom.Text, txtAjoutPrenom.Text, txtAjoutPoste.Text, txtAjoutSalaire.Text);
                retourMenu(this,null);
            }
            else
                MessageBox.Show("Erreur : Il y existe déjà un employé avec le même nom et même prénom!!!");
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

        #region validation textbox
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
            if (e.Key == Key.OemComma)
            {
                e.Handled = false;
            }
        }
        #endregion
    }
}
