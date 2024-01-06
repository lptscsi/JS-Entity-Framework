using RIAPP.DataService.Core;
using RIAPP.DataService.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAppDemo.BLL.Validators
{
    public class ProductValidator : BaseValidator<Expando>
    {
        public override Task<IEnumerable<ValidationErrorInfo>> ValidateModelAsync(
            Expando product,
            string[] modifiedField)
        {
            LinkedList<ValidationErrorInfo> errors = new LinkedList<ValidationErrorInfo>();
            if (Array.IndexOf(modifiedField, "Name") > -1 && product["Name"] != null)
            {
                string name = (product["Name"] as string);
                if (name.StartsWith("Ugly", StringComparison.OrdinalIgnoreCase))
                {
                    errors.AddLast(new ValidationErrorInfo { fieldName = "Name", message = "Ugly name" });
                }
            }

            if (Array.IndexOf(modifiedField, "Weight") > -1 && product["Weight"] != null)
            {
                int weight = (int)Convert.ChangeType(product["Weight"], typeof(int));
                if (weight > 20000)
                {
                    errors.AddLast(new ValidationErrorInfo
                    {
                        fieldName = "Weight",
                        message = "Weight must be less than 20000"
                    });
                }
            }

            if (Array.IndexOf(modifiedField, "SellEndDate") > -1 && product["SellEndDate"] != null)
            {
                DateTime sellEndDate = (DateTime)product["SellEndDate"];
                DateTime sellStartDate = (DateTime)product["SellStartDate"];
                if (sellEndDate < sellStartDate)
                {
                    errors.AddLast(new ValidationErrorInfo
                    {
                        fieldName = "SellEndDate",
                        message = "SellEndDate must be after SellStartDate"
                    });
                }
            }

            if (Array.IndexOf(modifiedField, "SellStartDate") > -1 && product["SellStartDate"] != null)
            {
                DateTime sellStartDate = (DateTime)product["SellStartDate"];
                if (sellStartDate > DateTime.Today)
                {
                    errors.AddLast(new ValidationErrorInfo
                    {
                        fieldName = "SellStartDate",
                        message = "SellStartDate must be prior today"
                    });
                }
            }

            if (ChangeType == ChangeType.Updated)
            {
                DateTime newDate = (DateTime)product["ModifiedDate"];
                DateTime prevDate = (DateTime)Original["ModifiedDate"];

                if (prevDate >= newDate)
                {
                    errors.AddLast(new ValidationErrorInfo
                    {
                        fieldName = "ModifiedDate",
                        message = "ModifiedDate must be greater than the previous ModifiedDate"
                    });
                }
            }

            if (Array.IndexOf(modifiedField, "ProductNumber") > -1 && product["ProductNumber"] != null)
            {
                string prodNumber = product["ProductNumber"] as string;
                if (prodNumber?.StartsWith("00") == true)
                {
                    errors.AddLast(new ValidationErrorInfo
                    {
                        fieldName = "ProductNumber",
                        message = "ProductNumber must not start from 00"
                    });
                }
            }

            return Task.FromResult(errors.AsEnumerable());
        }
    }
}