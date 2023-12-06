using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CompanyHR
{
    internal class Manager : Employee
    {
        static List<string> managementTitles = new List<string>();
        private const string MANAGEMENT_TITLES_FILE_NAME = "management_titles.txt";

        static public TextBox textBoxBonus;
        static public DataGridView dataGridViewSubs;

        new public static void Init()
        {
            string managementData = Employee.DIR_TO_SAVE + "\\" + MANAGEMENT_TITLES_FILE_NAME;
            string[] lines = File.ReadAllLines(managementData);
            foreach (string line in lines)
            {
                managementTitles.Add(line);
            }
        }

        public static bool IsManagementTitle(string title)
        {
            return managementTitles.Contains(title);
        }

        public static bool IsManagerData(string line)
        {
            foreach (string mgmTitle in managementTitles)
                if (line.IndexOf(mgmTitle) >= 0)
                    return true;
            return false;
        }

        double bonus;
        List<Employee> subordinates = new List<Employee>();

        public static Employee CreateManager(string firstNameParam, string lastNameParam,
            int yobParam, string titleParam, double salaryParam, double bonusParam)
        {
            if (bonusParam < 0)
            {
                MessageBox.Show("Wrong bonus");

                return null;
            }
            return new Manager(firstNameParam, lastNameParam, yobParam, titleParam, salaryParam, bonusParam);
        }

        protected Manager(string firstNameParam, string lastNameParam,
            int yobParam, string titleParam, double salaryParam, double bonusParam) :
            base(firstNameParam, lastNameParam, yobParam, titleParam, salaryParam)
        {
            bonus = bonusParam;
        }

        public Manager(string line) : base(line)
        {
            string[] properties = line.Split(fieldSeparator);
            bonus = double.Parse(properties[6]);
        }

        protected override int EndOfPersonalPos()
        {
            return 7;
        }

        protected override void AddToContainers(string line, int endPos)
        {
            base.AddToContainers(line, endPos);
            string[] properties = line.Split(fieldSeparator);
            if (properties.Length <= EndOfPersonalPos() + 1)
                return;
            string[] allSubIDs = properties[EndOfPersonalPos() + 1].Split(languageSeparator);
            foreach (string empId in allSubIDs)
            {
                if (empId.Trim() == "")
                    continue;
                if (Employee.GetById(int.Parse(empId)) != null)
                    subordinates.Add(Employee.GetById(int.Parse(empId)));
            }
        }

        protected override string ConstructPersonalData()
        {
            return base.ConstructPersonalData() + Employee.fieldSeparator + bonus.ToString();
        }

        protected override string ConstructAdditionalData()
        {
            string result = base.ConstructAdditionalData() + Employee.fieldSeparator;
            foreach (Employee emp in subordinates)
            {
                result = result + emp.GetID().ToString() + Employee.languageSeparator;
            }
            return result;
        }

        public override void Display()
        {
            base.Display();
            textBoxBonus.Text = bonus.ToString();
            dataGridViewSubs.Rows.Clear();
            foreach (Employee emp in subordinates)
                dataGridViewSubs.Rows.Add(emp.GetID(), emp.GetFirstName(),
                    emp.GetLastName(), emp.title);
        }

        public override void Update()
        {
            base.Update();
            bonus = double.Parse(textBoxBonus.Text);
        }

        public static void EnableMgmComponents()
        {
            textBoxBonus.ReadOnly = false;
            dataGridViewSubs.Enabled = true;
        }

        public void AddSubordinate(Employee emp)
        {
            subordinates.Add(emp);
        }

        public bool HasSubordinate(Employee emp)
        {
            return subordinates.Contains(emp);
        }

        public void RemoveSubordinate(Employee emp)
        {
            if (subordinates.Contains(emp))
                subordinates.Remove(emp);
        }

        public static void UpdateManagers(Employee emp)
        {
            Employee.comboBoxMgr.Items.Add(emp);
        }
    }
}
