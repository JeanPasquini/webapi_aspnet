﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace webAPI_ASPNET.Models
{
    public class Button
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public string BUTTONNAME { get; set; }
        public string DESCRIPTION { get; set; }
    }

    public class ButtonRelation
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public int IDUSER { get; set; }
        public int IDBUTTON { get; set; }
    }
}