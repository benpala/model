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
            _Paie.MontantBrute += _Paie.MontantPrime;
            float taux = _Paie.getTauxFederal(_Paie.MontantBrute, _Paie.NombreHeure, _Paie.NombreHeureSupp, _Paie.idPeriode);
            _Paie.MontantNet = (_Paie.MontantBrute * (1 - taux));
           
        }

    }
}
