using nutritionoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace nutritionoffice.Classes
{
    public class BodyFatAndDensityClass
    {
        public BodyFatAndDensityClass() { }

        public BodyFatAndDensityClass(Customer.sex sex, double age, double chest, double axilla,double tricep, double subscapular, double abdominal, double suprailiac, double thigh) {
            Sex = sex;
            Age = age;
            Chest.Value = chest;
            Axilla.Value = axilla;
            Tricep.Value = tricep;
            Subscapular.Value = subscapular;
            Abdominal.Value = abdominal;
            Subscapular.Value = suprailiac;
            Thigh.Value = thigh;
        }
        public Customer.sex Sex { get; set; }
        public double Age { get; set; }

        public Skinfold Chest { get; set; } = new Skinfold("Chest");
        public Skinfold Axilla { get; set; } = new Skinfold("Axilla");
        public Skinfold Tricep { get; set; } = new Skinfold("Tricep");
        public Skinfold Subscapular { get; set; } = new Skinfold("Subscapular");
        public Skinfold Abdominal { get; set; } = new Skinfold("Abdominal");
        public Skinfold Suprailiac { get; set; } = new Skinfold("Suprailiac");
        public Skinfold Thigh { get; set; } = new Skinfold("Thigh");

        
        
    }

    public class Skinfold
    {
        public Skinfold(string name) { _Name = name; Value = 0d; }
        private string _Name;
        public string Name { get { return new ResourceManager(typeof(Resource)).GetString(_Name); } }
        public double Value { get; set; }
    }
}