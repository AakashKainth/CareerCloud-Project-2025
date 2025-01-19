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
    public class ApplicantEducationRepository : IDataRepository<ApplicantEducationPoco>
    {
        private readonly string _connStr;

        public ApplicantEducationRepository()
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



        public void Add(params ApplicantEducationPoco[] items)
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

                        foreach (ApplicantEducationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantEducationPoco item is null.");
                            }

                            cmd.CommandText = @"INSERT INTO [dbo].[Applicant_Educations]
                                            ([Id]
                                            ,[Applicant]
                                            ,[Major]
                                            ,[Certificate_Diploma]
                                            ,[Start_Date]
                                            ,[Completion_Date]
                                            ,[Completion_Percent])
                                      VALUES
                                            (@Id,
                                            @Applicant,
                                            @Major, 
                                            @Certificate_Diploma,
                                            @Start_Date, 
                                            @Completion_Date,
                                            @Completion_Percent)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Major", item.Major);
                            cmd.Parameters.AddWithValue("@Certificate_Diploma", item.CertificateDiploma);
                            cmd.Parameters.AddWithValue("@Start_Date", item.StartDate);
                            cmd.Parameters.AddWithValue("@Completion_Date", item.CompletionDate);
                            cmd.Parameters.AddWithValue("@Completion_Percent", item.CompletionPercent);

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

        public IList<ApplicantEducationPoco> GetAll(params Expression<Func<ApplicantEducationPoco, object>>[] navigationProperties)
        {
            List<ApplicantEducationPoco> appPocos = new List<ApplicantEducationPoco>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    SqlCommand cmd = new SqlCommand(@"SELECT [Id]
                                                      ,[Applicant]
                                                      ,[Major]
                                                      ,[Certificate_Diploma]
                                                      ,[Start_Date]
                                                      ,[Completion_Date]
                                                      ,[Completion_Percent]
                                                      ,[Time_Stamp]
                                                  FROM [dbo].[Applicant_Educations]", conn);
                    conn.Open();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ApplicantEducationPoco poco = new ApplicantEducationPoco
                            {
                                Id = rdr.GetGuid(0),
                                Applicant = rdr.GetGuid(1),
                                Major = rdr.GetString(2),
                                CertificateDiploma = rdr.GetString(3),
                                StartDate = rdr.GetDateTime(4),
                                CompletionDate = rdr.GetDateTime(5),
                                CompletionPercent = rdr.GetByte(6),
                                TimeStamp = (byte[])rdr[7]
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

        public IList<ApplicantEducationPoco> GetList(Expression<Func<ApplicantEducationPoco, bool>> where, params Expression<Func<ApplicantEducationPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public ApplicantEducationPoco GetSingle(Expression<Func<ApplicantEducationPoco, bool>> where, params Expression<Func<ApplicantEducationPoco, object>>[] navigationProperties)
        {
            IList<ApplicantEducationPoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params ApplicantEducationPoco[] items)
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

                        foreach (ApplicantEducationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantEducationPoco item is null.");
                            }

                            cmd.CommandText = @"DELETE FROM [dbo].[Applicant_Educations]
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

        public void Update(params ApplicantEducationPoco[] items)
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

                        foreach (ApplicantEducationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantEducationPoco item is null.");
                            }

                            cmd.CommandText = @"UPDATE [dbo].[Applicant_Educations]
                                       SET [Applicant] = @Applicant
                                          ,[Major] = @Major
                                          ,[Certificate_Diploma] = @Certificate_Diploma
                                          ,[Start_Date] = @Start_Date
                                          ,[Completion_Date] = @Completion_Date
                                          ,[Completion_Percent] = @Completion_Percent
                                     WHERE Id = @Id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Major", item.Major);
                            cmd.Parameters.AddWithValue("@Certificate_Diploma", item.CertificateDiploma);
                            cmd.Parameters.AddWithValue("@Start_Date", item.StartDate);
                            cmd.Parameters.AddWithValue("@Completion_Date", item.CompletionDate);
                            cmd.Parameters.AddWithValue("@Completion_Percent", item.CompletionPercent);
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
