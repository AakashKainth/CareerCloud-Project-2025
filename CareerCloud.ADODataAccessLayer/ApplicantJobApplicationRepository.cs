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
    public class ApplicantJobApplicationRepository : IDataRepository<ApplicantJobApplicationPoco>
    {
        private readonly string _connStr;

        public ApplicantJobApplicationRepository()
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

        public void Add(params ApplicantJobApplicationPoco[] items)
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
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transaction;

                        foreach (ApplicantJobApplicationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantJobApplicationPoco item is null.");
                            }

                            cmd.CommandText = @"INSERT INTO [dbo].[Applicant_Job_Applications]
                                            ([Id]
                                            ,[Applicant]
                                            ,[Job]
                                            ,[Application_Date])
                                      VALUES
                                            (@Id,
                                            @Applicant,
                                            @Job, 
                                            @Application_Date)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Job", item.Job);
                            cmd.Parameters.AddWithValue("@Application_Date", item.ApplicationDate);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public void CallStoredProc(string name, params Tuple<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicantJobApplicationPoco> GetAll(params Expression<Func<ApplicantJobApplicationPoco, object>>[] navigationProperties)
        {
            List<ApplicantJobApplicationPoco> appPocos = new List<ApplicantJobApplicationPoco>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    SqlCommand cmd = new SqlCommand(@"SELECT [Id]
                                                      ,[Applicant]
                                                      ,[Job]
                                                      ,[Application_Date]
                                                      ,[Time_Stamp]
                                                  FROM [dbo].[Applicant_Job_Applications]", conn);
                    conn.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ApplicantJobApplicationPoco poco = new ApplicantJobApplicationPoco
                            {
                                Id = rdr.GetGuid(0),
                                Applicant = rdr.GetGuid(1),
                                Job = rdr.GetGuid(2),
                                ApplicationDate = rdr.GetDateTime(3),
                                TimeStamp = (byte[])rdr[4]
                            };
                            appPocos.Add(poco);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("An error occurred while retrieving data.", ex);
            }

            return appPocos;
        }

        public IList<ApplicantJobApplicationPoco> GetList(Expression<Func<ApplicantJobApplicationPoco, bool>> where, params Expression<Func<ApplicantJobApplicationPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public ApplicantJobApplicationPoco GetSingle(Expression<Func<ApplicantJobApplicationPoco, bool>> where, params Expression<Func<ApplicantJobApplicationPoco, object>>[] navigationProperties)
        {
            IList<ApplicantJobApplicationPoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params ApplicantJobApplicationPoco[] items)
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
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transaction;

                        foreach (ApplicantJobApplicationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantJobApplicationPoco item is null.");
                            }

                            cmd.CommandText = @"DELETE FROM [dbo].[Applicant_Job_Applications]
                                       WHERE Id = @Id";
                            cmd.Parameters.Clear();
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
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public void Update(params ApplicantJobApplicationPoco[] items)
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
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transaction;

                        foreach (ApplicantJobApplicationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantJobApplicationPoco item is null.");
                            }

                            cmd.CommandText = @"UPDATE [dbo].[Applicant_Job_Applications]
                                       SET [Applicant] = @Applicant
                                          ,[Job] = @Job
                                          ,[Application_Date] = @Application_Date
                                     WHERE Id = @Id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Job", item.Job);
                            cmd.Parameters.AddWithValue("@Application_Date", item.ApplicationDate);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
