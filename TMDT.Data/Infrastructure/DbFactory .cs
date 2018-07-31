namespace TMDT.Data.Infrastructure
{
    public class DbFactory : Disposable,IDbFactory
    {
        private TMDTDbContext dbContext;

        public TMDTDbContext Init()
        {
            return dbContext ?? (dbContext = new TMDTDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
