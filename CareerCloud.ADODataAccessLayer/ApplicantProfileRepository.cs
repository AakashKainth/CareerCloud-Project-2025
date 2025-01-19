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
    public class ApplicantProfileRepository : IDataRepository<ApplicantProfilePoco>
    {
        private readonly string _connStr;

        public ApplicantProfileRepository()
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

        public void Add(params ApplicantProfilePoco[] items)
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
                        foreach (ApplicantProfilePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantProfilePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Applicant_Profiles]
                                                              ([Id], [Login], [Current_Salary], [Current_Rate], [Currency], [Country_Code], [State_Province_Code], [Street_Address], [City_Town], [Zip_Postal_Code])
                                                              VALUES (@Id, @Login, @Current_Salary, @Current_Rate, @Currency, @Country_Code, @State_Province_Code, @Street_Address, @City_Town, @Zip_Postal_Code)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Login", item.Login);
                            cmd.Parameters.AddWithValue("@Current_Salary", item.CurrentSalary);
                            cmd.Parameters.AddWithValue("@Current_Rate", item.CurrentRate);
                            cmd.Parameters.AddWithValue("@Currency", item.Currency);
                            cmd.Parameters.AddWithValue("@Country_Code", item.Country);
                            cmd.Parameters.AddWithValue("@State_Province_Code", item.Province);
                            cmd.Parameters.AddWithValue("@Street_Address", item.Street);
                            cmd.Parameters.AddWithValue("@City_Town", item.City);
                            cmd.Parameters.AddWithValue("@Zip_Postal_Code", item.PostalCode);

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

        public IList<ApplicantProfilePoco> GetAll(params Expression<Func<ApplicantProfilePoco, object>>[] navigationProperties)
        {
            List<ApplicantProfilePoco> pocos = new List<ApplicantProfilePoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Applicant_Profiles]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ApplicantProfilePoco poco = new ApplicantProfilePoco
                        {
                            Id = reader.GetGuid(0),
                            Login = reader.GetGuid(1),
                            CurrentSalary = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
                            CurrentRate = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3),
                            Currency = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Country = reader.GetString(5),
                            Province = reader.GetString(6),
                            Street = reader.GetString(7),
                            City = reader.IsDBNull(8) ? null : reader.GetString(8),
                            PostalCode = reader.IsDBNull(9) ? null : reader.GetString(9),
                            TimeStamp = (byte[])reader[10]
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<ApplicantProfilePoco> GetList(Expression<Func<ApplicantProfilePoco, bool>> where, params Expression<Func<ApplicantProfilePoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public ApplicantProfilePoco GetSingle(Expression<Func<ApplicantProfilePoco, bool>> where, params Expression<Func<ApplicantProfilePoco, object>>[] navigationProperties)
        {
            IList<ApplicantProfilePoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params ApplicantProfilePoco[] items)
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
                        foreach (ApplicantProfilePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantProfilePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Applicant_Profiles] WHERE Id = @Id", conn, transaction);
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

        public void Update(params ApplicantProfilePoco[] items)
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
                        foreach (ApplicantProfilePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "ApplicantProfilePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Applicant_Profiles]
                                                              SET [Login] = @Login, [Current_Salary] = @Current_Salary, [Current_Rate] = @Current_Rate, [Currency] = @Currency, [Country_Code] = @Country_Code, [State_Province_Code] = @State_Province_Code, [Street_Address] = @Street_Address, [City_Town] = @City_Town, [Zip_Postal_Code] = @Zip_Postal_Code
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Login", item.Login);
                            cmd.Parameters.AddWithValue("@Current_Salary", item.CurrentSalary);
                            cmd.Parameters.AddWithValue("@Current_Rate", item.CurrentRate);
                            cmd.Parameters.AddWithValue("@Currency", item.Currency);
                            cmd.Parameters.AddWithValue("@Country_Code", item.Country);
                            cmd.Parameters.AddWithValue("@State_Province_Code", item.Province);
                            cmd.Parameters.AddWithValue("@Street_Address", item.Street);
                            cmd.Parameters.AddWithValue("@City_Town", item.City);
                            cmd.Parameters.AddWithValue("@Zip_Postal_Code", item.PostalCode);

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