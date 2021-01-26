using Filed.PaymentProcess.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filed.PaymentProcess.API.Model
{
    public class PaymentStatus : IEntity
    {
        public int Id { get; set; }
        public int PayDetailId { get; set; }
        public string Status { get; set; }
    }
}
