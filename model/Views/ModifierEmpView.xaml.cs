using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using model.Models;
using model.Models.Args;
using model.Service;
using model.Service.MySql;
using System.Windows.Data;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ModifierEmpView.xaml
    /// </summary>
    public partial class ModifierEmpView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        MySqlEmployeService _ServiceMysql = new MySqlEmployeService();
        private ObservableCollection<LiaisonProjetEmploye> _LiaisonProjetEmploye;
        private Employe _Employe;

        public ModifierEmpView()
        {
            InitializeComponent();
            DataContext = this;

        }

        public ModifierEmpView(IDictionary<string, object> parametre) : this()
        {
            _Employe = parametre["Employe"] as Employe;
            LiaisonProjetEmploye = new ObservableCollection<LiaisonProjetEmploye>(_ServiceMysql.GetLiaison(_Employe.ID.ToString()));
        }

        public Employe Employe
        {
            get
            {
                return _Employe;
            }

            set
            {
                if (_Employe == value)
                {
                    return;
                }
                _Employe = value;
            }
        }
        public ObservableCollection<LiaisonProjetEmploye> LiaisonProjetEmploye
        {
            get
            {
                return _LiaisonProjetEmploye;
            }
            set
            {
                if (_LiaisonProjetEmploye == value)
                {
                    return;
                }
                _LiaisonProjetEmploye = value;
            }
        }

        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void EnregistrerEmp(object sender, RoutedEventArgs e)
        {
            bool cbx = this.cbxHorsFonction.IsChecked.Value; // get value de check box horsFonction
            int id = Int32.Parse(_Employe.ID);               // get ID d'employe et convertir en INT

            // Update BD :information de l'employé (nom,prénom,poste,salaire)
            _ServiceMysql.UpdateInfoEmploye(_Employe,cbx);
            MessageBox.Show("Les informations sont modifiées");

            retourMenu(this, null);
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
        public void textSeulement(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z ) || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }
        public void numSeulement(object sender, KeyEventArgs e)
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
        public bool ValidMinLenght(string txt)
        {
            if (txt != null && txt.Length > 2 && txt.Length < 20)
                return false;
            else 
                return true;
        }
        public bool ValidSalaire(string salaire)
        {
            if (salaire != null && Convert.ToSingle(salaire) > 0 && Convert.ToSingle(salaire) < 500)
                return false;
            else 
                return true;
        }


        #endregion
    }
}
