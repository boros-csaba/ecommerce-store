using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public class QuizService : IQuizService
    {
        private readonly DataContext context;

        public QuizService(DataContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public QuizResult StartNewQuiz(int customerId, string quizName)
        {
            var quiz = new QuizResult
            {
                QuizName = quizName,
                CustomerId = customerId,
                StartDate = Helper.Now
            };
            context.QuizResults.Add(quiz);
            context.SaveChanges();
            return quiz;
        }

        public void SaveQuizAnswer(int customerId, int quizId, int answerCount, string answer)
        {
            var quiz = context.QuizResults.First(a => a.Id == quizId && a.CustomerId == customerId);
            switch (answerCount)
            {
                case 0:
                    quiz.Answer1 = answer;
                    quiz.Answer1Date = Helper.Now;
                    break;
                case 1:
                    quiz.Answer2 = answer;
                    quiz.Answer2Date = Helper.Now;
                    break;
                case 2:
                    quiz.Answer3 = answer;
                    quiz.Answer3Date = Helper.Now;
                    break;
                case 3:
                    quiz.Answer4 = answer;
                    quiz.Answer4Date = Helper.Now;
                    break;
                case 4:
                    quiz.Answer5 = answer;
                    quiz.Answer5Date = Helper.Now;
                    break;
                case 5:
                    quiz.Answer6 = answer;
                    quiz.Answer6Date = Helper.Now;
                    break;
            }
            context.SaveChanges();
        }

        public void SaveQuizResult(int customerId, int quizId, string result)
        {
            var quiz = context.QuizResults.First(a => a.Id == quizId && a.CustomerId == customerId);
            quiz.Result = result;
            context.SaveChanges();
        }
    }
}
