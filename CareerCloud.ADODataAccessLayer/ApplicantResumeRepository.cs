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
    public class ApplicantResumeRepository : IDataRepository<ApplicantResumePoco>
    {
        private readonly string _connStr;

        public ApplicantResumeRepository()
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

        public void Add(params ApplicantResumePoco[] items)
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
                        foreach (ApplicantResumePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantResumePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Applicant_Resumes]
                                                              ([Id], [Applicant], [Resume], [Last_Updated])
                                                              VALUES (@Id, @Applicant, @Resume, @Last_Updated)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Resume", item.Resume);
                            cmd.Parameters.AddWithValue("@Last_Updated", item.LastUpdated);

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

        public IList<ApplicantResumePoco> GetAll(params Expression<Func<ApplicantResumePoco, object>>[] navigationProperties)
        {
            List<ApplicantResumePoco> pocos = new List<ApplicantResumePoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Applicant_Resumes]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ApplicantResumePoco poco = new ApplicantResumePoco
                        {
                            Id = reader.GetGuid(0),
                            Applicant = reader.GetGuid(1),
                            Resume = reader.GetString(2),
                            LastUpdated = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3)
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<ApplicantResumePoco> GetList(Expression<Func<ApplicantResumePoco, bool>> where, params Expression<Func<ApplicantResumePoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public ApplicantResumePoco GetSingle(Expression<Func<ApplicantResumePoco, bool>> where, params Expression<Func<ApplicantResumePoco, object>>[] navigationProperties)
        {
            IList<ApplicantResumePoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params ApplicantResumePoco[] items)
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
                        foreach (ApplicantResumePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantResumePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Applicant_Resumes] WHERE Id = @Id", conn, transaction);
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

        public void Update(params ApplicantResumePoco[] items)
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
                        foreach (ApplicantResumePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantResumePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Applicant_Resumes]
                                                              SET [Applicant] = @Applicant, [Resume] = @Resume, [Last_Updated] = @Last_Updated
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Resume", item.Resume);
                            cmd.Parameters.AddWithValue("@Last_Updated", item.LastUpdated);

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

