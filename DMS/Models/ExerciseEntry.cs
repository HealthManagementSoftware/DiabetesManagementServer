﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
    public class ExerciseEntry
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        //public string UserId { get; set; }
        public Patient Patient { get; set; }
        public string Name { get; set; }
        public int Minutes { get; set; }
        public int Steps { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Timestamp { get; set; }

        //public override string ToString()
        //{
        //    return "EXERCISE ENTRY:"
        //        + "\nId: " + Id
        //        + "\nUserName: " + UserName
        //        //+ "\nUserId: " + UserId
        //        + "\nExercise name: " + Name
        //        + "\nMinutes: " + Minutes
        //        + "\nSteps" + Steps
        //        + "\nCreatedAt: " + CreatedAt
        //        + "\nUpdatedAt: " + UpdatedAt
        //        + "\nTimestamp: " + Timestamp;

        //} // ToString


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

        } // ToString

    } // class

} // namespace