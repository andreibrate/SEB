using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.DataAccess.Interfaces;
using SEB.Models;

namespace SEB.BusinessLogic
{
    public class ExerciseHandler
    {
        private readonly IExerciseRepo _exerciseRepo;
        public ExerciseHandler(IExerciseRepo exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }

        public void AddExercise(Exercise exercise)
        {
            if (exercise.Count <= 0 || exercise.Duration <= 0)
            {
                throw new Exception("Exercise must have positive count and duration.");
            }

            _exerciseRepo.AddExercise(exercise);
        }
        public List<Exercise> GetExercisesByUserId(Guid userId)
        {
            return _exerciseRepo.GetExercisesByUserId(userId);
        }
    }
}
