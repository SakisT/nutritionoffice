using nutritionoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nutritionoffice.ViewModels
{
    public class SurveyView
    {
        [System.ComponentModel.DataAnnotations.Key]
        public Survey Survey { get; set; }

        public virtual ICollection<QuestionView> Questions { get; set; }

        
    }

    public class QuestionView
    {
        public Question Question { get; set; }

        public bool Deleted { get; set; }

        public virtual ICollection<QuestionAnswerView> QuestionAnswers { get; set; }
    }

    public  class QuestionAnswerView
    {
        public QuestionAnswer QuestionAnswer { get; set; }
        public bool Deleted { get; set; }
    }


}