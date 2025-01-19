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
    public class CompanyJobEducationRepository : IDataRepository<CompanyJobEducationPoco>
    {
        private readonly string _connStr;

        public CompanyJobEducationRepository()
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

        public void Add(params CompanyJobEducationPoco[] items)
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
                        foreach (CompanyJobEducationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyJobEducationPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Company_Job_Educations]
                                                              ([Id], [Job], [Major], [Importance])
                                                              VALUES (@Id, @Job, @Major, @Importance)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Job", item.Job);
                            cmd.Parameters.AddWithValue("@Major", item.Major);
                            cmd.Parameters.AddWithValue("@Importance", item.Importance);

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

        public IList<CompanyJobEducationPoco> GetAll(params Expression<Func<CompanyJobEducationPoco, object>>[] navigationProperties)
        {
            List<CompanyJobEducationPoco> pocos = new List<CompanyJobEducationPoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Company_Job_Educations]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CompanyJobEducationPoco poco = new CompanyJobEducationPoco
                        {
                            Id = reader.GetGuid(0),
                            Job = reader.GetGuid(1),
                            Major = reader.GetString(2),
                            Importance = reader.GetInt16(3),
                            TimeStamp = (byte[])reader[4]
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<CompanyJobEducationPoco> GetList(Expression<Func<CompanyJobEducationPoco, bool>> where, params Expression<Func<CompanyJobEducationPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public CompanyJobEducationPoco GetSingle(Expression<Func<CompanyJobEducationPoco, bool>> where, params Expression<Func<CompanyJobEducationPoco, object>>[] navigationProperties)
        {
            IList<CompanyJobEducationPoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params CompanyJobEducationPoco[] items)
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
                        foreach (CompanyJobEducationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyJobEducationPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Company_Job_Educations] WHERE Id = @Id", conn, transaction);
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

        public void Update(params CompanyJobEducationPoco[] items)
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
                        foreach (CompanyJobEducationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyJobEducationPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Company_Job_Educations]
                                                              SET [Job] = @Job, [Major] = @Major, [Importance] = @Importance
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Job", item.Job);
                            cmd.Parameters.AddWithValue("@Major", item.Major);
                            cmd.Parameters.AddWithValue("@Importance", item.Importance);

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



