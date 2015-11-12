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
using System.Linq;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;

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
            List<LiaisonProjetEmploye> ToutProjet = new List<LiaisonProjetEmploye>(_ServiceMysql.GetLiaison(null));
            LiaisonProjetEmploye = new ObservableCollection<LiaisonProjetEmploye>(ComparerListe(ToutProjet, _ServiceMysql.GetLiaison(_Employe.ID.ToString())));

        }
        #region GET SET
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
        #endregion
        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void EnregistrerEmp(object sender, RoutedEventArgs e)
        {
            bool cbx = this.chxHorsFonction.IsChecked.Value; // get value de check box horsFonction
            int id = Int32.Parse(_Employe.ID);               // get ID d'employe et convertir en INT

            //Validation nom,prénom
            if (txtNom.Text.Length < 2 || txtNom.Text.Length > 20 || txtPrenom.Text.Length < 2 || txtPrenom.Text.Length > 20)
                MessageBox.Show("Le nombre de caractère pour nom et prénom doit être entre 2 et 20 !!");
            //Validation Poste
            else if (txtPoste.Text.Length < 3 || txtPoste.Text.Length > 20)
                MessageBox.Show("Le nombre de caractère pour la poste doit être entre 3 et 20 !!");
            //validation salaire
            else if (!ValidSalaire(txtSalaire.Text))
                MessageBox.Show("Le salaire ne peut pas laisser vide et il doit être entre 0 et 500 !!");
            else
            { //Update la BD
                _ServiceMysql.UpdateInfoEmploye(_Employe,cbx);
                _ServiceMysql.UpdateProjetEmploye(LiaisonProjetEmploye, _Employe.ID);
                MessageBox.Show("Les informations ont été modifiées");
                retourMenu(this, null);
            }
        }
        public IList<LiaisonProjetEmploye> ComparerListe(IList<LiaisonProjetEmploye> l1, IList<LiaisonProjetEmploye> l2)
        {
            foreach(LiaisonProjetEmploye l in l1)
            {
                foreach(LiaisonProjetEmploye ll in l2)
                {
                    if (l.ProjNom == ll.ProjNom)
                        l.Occupe = ll.Occupe;
                }
            }
            return l1;
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
            if (salaire.Length > 0)
            {
                int nbPoint = salaire.Count(x => x == '.');
                salaire = salaire.Replace('.', ',');
                if (nbPoint == 0 || nbPoint == 1)
                {
                    float sal = Convert.ToSingle(salaire);
                    if (sal >= 0 && sal <= 500)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        #endregion

        #region Photo
        private void btnUplaod(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "Tous les formats |*.jpg;*.png|" +
              "JPG (*.jpg;)|*.jpg;|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
               
               /* string name = System.IO.Path.GetFileName(op.FileName);
                string destinationPath = GetDestinationPath(name, "/image");
                File.Copy(op.FileName, destinationPath, true);*/

            }
        }
        /* private static String GetDestinationPath(string filename, string foldername)
         {
             String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

             appStartPath = String.Format(appStartPath + "\\{0}\\" + filename, foldername);
             return appStartPath;
         }*/
        #endregion
    }
}
