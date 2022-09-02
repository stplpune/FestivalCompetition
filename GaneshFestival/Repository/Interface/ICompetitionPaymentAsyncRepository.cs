using GaneshFestival.Model;

namespace GaneshFestival.Repository.Interface
{
    public interface ICompetitionPaymentAsyncRepository
    {
        Task<ReturnMsg> SaveUpdateVehiclePayment(VehiclePayment vehiclePayment);
        Task<hashandTranid> PaymentGateway_Hash(HashModel hashModels);
        public Task<long> UpdatePaymentStatus(PaymentStatusModel payment);
    }
}
