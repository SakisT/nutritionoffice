namespace nutritionoffice.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<nutritionoffice.Models.ndbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(nutritionoffice.Models.ndbContext context)
        {

            Array.ForEach(context.Appointments.Where(r => !r.FromTime_Hour.HasValue).ToArray(), r => r.FromTime_Hour = r.FromTime.Hour);
            Array.ForEach(context.Appointments.Where(r => !r.FromTime_Minutes.HasValue).ToArray(), r => r.FromTime_Minutes = r.FromTime.Minute);
            Array.ForEach(context.Appointments.Where(r => !r.ToTime_Hour.HasValue).ToArray(), r => r.ToTime_Hour = r.ToTime.Hour);
            Array.ForEach(context.Appointments.Where(r => !r.ToTime_Minutes.HasValue).ToArray(), r => r.ToTime_Minutes = r.ToTime   .Minute);

            Array.ForEach(context.Reminders.Where(r => !r.Time_Hour.HasValue).ToArray(), r => r.Time_Hour = r.OnDate.Hour);
            Array.ForEach(context.Reminders.Where(r => !r.Time_Minutes.HasValue).ToArray(), r => r.Time_Minutes = r.OnDate.Minute);

            context.SaveChanges();
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
