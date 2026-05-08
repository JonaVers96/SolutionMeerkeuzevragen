using System;
using MeerkeuzevragenBL.Interfaces;
using MeerkeuzevragenBL.Managers;
using MeerkeuzevragenDL_File;
using MeerkeuzevragenDL_SQL;

namespace MeerkeuzevragenFactory
{
    public static class ManagerFactory
    {
        public static MeerkeuzevragenManager CreateManager(string connectionString)
        {
            IMeerkeuzevragenFileProcessor fileProcessor = new MeerkeuzevragenFileProcessor();
            IMeerkeuzevragenRepository repository = new MeerkeuzevragenRepository(connectionString);
            
            return new MeerkeuzevragenManager(fileProcessor, repository);
        }
    }
}
