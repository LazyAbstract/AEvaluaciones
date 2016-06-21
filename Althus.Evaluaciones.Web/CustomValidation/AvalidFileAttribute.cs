using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Althus.Evaluaciones.Web.CustomValidation
{
    public class AvalidFileAttribute : ValidationAttribute
    {
        public int MaxLength { get; set; }
        public string[] Allowed { get; set; }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return false;
            }

            if (file.ContentLength > MaxLength)
            {
                return false;
            }

            if (string.IsNullOrEmpty(file.FileName) && string.IsNullOrWhiteSpace(file.FileName))
            {
                return false;
            }

            if (!Allowed.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                return false;
            }

            return true;
        }
    }
}