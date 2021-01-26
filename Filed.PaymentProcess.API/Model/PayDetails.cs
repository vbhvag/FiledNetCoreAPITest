using Filed.PaymentProcess.API.Data;
using Filed.PaymentProcess.API.Filters;
using System;
using System.ComponentModel.DataAnnotations;

namespace Filed.PaymentProcess.API.Model
{
    public class PayDetails : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Credit Card Number is required")]
        [StringLength(maximumLength:16, ErrorMessage ="Credit card number must not exceed 16-digits")]
        public string CreditCardNumber { get; set; }
        [Required(ErrorMessage = "Card holder name is required")]
        [StringLength(maximumLength: 255, ErrorMessage = "Card holder name too long")]
        public string CardHolder { get; set; }
        [Required(ErrorMessage = "Credit Card expiry date is required")]
        [CheckDateRange]
        public DateTime ExpirationDate { get; set; }
        [Range(100,999,ErrorMessage ="Provide valid 3-digit security code")]
        public int SecurityCode { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage ="Amount must be greater than -1")]
        public decimal Amount { get; set; }
    }
}