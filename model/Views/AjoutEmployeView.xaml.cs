using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using model.Service;
using model.Service.MySql;
using System;
using System.Collections.ObjectModel;
using model.Models;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutEmploye.xaml
    /// </summary>
    public partial class AjoutEmployeView : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        MySqlEmployeService _ServiceMysql = new MySqlEmployeService();
        private ObservableCollection<LiaisonProjetEmploye> _LiaisonProjetEmploye;
        public AjoutEmployeView()
        {
            InitializeComponent();
            DataContext = this;
            LiaisonProjetEmploye = new ObservableCollection<LiaisonProjetEmploye>(_ServiceMysql.GetLiaison(null));
            
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

        private void AjouterEmploye(object sender, RoutedEventArgs e)
        {
            //Validation nom,prénom
            if (txtAjoutNom.Text.Length < 2 || txtAjoutNom.Text.Length > 20 || txtAjoutPrenom.Text.Length < 2 || txtAjoutPrenom.Text.Length > 20)
                MessageBox.Show("Le nombre de caractère pour nom et prénom doit être entre 2 et 20 !!");
            //Validation Poste
            else if (txtAjoutPoste.Text.Length < 3 || txtAjoutPoste.Text.Length > 20)
                MessageBox.Show("Le nombre de caractère pour la poste doit être entre 3 et 20 !!");
             //Vérification si le même nom et prénom de l'employé existe ou non
            else if(_ServiceMysql.ExisteEmploye(txtAjoutNom.Text, txtAjoutPrenom.Text,null))
                MessageBox.Show("Erreur : Il y existe déjà un employé avec le même nom et le même prénom!!!");
            //validation salaire
            else if (!ValidSalaire(txtAjoutSalaire.Text))
                MessageBox.Show("Le salaire ne peut pas laisser vide et il doit être entre 0 et 500 !!");
            else
            {   //Insérer dans la BD
                _ServiceMysql.AjoutUnEmploye(txtAjoutNom.Text, txtAjoutPrenom.Text, txtAjoutPoste.Text, txtAjoutSalaire.Text,this.chxHorsFonction.IsChecked.Value, LiaisonProjetEmploye);
                /* string filepath = "EmployePhoto.jpg";
                 string name = System.IO.Path.GetFileName(filepath);
                 string destinationPath = GetDestinationPath(name, "image");

                 File.Copy(filepath, destinationPath, true);*/
                
                MessageBox.Show("Le nouveau employé est ajouté!");
                retourMenu(this, null);
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
        public bool ValidSalaire(string salaire)
        {
            if(salaire.Length > 0)
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
              /*  MemoryStream ms = new MemoryStream();
                Byte[] bindata = _ServiceMysql.GetPhoto();
                ms.Write(bindata, 0, bindata.Length);
                imgPhoto.Source = ToImage(bindata);*/

                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
                FileStream fs;
                BinaryReader br;
                string FileName = op.FileName;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                Byte[] ImageData = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                _ServiceMysql.AjouterPhoto(ImageData);
            }
        }
        /*   private static String GetDestinationPath(string filename, string foldername)
         {
             String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
             appStartPath = String.Format(appStartPath + "\\{0}\\" + filename, foldername);
             return appStartPath;
         }*/
        public BitmapImage ToImage(Byte[] image)
        {
            BitmapImage imageSource = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(image))
            {
                stream.Seek(0, SeekOrigin.Begin);
                imageSource.BeginInit();
                imageSource.StreamSource = stream;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
            }

            return imageSource;
        }
        public Byte[] ImageToByte(BitmapImage imageSource)
        {
            Stream stream = imageSource.StreamSource;
            Byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }
        #endregion
    }
}
