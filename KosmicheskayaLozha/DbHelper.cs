namespace KosmicheskayaLozha
{
    /// <summary>
    /// Создаёт контекст EF с отключёнными Dynamic Proxies и Lazy Loading.
    /// Это нужно потому что LocalDB — локальная БД, прокси не нужны.
    /// Также без этого ComboBox показывает "System.Data.Entity.DynamicProxies..."
    /// </summary>
    public static class DbHelper
    {
        public static KosmicheskayaLozhaEntities CreateContext()
        {
            var db = new KosmicheskayaLozhaEntities();
            db.Configuration.ProxyCreationEnabled  = false;
            db.Configuration.LazyLoadingEnabled    = false;
            return db;
        }
    }
}
