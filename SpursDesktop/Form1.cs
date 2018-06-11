using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SpursDesktop
{
    public partial class Form1 : Form
    {
        // Папка по умолчанию
        String PATH = "spurs/";
        // Коллекция карточек вопросов
        List<Quest> DataBase = new List<Quest>();
        // Начало и конец файла
        String BEG = "<table border=\"1\"><tbody>";
        String END = "</tbody></table>";
        // Порядковый номер выбранного элемента
        int numb;
        public Form1()
        {
            InitializeComponent();
            // Проверяем существование папки
            if (!Directory.Exists(PATH))
                // Если её нет, создаём
                Directory.CreateDirectory(PATH);
            // Получаем имена всех файлов из папки
            String[] files = Directory.GetFiles(PATH);
            // Заполняем выпадающий список
            FillComboBox(files);
        }

        // Заполнение comboBox
        protected void FillComboBox(String[] files)
        {
            // Создаём новую коллекцию из String
            List<String> DBs = new List<string>();
            // Заносим в неё имена подходящих файлов
            foreach(String i in files)
            {
                if (i.EndsWith(".html"))
                    DBs.Add(Path.GetFileName(i));
            }
            // добавляем пункт создания
            DBs.Add("--Создать новый элемент");
            // привязываем список к коллекции
            comboBox1.DataSource = DBs;
        }

        // Если изменился выбор в списке
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем имя файла
            String fname = ((ComboBox)sender).Text;
            if (fname == "--Создать новый элемент")
            {
                CreateNewDB();
                return;
            }
            // Создаём папку для изображений
            Directory.CreateDirectory(PATH+Path.GetFileNameWithoutExtension(fname));
            // Открываем файл
            StreamReader streamReader = new StreamReader(PATH + fname,Encoding.Unicode);
            // Копируем его содержимое в переменную
            String HTML = streamReader.ReadToEnd();
            // Закрываем файл
            streamReader.Close();
            // Заполняем БД
            FillDB(HTML);
            // Заполняем список вопросов
            FillQuestList();
            // Выключаем возможность измнения
            SetEditEnabled(false);
        }

        // Создание новой базы данных
        private void CreateNewDB()
        {
            // Выключаем все кнопки
            SetEditEnabled(false);
            listBox2.Enabled = false;
            button3.Enabled = false;
            // Изменяемм тип выпадающего списка
            comboBox1.DropDownStyle = ComboBoxStyle.Simple;
            // Включаем кнопку создания
            button6.Visible = true;
            // Обнуляем текст в поле
            comboBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String name = comboBox1.Text;
            if (!name.EndsWith(".html"))
                name += ".html";
            FileStream file = new FileStream(PATH + "/" + name, FileMode.Create);
            file.Close();
            FillComboBox(Directory.GetFiles(PATH));
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            button6.Visible = false;
            comboBox1.SelectedItem = name;
            // Включаем минимальный набор кнопок
            listBox2.Enabled = true;
            button3.Enabled = true;
        }

        // Заполнение БД
        private void FillDB (String HTML)
        {
            // Удаляем ненужные строки и переносы
            HTML = HTML.Replace(BEG, "");
            HTML = HTML.Replace(END, "");
            HTML = HTML.Replace("\n", "");
            // Разбиваем построчно
            String[] que = HTML.Split(new String[] { "<tr>", "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            // Очищаем ДБ
            DataBase.Clear();
            // Заносим каждую строку в БД 
            foreach (String i in que)
            {
                DataBase.Add(new Quest(i));
            }
        }   
        
        // Заполняем список вопросов
        private void FillQuestList()
        {
            listBox2.Items.Clear();
            foreach(Quest i in DataBase)
            {
                listBox2.Items.Add(i.GetQuestion());
            }
            // Если не выбран подходящий элемент - запрещаем изменения
            if (listBox2.SelectedIndex == -1)
                SetEditEnabled(false);
        }

        // Был выбран вопрос
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем номер вопроса
            numb = ((ListBox)sender).SelectedIndex;
            // Если вопрос не выбран - завершаем
            if (numb == -1)
                return;
            // Записываем текст вопроса
            textBox1.Text = DataBase[numb].GetQuestion();
            // Записываем текст ответа
            textBox2.Text = (DataBase[numb].GetAnswer()).Replace("\n", "\r\n");
            // Заполняем раздел картинок
            FillImages();
            // Разрешаем изменения
            SetEditEnabled(true);
        }

        private void FillImages()
        {
            String[] images = DataBase[numb].GetImage().Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            listBox1.Items.Clear();
            foreach (String i in images)
            {
                listBox1.Items.Add(i);
            }
        }

        private void SetEditEnabled(bool f)
        {
            if (!f)
            {
                textBox1.Text = textBox2.Text = "";
                listBox1.Items.Clear();
            }
            textBox1.Enabled = f;
            textBox2.Enabled = f;
            listBox1.Enabled = f;
            button1.Enabled = f;
            button2.Enabled = f;
            button4.Enabled = f;
            button5.Enabled = f;
        }

        //Если изменился текст вопроса
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            String text = ((TextBox)sender).Text;
            if (text == "")
                return;
            DataBase[numb].SetQuestion(text);
            listBox2.Items[numb] = text;
        }

        // Если изменился текст ответа
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            String text = ((TextBox)sender).Text;
            if (text == "")
                return;
            DataBase[numb].SetAnswer(text.Replace("\r",""));
        }

        // Добавление нового вопроса
        private void button3_Click(object sender, EventArgs e)
        {
            // Добавляем пустую форму в ДБ
            DataBase.Add(new Quest());
            // Обновляем список
            FillQuestList();
            // Выделяем последний элемент
            listBox2.SelectedIndex = listBox2.Items.Count - 1;
        }

        // Удаляем выбранный элемент
        private void button4_Click(object sender, EventArgs e)
        {
            // Получаем индекс элемента
            int ind = listBox2.SelectedIndex;
            // Удаляем элемент из БД
            DataBase.RemoveAt(ind);
            // Заполняем список вопросов
            FillQuestList();
            // Устанавливаем выделение
            if (ind < listBox2.Items.Count)
                listBox2.SelectedIndex = ind;
            else
                listBox2.SelectedIndex = ind - 1;
        }


        ///// Работа с картинками
        // Добавить картинку
        private void button1_Click(object sender, EventArgs e)
        {
            // Открываем диалог выбора файла
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";
            if (open.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            String filename = open.FileName;
            String ext = Path.GetExtension(filename);
            // Выбираем файлу новое имя
            String newName = GetEmptyImageNumber(ext);
            // Копируем картинку
            File.Copy(filename, newName);
            // Поулчаем имя для BD
            String newIMGName = Path.GetFileNameWithoutExtension(comboBox1.Text) + "/" + Path.GetFileName(newName);
            DataBase[numb].SetImage(DataBase[numb].GetImage() + newIMGName + "\n");
            FillImages();
        }
        // Выбор имени картинки
        private String GetEmptyImageNumber(String ext)
        {
            // Получаем новый путь
            String newPath = PATH + Path.GetFileNameWithoutExtension(comboBox1.Text)+"/";
            // Находим первое незанятое имя
            int imageNumber = 0;
            while (File.Exists(newPath + imageNumber.ToString() + ext))
                imageNumber++;
            // Возвращаем имя
            return newPath + imageNumber.ToString() + ext;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;
            String filename = (String)listBox1.SelectedItem;
            DataBase[numb].SetImage(DataBase[numb].GetImage().Replace(filename + "\n", ""));
            File.Delete(PATH+"/"+filename);
            FillImages();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Сохраняем файл
            SaveDataBase();
        }

        // Функция сохранения БД в файл
        private void SaveDataBase()
        {
            FileStream file = new FileStream(PATH + comboBox1.Text, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(file, Encoding.Unicode);
            streamWriter.Write(BEG + "\n");
            foreach (Quest q in DataBase)
                streamWriter.Write(q.GetHTML() + "\n");
            streamWriter.Write(END);
            streamWriter.Close();
            file.Close();
        }
    }
}
