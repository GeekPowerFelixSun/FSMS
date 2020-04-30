using System;

namespace FSMS.Data.Repository
{
    public class RepositoryFactory
    {
        public Repository BaseRepository()
        {
            return new Repository(DbFactory.Base());
        }
    }
}
