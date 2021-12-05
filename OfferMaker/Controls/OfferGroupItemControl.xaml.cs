using System;
using System.Collections.Generic;
using System.Linq;
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

namespace OfferMaker.Controls
{
    /// <summary>
    /// Interaction logic for OfferGroupItemControl.xaml
    /// </summary>
    public partial class OfferGroupItemControl : UserControl
    {
        List<string> AllHints { get; set; }

        public OfferGroupItemControl()
        {
            InitializeComponent();
            //AllHints = Global.Main.GetHints();
            AllHints = new List<string>() { "111","222","333","444","555","666","777", "888", "999", "11", "22", "33", "44", "55", "66", "77", "88", "99", "1111", "2222", "3333", };
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                var hints = GetHints(textBox.Text);
                groupComboBox.ItemsSource = hints;
                if (hints.Count > 0)
                    groupComboBox.IsDropDownOpen = true;
                else
                    groupComboBox.IsDropDownOpen = false;
            }
            else
            {
                groupComboBox.IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Получаем фразы, начинающиеся с введённой строки, 
        /// устанавливая при этом глубину: int depth = 7;
        /// int depth = 7; - это очень плохая практика, необходимо предусмотреть установку данной переменной где-то в конфигурационном файле по умолчанию
        /// и, возможно, её изменение в настройках пользовательского интерфеса.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private List<string> GetHints(string text)
        {
            text = text.ToLower().Trim();
            int depth = 7; //максимальное кол-во отображаемых элементов
            List<string> tempList = new List<string>();

            //ищем вхождения введённого текста в хинтах в начале строки
            foreach (var hint in AllHints)
            {
                if (hint.ToLower().Trim().StartsWith(text))
                    tempList.Add(hint);
            }

            //ищем вхождения введённого текста в хинтах по всему хинту
            if (tempList.Count < depth)
            {
                //перебираем только хинты, которых ещё нет в списке
                foreach (var hint in AllHints)
                {
                    if (!tempList.Contains(hint))
                    {
                        if (hint.ToLower().Trim().Contains(text))
                            tempList.Add(hint);
                    }
                    if (tempList.Count == depth) break;
                }
            }

            return tempList.Count >= depth ? tempList.Take(depth).ToList() : tempList;
        }

        private void groupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ComboBox)sender).SelectedValue!=null)
                groupTitleTextBox.Text = ((ComboBox)sender).SelectedValue.ToString();
        }
    }
}
