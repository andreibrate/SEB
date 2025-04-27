using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.HTTP.Endpoints;
using SEB.Models.Enums;

namespace SEB.Models
{
    public class Exercise
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ExerciseTypes Type { get; set; }
        public int Count { get; set; }
        public int Duration { get; set; } // seconds

        public Exercise()
        {
            Type = ExerciseTypes.PushUps;
            Count = 0;
            Duration = 0;
        }

        public Exercise(Guid userId, ExerciseTypes type, int count, int duration)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Type = type;
            Count = count;
            Duration = duration;
        }
    }
}
