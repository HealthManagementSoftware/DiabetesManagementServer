using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
    public class MealEntry
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        //public string UserId { get; set; }
        public Patient Patient { get; set; }
        public int TotalCarbs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Timestamp { get; set; }

        public List<MealItem> MealItems { get; set; }

        public MealEntry()
        {
            MealItems = new List<MealItem>();

        } // constructor

        //public override string ToString()
        //{
        //    string mealItemString = "";
        //    foreach ( MealItem mealItem in MealItems )
        //        mealItemString += mealItem.ToString();

        //    return "MEAL ENTRY:"
        //        + "\nId: " + Id
        //        + "\nUserName: " + UserName
        //        //+ "\nUserId: " + UserId
        //        + "\nTotal Carbs: " + TotalCarbs
        //        + "\nCreatedAt: " + CreatedAt
        //        + "\nUpdatedAt: " + UpdatedAt
        //        + "\nTimestamp: " + Timestamp
        //        + "\n" + mealItemString;

        //} // ToString


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

        } // ToString

    } // class

} // namespace