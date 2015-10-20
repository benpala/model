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
using System.Windows.Threading;
using model.Models;
using model.Models.Args;
using model.Service;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour employe.xaml
    /// </summary>
    public partial class EmployeView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private IEmployeService _ServiceEmploye;
        private IApplicationService _applicationService;

        public RetrieveEmployeArgs RetrieveArgs { get; set; }
        private ObservableCollection<Employe> _employe = new ObservableCollection<Employe>();
        public EmployeView()
        {
            InitializeComponent();

            DataContext = this;
            RetrieveArgs = new RetrieveEmployeArgs();
            _ServiceEmploye = ServiceFactory.Instance.GetService<IEmployeService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Employe = new ObservableCollection<Employe>(_ServiceEmploye.RetrieveAll());
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                int compteur = 0;
                foreach (Employe emp in Employe)
                {
                    DataGridRow row = (DataGridRow)mygrid.ItemContainerGenerator.ContainerFromIndex(compteur);
                    if (emp.HorsFonction == true)
                    {
                        var converter = new System.Windows.Media.BrushConverter();
                        var brush = (Brush)converter.ConvertFromString("#DC143C");
                        row.Background = brush;
                        compteur++;
                    }
                    else
                    {
                        var converter = new System.Windows.Media.BrushConverter();
                        var brush = (Brush)converter.ConvertFromString("#00FF00");
                        row.Background = brush;
                        compteur++;
                    }
                }
                compteur = 0;
            }));

            
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

        public ObservableCollection<Employe> Employe
        {
            get
            {
                return _employe;
            }
            set
            {
                if (_employe == value)
                {
                    return;
                }

                RaisePropertyChanging();
                _employe = value;
                RaisePropertyChanged();
            }
        }
       
        //Boutton d'ajout d'un employé.
        private void click_addEmploye(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<AjoutEmployeView>(new AjoutEmployeView());
            
        }
        
        private void click_Modifier(object sender, RoutedEventArgs e)
        {
            var obj = (Employe)((sender as Button).CommandParameter);
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            Dictionary<string, object> parametre = new Dictionary<string, object>() { { "Employe", obj } };
            applicationService.ChangeView<ModifierEmpView>(new ModifierEmpView(parametre));
        }
    }
}
