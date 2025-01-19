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
    public class ApplicantWorkHistoryRepository : IDataRepository<ApplicantWorkHistoryPoco>
    {
        private readonly string _connStr;

        public ApplicantWorkHistoryRepository()
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

        public void Add(params ApplicantWorkHistoryPoco[] items)
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
                        foreach (ApplicantWorkHistoryPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantWorkHistoryPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Applicant_Work_History]
                                                              ([Id], [Applicant], [Company_Name], [Country_Code], [Location], [Job_Title], [Job_Description], [Start_Month], [Start_Year], [End_Month], [End_Year])
                                                              VALUES (@Id, @Applicant, @Company_Name, @Country_Code, @Location, @Job_Title, @Job_Description, @Start_Month, @Start_Year, @End_Month, @End_Year)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Company_Name", item.CompanyName);
                            cmd.Parameters.AddWithValue("@Country_Code", item.CountryCode);
                            cmd.Parameters.AddWithValue("@Location", item.Location);
                            cmd.Parameters.AddWithValue("@Job_Title", item.JobTitle);
                            cmd.Parameters.AddWithValue("@Job_Description", item.JobDescription);
                            cmd.Parameters.AddWithValue("@Start_Month", item.StartMonth);
                            cmd.Parameters.AddWithValue("@Start_Year", item.StartYear);
                            cmd.Parameters.AddWithValue("@End_Month", item.EndMonth);
                            cmd.Parameters.AddWithValue("@End_Year", item.EndYear);

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

        public IList<ApplicantWorkHistoryPoco> GetAll(params Expression<Func<ApplicantWorkHistoryPoco, object>>[] navigationProperties)
        {
            List<ApplicantWorkHistoryPoco> pocos = new List<ApplicantWorkHistoryPoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Applicant_Work_History]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ApplicantWorkHistoryPoco poco = new ApplicantWorkHistoryPoco
                        {
                            Id = reader.GetGuid(0),
                            Applicant = reader.GetGuid(1),
                            CompanyName = reader.GetString(2),
                            CountryCode = reader.GetString(3),
                            Location = reader.GetString(4),
                            JobTitle = reader.GetString(5),
                            JobDescription = reader.GetString(6),
                            StartMonth = reader.GetInt16(7),
                            StartYear = reader.GetInt32(8),
                            EndMonth = reader.GetInt16(9),
                            EndYear = reader.GetInt32(10),
                            TimeStamp = (byte[])reader[11]
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<ApplicantWorkHistoryPoco> GetList(Expression<Func<ApplicantWorkHistoryPoco, bool>> where, params Expression<Func<ApplicantWorkHistoryPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public ApplicantWorkHistoryPoco GetSingle(Expression<Func<ApplicantWorkHistoryPoco, bool>> where, params Expression<Func<ApplicantWorkHistoryPoco, object>>[] navigationProperties)
        {
            IList<ApplicantWorkHistoryPoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params ApplicantWorkHistoryPoco[] items)
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
                        foreach (ApplicantWorkHistoryPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantWorkHistoryPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Applicant_Work_History] WHERE Id = @Id", conn, transaction);
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

        public void Update(params ApplicantWorkHistoryPoco[] items)
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
                        foreach (ApplicantWorkHistoryPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantWorkHistoryPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Applicant_Work_History]
                                                              SET [Applicant] = @Applicant, [Company_Name] = @Company_Name, [Country_Code] = @Country_Code, [Location] = @Location, [Job_Title] = @Job_Title, [Job_Description] = @Job_Description, [Start_Month] = @Start_Month, [Start_Year] = @Start_Year, [End_Month] = @End_Month, [End_Year] = @End_Year
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Company_Name", item.CompanyName);
                            cmd.Parameters.AddWithValue("@Country_Code", item.CountryCode);
                            cmd.Parameters.AddWithValue("@Location", item.Location);
                            cmd.Parameters.AddWithValue("@Job_Title", item.JobTitle);
                            cmd.Parameters.AddWithValue("@Job_Description", item.JobDescription);
                            cmd.Parameters.AddWithValue("@Start_Month", item.StartMonth);
                            cmd.Parameters.AddWithValue("@Start_Year", item.StartYear);
                            cmd.Parameters.AddWithValue("@End_Month", item.EndMonth);
                            cmd.Parameters.AddWithValue("@End_Year", item.EndYear);

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


