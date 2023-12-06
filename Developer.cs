using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CompanyHR
{
    class Developer: Employee
    {
        public const string DEV_TITLE = "developer";
        private const string SKILLS_FILE_NAME = "skills.txt";

        public static TextBox textBoxBonus;
        public static CheckedListBox checkedListBoxSkills;

        static private List<string> allSkills = new List<string>();


        double bonus;
        List<string> skills = new List<string>();

        public static Developer CreateDeveloper(string firstNameParam, string lastNameParam,
            int yobParam, double salaryParam, double bonusParam)
        {
            if (bonusParam < 0)
                return null;
            return new Developer(firstNameParam, lastNameParam, yobParam, salaryParam, bonusParam);
        }
        private Developer(string firstNameParam, string lastNameParam,
            int yobParam, double salaryParam, double bonusParam) :
            base(firstNameParam, lastNameParam, yobParam, DEV_TITLE, salaryParam)
        {
            bonus = bonusParam;
        }

        public Developer(string line): base(line)
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
            if (properties.Length <= 8)
                return;
            string[] allSkills = properties[8].Split(languageSeparator);
            skills = new List<string>(allSkills);
        }

        public override void Display()
        {
            base.Display();
            textBoxBonus.Text = bonus.ToString();
            checkedListBoxSkills.Enabled = true;
            if (checkedListBoxSkills.Items.Count == 0)
                return;
            foreach (string skill in skills)
            {
                int index = checkedListBoxSkills.Items.IndexOf(skill);
                if (index < 0)
                    continue;
                checkedListBoxSkills.SetItemChecked(index, true);
            }
        }

        public override void Update()
        {
            base.Update();
            bonus = double.Parse(textBoxBonus.Text);
            //ReassignSkills();
        }

        public void ReassignSkills()
        {
            skills.Clear();
            foreach (object skill in checkedListBoxSkills.CheckedItems)
            {
                skills.Add(skill.ToString());
            }
        }

        protected override string ConstructPersonalData()
        {
            return base.ConstructPersonalData() + Employee.fieldSeparator + bonus.ToString();
        }

        protected override string ConstructAdditionalData()
        {
            return base.ConstructAdditionalData() + Employee.fieldSeparator +
                String.Join(languageSeparator.ToString(), skills);
        }

        public override void ReassignAdditionalData()
        {
            base.ReassignAdditionalData();
            ReassignSkills();
        }

        public static void ClearSkillsListBox()
        {
            foreach (int index in checkedListBoxSkills.CheckedIndices)
            {
                checkedListBoxSkills.SetItemChecked(index, false);
            }
        }

        new public static void Init()
        {
            string langData = Employee.DIR_TO_SAVE + "\\" + SKILLS_FILE_NAME;
            string[] lines = File.ReadAllLines(langData);
            foreach (string line in lines)
            {
                allSkills.Add(line);
            }
        }

        public static List<string> GetAllSkills()
        {
            return new List<string>(allSkills);
        }

        public override void EnableCompomnents()
        {
            EnableDevComponents();
        }

        public static void EnableDevComponents()
        {
            textBoxBonus.ReadOnly = false;
            checkedListBoxSkills.Enabled = true;
        }
    }
}
