using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;

namespace nutritionoffice.Models
{
    public class Company
    {
        [Key, Required]
        public int id { get; set; }

        private bool _IsDemo = true;

        [Display(ResourceType = typeof(Resource), Name = "Demo")]
        public bool IsDemo
        {
            get
            {
                return _IsDemo;
            }
            set
            {
                _IsDemo = value;
            }
        }

        [DataType(dataType: DataType.Date), DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true), Display(ResourceType = typeof(Resource), Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [DataType(dataType: DataType.Date), DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true), Display(ResourceType = typeof(Resource), Name = "LastPayment")]
        public DateTime? LastPayment { get; set; }

        [Range(typeof(decimal), minimum: "0", maximum: "3000"), Display(ResourceType = typeof(Resource), Name = "Euro")]
        public decimal Euro { get; set; }

        [DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "Remarks")]
        public string Notes { get; set; }

        [Required]
        [StringLength(100), Display(ResourceType = typeof(Resource), Name = "CompanyName")]
        public string CompanyName { get; set; }

        [StringLength(100), Display(ResourceType = typeof(Resource), Name = "OwnersName")]
        public string Owner { get; set; }

        [StringLength(25), Display(ResourceType = typeof(Resource), Name = "PhoneNumber")]
        public string Phone { get; set; }

        [StringLength(50), DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [StringLength(maximumLength: 11, ErrorMessage = "SMS Sign Should be less than 12 characters"), Display(ResourceType = typeof(Resource), Name = "SMSSign"), RegularExpression(pattern: "[A-Z]+", ErrorMessage = "Only Capitals please!!")]
        public string SMSSign { get; set; }

        [StringLength(30), Display(ResourceType = typeof(Resource), Name = "SMSUserName")]
        public string SMSUserName { get; set; }

        [StringLength(30), DataType(DataType.Password), Display(ResourceType = typeof(Resource), Name = "SMSPassword")]
        public string SMSPassword { get; set; }

        [StringLength(25), Display(ResourceType = typeof(Resource), Name = "EmergencyOrMobileNumber")]
        public string EmergencyPhone { get; set; }

        [StringLength(60), Display(ResourceType = typeof(Resource), Name = "OfficeAddress")]
        public string Address { get; set; }

        [StringLength(60), Display(ResourceType = typeof(Resource), Name = "OfficeCity")]
        public string City { get; set; }

        [StringLength(10), Display(ResourceType = typeof(Resource), Name = "OfficePostCode")]
        public string PostCode { get; set; }

        [StringLength(50)]
        public string FaceBook { get; set; }

        [StringLength(50)]
        public string Twitter { get; set; }

        [StringLength(50)]
        public string Instagram { get; set; }

        [StringLength(50), Display(Name = "SMTP Host Address")]
        public string SMTPHost { get; set; }

        [Display(Name = "SMTP Enable SSL")]
        public bool SMTPEnableSSL { get; set; }

        [Display(Name = "SMTP Port")]
        public int SMTPPort { get; set; }

        [StringLength(50), Display(Name = "SMTP login UserName")]
        public string SMTPUserName { get; set; }

        [StringLength(50), Display(Name = "SMTP login Password"), DataType(DataType.Password)]
        public string SMTPPassword { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Logo")]
        public byte[] logo { get; set; }

        public virtual ICollection<TargetGroup> TargetGroups { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Customers")]
        public virtual ICollection<Customer> Customers { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "RecipeCategories")]
        public virtual ICollection<RecipeCategory> RecipeCategories { get; set; }

        public virtual ICollection<FoodCategory> FoodCategories { get; set; }

        public virtual Picture Picture { get; set; }

        public virtual ICollection<AgeRange> AgeRanges { get; set; }
    }

    public class Picture
    {
        [ForeignKey("Company")]
        public int PictureID { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "CompanyName")]
        public int CompanyID { get; set; }

        public virtual Company Company { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "HomePageLogo")]
        public byte[] Logo { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ReportBackgroundPortrait")]
        public byte[] ReportBackgroundPortrait { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ReportBackgroundLandscape")]
        public byte[] ReportBackgroundLandscape { get; set; }

    }

    public class AgeRange
    {
        [Key, Required]
        public int id { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [ForeignKey("CompanyID")]
        public virtual Company Company { get; set; }

        [Required, Range(minimum: 0d, maximum: 120, ErrorMessage = "From Age must be between 0 and 120.")]
        public double FromAge { get; set; }

        [Required, Range(minimum: 0d, maximum: 120, ErrorMessage = "To Age must be between 0 and 120.")]
        public double ToAge { get; set; }

        public string RangeText
        {
            get
            {
                return string.Format($"{FromAge:N1} - {ToAge:N1}");
            }
        }
    }

    public class TargetGroup
    {
        [Key, Required]
        public int id { get; set; }

        [StringLength(40), Required]
        [Display(ResourceType = typeof(Resource), Name = "TargetGroupID")]
        public string Name { get; set; }

        public int CompanyID { get; set; }

        [ForeignKey(name: "CompanyID")]
        public virtual Company Company { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "IsActivefemale")]
        public bool IsActive { get; set; } = true;

        [Display(ResourceType = typeof(Resource), Name = "Remarks"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }

    public class Customer
    {
        [Key, Required]
        public int id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "TargetGroupID")]
        public int TargetGroupID { get; set; }
        public virtual TargetGroup TargetGroup { get; set; }

        public int CompanyID { get; set; }

        [ForeignKey(name: "CompanyID")]
        public virtual Company Company { get; set; }

        [Required, StringLength(50), Display(ResourceType = typeof(Resource), Name = "Surname")]
        public string LastName { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "Firstname")]
        public string FirstName { get; set; }

        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "FullName")]
        public string FullName { get { return string.Format($"{LastName?.Trim()} {FirstName?.Trim()}"); } }

        [Display(ResourceType = typeof(Resource), Name = "DateOfBirth"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "Sex")]
        public sex Sex { get; set; } = sex.Female;

        [StringLength(25), Display(ResourceType = typeof(Resource), Name = "PhoneNumber")]
        public string Phone { get; set; }

        [StringLength(25), Display(ResourceType = typeof(Resource), Name = "MobilePhone")]
        public string Mobile { get; set; }

        [StringLength(35), Display(Name = "email"), DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [StringLength(35), Display(Name = "FaceBook")]
        public string Facebook { get; set; }

        [StringLength(45), Display(ResourceType = typeof(Resource), Name = "City")]
        public string City { get; set; }

        [StringLength(100), Display(ResourceType = typeof(Resource), Name = "HomeAddress")]
        public string Address { get; set; }

        [StringLength(15), Display(ResourceType = typeof(Resource), Name = "PostCode")]
        public string PostCode { get; set; }

        public enum sex
        {
            [Display(ResourceType = typeof(Resource), Name = "ManSex")]
            Male = 2,
            [Display(ResourceType = typeof(Resource), Name = "WomanSex")]
            Female = 4
        }

        [Display(ResourceType = typeof(Resource), Name = "Remarks"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "CreatedOn"), DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "LastChangedOn"), DataType(DataType.DateTime)]
        public DateTime? EditedOn { get; set; }

        public Guid CustomerGUID { get; set; }

        public virtual ICollection<Measurement> Measurements { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }

        public virtual ICollection<Diet> Diets { get; set; }

    }

    public class BasicQuestionnaire
    {
        [Key, Required]
        public int id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Date"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime QuestionnareDate { get; set; }

        [StringLength(100), Display(ResourceType = typeof(Resource), Name = "Job")]
        public string JobName { get; set; }

        [StringLength(10), Display(ResourceType = typeof(Resource), Name = "WorkingHoursDaily")]
        public string JobHoursPerDay { get; set; }

        [StringLength(500), DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "DailyActivityDescription")]
        public string DailyActivityDescription { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "CardiovascularProblems")]
        public ExistingProblems CardioVascularProblems { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "HighBloodPressure")]
        public ExistingProblems HighBloodPressure { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "LowBloodPressure")]
        public ExistingProblems LowBloodPressure { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Diabetes")]
        public ExistingProblems Diabetes { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Hypoglycemia")]
        public ExistingProblems Hypoglycemia { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Asthma")]
        public ExistingProblems Asthma { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "BreathingProblems")]
        public ExistingProblems BreathingProblems { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Arthritis")]
        public ExistingProblems Arthritis { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "HighCholesterol")]
        public ExistingProblems HighCholesterol { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "HighTriglycerides")]
        public ExistingProblems HighTriglycerides { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Allergies")]
        public ExistingProblems Allergies { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Ulcer")]
        public ExistingProblems Ulcer { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "MaxWeightEver")]
        public int MaxWeightEver { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "MaxWeightAge")]
        public int MaxWeightAge { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "MinWeightEver")]
        public int MinWeightEver { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "MaxWeightAge")]
        public int MinWeightAge { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "WeightIncreasedOnLastPeriod")]
        public bool WeightIncreasedOnLastPeriod { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "WeightDecreasedOnLastPeriod")]
        public bool WeightDecreasedOnLastPeriod { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "OverWeightOnEarlyYears")]
        public bool OverWeightOnEarlyYears { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "NumerOfDailyMeals")]
        public string DailyMeals { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "LackOfAppetite")]
        public bool LackOfAppetite { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "Bulimia")]
        public bool Bulimia { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "HungryHours")]
        public string HungryHours { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "BuyingFruitsFrequency")]
        public string BuyingFruitsFrequency { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "WeeklyConsumingFruits")]
        public string WeeklyConsumingFruits { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "WeeklyMealsOutOfHome")]
        public string WeeklyMealsOutOfHome { get; set; }

        [StringLength(100), Display(ResourceType = typeof(Resource), Name = "WeeklyConsumingSweetsByKind")]
        public string WeeklyConsumingSweetsByKind { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "DigestiveSystemFunctionality")]
        public string DigestiveSystemFunctionality { get; set; }

        [StringLength(50), Display(ResourceType = typeof(Resource), Name = "FluidIntake")]
        public string FluidIntake { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Remarks"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }

    [ComplexType]
    public class ExistingProblems
    {
        [Display(ResourceType = typeof(Resource), Name = "Exists")]
        public bool Exists { get; set; }
        [StringLength(250), Display(ResourceType = typeof(Resource), Name = "Explaination")]
        public string Explaination { get; set; }
    }

    public class DailyRecall
    {
        [Key, Required]
        public int id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "CustomerID")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [StringLength(250), DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "BreakfastDescription")]
        public string Breakfast { get; set; }

        [StringLength(250), DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "MorningSnackDescription")]
        public string MorningSnack { get; set; }

        [StringLength(250), DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "LunchDescription")]
        public string Lunch { get; set; }

        [StringLength(250), DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "EveningSnackDescription")]
        public string EveningSnack { get; set; }

        [StringLength(250), DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "DinnerDescription")]
        public string Dinner { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Milk")]
        public LikesDislikes Milk { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Yoghurt")]
        public LikesDislikes Yoghurt { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "WhiteCheese")]
        public LikesDislikes WhiteCheese { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "YellowCheese")]
        public LikesDislikes YellowCheese { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "CottageCheese")]
        public LikesDislikes CottageCheese { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Chicken")]
        public LikesDislikes Chicken { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Turkey")]
        public LikesDislikes Turkey { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Hamburger")]
        public LikesDislikes Hamburger { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Beef")]
        public LikesDislikes Beef { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Pork")]
        public LikesDislikes Pork { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "InOilFood")]
        public LikesDislikes InOilFood { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Legumes")]
        public LikesDislikes Legumes { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Cereals")]
        public LikesDislikes Cereals { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Nuts")]
        public LikesDislikes Nuts { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Alcohol")]
        public LikesDislikes Alcohol { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "JunkFood")]
        public LikesDislikes JunkFood { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Salads")]
        public LikesDislikes Salads { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Fruits")]
        public LikesDislikes Fruits { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "A")]
        public string LikeA { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "B")]
        public string LikeB { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "C")]
        public string LikeC { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "D")]
        public string LikeD { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "E")]
        public string LikeE { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "A")]
        public string DislikeA { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "B")]
        public string DislikeB { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "C")]
        public string DislikeC { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "D")]
        public string DislikeD { get; set; }

        [StringLength(150), Display(ResourceType = typeof(Resource), Name = "E")]
        public string DislikeE { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Remarks"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }

    [ComplexType]
    public class LikesDislikes
    {
        public bool Like { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }
    }
    public class Appointment
    {
        [Key, Required]
        public int id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Date"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time), Display(ResourceType = typeof(Resource), Name = "FromTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime FromTime { get; set; }

        [DataType(DataType.Time), Display(ResourceType = typeof(Resource), Name = "ToTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
        public DateTime ToTime { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Remarks"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Status")]
        public AppointmentState State { get; set; }
        public enum AppointmentState
        {
            [Display(ResourceType = typeof(Resource), Name = "Active")]
            Active = 2,
            [Display(ResourceType = typeof(Resource), Name = "HasCompleted")]
            Completed = 4,
            [Display(ResourceType = typeof(Resource), Name = "Postpone")]
            Postpone = 8,
            [Display(ResourceType = typeof(Resource), Name = "AppointmentCanceled")]
            Canceled = 16
        }
    }

    public class Reminder
    {
        [Key, Required]
        public int id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Customer")]
        public int? CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        public int? AppointmentID { get; set; }

        public virtual Appointment Appointment { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "DateofReminder"), DisplayFormat(DataFormatString = "{0:d/M/yyyy H:mm}", ApplyFormatInEditMode = true)]
        public DateTime OnDate { get; set; }

        [StringLength(25), Display(ResourceType = typeof(Resource), Name = "MobilePhone")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "SendSMS")]
        public bool SendSMS { get; set; }

        [StringLength(10), Display(ResourceType = typeof(Resource), Name = "CodeReturnedFromProvider")]
        public string SMSResultCode { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "SMSStatus")]
        public ReminderState? SMSState { get; set; }

        [StringLength(35), Display(ResourceType = typeof(Resource), Name = "email"), DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "SendEmail")]
        public bool SendEmail { get; set; }

        [DataType(DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "Message")]
        public string Message { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "MailStatus")]
        public ReminderState MailState { get; set; }

        //[Display(Name = "SMS Status")]
        //public ReminderState SMSStatus { get; set; }
        public enum ReminderState
        {
            [Display(ResourceType = typeof(Resource), Name = "Active")]
            Active = 2,
            [Display(ResourceType = typeof(Resource), Name = "HasCompleted")]
            Completed = 4,
            [Display(ResourceType = typeof(Resource), Name = "ReminderCanceled")]
            Canceled = 8
        }
    }

    public class Measurement
    {
        public int id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Customer")]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Date"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Age")]
        public double Age
        {
            get
            {
                double ReturnValue = 0d;
                if (Customer != null)
                {
                    ReturnValue = Math.Round((Date - Customer.BirthDate).TotalDays / 365, 1);

                    double rest = ReturnValue % 1;
                    if (rest != 0)
                    {
                        ReturnValue = (rest > 0.75) ? ReturnValue = (int)ReturnValue + 1 : (rest < 0.25) ? ReturnValue = (int)ReturnValue : ReturnValue = (int)ReturnValue + 0.5;
                    }
                }
                return ReturnValue;
            }
        }

        [Display(ResourceType = typeof(Resource), Name = "Height")]
        [Range(0.4, 2.1, ErrorMessage = "Το ύψος πρέπει να είναι μεταξύ 0.4 και 2.1 μέτρα")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Height { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Weight")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal Weight { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Fat")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal Fat { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "RatioWeist")]
        public decimal WaistHipRatio { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Triglycerides")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal triglycerides { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Cholesterol")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal cholesterol { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "BloodPressure")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal BloodPressure { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Ferrum")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal Ferrum { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Spelter")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal spelter { get; set; }

        [Display(Name = "e-FAT")]
        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true)]
        public decimal e_FAT { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Chest")]
        public int? Chest { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Belly")]
        public int? Belly { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Quadriceps")]
        public int? Quadriceps { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Triceps")]
        public int? Triceps { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Diceps")]
        public int? Diceps { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Ypoplatios")]
        public int? Ypoplatios { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Iliac")]
        public int? Iliac { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Armpit")]
        public int? Armpit { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Shank")]
        public int? Shank { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "LowerMean")]
        public int? LowerMean { get; set; }

        [DataType(DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "Remarks")]
        public string Notes { get; set; }

        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "BMI"), DisplayFormat(DataFormatString = "{0:N1}")]
        public double BMI
        {
            get
            {
                if (Customer != null)
                {
                    if (Height > 0.4m && Weight > 0)
                    {
                        return ((double)Weight / (double)(Height * Height));
                    }
                    else
                    {
                        return 0d;
                    }

                }
                else
                {
                    return 0d;
                }
            }
        }

        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "JP-7 JP-F7")]
        public Calculation JP7
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Chest.HasValue && Belly.HasValue && Quadriceps.HasValue && Triceps.HasValue && Ypoplatios.HasValue && Iliac.HasValue && Armpit.HasValue)
                    {
                        sumofskinfolds = (Chest + Belly + Quadriceps + Triceps + Ypoplatios + Iliac + Armpit);
                        bodydensity = (Customer.Sex == Customer.sex.Male) ? 1.112 - (0.00043499 * sumofskinfolds) + (0.00000055 * sumofskinfolds * sumofskinfolds) - (0.00028826 * Age) : 1.097 - (0.00046971 * sumofskinfolds) + (0.00000056 * sumofskinfolds * sumofskinfolds) - (0.00012828 * Age);
                        bodyfat = (495 / bodydensity - 450) / 100;
                    }

                    return new Calculation
                    {
                        Name = (this.Customer.Sex == Customer.sex.Male) ? "JP-7" : "JP-F7",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(Name = "JP-3 JP-F3")]
        public Calculation JP3
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Customer.Sex == Customer.sex.Male)//Αν πρόκειτε για άντρα
                    {
                        if (Chest.HasValue && Belly.HasValue && Quadriceps.HasValue)
                        {
                            sumofskinfolds = (Chest + Belly + Quadriceps);
                            bodydensity = 1.10938 - (0.0008267 * sumofskinfolds) + (0.0000016 * sumofskinfolds * sumofskinfolds) - (0.0002574 * Age);
                            bodyfat = (495 / bodydensity - 450) / 100;
                        }
                    }
                    else//Αν είναι γυναίκα
                    {
                        if (Belly.HasValue && Triceps.HasValue && Iliac.HasValue)
                        {
                            sumofskinfolds = (Belly + Triceps + Iliac);
                            bodydensity = 1.089733 - (0.0009245 * sumofskinfolds) + (0.0000025 * sumofskinfolds * sumofskinfolds) - (0.0000979 * Age);
                            bodyfat = (495 / bodydensity - 450) / 100;
                        }
                    }
                    return new Calculation
                    {
                        Name = (this.Customer.Sex == Customer.sex.Male) ? "JP-3" : "JP-F3",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(Name = "Pa-3 Pa-F3")]
        public Calculation Pa3
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Customer.Sex == Customer.sex.Male)//Αν πρόκειτε για άντρα
                    {
                        if (Chest.HasValue && Triceps.HasValue && Ypoplatios.HasValue)
                        {
                            sumofskinfolds = (Chest + Triceps + Ypoplatios);
                            bodydensity = 1.1125025 - (0.0013125 * sumofskinfolds) + (0.0000055 * sumofskinfolds * sumofskinfolds) - (0.000244 * Age);
                            bodyfat = (495 / bodydensity - 450) / 100;
                        }
                    }
                    else//Αν είναι γυναίκα
                    {
                        if (Belly.HasValue && Diceps.HasValue && Armpit.HasValue)
                        {
                            sumofskinfolds = (Belly + Diceps + Armpit);
                            bodydensity = 1.0902369 - (0.0009379 * sumofskinfolds) + (0.0000026 * sumofskinfolds * sumofskinfolds) - (0.00001087 * Age);
                            bodyfat = (495 / bodydensity - 450) / 100;
                        }
                    }
                    return new Calculation
                    {
                        Name = (this.Customer.Sex == Customer.sex.Male) ? "Pa-3" : "Pa-F3",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(Name = "L-3")]
        public Calculation L3
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Belly.HasValue && Triceps.HasValue && Ypoplatios.HasValue)
                    {
                        sumofskinfolds = (Belly + Triceps + Ypoplatios);
                        bodydensity = 1.0982 - 0.000815 * sumofskinfolds + 0.0000084 * sumofskinfolds * sumofskinfolds;
                        if (Customer.Sex == Customer.sex.Male) { bodyfat = (495 / bodydensity - 450) / 100; }
                    }
                    return new Calculation
                    {
                        Name = "L-3",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(Name = "??-3")]
        public Calculation Quote3
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Chest.HasValue && Belly.HasValue && Iliac.HasValue && Armpit.HasValue)
                    {
                        sumofskinfolds = (Chest + Belly + Iliac + Armpit);
                        //bodydensity = 1.0982 - 0.000815 * sumofskinfolds + 0.0000084 * sumofskinfolds * sumofskinfolds;
                        if (Customer.Sex == Customer.sex.Male) { bodyfat = (0.27784 * sumofskinfolds - 0.00053 * sumofskinfolds * sumofskinfolds + 0.12437 * Age - 3.28791) / 100; }
                    }
                    return new Calculation
                    {
                        Name = "??-3",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(Name = "FS-4")]
        public Calculation FS4
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Belly.HasValue && Triceps.HasValue && Ypoplatios.HasValue && Armpit.HasValue)
                    {
                        bodydensity = 1.10647 - 0.00144 * Belly.Value - 0.00077 * Triceps.Value - 0.00162 * Ypoplatios.Value + 0.00071 * Armpit.Value;
                        if (Customer.Sex == Customer.sex.Male) { bodyfat = (495 / bodydensity - 450) / 100; }
                    }
                    return new Calculation
                    {
                        Name = "FS-4",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(Name = "S-2")]
        public Calculation S2
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Quadriceps.HasValue & Ypoplatios.HasValue)
                    {
                        bodydensity = 1.1043 - 0.001327 * Quadriceps.Value - 0.00131 * Ypoplatios.Value;
                        if (Customer.Sex == Customer.sex.Male) { bodyfat = (495 / bodydensity - 450) / 100; }
                    }
                    return new Calculation
                    {
                        Name = "S-2",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(Name = "P-9")]
        public Calculation P9
        {
            get
            {
                if (Customer != null)
                {
                    int? sumofskinfolds = (int?)null;
                    double? bodydensity = (double?)null;
                    double? bodyfat = (double?)null;
                    //int Age = DateTime.Today.Year - Customer.BirthDate.Year;
                    if (Chest.HasValue && Belly.HasValue && Quadriceps.HasValue && Triceps.HasValue && Diceps.HasValue && Ypoplatios.HasValue && Iliac.HasValue && Armpit.HasValue && Shank.HasValue && LowerMean.HasValue)
                    {
                        sumofskinfolds = Chest.Value + Belly.Value + Quadriceps.Value + Triceps.Value + Diceps.Value + Ypoplatios.Value + Iliac.Value + Armpit.Value + Shank.Value + LowerMean.Value;
                        //bodydensity = 1.1043 - 0.001327 * Quadriceps.Value - 0.00131 * Ypoplatios.Value;
                        if (Customer.Sex == Customer.sex.Male) { bodyfat = (27 * sumofskinfolds / (Convert.ToDouble(Weight) * 2.204622622)) / 100; }
                    }
                    return new Calculation
                    {
                        Name = "P-9",
                        SumOfSkinFolds = sumofskinfolds,
                        BodyDensity = bodydensity,
                        BodyFat = bodyfat
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "AverageBodyFat")]
        public double AverageBodyFat
        {
            get
            {
                if (Customer != null)
                {
                    double?[] Values = new double?[] { JP7.BodyFat, JP3.BodyFat, Pa3.BodyFat, L3.BodyFat, Quote3.BodyFat, FS4.BodyFat, S2.BodyFat, P9.BodyFat };
                    return Convert.ToDouble(Values.Average().GetValueOrDefault(0)) / 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "STDEVBodyFat")]
        public double STDEVBodyFat
        {
            get
            {
                if (Customer != null)
                {
                    double?[] Values = new double?[] { JP7.BodyFat, JP3.BodyFat, Pa3.BodyFat, L3.BodyFat, Quote3.BodyFat, FS4.BodyFat, S2.BodyFat, P9.BodyFat };
                    if (Values.Count() > 1)
                    {
                        double average = Values.Average().GetValueOrDefault(0);

                        double Sum = Values.Sum(r => (r.GetValueOrDefault(0) - average) * (r.GetValueOrDefault(0) - average));

                        return Math.Sqrt(Sum / Values.Count()) / 100;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "MedianBodyFat")]
        public double MedianBodyFat
        {
            get
            {
                if (Customer != null)
                {
                    double?[] Values = new double?[] { JP7.BodyFat, JP3.BodyFat, Pa3.BodyFat, L3.BodyFat, Quote3.BodyFat, FS4.BodyFat, S2.BodyFat, P9.BodyFat }.Where(r => r.HasValue).ToArray();

                    if (Values.Count() > 1)
                    {
                        int count = Values.Length;
                        Array.Sort(Values);

                        double medianValue = 0;

                        if (Values.Count() % 2 == 0)
                        {
                            // count is even, need to get the middle two elements, add them together, then divide by 2
                            int middleElement1 = (int)Values[(count / 2) - 1].Value;
                            int middleElement2 = (int)Values[(count / 2)].Value;
                            medianValue = (middleElement1 + middleElement2) / 2;
                        }
                        else
                        {
                            // count is odd, simply get the middle element.
                            medianValue = Values[(count / 2)].Value;
                        }

                        return (medianValue / 100);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "FatWeightRelation")]
        public double FatWeightRelation
        {
            get
            {
                if (Customer != null)
                {
                    return (Convert.ToDouble(Weight) * 2.204622622) * (double)AverageBodyFat;
                }
                return 0;
            }

        }
        [ReadOnly(true), Display(ResourceType = typeof(Resource), Name = "LeanWeight")]
        public double LeanWeight
        {
            get
            {
                if (Customer != null)
                {
                    return (Convert.ToDouble(Weight) * 2.204622622) - FatWeightRelation;
                }
                return 0;
            }

        }
    }

    [ComplexType]
    public class Calculation
    {
        [Display(ResourceType = typeof(Resource), Name = "CalculationMethod")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "SumOfSkinFolds")]
        public int? SumOfSkinFolds { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "BodyDensity")]
        public double? BodyDensity { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "BodyFat"), DisplayFormat(DataFormatString = "{0:P1}")]
        public double? BodyFat { get; set; }
    }

    public class FoodCategory
    {
        [Key, Required]
        public int id { get; set; }

        public int? CompanyID { get; set; }
        [ForeignKey("CompanyID")]
        public virtual Company Company { get; set; }

        [StringLength(80), Display(ResourceType = typeof(Resource), Name = "FoodCategoryEn")]
        public string EnglishName { get; set; }

        [Required, StringLength(80), Display(ResourceType = typeof(Resource), Name = "FoodCategoryGR")]
        public string GreekName { get; set; }
        public virtual ICollection<Food> Foods { get; set; }
    }

    public class Food
    {
        [Key]
        [Required]
        public int id { get; set; }

        [Required, Display(ResourceType = typeof(Resource), Name = "FoodCategory")]
        public int FoodCategoryID { get; set; }
        public virtual FoodCategory FoodCategory { get; set; }

        [StringLength(255), Display(ResourceType = typeof(Resource), Name = "FoodNameEN")]
        public string EnglishName { get; set; }

        [Required, StringLength(255), Display(ResourceType = typeof(Resource), Name = "FoodNameGR")]
        public string GreekName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Breakfast")]
        public bool IsBreakfast { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Snack")]
        public bool IsSnack { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Lunch")]
        public bool IsLunch { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Dinner")]
        public bool IsDinner { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "IsCollagene")]
        public bool IsCollagene { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "IsAntioxidant")]
        public bool IsAntioxidant { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "IsDetox")]
        public bool IsDetox { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "IsDiatrofogenomiki")]
        public bool IsDiatrofogenomiki { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "IsMenopause")]
        public bool IsMenopause { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Equivalent")]
        public decimal? Equivalent { get; set; }


        [Display(ResourceType = typeof(Resource), Name = "Energy"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Energy { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Protein"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Protein { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Carbohydrates"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Carbohydrates { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Calcium"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Calcium { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Water"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Water { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "LipidTotal"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Lipid_Tot { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Ash"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Ash { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "FiberTD"), DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Fiber_TD { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "SugarTotal"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? SugarTot { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Iron"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Iron { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Magnesium"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Magnesium { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Phosphorus"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Phosphorus { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Potassium"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Potassium { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Sodium"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Sodium { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Zinc"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Zinc { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Copper"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Copper { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Manganese"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Manganese { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Selenium"), DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Selenium { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminC"), DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Vitamin_C { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Thiamin"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Thiamin { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Riboflavin"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Riboflavin { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Niacin"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Niacin { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Pantothenic"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Pantothenic { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminB6"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Vitamin_B_6 { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Folatetotal"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Folate_Tot { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Folicacid"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Folic_acid { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Folatefood"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Folate_food { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "FolateDFE"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Folate_DFE { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Choline"), DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Choline { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminB12"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Vitamin_B_12 { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminAIU"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Vitamin_A_IU { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminARAE"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Vitamin_A_RAE { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Retinol"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Retinol { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Carotenealpha"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Carotene_alpha { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Carotenebeta"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Carotene_beta { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Cryptoxanthinbeta"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Cryptoxanthin_beta { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Lycopene"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Lycopene { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Luteinzeaxanthin"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Lutein_zeaxanthin { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminE"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Vitamin_E { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminD"), DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Vitamin_D { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminDIU"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Vitamin_D_IU { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "VitaminK"), DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Vitamin_K { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "FatSat"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Fat_Sat { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "FatMono"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Fat_Mono { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "FatPoly"), DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public decimal? Fat_Poly { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Cholesterol"), DisplayFormat(ApplyFormatInEditMode = true, NullDisplayText = "")]
        public int? Cholesterol { get; set; }

        public ICollection<DietDetail> DietDetails { get; set; }
    }

    public class RecipeCategory
    {
        [Key, Required]
        public int id { get; set; }

        public int CompanyID { get; set; }

        [ForeignKey(name: "CompanyID")]
        public virtual Company Company { get; set; }

        [Required, StringLength(50), Display(ResourceType = typeof(Resource), Name = "RecipeCategory")]
        public string Name { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }

    }

    public class Recipe
    {
        [Key, Required]
        public int id { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "RecipeCategoryID")]
        public int RecipeCategoryID { get; set; }

        [ForeignKey(name: "RecipeCategoryID")]
        public virtual RecipeCategory RecipeCategory { get; set; }

        [Required, StringLength(50), Display(ResourceType = typeof(Resource), Name = "RecipeName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "SortDescription"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "AdditionalNotes"), DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [StringLength(80)]
        public string FileGuid { get; set; }
    }

    public class Diet
    {
        [Key, Required]
        public int ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Customer")]
        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true), Display(ResourceType = typeof(Resource), Name = "StartDate")]
        public DateTime StartDate { get; set; }

        [StringLength(120), DataType(DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "DietName")]
        public string DietName { get; set; }

        [DataType(dataType: DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "Remarks")]
        public string Notes { get; set; }

        private DietType _Type = DietType.GeneralUse;
        [Display(ResourceType = typeof(Resource), Name = "Type")]
        public DietType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public enum DietType
        {
            [Display(ResourceType = typeof(Resource), Name = "GeneralUse")]
            GeneralUse = 2,
            [Display(ResourceType = typeof(Resource), Name = "CollageneSynthesis")]
            CollagentSynthesis = 4,
            [Display(ResourceType = typeof(Resource), Name = "Detox")]
            Detox = 8,
            [Display(ResourceType = typeof(Resource), Name = "Antioxidant")]
            Αntioxidant = 16,
            [Display(ResourceType = typeof(Resource), Name = "Diatrofogenomiki")]
            Diatrofogenomiki = 32,
            [Display(ResourceType = typeof(Resource), Name = "Menopause")]
            Menopause = 64
        }

        public IList<DietDailyMeal> DietDailyMeals { get; set; }
    }

    public class DietDailyMeal
    {
        [Key, Required]
        public int id { get; set; }

        [Required]
        public int DietID { get; set; }

        [ForeignKey("DietID")]
        public virtual Diet Diet { get; set; }

        [Required, Range(minimum: 1, maximum: 7, ErrorMessage = "Day should be between 1 and 7")]
        public int DayIndex { get; set; }

        [Required, Range(minimum: 0, maximum: 4, ErrorMessage = "Meal should be between 0 and 4")]
        public int MealIndex { get; set; }

        [StringLength(100, ErrorMessage = "Maximum Notes length is 100 characters"), Display(ResourceType = typeof(Resource), Name = "Remarks")]
        public string Notes { get; set; }

        public IList<DietDetail> DietDetails { get; set; }

    }

    public class DietDetail
    {
        [Key, Required]
        public int ID { get; set; }

        public int DietDailyMealID { get; set; }

        [ForeignKey("DietDailyMealID")]
        public virtual DietDailyMeal DietDailyMeal { get; set; }

        public int Group { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "FoodName")]
        public int FoodID { get; set; }

        [ForeignKey("FoodID")]
        public virtual Food Food { get; set; }

        [DisplayFormat(DataFormatString = "{0:N1}", ApplyFormatInEditMode = true, NullDisplayText = "-")Display(ResourceType = typeof(Resource), Name = "Quantity")]

        public decimal Quantity { get; set; }

        [StringLength(15), Display(ResourceType = typeof(Resource), Name = "QuantityType")]
        public string QuantityType { get; set; }

        [StringLength(30), Display(ResourceType = typeof(Resource), Name = "Remarks")]
        public string Remarks { get; set; }

    }

    public class nMessage
    {

        [Key, Required]
        public int id { get; set; }

        public int CompanyID { get; set; }

        [ForeignKey(name: "CompanyID")]
        public virtual Company Company { get; set; }

        [StringLength(40), Display(ResourceType = typeof(Resource), Name = "MessageRecipient")]
        public string Recipient { get; set; }

        [StringLength(80), Display(ResourceType = typeof(Resource), Name = "MessageSubject")]
        public string Subject { get; set; }

        [DataType(DataType.MultilineText), Display(ResourceType = typeof(Resource), Name = "MessageBody")]
        public string MessageBody { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Attatchments")]
        public string Attatchments { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "DeliveryType")]
        public DeliveryType Type { get; set; }

        public enum DeliveryType
        {
            email = 1,
            SMS = 2
        }

        [Display(ResourceType = typeof(Resource), Name = "Status")]
        public MessageStatus Status { get; set; }
        public enum MessageStatus
        {
            Pending = 1,
            Send = 2,
            Delivered = 3,
            Failed = 4
        }

        [StringLength(40), Display(ResourceType = typeof(Resource), Name = "StatusCode")]
        public string StatusCode { get; set; }

    }

    public class NormalRates
    {
        [Key, Required]
        public int id { get; set; }

        public ValueType Type { get; set; }

        public Customer.sex? Sex { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "FromAge")]
        public double FromAge { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "to")]
        public double ToAge { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ValueFrom")]
        public decimal FromValue { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "to")]
        public decimal ToValue { get; set; }

        [StringLength(20), Display(ResourceType = typeof(Resource), Name = "ValueUnit")]
        public string ValueUnit { get; set; }

        [StringLength(80), Display(ResourceType = typeof(Resource), Name = "Remarks")]
        public string Notes { get; set; }

        [StringLength(12), Display(ResourceType = typeof(Resource), Name = "Color")]
        public string Color { get; set; }

        public enum ValueType
        {
            [Display(ResourceType = typeof(Resource), Name = "BMI")]
            BMI = 1,
            [Display(ResourceType = typeof(Resource), Name = "Cholesterol")]
            Cholesterol = 2,
            [Display(ResourceType = typeof(Resource), Name = "BloodPressure")]
            BloodPressure = 3,
            [Display(ResourceType = typeof(Resource), Name = "Ferrum")]
            Ferrum = 4,
            [Display(ResourceType = typeof(Resource), Name = "Triglycerides")]
            Triglycerides = 5,
            [Display(ResourceType = typeof(Resource), Name = "Spelter")]
            Spelter = 6
        }
    }

    public class ndbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<TargetGroup> TargetGroups { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<BasicQuestionnaire> BasicQuestionnairies { get; set; }

        public DbSet<DailyRecall> DailyRecalls { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<Measurement> Measurements { get; set; }

        public DbSet<FoodCategory> FoodCategories { get; set; }

        public DbSet<Food> Foods { get; set; }

        public DbSet<RecipeCategory> RecipeCategories { get; set; }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Diet> Diets { get; set; }

        public DbSet<DietDailyMeal> DietDailyMeals { get; set; }

        public DbSet<DietDetail> DietDetails { get; set; }

        public DbSet<nMessage> Messages { get; set; }

        public DbSet<Picture> Pictures { get; set; }

        public DbSet<NormalRates> NormalRates { get; set; }

        public DbSet<AgeRange> AgeRanges { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasRequired(r => r.TargetGroup).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Food>().Property(m => m.Equivalent).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Fiber_TD).HasPrecision(5, 1);
            modelBuilder.Entity<Food>().Property(m => m.Copper).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Manganese).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Selenium).HasPrecision(5, 1);
            modelBuilder.Entity<Food>().Property(m => m.Vitamin_C).HasPrecision(5, 1);
            modelBuilder.Entity<Food>().Property(m => m.Thiamin).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Riboflavin).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Niacin).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Pantothenic).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Vitamin_B_6).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Choline).HasPrecision(5, 1);
            modelBuilder.Entity<Food>().Property(m => m.Vitamin_K).HasPrecision(5, 1);
            modelBuilder.Entity<Food>().Property(m => m.Fat_Sat).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Fat_Mono).HasPrecision(10, 3);
            modelBuilder.Entity<Food>().Property(m => m.Fat_Poly).HasPrecision(10, 3);

        }
    }
}