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
    public class ApplicantSkillRepository : IDataRepository<ApplicantSkillPoco>
    {
        private readonly string _connStr;

        public ApplicantSkillRepository()
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

        public void Add(params ApplicantSkillPoco[] items)
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
                        foreach (ApplicantSkillPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantSkillPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Applicant_Skills]
                                                              ([Id], [Applicant], [Skill], [Skill_Level], [Start_Month], [Start_Year], [End_Month], [End_Year])
                                                              VALUES (@Id, @Applicant, @Skill, @Skill_Level, @Start_Month, @Start_Year, @End_Month, @End_Year)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Skill", item.Skill);
                            cmd.Parameters.AddWithValue("@Skill_Level", item.SkillLevel);
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

        public IList<ApplicantSkillPoco> GetAll(params Expression<Func<ApplicantSkillPoco, object>>[] navigationProperties)
        {
            List<ApplicantSkillPoco> pocos = new List<ApplicantSkillPoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Applicant_Skills]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ApplicantSkillPoco poco = new ApplicantSkillPoco
                        {
                            Id = reader.GetGuid(0),
                            Applicant = reader.GetGuid(1),
                            Skill = reader.GetString(2),
                            SkillLevel = reader.GetString(3),
                            StartMonth = reader.GetByte(4),
                            StartYear = reader.GetInt32(5),
                            EndMonth = reader.GetByte(6),
                            EndYear = reader.GetInt32(7),
                            TimeStamp = (byte[])reader[8]
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<ApplicantSkillPoco> GetList(Expression<Func<ApplicantSkillPoco, bool>> where, params Expression<Func<ApplicantSkillPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public ApplicantSkillPoco GetSingle(Expression<Func<ApplicantSkillPoco, bool>> where, params Expression<Func<ApplicantSkillPoco, object>>[] navigationProperties)
        {
            IList<ApplicantSkillPoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params ApplicantSkillPoco[] items)
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
                        foreach (ApplicantSkillPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantSkillPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Applicant_Skills] WHERE Id = @Id", conn, transaction);
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

        public void Update(params ApplicantSkillPoco[] items)
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
                        foreach (ApplicantSkillPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantSkillPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Applicant_Skills]
                                                              SET [Applicant] = @Applicant, [Skill] = @Skill, [Skill_Level] = @Skill_Level, [Start_Month] = @Start_Month, [Start_Year] = @Start_Year, [End_Month] = @End_Month, [End_Year] = @End_Year
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Applicant", item.Applicant);
                            cmd.Parameters.AddWithValue("@Skill", item.Skill);
                            cmd.Parameters.AddWithValue("@Skill_Level", item.SkillLevel);
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

