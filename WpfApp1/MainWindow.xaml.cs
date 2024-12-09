using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CompressModule;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string debug = "";




            // Загружаем изображение
            Module.LoadImage("Emblem.png", "my_image");


            // Записываем сжатое изображение в файл
            Module.WriteImage("my_image", "Emb.pib2201_kursovaya");


            // Удаляем изображение из словаря
            Module.DeleteImage("my_image");


            // Читаем ранее записанный файл
            Module.ReadImage("Emb.pib2201_kursovaya", "new_image");


            // Достаём ихображение из слования
            Image image = Module.GetImage("new_image");


            //Сигнал об успешном выполнение всех пунктов (Должно быть True)
            MessageBox.Show((image is not null).ToString());


            InitializeComponent();
            
        }
    }
}