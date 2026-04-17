using System;

namespace KosmicheskayaLozha.Models
{
    public class Appointment
    {
        public int      Id                  { get; set; }
        public int?     ClientId            { get; set; }
        public int      MasterId            { get; set; }
        public int      ServiceTypeId       { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string   PaymentMethod       { get; set; }
        public string   Comment             { get; set; }
        public bool     IsCompleted         { get; set; }
        public bool     IsCancelled         { get; set; }

        // Навигационные свойства (заполняются при чтении)
        public string ClientFullName    { get; set; }
        public string ClientPhone       { get; set; }
        public string MasterFullName    { get; set; }
        public string ServiceTypeName   { get; set; }
        public decimal ServiceTypePrice { get; set; }

        public bool IsAvailable => !ClientId.HasValue && !IsCancelled && !IsCompleted
                                   && AppointmentDateTime > DateTime.Now;

        public string StatusText =>
            IsCancelled  ? "Отменена"   :
            IsCompleted  ? "Выполнена"  :
            ClientId.HasValue ? "Занята" :
            "Свободна";

        public string DateTimeStr => AppointmentDateTime.ToString("dd.MM.yyyy HH:mm");
    }
}
