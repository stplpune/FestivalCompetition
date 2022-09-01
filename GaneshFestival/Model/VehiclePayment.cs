using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaneshFestival.Model
{
    public class VehiclePayment
    {
        public int CreatedThr { get; set; }
        public long UserId { get; set; }
        public string GSTNo { get; set; }
        public long NoOfVehicles { get; set; }
        public decimal PaymentAmount { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string ProductInfo { get; set; }
        public string FirstName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Mode { get; set; }
        public string Error { get; set; }
        public string PgType { get; set; }
        public string BankRefNum { get; set; }
        public string PayuMoneyId { get; set; }
        public decimal AdditionalCharges { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TransactionCost { get; set; }
        public string VehicleIds { get; set; }
        public int AmcTypeId { get; set; }


    }
    public class ReturnMsg
    { 
        public bool IsSuccess { get; set; }
        public string Msg { get; set; }
        public string MobileNos { get; set; }
        public string PaymentStatus { get; set; }

    }
    public class VehiclePaymentOrder
    { 
        public int CreatedThr { get; set; }
        public long UserId { get; set; }
        public long VehicleOwnerId { get; set; }
        public string GSTNo { get; set; }
        public long NoOfVehicle { get; set; }
        public decimal PaymentAmount { get; set; }
        public string TrasnsactionId { get; set; }
        public string Status { get; set; }
        public string ProductInfo { get; set; }
        public string FirstName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TransactionCost { get; set; }
        public string VehicleIds { get; set; }
        public int AmcTypeId { get; set; }
    }
}
