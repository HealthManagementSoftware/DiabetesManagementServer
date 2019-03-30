﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public class Patient : ApplicationUser
    {
        public string DoctorUserName { get; set; }
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public List<GlucoseEntry> GlucoseEntries { get; set; }
        public List<ExerciseEntry> ExerciseEntries { get; set; }
        public List<MealEntry> MealEntries { get; set; }

        public Patient()
        {
            Doctor = new Doctor();
            GlucoseEntries = new List<GlucoseEntry>();
            ExerciseEntries = new List<ExerciseEntry>();
            MealEntries = new List<MealEntry>();

        } // constructor


        public static Patient Clone( Patient oldPatient)
        {
            return new Patient() {
                FirstName = oldPatient.FirstName,
                LastName = oldPatient.LastName,
                Address1 = oldPatient.Address1,
                Address2 = oldPatient.Address2,
                City = oldPatient.City,
                State = oldPatient.State,
                Zip1 = oldPatient.Zip1,
                Zip2 = oldPatient.Zip2,
                Email = oldPatient.Email,
                PhoneNumber = oldPatient.PhoneNumber,
                DoctorUserName = oldPatient.DoctorUserName,
                AccessFailedCount = oldPatient.AccessFailedCount,
                ConcurrencyStamp = oldPatient.ConcurrencyStamp,
                CreatedAt = oldPatient.CreatedAt,
                UpdatedAt = oldPatient.UpdatedAt,
                Doctor = oldPatient.Doctor,
                DoctorId = oldPatient.DoctorId,
                EmailConfirmed = oldPatient.EmailConfirmed,
                Height = oldPatient.Height,
                Weight = oldPatient.Weight,
                Id = oldPatient.Id,
                NormalizedEmail = oldPatient.NormalizedEmail,
                NormalizedUserName = oldPatient.NormalizedUserName,
                LockoutEnabled = oldPatient.LockoutEnabled,
                LockoutEnd = oldPatient.LockoutEnd,
                PhoneNumberConfirmed = oldPatient.PhoneNumberConfirmed,
                PasswordHash = oldPatient.PasswordHash,
                RemoteLoginExpiration = oldPatient.RemoteLoginExpiration,
                RemoteLoginToken = oldPatient.RemoteLoginToken,
                SecurityStamp = oldPatient.SecurityStamp,
                TwoFactorEnabled = oldPatient.TwoFactorEnabled,
                UserName = oldPatient.UserName
            };

        } // Clone


        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            string glucoseString = "";
            foreach ( GlucoseEntry entry in GlucoseEntries )
                glucoseString += "\n" + entry.ToString();

            string exerciseString = "";
            foreach ( ExerciseEntry entry in ExerciseEntries )
                exerciseString += "\n" + entry.ToString();

            string mealEntryString = "";
            foreach ( MealEntry entry in MealEntries )
                mealEntryString += "\n" + entry.ToString();

            return "First Name: " + FirstName
                + "\nLast Name: " + LastName
                + "\nEmail: " + Email
                + "\nPhone Number: " + PhoneNumber
                + "\nAddress 1: " + Address1
                + "\nAddress 2: " + Address2
                + "\nCity: " + City
                + "\nState: " + State
                + "\nZip1: " + Zip1
                + "\nZip2: " + Zip2
                + "\nUserName: " + UserName
                + "\nGlucoseEntries: " + glucoseString
                + "\nExerciseEntries: " + exerciseString
                + "\nMealEntries: " + mealEntryString
                + "\nDoctorUserName: " + DoctorUserName;

        } // ToString

    } // class

} // namespace