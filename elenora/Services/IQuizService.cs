using elenora.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elenora.Services
{
    public interface IQuizService
    {
        public QuizResult StartNewQuiz(int customerId, string quizName);
        public void SaveQuizAnswer(int customerId, int quizId, int answerCount, string answer);
        public void SaveQuizResult(int customerId, int quizId, string result);
    }
}
