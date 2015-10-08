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
using System.Windows.Navigation;
using System.Windows.Shapes;
using model.Models;
using model.Models.Args;
using model.Service;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ModifierEmpView.xaml
    /// </summary>
    public partial class ModifierEmpView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private IProjetService _ServiceProjet;
        private IApplicationService _applicationService;
        public RetrieveProjetArgs RetrieveArgs { get; set; }
        private ObservableCollection<Projet> _projet = new ObservableCollection<Projet>();

        private Employe _Employe;
        public ModifierEmpView()
        {
            InitializeComponent();
            DataContext = this;
            RetrieveArgs = new RetrieveProjetArgs();
            _ServiceProjet = ServiceFactory.Instance.GetService<IProjetService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            //ProjetEmploye = new ObservableCollection<Projet>(_ServiceProjet.retrieveAll());
        }
        public ObservableCollection<Projet> ProjetEmploye
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

        public ModifierEmpView(IDictionary<string, object> parametre): this()
        {
            _Employe = parametre["Employe"] as Employe;
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

        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void EnregistrerEmp(object sender, RoutedEventArgs e)
        {
            
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

        //Accepter seulement text sans chiffre
        //Accepter seulement chiffre + point décimal
        #region
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
        #endregion
    }
}
