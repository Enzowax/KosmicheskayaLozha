using System;

namespace KosmicheskayaLozha
{
    public partial class Appointments
    {
        public string ClientFullName  => Users?.FullName  ?? "—";
        public string ClientPhone     => Users?.Phone     ?? "—";
        public string MasterFullName  => Users1?.FullName ?? "—";
        public string ServiceTypeName => ServiceTypes?.Name ?? "—";
        public decimal ServiceTypePrice => ServiceTypes?.Price ?? 0;

        public bool IsAvailable =>
            ClientId == null && !IsCancelled && !IsCompleted
            && AppointmentDateTime > DateTime.Now;

        public string StatusText =>
            IsCancelled       ? "Отменена"  :
            IsCompleted       ? "Выполнена" :
            ClientId.HasValue ? "Занята"    : "Свободна";

        public string DateTimeStr =>
            AppointmentDateTime.ToString("dd.MM.yyyy HH:mm");
    }
}
