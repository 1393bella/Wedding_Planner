using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wedding_Planner.Models
{
        public class WeddPlan
        {
        [Key]
        public int WeddPlanId { get; set; }

        [Required(ErrorMessage = "Groom Name is required")]
        [MinLength (2, ErrorMessage = "Full Name must be at least 2 characters.")]
        public string WedderOne { get; set; }


        [Required(ErrorMessage = "Bride Name is required")]
        [MinLength (2, ErrorMessage = "Full Name must be at least 2 characters.")]
        public string WedderTwo { get; set; }

        [Required]
        [RestrictedDate(ErrorMessage="Please select valid Upcoming Date, for Wedding Date !!!")]
        [DataType (DataType.Date)]
        public DateTime WeddingDate { get; set; }

        [Required]
        [MinLength (2, ErrorMessage = "Address must be at least 2 characters.")]
        public string Address { get; set; }
        public List<RSVP> GuestList { get; set; }
        public int WeddingPlanner { get; set; }

        }
  

          public class RSVP
        {
        [Key]
        public int RSVPId { get; set; }
        public int WeddPlanId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public WeddPlan WeddPlan { get; set; }

        }
        public class User
        {
            // auto-implemented properties need to match the columns in your table
            // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
            [Key]
            public int UserId {get;set;}


            [Required(ErrorMessage = "First Name is required")]
            [MinLength (2, ErrorMessage = "First Name must be at least 2 characters.")]
            //[Display(Name="First Name")] we can display from the back too in label
            public string FirstName {get;set;}


            [Required(ErrorMessage = "Last Name is required")]
            [MinLength (2, ErrorMessage = "Last Name must be at least 2 characters.")]
            //[Display(Name="Last Name")] we can display from the back too in label
            public string LastName {get;set;}


            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Use a valid Email")]
            public string Email {get;set;}


            [Required]
            [DataType(DataType.Password)]
            [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]

            public string Password {get;set;}
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now;
            // Will not be mapped to your users table!
            [NotMapped]
            [Compare("Password")]
            [DataType(DataType.Password)]
            public string Confirm {get;set;}
            public List<RSVP> Atending { get; set; }
        }

       public class Login
        {
            [Required]
            public string Email{get;set;}

            [Required]
            public string Password {get;set;}


        }

       public class RestrictedDate : ValidationAttribute 
        {
        //validation to have a past data, not future
        public override bool IsValid (object submittedDate) 
            {
            DateTime date = (DateTime) submittedDate;
            return date > DateTime.Now;
            }
        }


    
}