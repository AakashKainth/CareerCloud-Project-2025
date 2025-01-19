using CareerCloud.DataAccessLayer;
using CareerCloud.Pocos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace CareerCloud.ADODataAccessLayer
{
    public class CompanyJobRepository : IDataRepository<CompanyJobPoco>
    {
        private readonly string _connStr;

        public CompanyJobRepository()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            _connStr = configuration.GetConnectionString("DataConnection");
            if (string.IsNullOrEmpty(_connStr))
            {
                throw new InvalidOperationException("Connection string 'DataConnection' is not found or empty.");
            }
        }

        public void Add(params CompanyJobPoco[] items)
        {
            TransactionManager.ImplicitDistributedTransactions = true; // Enable implicit distributed transactions

            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("No items to add.");
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (CompanyJobPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyJobPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Company_Jobs]
                                                              ([Id], [Company], [Profile_Created], [Is_Inactive], [Is_Company_Hidden])
                                                              VALUES (@Id, @Company, @Profile_Created, @Is_Inactive, @Is_Company_Hidden)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Company", item.Company);
                            cmd.Parameters.AddWithValue("@Profile_Created", item.ProfileCreated);
                            cmd.Parameters.AddWithValue("@Is_Inactive", item.IsInactive);
                            cmd.Parameters.AddWithValue("@Is_Company_Hidden", item.IsCompanyHidden);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void CallStoredProc(string name, params Tuple<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<CompanyJobPoco> GetAll(params Expression<Func<CompanyJobPoco, object>>[] navigationProperties)
        {
            List<CompanyJobPoco> pocos = new List<CompanyJobPoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Company_Jobs]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CompanyJobPoco poco = new CompanyJobPoco
                        {
                            Id = reader.GetGuid(0),
                            Company = reader.GetGuid(1),
                            ProfileCreated = reader.GetDateTime(2),
                            IsInactive = reader.GetBoolean(3),
                            IsCompanyHidden = reader.GetBoolean(4),
                            TimeStamp = (byte[])reader[5]
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<CompanyJobPoco> GetList(Expression<Func<CompanyJobPoco, bool>> where, params Expression<Func<CompanyJobPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public CompanyJobPoco GetSingle(Expression<Func<CompanyJobPoco, bool>> where, params Expression<Func<CompanyJobPoco, object>>[] navigationProperties)
        {
            IList<CompanyJobPoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params CompanyJobPoco[] items)
        {
            TransactionManager.ImplicitDistributedTransactions = true; // Enable implicit distributed transactions

            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("No items to remove.");
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (CompanyJobPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyJobPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Company_Jobs] WHERE Id = @Id", conn, transaction);
                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Update(params CompanyJobPoco[] items)
        {
            TransactionManager.ImplicitDistributedTransactions = true; // Enable implicit distributed transactions

            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("No items to update.");
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (CompanyJobPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyJobPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Company_Jobs]
                                                              SET [Company] = @Company, [Profile_Created] = @Profile_Created, [Is_Inactive] = @Is_Inactive, [Is_Company_Hidden] = @Is_Company_Hidden
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Company", item.Company);
                            cmd.Parameters.AddWithValue("@Profile_Created", item.ProfileCreated);
                            cmd.Parameters.AddWithValue("@Is_Inactive", item.IsInactive);
                            cmd.Parameters.AddWithValue("@Is_Company_Hidden", item.IsCompanyHidden);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}



