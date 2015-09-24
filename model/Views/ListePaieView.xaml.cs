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
using System.Windows.Controls.Primitives;
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
    /// Logique d'interaction pour listePaie.xaml
    /// </summary>
    public partial class ListePaieView : UserControl, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private IPaiesService _ServicePaie;
        private IApplicationService _applicationService;

        public RetrievePaieArgs RetrieveArgs { get; set; }
        private ObservableCollection<Paie> _paie = new ObservableCollection<Paie>();

        public ListePaieView()
        {
            InitializeComponent();

            DataContext = this;
            RetrieveArgs = new RetrievePaieArgs();
            _ServicePaie = ServiceFactory.Instance.GetService<IPaiesService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Paies = new ObservableCollection<Paie>(_ServicePaie.RetrieveAll());
           
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

        public ObservableCollection<Paie> Paies
        {
            get
            {
                return _paie;
            }
            set
            {

                if (_paie == value)
                {
                    return;
                }

                RaisePropertyChanging();
                _paie = value;
                RaisePropertyChanged();
            }

        }

        // Boutton de modification et imprésion des relevés.
        private void click_modifierImprimer(object sender, RoutedEventArgs e)
        {
            Paie obj = (Paie)((sender as Button).CommandParameter);
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Dictionary<string, object> parametre = new Dictionary<string,object>(){{"paie", obj}};
            applicationService.ChangeView<DetailPaieView>(new DetailPaieView(parametre));
        }

        private void click_genereReleve(object sender, RoutedEventArgs e)
        {
             MessageBox.Show("Votre génération de paye a réussis.");
        }
    }
}
