using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CompanyHR
{
    class Employee
    {
        static private int maxID = 0;
        static private List<Employee> allEmployees = new List<Employee>();
        protected const char fieldSeparator = '|';
        protected const char languageSeparator = ',';
        protected const string DIR_TO_SAVE = "C:\\Users\\DELL\\source\\repos\\HR\\Simple_HR";
        private const string FILE_NAME = "employees.txt";
        private const string TITLE_FILE_NAME = "titles.txt";
        private const string LANGUAGES_FILE_NAME = "languages.txt";
        static private List<string> titles = new List<string>();
        static private List<string> allLanguages = new List<string>();

        public static TextBox textBoxID;
        public static TextBox textBoxFirstName;
        public static TextBox textBoxLastName;
        public static TextBox textBoxYob;
        public static ComboBox comboBoxTitle;
        public static TextBox textBoxSalary;
        public static CheckedListBox checkedListBoxLanguages;
        public static ComboBox comboBoxMgr;


        private int ID;
        private string firstName;
        private string lastName;
        public int yob;
        public string title;
        public double salary;
        private List<string> languages = new List<string>();

        public static List<Employee> GetAllEmployees()
        {
            return allEmployees;
        }

        public static Employee CreateEmployee(string firstNameParam, string lastNameParam,
            int yobParam, string titleParam, double salaryParam)
        {
            if (salaryParam < 0)
            {
                MessageBox.Show("The salary is wrong.");
                textBoxSalary.Focus();
                return null;
            }
            return new Employee(firstNameParam, lastNameParam,
                yobParam, titleParam, salaryParam);
        }
        protected Employee(string firstNameParam, string lastNameParam,
            int yobParam, string titleParam, double salaryParam)
        {
            ID = ++maxID;
            firstName = firstNameParam;
            lastName = lastNameParam;
            yob = yobParam;
            title = titleParam;
            salary = salaryParam;
            allEmployees.Add(this);
        }

        protected Employee(string line)
        {
            string[] properties = line.Split(fieldSeparator);
            ID = int.Parse(properties[0]);
            maxID = ID;
            firstName = properties[1];
            lastName = properties[2];
            yob = int.Parse(properties[3]);
            title = properties[4];
            salary = double.Parse(properties[5]);
            allEmployees.Add(this);
            AddToContainers(line, EndOfPersonalPos());
        }

        protected virtual int EndOfPersonalPos()
        {
            return 6;
        }
        protected virtual void AddToContainers(string line, int endPos)
        {
            string[] properties = line.Split(fieldSeparator);
            if (properties.Length <= endPos)
                return;
            string[] allLangs = properties[endPos].Split(languageSeparator);
            languages = new List<string>(allLangs);
        }
        public int GetID()
        {
            return ID;
        }

        public string GetFirstName()
        {
            return firstName;
        }

        public string GetLastName()
        {
            return lastName;
        }

        public List<string> GetLanguages()
        {
            return languages;
        }

        public void AddLanguage(string lang)
        {
            languages.Add(lang);
        }

        private string StringToSave()
        {
            return ConstructPersonalData() + fieldSeparator + ConstructAdditionalData();
        }

        protected virtual string ConstructPersonalData()
        {
            return ID.ToString() + fieldSeparator + firstName + fieldSeparator
                + lastName + fieldSeparator + yob.ToString() + fieldSeparator
                + title + fieldSeparator + salary.ToString();
        }

        protected virtual string ConstructAdditionalData()
        {
            return String.Join(languageSeparator.ToString(), languages);
        }

        public virtual void Display()
        {
            Employee.textBoxID.Text = ID.ToString();
            Employee.textBoxFirstName.Text = firstName;
            Employee.textBoxLastName.Text = lastName;
            Employee.textBoxYob.Text = yob.ToString();
            Employee.comboBoxTitle.Text = title;
            Employee.textBoxSalary.Text = salary.ToString();
            Employee.ClearLangListBox();
            if (checkedListBoxLanguages.Items.Count == 0)
                return;
            foreach (string lang in languages)
            {
                int index = checkedListBoxLanguages.Items.IndexOf(lang);
                if (index < 0)
                    continue;
                checkedListBoxLanguages.SetItemChecked(index, true);
            }
            
            foreach(Employee emp in allEmployees)
            {
                if ((emp is Manager) && ((Manager)emp).HasSubordinate(this))
                    comboBoxMgr.SelectedItem = emp;
            }

            EnableCompomnents();
        }

        public virtual void Update()
        {
            title = Employee.comboBoxTitle.Text;
            salary = double.Parse(Employee.textBoxSalary.Text);
            ReassignAdditionalData();

            foreach (Employee emp in allEmployees)
                if (emp is Manager)
                    ((Manager)emp).RemoveSubordinate(this);
            if (comboBoxMgr.SelectedIndex >= 0)
            {
                Manager mgr = (Manager)comboBoxMgr.Items[comboBoxMgr.SelectedIndex];
                mgr.AddSubordinate(this);
            }
        }

        public virtual void ReassignAdditionalData()
        {
            languages.Clear();
            foreach (object lang in checkedListBoxLanguages.CheckedItems)
            {
                AddLanguage(lang.ToString());
            }
        }

        public static void ClearLangListBox()
        {
            foreach (int index in checkedListBoxLanguages.CheckedIndices)
            {
                checkedListBoxLanguages.SetItemChecked(index, false);
            }
        }
        public static void SaveAll()
        {
            string fileToSave = Employee.DIR_TO_SAVE + "\\" + FILE_NAME;
            StreamWriter sw = File.CreateText(fileToSave);
            foreach (Employee em in allEmployees)
            {
                sw.WriteLine(em.StringToSave());
            }
            sw.Close();
        }

        public static void LoadFromFile()
        {
            string fileData = Employee.DIR_TO_SAVE + "\\" + FILE_NAME;
            string[] lines = File.ReadAllLines(fileData);
            foreach (string line in lines)
            {
                Employee emp;
                if (line.IndexOf(Developer.DEV_TITLE) >= 0)
                    emp = new Developer(line);
                else if (Manager.IsManagerData(line))
                    emp = new Manager(line);
                else
                    emp = new Employee(line);
            }
        }
        public static Employee GetById(int id)
        {
            foreach (Employee emp in allEmployees)
                if (emp.ID == id)
                    return emp;
            return null;
        }

        public static void Init()
        {
            string titleData = Employee.DIR_TO_SAVE + "\\" + TITLE_FILE_NAME;
            string[] lines = File.ReadAllLines(titleData);
            foreach (string line in lines)
            {
                titles.Add(line);
            }

            string langData = Employee.DIR_TO_SAVE + "\\" + LANGUAGES_FILE_NAME;
            lines = File.ReadAllLines(langData);
            foreach (string line in lines)
            {
                allLanguages.Add(line);
            }
        }

        public static List<string> GetAllTitles()
        {
            return new List<string>(titles);
        }

        public static List<string> GetAllLanguages()
        {
            return new List<string>(allLanguages);
        }

        public virtual void EnableCompomnents()
        {

        }

        public override string ToString()
        {
            return firstName + " " + lastName;
        }
    }
   
}
