using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanyHR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Employee.textBoxID = textBoxID;
            Employee.textBoxFirstName = textBoxFirstName;
            Employee.textBoxLastName = textBoxLastName;
            Employee.textBoxYob = textBoxYOB;
            Employee.comboBoxTitle = comboBoxTitle;
            Employee.textBoxSalary = textBoxSalary;
            Employee.checkedListBoxLanguages = checkedListBoxLanguages;
            Employee.comboBoxMgr = comboBoxManager;
            Developer.textBoxBonus = textBoxBonus;
            Developer.checkedListBoxSkills = checkedListBoxSkills;
            Manager.textBoxBonus = textBoxBonus;
            Manager.dataGridViewSubs = dataGridViewSubs;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            textBoxID.Text = "";
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            textBoxYOB.Text = "";
            comboBoxTitle.SelectedIndex = -1;
            textBoxSalary.Text = "";
            textBoxBonus.Text = "";
            textBoxFirstName.Focus();
            Employee.ClearLangListBox();
            ResetComponents();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text == "")
            {
                Employee emp;
                if (comboBoxTitle.Text == Developer.DEV_TITLE)
                    emp = Developer.CreateDeveloper(textBoxFirstName.Text, textBoxLastName.Text,
                        int.Parse(textBoxYOB.Text), double.Parse(textBoxSalary.Text), double.Parse(textBoxBonus.Text));
                else if (Manager.IsManagementTitle(comboBoxTitle.Text))
                {
                    emp = Manager.CreateManager(textBoxFirstName.Text, textBoxLastName.Text,
                        int.Parse(textBoxYOB.Text), comboBoxTitle.Text, double.Parse(textBoxSalary.Text), double.Parse(textBoxBonus.Text));
                    Manager.UpdateManagers(emp);
                }
                else
                    emp = Employee.CreateEmployee(textBoxFirstName.Text, textBoxLastName.Text,
                        int.Parse(textBoxYOB.Text), comboBoxTitle.Text, double.Parse(textBoxSalary.Text));
                if (emp == null)
                    return;
                textBoxID.Text = emp.GetID().ToString();
                emp.ReassignAdditionalData();
                dataGridView.Rows.Add(emp.GetID(), emp.GetFirstName(),
                    emp.GetLastName(), emp.title);
                dataGridView.Rows[dataGridView.Rows.Count - 2].Selected = true;
            }
            else
            {
                int empID = int.Parse(textBoxID.Text);
                Employee emp = Employee.GetById(empID);
                if (emp == null)
                    return;
                emp.Update();
            }
            Employee.SaveAll();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Employee.Init();
            Developer.Init();
            Manager.Init();
            comboBoxTitle.Items.AddRange(Employee.GetAllTitles().ToArray());
            checkedListBoxLanguages.Items.AddRange(Employee.GetAllLanguages().ToArray());
            checkedListBoxSkills.Items.AddRange(Developer.GetAllSkills().ToArray());
            dataGridView.Rows.Clear();
            Employee.LoadFromFile();

            foreach (Employee emp in Employee.GetAllEmployees())
            {
                dataGridView.Rows.Add(emp.GetID(), emp.GetFirstName(),
                    emp.GetLastName(), emp.title);
                if (Manager.IsManagementTitle(emp.title))
                    comboBoxManager.Items.Add(emp);
            }
        }

        private void ResetComponents()
        {
            textBoxBonus.Text = "";
            textBoxBonus.ReadOnly = true;
            foreach (int index in checkedListBoxSkills.CheckedIndices)
            {
                checkedListBoxSkills.SetItemChecked(index, false);
            }
            checkedListBoxSkills.Enabled = false;
            comboBoxManager.SelectedIndex = -1;
        }

        private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.Rows.Count == 0)
                return;
            int empID = int.Parse(dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
            Employee emp = Employee.GetById(empID);
            if (emp == null)
                return;
            ResetComponents();
            emp.Display();
        }

        private void comboBoxTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetComponents();
            if (comboBoxTitle.Text == Developer.DEV_TITLE)
                Developer.EnableDevComponents();
            else if (Manager.IsManagementTitle(comboBoxTitle.Text))
                Manager.EnableMgmComponents();
        }
    }
}
