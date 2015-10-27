using model.Models;
using model.Models.Args;
using model.Service;
using model.Service.MySql;
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

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour RapportView.xaml
    /// </summary>
    public partial class RapportView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private ObservableCollection<Employe> _employe = new ObservableCollection<Employe>();
        private IEmployeService _ServiceEmploye;
        private IApplicationService _applicationService;

        public RetrieveEmployeArgs RetrieveArgs { get; set; }

        public RapportView()
        {
            InitializeComponent();
            RetrieveArgs = new RetrieveEmployeArgs();
            _ServiceEmploye = ServiceFactory.Instance.GetService<IEmployeService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            Employe = new ObservableCollection<Employe>(_ServiceEmploye.RetrieveAll());
            Employe.Insert(0,new Employe("Tous"));
            DataContext = this;

        }
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
    }
}
