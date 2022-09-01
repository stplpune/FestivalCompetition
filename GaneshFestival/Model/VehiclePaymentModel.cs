using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaneshFestival.Model
{
    public class VehiclePaymentModel
    {
        public int RowNumber { get; set; }
        public DateTime PaymentDate1 { get; set; }
        public long Id { get; set; }
        public int VehicleOwnerId { get; set; }
        public string GSTNo { get; set; }
        public int NoOfVehicle { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal AdditionalCharges { get; set; }
        public int IsPaymentSuccess { get; set; }
        public string TransactionId { get; set; }
        public string PayUMoneyId { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public string Error { get; set; }
        public string PgType { get; set; }
        public string BankRefNum { get; set; }
        public string ProductInfo { get; set; }
        public string FirstName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string CreatedThr { get; set; }
        public string PaymentDate { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public string VehicleIds { get; set; }
    }

    public class payment
    { 
        public int RowNumber { get; set; }
        public long VehicleId { get; set; }
        public string VehicleNo { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehTypeName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceSimNo { get; set; }
        public string RenewalRemainingDays { get; set; }
        public int DeviceCompanyId { get; set; }


    }
    public class payment1
    {
       
        public long VehicleId { get; set; }
        public string VehicleNo { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehTypeName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceSimNo { get; set; }
        public string RenewalRemainingDays { get; set; }
        public int DeviceCompanyId { get; set; }


    }
    public class PayUCredentials
    { 
        public string Key { get; set; }
        public string Salt { get; set; }
        public string BaseUrl { get; set; }
        public string ResponseUrl { get; set; }
    }
    public class AmcType
    { 
        public int AmcTypeId { get; set; }
        public decimal Rate { get; set; }
        public decimal GST { get; set; }
        public decimal TransactionPercentage { get; set; }

    }
}
