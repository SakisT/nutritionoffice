namespace nutritionoffice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CustomerID = c.Int(nullable: false),
                    Date = c.DateTime(nullable: false),
                    FromTime = c.DateTime(nullable: false),
                    ToTime = c.DateTime(nullable: false),
                    Notes = c.String(),
                    State = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);

            CreateTable(
                "dbo.Customers",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    TargetGroupID = c.Int(nullable: false),
                    CompanyID = c.Int(nullable: false),
                    LastName = c.String(maxLength: 50),
                    FirstName = c.String(maxLength: 50),
                    BirthDate = c.DateTime(nullable: false),
                    Sex = c.Int(nullable: false),
                    Phone = c.String(maxLength: 25),
                    Mobile = c.String(maxLength: 25),
                    email = c.String(maxLength: 35),
                    Facebook = c.String(maxLength: 35),
                    City = c.String(maxLength: 45),
                    Address = c.String(maxLength: 100),
                    PostCode = c.String(maxLength: 15),
                    Notes = c.String(),
                    CreatedOn = c.DateTime(),
                    EditedOn = c.DateTime(),
                    TargetGroup_id = c.Int(),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .ForeignKey("dbo.TargetGroups", t => t.TargetGroup_id)
                .ForeignKey("dbo.TargetGroups", t => t.TargetGroupID)
                .Index(t => t.TargetGroupID)
                .Index(t => t.CompanyID)
                .Index(t => t.TargetGroup_id);

            CreateTable(
                "dbo.Companies",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CompanyName = c.String(nullable: false, maxLength: 100),
                    Owner = c.String(maxLength: 100),
                    Phone = c.String(maxLength: 25),
                    email = c.String(maxLength: 50),
                    SMSSign = c.String(maxLength: 11),
                    EmergencyPhone = c.String(maxLength: 25),
                    Address = c.String(maxLength: 60),
                    City = c.String(maxLength: 60),
                    PostCode = c.String(maxLength: 10),
                    FaceBook = c.String(maxLength: 50),
                    Twitter = c.String(maxLength: 50),
                    Instagram = c.String(maxLength: 50),
                    SMTPHost = c.String(maxLength: 50),
                    SMTPEnableSSL = c.Boolean(nullable: false),
                    SMTPPort = c.Int(nullable: false),
                    SMTPUserName = c.String(maxLength: 50),
                    SMTPPassword = c.String(maxLength: 50),
                    logo = c.Binary(),
                })
                .PrimaryKey(t => t.id);

            CreateTable(
                "dbo.RecipeCategories",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CompanyID = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .Index(t => t.CompanyID);

            CreateTable(
                "dbo.Recipes",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    RecipeCategoryID = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 50),
                    Description = c.String(),
                    Notes = c.String(),
                    FileGuid = c.String(maxLength: 80),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.RecipeCategories", t => t.RecipeCategoryID, cascadeDelete: true)
                .Index(t => t.RecipeCategoryID);

            CreateTable(
                "dbo.TargetGroups",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 40),
                    CompanyID = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    Notes = c.String(),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .Index(t => t.CompanyID);

            CreateTable(
                "dbo.Diets",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    CustomerID = c.Int(nullable: false),
                    StartDate = c.DateTime(nullable: false),
                    DietName = c.String(maxLength: 120),
                    Notes = c.String(),
                    Type = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);

            CreateTable(
                "dbo.DietDailyMeals",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    DietID = c.Int(nullable: false),
                    DayIndex = c.Int(nullable: false),
                    MealIndex = c.Int(nullable: false),
                    Notes = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Diets", t => t.DietID, cascadeDelete: true)
                .Index(t => t.DietID);

            CreateTable(
                "dbo.DietDetails",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    DietDailyMealID = c.Int(nullable: false),
                    Group = c.Int(nullable: false),
                    FoodID = c.Int(nullable: false),
                    Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    QuantityType = c.String(maxLength: 15),
                    Remarks = c.String(maxLength: 30),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DietDailyMeals", t => t.DietDailyMealID, cascadeDelete: true)
                .ForeignKey("dbo.Foods", t => t.FoodID, cascadeDelete: true)
                .Index(t => t.DietDailyMealID)
                .Index(t => t.FoodID);

            CreateTable(
                "dbo.Foods",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    FoodCategoryID = c.Int(nullable: false),
                    EnglishName = c.String(maxLength: 255),
                    GreekName = c.String(maxLength: 255),
                    IsBreakfast = c.Boolean(nullable: false),
                    IsSnack = c.Boolean(nullable: false),
                    IsLunch = c.Boolean(nullable: false),
                    IsDinner = c.Boolean(nullable: false),
                    IsCollagene = c.Boolean(nullable: false),
                    IsAntioxidant = c.Boolean(nullable: false),
                    IsDetox = c.Boolean(nullable: false),
                    IsDiatrofogenomiki = c.Boolean(nullable: false),
                    IsMenopause = c.Boolean(nullable: false),
                    Equivalent = c.Decimal(precision: 10, scale: 3),
                    Energy = c.Int(),
                    Protein = c.Decimal(precision: 18, scale: 2),
                    Carbohydrates = c.Decimal(precision: 18, scale: 2),
                    Calcium = c.Int(),
                    Water = c.Decimal(precision: 18, scale: 2),
                    Lipid_Tot = c.Decimal(precision: 18, scale: 2),
                    Ash = c.Decimal(precision: 18, scale: 2),
                    Fiber_TD = c.Decimal(precision: 5, scale: 1),
                    SugarTot = c.Decimal(precision: 18, scale: 2),
                    Iron = c.Decimal(precision: 18, scale: 2),
                    Magnesium = c.Int(),
                    Phosphorus = c.Int(),
                    Potassium = c.Int(),
                    Sodium = c.Int(),
                    Zinc = c.Decimal(precision: 18, scale: 2),
                    Copper = c.Decimal(precision: 10, scale: 3),
                    Manganese = c.Decimal(precision: 10, scale: 3),
                    Selenium = c.Decimal(precision: 5, scale: 1),
                    Vitamin_C = c.Decimal(precision: 5, scale: 1),
                    Thiamin = c.Decimal(precision: 10, scale: 3),
                    Riboflavin = c.Decimal(precision: 10, scale: 3),
                    Niacin = c.Decimal(precision: 10, scale: 3),
                    Pantothenic = c.Decimal(precision: 10, scale: 3),
                    Vitamin_B_6 = c.Decimal(precision: 10, scale: 3),
                    Folate_Tot = c.Int(),
                    Folic_acid = c.Int(),
                    Folate_food = c.Int(),
                    Folate_DFE = c.Int(),
                    Choline = c.Decimal(precision: 5, scale: 1),
                    Vitamin_B_12 = c.Decimal(precision: 18, scale: 2),
                    Vitamin_A_IU = c.Int(),
                    Vitamin_A_RAE = c.Int(),
                    Retinol = c.Int(),
                    Carotene_alpha = c.Int(),
                    Carotene_beta = c.Int(),
                    Cryptoxanthin_beta = c.Int(),
                    Lycopene = c.Int(),
                    Lutein_zeaxanthin = c.Int(),
                    Vitamin_E = c.Decimal(precision: 18, scale: 2),
                    Vitamin_D = c.Decimal(precision: 18, scale: 2),
                    Vitamin_D_IU = c.Int(),
                    Vitamin_K = c.Decimal(precision: 5, scale: 1),
                    Fat_Sat = c.Decimal(precision: 10, scale: 3),
                    Fat_Mono = c.Decimal(precision: 10, scale: 3),
                    Fat_Poly = c.Decimal(precision: 10, scale: 3),
                    Cholesterol = c.Int(),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.FoodCategories", t => t.FoodCategoryID, cascadeDelete: true)
                .Index(t => t.FoodCategoryID)
                .Index(t => t.EnglishName, unique: true)
                .Index(t => t.GreekName, unique: true);

            CreateTable(
                "dbo.FoodCategories",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    EnglishName = c.String(maxLength: 80),
                    GreekName = c.String(maxLength: 80),
                })
                .PrimaryKey(t => t.id)
                .Index(t => t.EnglishName, unique: true)
                .Index(t => t.GreekName, unique: true);

            CreateTable(
                "dbo.Measurements",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CustomerID = c.Int(nullable: false),
                    Date = c.DateTime(nullable: false),
                    Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Fat = c.Decimal(nullable: false, precision: 18, scale: 2),
                    WaistHipRatio = c.Decimal(nullable: false, precision: 18, scale: 2),
                    triglycerides = c.Decimal(nullable: false, precision: 18, scale: 2),
                    cholesterol = c.Decimal(nullable: false, precision: 18, scale: 2),
                    BloodPressure = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Ferrum = c.Decimal(nullable: false, precision: 18, scale: 2),
                    spelter = c.Decimal(nullable: false, precision: 18, scale: 2),
                    e_FAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Chest = c.Int(),
                    Belly = c.Int(),
                    Quadriceps = c.Int(),
                    Triceps = c.Int(),
                    Diceps = c.Int(),
                    Ypoplatios = c.Int(),
                    Iliac = c.Int(),
                    Armpit = c.Int(),
                    Shank = c.Int(),
                    LowerMean = c.Int(),
                    Notes = c.String(),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);

            CreateTable(
                "dbo.Reminders",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CustomerID = c.Int(),
                    AppointmentID = c.Int(),
                    OnDate = c.DateTime(nullable: false),
                    Mobile = c.String(maxLength: 25),
                    SendSMS = c.Boolean(nullable: false),
                    SMSResultCode = c.String(maxLength: 10),
                    SMSState = c.Int(),
                    email = c.String(maxLength: 35),
                    SendEmail = c.Boolean(nullable: false),
                    Message = c.String(),
                    MailState = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Appointments", t => t.AppointmentID)
                .ForeignKey("dbo.Customers", t => t.CustomerID)
                .Index(t => t.CustomerID)
                .Index(t => t.AppointmentID);

            CreateTable(
                "dbo.BasicQuestionnaires",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CustomerID = c.Int(nullable: false),
                    QuestionnareDate = c.DateTime(nullable: false),
                    JobName = c.String(maxLength: 100),
                    JobHoursPerDay = c.String(maxLength: 10),
                    DailyActivityDescription = c.String(maxLength: 500),
                    CardioVascularProblems_Exists = c.Boolean(nullable: false),
                    CardioVascularProblems_Explaination = c.String(maxLength: 250),
                    HighBloodPressure_Exists = c.Boolean(nullable: false),
                    HighBloodPressure_Explaination = c.String(maxLength: 250),
                    LowBloodPressure_Exists = c.Boolean(nullable: false),
                    LowBloodPressure_Explaination = c.String(maxLength: 250),
                    Diabetes_Exists = c.Boolean(nullable: false),
                    Diabetes_Explaination = c.String(maxLength: 250),
                    Hypoglycemia_Exists = c.Boolean(nullable: false),
                    Hypoglycemia_Explaination = c.String(maxLength: 250),
                    Asthma_Exists = c.Boolean(nullable: false),
                    Asthma_Explaination = c.String(maxLength: 250),
                    BreathingProblems_Exists = c.Boolean(nullable: false),
                    BreathingProblems_Explaination = c.String(maxLength: 250),
                    Arthritis_Exists = c.Boolean(nullable: false),
                    Arthritis_Explaination = c.String(maxLength: 250),
                    HighCholesterol_Exists = c.Boolean(nullable: false),
                    HighCholesterol_Explaination = c.String(maxLength: 250),
                    HighTriglycerides_Exists = c.Boolean(nullable: false),
                    HighTriglycerides_Explaination = c.String(maxLength: 250),
                    Allergies_Exists = c.Boolean(nullable: false),
                    Allergies_Explaination = c.String(maxLength: 250),
                    Ulcer_Exists = c.Boolean(nullable: false),
                    Ulcer_Explaination = c.String(maxLength: 250),
                    MaxWeightEver = c.Int(nullable: false),
                    MaxWeightAge = c.Int(nullable: false),
                    MinWeightEver = c.Int(nullable: false),
                    MinWeightAge = c.Int(nullable: false),
                    WeightIncreasedOnLastPeriod = c.Boolean(nullable: false),
                    WeightDecreasedOnLastPeriod = c.Boolean(nullable: false),
                    OverWeightOnEarlyYears = c.Boolean(nullable: false),
                    DailyMeals = c.String(maxLength: 50),
                    LackOfAppetite = c.Boolean(nullable: false),
                    Bulimia = c.Boolean(nullable: false),
                    HungryHours = c.String(maxLength: 50),
                    BuyingFruitsFrequency = c.String(maxLength: 50),
                    WeeklyConsumingFruits = c.String(maxLength: 50),
                    WeeklyMealsOutOfHome = c.String(maxLength: 50),
                    WeeklyConsumingSweetsByKind = c.String(maxLength: 100),
                    DigestiveSystemFunctionality = c.String(maxLength: 50),
                    FluidIntake = c.String(maxLength: 50),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);

            CreateTable(
                "dbo.DailyRecalls",
                c => new
                {
                    id = c.Int(nullable: false, identity: true),
                    CustomerID = c.Int(nullable: false),
                    Breakfast = c.String(maxLength: 250),
                    MorningSnack = c.String(maxLength: 250),
                    Lunch = c.String(maxLength: 250),
                    EveningSnack = c.String(maxLength: 250),
                    Dinner = c.String(maxLength: 250),
                    Milk_Like = c.Boolean(nullable: false),
                    Milk_Comments = c.String(maxLength: 250),
                    Yoghurt_Like = c.Boolean(nullable: false),
                    Yoghurt_Comments = c.String(maxLength: 250),
                    WhiteCheese_Like = c.Boolean(nullable: false),
                    WhiteCheese_Comments = c.String(maxLength: 250),
                    YellowCheese_Like = c.Boolean(nullable: false),
                    YellowCheese_Comments = c.String(maxLength: 250),
                    CottageCheese_Like = c.Boolean(nullable: false),
                    CottageCheese_Comments = c.String(maxLength: 250),
                    Chicken_Like = c.Boolean(nullable: false),
                    Chicken_Comments = c.String(maxLength: 250),
                    Turkey_Like = c.Boolean(nullable: false),
                    Turkey_Comments = c.String(maxLength: 250),
                    Hamburger_Like = c.Boolean(nullable: false),
                    Hamburger_Comments = c.String(maxLength: 250),
                    Beef_Like = c.Boolean(nullable: false),
                    Beef_Comments = c.String(maxLength: 250),
                    Pork_Like = c.Boolean(nullable: false),
                    Pork_Comments = c.String(maxLength: 250),
                    InOilFood_Like = c.Boolean(nullable: false),
                    InOilFood_Comments = c.String(maxLength: 250),
                    Legumes_Like = c.Boolean(nullable: false),
                    Legumes_Comments = c.String(maxLength: 250),
                    Cereals_Like = c.Boolean(nullable: false),
                    Cereals_Comments = c.String(maxLength: 250),
                    Nuts_Like = c.Boolean(nullable: false),
                    Nuts_Comments = c.String(maxLength: 250),
                    Alcohol_Like = c.Boolean(nullable: false),
                    Alcohol_Comments = c.String(maxLength: 250),
                    JunkFood_Like = c.Boolean(nullable: false),
                    JunkFood_Comments = c.String(maxLength: 250),
                    Salads_Like = c.Boolean(nullable: false),
                    Salads_Comments = c.String(maxLength: 250),
                    Fruits_Like = c.Boolean(nullable: false),
                    Fruits_Comments = c.String(maxLength: 250),
                })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.DailyRecalls", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.BasicQuestionnaires", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Customers", "TargetGroupID", "dbo.TargetGroups");
            DropForeignKey("dbo.Reminders", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Reminders", "AppointmentID", "dbo.Appointments");
            DropForeignKey("dbo.Measurements", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Foods", "FoodCategoryID", "dbo.FoodCategories");
            DropForeignKey("dbo.DietDetails", "FoodID", "dbo.Foods");
            DropForeignKey("dbo.DietDetails", "DietDailyMealID", "dbo.DietDailyMeals");
            DropForeignKey("dbo.DietDailyMeals", "DietID", "dbo.Diets");
            DropForeignKey("dbo.Diets", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Customers", "TargetGroup_id", "dbo.TargetGroups");
            DropForeignKey("dbo.TargetGroups", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Recipes", "RecipeCategoryID", "dbo.RecipeCategories");
            DropForeignKey("dbo.RecipeCategories", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Customers", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Appointments", "CustomerID", "dbo.Customers");
            DropIndex("dbo.DailyRecalls", new[] { "CustomerID" });
            DropIndex("dbo.BasicQuestionnaires", new[] { "CustomerID" });
            DropIndex("dbo.Reminders", new[] { "AppointmentID" });
            DropIndex("dbo.Reminders", new[] { "CustomerID" });
            DropIndex("dbo.Measurements", new[] { "CustomerID" });
            DropIndex("dbo.FoodCategories", new[] { "GreekName" });
            DropIndex("dbo.FoodCategories", new[] { "EnglishName" });
            DropIndex("dbo.Foods", new[] { "GreekName" });
            DropIndex("dbo.Foods", new[] { "EnglishName" });
            DropIndex("dbo.Foods", new[] { "FoodCategoryID" });
            DropIndex("dbo.DietDetails", new[] { "FoodID" });
            DropIndex("dbo.DietDetails", new[] { "DietDailyMealID" });
            DropIndex("dbo.DietDailyMeals", new[] { "DietID" });
            DropIndex("dbo.Diets", new[] { "CustomerID" });
            DropIndex("dbo.TargetGroups", new[] { "CompanyID" });
            DropIndex("dbo.Recipes", new[] { "RecipeCategoryID" });
            DropIndex("dbo.RecipeCategories", new[] { "CompanyID" });
            DropIndex("dbo.Customers", new[] { "TargetGroup_id" });
            DropIndex("dbo.Customers", new[] { "CompanyID" });
            DropIndex("dbo.Customers", new[] { "TargetGroupID" });
            DropIndex("dbo.Appointments", new[] { "CustomerID" });
            DropTable("dbo.DailyRecalls");
            DropTable("dbo.BasicQuestionnaires");
            DropTable("dbo.Reminders");
            DropTable("dbo.Measurements");
            DropTable("dbo.FoodCategories");
            DropTable("dbo.Foods");
            DropTable("dbo.DietDetails");
            DropTable("dbo.DietDailyMeals");
            DropTable("dbo.Diets");
            DropTable("dbo.TargetGroups");
            DropTable("dbo.Recipes");
            DropTable("dbo.RecipeCategories");
            DropTable("dbo.Companies");
            DropTable("dbo.Customers");
            DropTable("dbo.Appointments");
        }
    }
}
